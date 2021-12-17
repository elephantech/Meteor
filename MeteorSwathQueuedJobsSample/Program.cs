using System;
using System.Threading;

using Ttp.Meteor.MeteorSwath;
using Ttp.Meteor;

namespace MeteorSwathQueuedJobsSample
{
    /// <summary>
    /// Sample application demonstrating how to use MeteorSwath queued jobs mode
    /// </summary>
    class Program
    {
        /// <summary>
        /// Parser for the command line arguments sent by the user
        /// </summary>
        static CmdArgs _args = new CmdArgs();

        /// <summary>
        /// Which colour planes are listed in the Meteor configuration file
        /// </summary>
        static bool[] _planeUsed = new bool[MeteorConsts.MAX_PLANES];

        /// <summary>
        /// Flag set to true (asynchronously) by a CTRL+C press in the console.
        /// Causes the print job to be aborted and the application to exit.
        /// </summary>
        static volatile bool _abort = false;

        /// <summary>
        /// Thread which tracks the swath status, and uses ForcePd to trigger swaths
        /// </summary>
        static Thread _maintainJobThread;

        /// <summary>
        /// Is the PrintEngine running
        /// </summary>
        static bool _printEngineRunning;

        /// <summary>
        /// Is the application connected to MeteorSwath
        /// </summary>
        static bool _meteorSwathConnected;

        /// <summary>
        /// Log the Meteor return value if it's not RVAL_OK
        /// </summary>
        static bool CheckRval(eRET rVal, string msg) {
            if (rVal == eRET.RVAL_OK) {
                return true;
            }
            Logger.WriteLine(ConsoleColor.Red, $">!! {msg} failed: Error code: {rVal}");
            return false;
        }

        static void MaintainJobsThreadMain() {

            UInt32 jobId = FIRST_JOB_ID;
            int logColour = 2;
            while (!_abort) {
                eRET rVal;

                // Wait until the job has started, and note down how many swaths there are in the job.
                //
                // SwathsRequired can be -1 or 0 while the job is starting up.
                //
                // It will be -1 if the job ID we're asking for does not exist (i.e. the command has not
                // been sent from the application, or processed by Meteor yet), and it will be 0 if the swath 
                // count is still being calculated.
                //
                TSwathSeparatorStatus meteorSwathStatus;
                do {
                    rVal = MeteorSwathInterface.MeteorSwathGetStatusForJob(jobId, out meteorSwathStatus);
                } while ((rVal == eRET.RVAL_BUSY || meteorSwathStatus.SwathsRequired <= 0) && !_abort);
                if (!CheckRval(rVal, $"MeteorSwathGetStatusForJob({jobId})")) { return; }
                if (_abort) { return; }

                int swathsInJob = meteorSwathStatus.SwathsRequired;
                Logger.WriteLine(logColour, $">>> Meteor Swath Job {jobId} Started: {swathsInJob} swaths");

                // When MeteorSwath is running in queued mode, a synchronous call to MeteorSwathMoveToNextJob
                // is required prior to starting the motion commands for the job.  This is where Meteor
                // will update the PCC head positions and encoder resolution for the job, so the printer
                // transport **MUST** be stationary throughout this call.
                //
                // This call is required even if the head positions and encoder resolution are identical to
                // the previous job
                //
                rVal = MeteorSwathInterface.MeteorSwathMoveToNextJob(jobId);
                if (!CheckRval(rVal, $"MeteorSwathMoveToNextJob({jobId})")) { return; }

                int swathIndex = 0;
                bool sentPd = false;
                while (swathIndex < swathsInJob && !_abort) {
                    TSwathDetails swathDetails;
                    do {
                        rVal = MeteorSwathInterface.MeteorSwathGetSwathDetailsForJob(swathIndex, jobId, out swathDetails);
                    } while (rVal == eRET.RVAL_BUSY && !_abort);
                    if (!CheckRval(rVal, $"MeteorSwathGetSwathDetailsForJob({swathIndex},{jobId})")) { return; }

                    if (swathDetails.SwathLoadedCount == 1 && !sentPd) {
                        do {
                            rVal = PrinterInterfaceCLS.PiSetSignal((int)SigTypes.SIG_FORCEPD, 0);
                        } while (rVal == eRET.RVAL_BUSY && !_abort);/// CHECK !!!!
                        if (!CheckRval(rVal, "MeteorSwathMoveToNextJob")) { return; }
                        Logger.WriteLine(logColour, $">>> Sent PD to start swath {swathIndex + 1}");
                        sentPd = true;
                    }
                    if (swathDetails.SwathPrintedCount == 1) {
                        Logger.WriteLine(logColour, $">>> Swath {swathIndex + 1} has printed");
                        swathIndex++;
                        sentPd = false;
                    }
                    Thread.Sleep(1000);
                }

                if (++jobId == FIRST_JOB_ID + _args.JobCount) {
                    Logger.WriteLine(logColour, $">>> All swaths printed");
                    return;
                }
            }
        }

        /// <summary>
        /// Id of the first job we send to MeteorSwath.  Subsequent jobs have incrementing Ids.
        /// </summary>
        const int FIRST_JOB_ID = 100;

        /// <summary>
        /// </summary>
        static void RunJob() {
            int logColour = 1;

            Logger.WriteLine(logColour, ">>> Running print job");


            // Put MeteorSwath into queued mode
            //
            MeteorSwathInterface.MeteorSwathSetParam(eSWATHPARAM.SWATH_ENABLE_QUEUED_JOBS, 1);

            // Start a second thread to monitor the swath load status and trigger a virtual print
            // via "force PD"
            //
            _maintainJobThread = new Thread(MaintainJobsThreadMain);
            _maintainJobThread.Start();

            for (Int32 i = 0; i < _args.JobCount; i++) {

                // ------------------------------------------------------------------
                // Any parameter changes must be sent prior to starting the next job
                //
                // The commented out code below sends some dummy parameters to demonstrate 
                // the mechanism, when running on head types which use CCP_HEAD_XOFFSET.
                //
                // They are entirely arbitrary values and won't produce any sensible
                // real-world resulsts.

/*
                Int32 xOffset = ((i % 2) == 0) ? -50000 : 0; // X offsets in centi-pixels
                PrinterInterfaceCLS.PiGetPrnStatus(out TAppStatus appStatus);
                for (Int32 pcc = 1; pcc <= appStatus.PccsRequired; pcc++) {
                    PrinterInterfaceCLS.PiGetPccStatus(pcc+1, out TAppPccStatus pccStatus);
                    for (Int32 head = 1; head <= MeteorConsts.MAX_HDCS_PER_PCC; head++) {
                        PrinterInterfaceCLS.PiSetParam((int)eCFGPARAM.CCP_HEAD_XOFFSET | (head << 8) | (pcc << 16), xOffset);
                    }
                }
                MeteorSwathInterface.MeteorSwathUpdateEncoderResolution((UInt32)(1 + i));
*/                


                // Start the MeteorSwath print job.
                //
                bool partialBuffers = false, biDirectional = false, firstSwathReverse = false;
                TiffFileDetails tiff = _args.TiffDetails(i);
                UInt32 widthPx = tiff.Width;
                UInt32 heightPx = tiff.Height;
                UInt32 jobId = (UInt32)(FIRST_JOB_ID + i);
                eRET rVal = MeteorSwathInterface.MeteorSwathStartOnlineJob(_args.GeometryIndex(i), partialBuffers, widthPx, heightPx, biDirectional, firstSwathReverse, jobId);
                if (!CheckRval(rVal, "MeteorSwathStartOnlineJob")) { return; }

                Logger.WriteLine(logColour, $">>> Started job {jobId}");

                // Send the image to each colour plane
                // Use an arbitrary x start value which should work with most head types and the X positions sent above
                // If the swaths do not trigger, then the xStart can be increased (or the head positions decreased).
                //
                Int32 xStart = 1000;
                int yPosition = 0;
                for (int plane = 0; plane < MeteorConsts.MAX_PLANES; plane++) {
                    if (_planeUsed[plane]) {
                        do {
                            rVal = MeteorSwathInterface.MeteorSwathSendTIFF(plane + 1, xStart, yPosition, tiff.FilePath);
                        } while ((rVal == eRET.RVAL_FULL || rVal == eRET.RVAL_NOMEM) && !_abort);

                        if (!CheckRval(rVal, "MeteorSwathSendTIFF")) {
                            return;
                        }
                    }
                }

                Logger.WriteLine(logColour, $">>> Sent image for job {jobId}");

                do {
                    rVal = MeteorSwathInterface.MeteorSwathProcessAllSwaths();
                } while (rVal == eRET.RVAL_FULL && !_abort);

                Logger.WriteLine(logColour, $">>> Processed swaths for job {jobId}");

                // --------------------------------------------------------------------
                // It is very important that the application continues to retry
                // MeteorSwathEndJob while it returns RVAL_FULL.  This allows
                // the swaths for the job to be processed and sent to the 
                // hardware, which must happen before any parameters (e.g.
                // which planes to use, the encoder divider, or the head X positions) 
                // are changed for the next job to be queued.
                // --------------------------------------------------------------------
                do {
                    rVal = MeteorSwathInterface.MeteorSwathEndJob();
                } while (rVal == eRET.RVAL_FULL && !_abort);

                Logger.WriteLine(logColour, $">>> Sent end job for job {jobId}");
            }

            Logger.WriteLine(logColour, $">>> All jobs sent");
            Logger.WriteLine(logColour, $">>> -------------");
        }

        /// <summary>
        /// Run the application, once the Meteor components have been found and the 
        /// command line arguments have been successfully parsed.
        /// </summary>
        static void RunApplication() {
            
            // Start the Meteor PrintEngine; null means select the last used config file
            //
            // (Alternatively, the full path to the config file can be sent here)
            //
            eRET rVal = PrinterInterfaceCLS.PiStartPrintEngine(null);
            _printEngineRunning = (eRET.RVAL_OK == rVal);
            if (!CheckRval(rVal, "PiStartPrintEngine")) { return; }

            // Connect to MeteorSwath
            //
            // (To use MeteorSwath, MeteorSwathConnect must be used instead of PiOpenPrinter)
            //
            rVal = MeteorSwathInterface.MeteorSwathConnect();
            _meteorSwathConnected = (eRET.RVAL_OK == rVal);
            if (!CheckRval(rVal, "MeteorSwathConnect")) { return; }

            // Check the Meteor plane configuration and note how many colour planes are configured
            // For simplicity in this example, the same print data is sent to every colour plane
            //
            rVal = PrinterInterfaceCLS.PiGetPlaneConfig(out MeteorPlaneConfig planeConfig);
            if (!CheckRval(rVal, "PiGetPlaneConfig")) { return; }
            for (int plane = 0; plane < MeteorConsts.MAX_PLANES; plane++) {
                _planeUsed[plane] = planeConfig.HeadCount[plane] != 0;
            }

            Logger.WriteLine(">>> -------------");

            // Make sure that the Meteor PrintEngine is running in scanning mode
            //
            rVal = PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_SCANNING, 1);
            if (!CheckRval(rVal, "PiSetParam(CCP_SCANNING)")) { return; }

            // For this sample application, we must be running on internal clock
            // "Force PD" will be used to trigger each swath
            //
            rVal = PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_PRINT_CLOCK_HZ, _args.PrintFrequency);
            if (!CheckRval(rVal, "PiSetParam(CCP_PRINT_CLOCK_HZ)")) { return; }

            // Set the Meteor bits per pixel to match the TIFF files  
            // We have already checked that there is at least one TIFF file, and that the bit depth for
            // all the TIFF files is the same
            //
            rVal = PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_BITS_PER_PIXEL, (int)_args.TiffDetails(0).Bpp);
            if (!CheckRval(rVal, "PiSetParam(CCP_BITS_PER_PIXEL)")) { return; }

            // Check that each geometry is defined in the Meteor configuration file
            //
            bool geometriesOk = true;
            for (int i = 0; i < _args.JobCount; i++) {
                UInt32 geoIndex = _args.GeometryIndex(i);
                rVal = MeteorSwathInterface.MeteorSwathGetGeometrySummary(geoIndex, out TSwathGeometrySummary summary);
                if (!CheckRval(rVal, "MeteorSwathGetGeometrySummary")) { return; }
                bool geometryDefined = summary.IsGeometryDefined != 0;
                if (!geometryDefined) {
                    Logger.WriteLine(ConsoleColor.Red, $">!! Geometry index {geoIndex} is not defined in the config file");
                    geometriesOk = false;
                }
            }

            if (!geometriesOk) { return; }

            // Wait for hardware to connect
            //
            // This sample code requires hardware to be present so that a simulated print
            // can be triggered using the PCC's internal print clock
            //
            // (Note that this sample application does not enable head power; its mechanism
            //  of triggering swaths via a "Force PD" is not suitable for real world printing)
            //
            Logger.WriteLine(">>> Waiting for hardware to connect");
            const int HW_STABLE_COUNT = 5;
            int hardwareConnected = 0;
            while (hardwareConnected < HW_STABLE_COUNT && !_abort) {
                rVal = PrinterInterfaceCLS.PiGetPrnStatus(out TAppStatus appStatus);
                if (rVal == eRET.RVAL_OK && appStatus.PccsAttached == appStatus.PccsRequired) {
                    hardwareConnected++;
                } else {
                    hardwareConnected = 0;
                }
                Thread.Sleep(150);
            }

            // Send each TIFF file as a separate job, running MeteorSwath in queued mode to
            // allow the jobs to be queued up.
            //
            if (!_abort) {
                RunJob();
            }
        }

        /// <summary>
        /// Application entry point.  
        /// Note that we cannot make any calls directly into the Meteor assemblies from this
        /// method, because the framework then attempts an assembly load before we've had the
        /// chance to call LocatePrinterInterface.
        /// </summary>
        static void Main(string[] args) {
            //
            //
            Logger.WriteLine(">>> Starting MeteorSwathQueuedJobsSample");
            Logger.WriteLine(">>> ====================================");
            // Hook into CTRL+C so we can clean up Meteor rather than just terminating the process
            //
            Console.CancelKeyPress += Console_CancelKeyPress;
            // Fix up the environment so we can load the PrintEngine and MeteorSwath components
            //
            MeteorPath.LocatePrinterInterface();
            // Should now be safe to call a method which uses the PrinterInterface
            //
            Main2(args);
        }

        /// <summary>
        /// Called from main once the environment for accessing the Meteor components has been set up
        /// </summary>
        static void Main2(string[] args) {
            // Process the command line arguments
            //
            if (!_args.ProcessArgs(args)) {
                return;
            }
            // Connect to Meteor and send the print job
            //
            RunApplication();
            // Wait for the status polling thread to exit
            //
            if (_maintainJobThread != null) {
                _maintainJobThread.Join();
                _maintainJobThread = null;
            }
            // Don't exit immediately, unless we're aborting.  It can be useful to
            // look at what's happening in Monitor at the end of the jobs, so the
            // PrintEngine still needs to be running.
            if (!_abort) {
                Logger.WriteLine("Press any key to continue ...");
                Console.ReadKey();
            }
            // Wind up Meteor
            //
            if (_meteorSwathConnected) {
                PrinterInterfaceCLS.PiAbort();
                while (!PrinterInterfaceCLS.PiCanClosePrinter()) {
                    Thread.Sleep(100);
                }
                MeteorSwathInterface.MeteorSwathDisconnect();
                _meteorSwathConnected = false;
            }
            if (_printEngineRunning) {
                PrinterInterfaceCLS.PiStopPrintEngine(0);
                _printEngineRunning = false;
            }
        }

        /// <summary>
        /// Handler for a CTRL+C key press.  Sets the _abort flag so that the main loop can exit cleanly.
        /// Note that this event is called asynchronously.
        /// </summary>
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            Logger.WriteLine(ConsoleColor.Red, "CTRL+C pressed; aborting ....");
            _abort = true;      // -- Set flag to tell the main loop to exit
            e.Cancel = true;    // -- Set to true, so the process will resume
        }
    }
}
