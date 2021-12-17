using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Ttp.Meteor.MeteorSwath;
using Ttp.Meteor;
using System.IO;

namespace MeteorSwathPartialBuffersSample
{
    /// <summary>
    /// Object which holds the details of a TIFF file to be printed
    /// </summary>
    class TiffFileDetails
    {
        public string FilePath { get; private set; }
        public UInt32 Width { get; private set; }
        public UInt32 Height { get; private set; }
        public UInt32 Bpp { get; private set; }
        public TiffFileDetails(string filePath) { FilePath = filePath; }
        public bool SetDetails(bool _filePerPlane) {
            if (_filePerPlane) { // 1 file per plane
                return SetDetailsOneTifPerPlane();
            } else { // 1 file repeated for each plane
                return SetDetailsOneTifAllPlanes();
            }
        }

        private bool SetDetailsOneTifPerPlane() {
            FilePaths = new List<string>();
            for (int plane = 0; plane < MeteorConsts.MAX_PLANES; plane++) {
                string filePathPlane = $"{FilePath}_Clr{plane + 1}.tif";

                if (File.Exists(filePathPlane)) {
                    FilePaths.Add(filePathPlane);
                    eRET rVal = PrinterInterfaceCLS.PiGetImageFileDetails(FilePaths[plane], out GenericImageFileDetails fileDetails);
                    if (rVal != eRET.RVAL_OK) {
                        if (plane == 0) {
                            Console.WriteLine($"*** Failed to get details for file '{FilePaths[plane]}': {rVal}");
                            return false;
                        }
                    }
                    if (plane == 0) { // Set details for tif 1
                        Height = fileDetails.Height;
                        Width = fileDetails.Width;
                        Bpp = fileDetails.BitsPerSample;
                    } else {
                        if (Height != fileDetails.Height) { // Check details against tif 1
                            Console.WriteLine($"*** Height of Clr'{plane + 1}' different from Clr1");
                            return false;
                        } else if (Width != fileDetails.Width) {
                            Console.WriteLine($"*** Width of Clr'{plane + 1}' different from Clr1");
                            return false;
                        } else if (Bpp != fileDetails.BitsPerSample) {
                            Console.WriteLine($"*** Bpp of Clr'{plane + 1}' different from Clr1");
                            return false;
                        }
                    }
                } else {
                    Console.WriteLine($"Found Tif files for plane up to Clr{plane}");
                    break;
                }
            }
            return true;
        }

        private bool SetDetailsOneTifAllPlanes() {
            eRET rVal = PrinterInterfaceCLS.PiGetImageFileDetails(FilePath, out GenericImageFileDetails fileDetails);
            if (rVal != eRET.RVAL_OK) {
                Console.WriteLine($"*** Failed to get details for file '{FilePath}': {rVal}");
                return false;
            }
            Height = fileDetails.Height;
            Width = fileDetails.Width;
            Bpp = fileDetails.BitsPerSample;
            return true;
        }

        public List<string> FilePaths { get; private set; }
    }

    /// <summary>
    /// Sample application demonstrating how to use MeteorSwath partial buffers to achieve seamless joins between images
    /// in the Y direction in a scanning printer
    /// </summary>
    class Program
    {
        /// <summary>
        /// List of TIFF files passed on the command line
        /// </summary>
        static List<TiffFileDetails> _tiffFileList = new List<TiffFileDetails>();
        /// <summary>
        /// Meteor swath geometry index to use; can be passed using -g=N on the command line
        /// </summary>
        static UInt32 _geometryIndex = 1;
        /// <summary>
        /// Do we expect hardware to connect.  If so, we wait for the PCCs to connect before
        /// starting the print job.
        /// Can be set to false via the command line to bypass this wait.
        /// </summary>
        static bool _runningWithHardware = true;
        /// <summary>
        /// Which colour planes are listed in the Meteor configuration file
        /// </summary>
        static bool[] _planeUsed = new bool[MeteorConsts.MAX_PLANES];
        /// <summary>
        /// The number of copies of each image to print
        /// </summary>
        static UInt32 _copiesPerImage = 20;

        /// <summary>
        /// Whether we are sending a tiff file per plane (send 1 tiff file to all planes is the default).
        /// Can be set via the command line.
        /// </summary>
        static bool _filePerPlane = false;

        /// <summary>
        /// Flag set to true (asynchronously) by a CTRL+C press in the console.
        /// Causes the print job to be aborted and the application to exit.
        /// </summary>
        static volatile bool _abort = false;

        /// <summary>
        /// Log the Meteor return value if its not RVAL_OK
        /// </summary>
        static bool CheckRval(eRET rVal, string msg) {
            if (rVal == eRET.RVAL_OK) {
                return true;
            }
            Console.WriteLine($"{msg} failed: Error code: {rVal}");
            return false;
        }

        /// <summary>
        /// Process and validate the command line arguments
        /// </summary>
        static bool ProcessArgs(string[] args) {
            bool ok = true;
            foreach (string s in args) {
                if (s.StartsWith("-g=")) {
                    if (!UInt32.TryParse(s.Substring(3), out _geometryIndex) || _geometryIndex < 1) {
                        Console.WriteLine($"*** Invalid geometry '{s}'");
                        ok = false;
                    }
                } else if (s.StartsWith("-h=")) {
                    if (!bool.TryParse(s.Substring(3), out _runningWithHardware)) {
                        Console.WriteLine($"*** Invalid using hardware flag '{s}'");
                        ok = false;
                    }
                } else if (s.StartsWith("-p")) {
                    _filePerPlane = true;
                } else {
                    _tiffFileList.Add(new TiffFileDetails(s));
                }
            }
            if (_tiffFileList.Count == 0) {
                ok = false;
            }
            if (ok) {
                foreach (TiffFileDetails t in _tiffFileList) {
                    if (!t.SetDetails(_filePerPlane)) {
                        ok = false;
                    }
                }
            }
            if (ok) {
                foreach (TiffFileDetails t in _tiffFileList) {
                    if (t.Width != _tiffFileList[0].Width) {
                        Console.WriteLine("*** All TIFF files must have the same width");
                        ok = false;
                        break;
                    }
                }
            }
            if (!ok) {
                Console.WriteLine("Usage:");
                Console.WriteLine("MeteorSwathPartialBuffersSample [-g=GEOMETRY INDEX ] [-h=true/false] TiffFile1.tif [ TiffFile2.tif ... ]");
                Console.WriteLine("Or for Tif per plane:");
                Console.WriteLine("MeteorSwathPartialBuffersSample [-g=GEOMETRY INDEX ] [-h=true/false] -p TiffFile1 [ TiffFile2 ... ]");
                Console.WriteLine("with tifs in format TiffFile1_Clr1.tif, TiffFile1_Clr2.tif, ...");
                Console.WriteLine("");
            }
            return ok;
        }

        /// <summary>
        /// Start the MeteorSwath print job, send and process the TIFF data, and check in the
        /// status when data has been loaded and printed.
        /// </summary>
        static void RunJob() {
            // Start the MeteorSwath print job.
            //
            bool partialBuffers = true, biDirectional = false, firstSwathReverse = false;
            UInt32 widthPx = _tiffFileList[0].Width;
            UInt32 heightPx = _copiesPerImage * (UInt32)(_tiffFileList.Sum(x => x.Height));
            UInt32 jobId = 100;
            eRET rVal = MeteorSwathInterface.MeteorSwathStartOnlineJob(_geometryIndex, partialBuffers, widthPx, heightPx, biDirectional, firstSwathReverse, jobId);
            if (!CheckRval(rVal, "MeteorSwathStartOnlineJob")) { return; }

            // Wait until the job has started, and note down how many swaths there are in the job
            //
            TSwathSeparatorStatus meteorSwathStatus;
            do {
                rVal = MeteorSwathInterface.MeteorSwathGetStatus(out meteorSwathStatus);
            } while ( (rVal == eRET.RVAL_BUSY || meteorSwathStatus.IsInJob == 0) && !_abort);
            int swathsInJob = meteorSwathStatus.SwathsRequired;
            Console.WriteLine($"Meteor Swath Job Started: {swathsInJob} swaths");


            // Arbitrary x start value (which should work with most default config files)
            //
            Int32 xStart = 1000;

            int swathIndex = 0;
            int tiffFileIndex = 0;
            int yPosition = 0;
            int imageCopiesSent = 0;
            int swathsLoaded = 0;
            int swathsPrinted = 0;

            while (!_abort) {
                if (imageCopiesSent < _copiesPerImage) {
                    // Send the next TIFF image to MeteorSwath.
                    // Note that the 'plane' parameter in MeteorSwathSendTIFF is indexed from 1.
                    //
                    TiffFileDetails fd = _tiffFileList[tiffFileIndex];
                    Console.WriteLine($"Sending {fd.FilePath} at y={yPosition}");
                    for (int plane = 0; plane < MeteorConsts.MAX_PLANES; plane++) {
                        if (_planeUsed[plane]) {
                            bool bLoggedSleep = false;
                            do {
                                // Optionally send an image per plane
                                string filePath;
                                if (_filePerPlane) {                                    
                                    if (fd.FilePaths.Count > plane) {
                                        filePath = fd.FilePaths[plane];
                                    } else {
                                        Console.WriteLine($"*** Plane {plane + 1} has no file to send for it!");
                                        return;
                                    }
                                } else {
                                    filePath = fd.FilePath;
                                }

                                // There are two cases where we can wait and then retry.
                                //
                                // RVAL_NOMEM means that we've exceeded the amount of memory available in the
                                // PrintEngine for TIFF buffering.  This will happen when we have a lot of
                                // swaths queued in memory; once some data has printed, the buffers will start
                                // to move and we can retry.
                                //
                                // A real application will probably need a more sophisticated mechanism to
                                // throttle data send such that the out of memory condition is not reached.
                                //
                                // We can also get RVAL_FULL if the queue of commands into the PrintEngine is
                                // full.  This is less likely (we will run out of TIFF memory first), but would
                                // be seen if MeteorSwathSendImage was used instead of MeteorSwathSendTIFF
                                //
                                rVal = MeteorSwathInterface.MeteorSwathSendTIFF(plane + 1, xStart, yPosition, filePath);
                                if (rVal == eRET.RVAL_NOMEM) {
                                    if (!bLoggedSleep) {
                                        Console.WriteLine("Out of memory for TIFF buffering ... sleeping");
                                        bLoggedSleep = true;
                                    }
                                    Thread.Sleep(1000);
                                }
                            } while ((rVal == eRET.RVAL_FULL || rVal == eRET.RVAL_NOMEM) && !_abort);

                            if (!CheckRval(rVal, "MeteorSwathSendTIFF")) {
                                return;
                            }
                        }
                    }

                    // Increase the Y position for the next image; here, the images are being printed with
                    // zero gap
                    //
                    yPosition += (int)fd.Height;

                    // Move onto the next TIFF image, and count how many copies of each image have been sent
                    //
                    if (++tiffFileIndex == _tiffFileList.Count) {
                        tiffFileIndex = 0;
                        imageCopiesSent++;
                    }
                }

                // When partial buffers are being used, the PrintEngine is not holding the print
                // data for the entire job.  Therefore MeteorSwathProcessAllSwaths cannot be
                // used; the swaths must be processed and sent to the hardware one at a time.
                //
                // We need to check that the full Y region for swath N is being held in PrintEngine
                // memory prior to calling MeteorSwathProcessOneSwath for that swath.  
                // If the region is not covered, more data must be sent before calling 
                // MeteorSwathProcessOneSwath.
                //
                if (swathIndex < swathsInJob) {
                    // Get the details for the next swath which needs to be processed
                    //
                    TSwathDetails swathDetails;
                    do {
                        rVal = MeteorSwathInterface.MeteorSwathGetSwathDetails(swathIndex, out swathDetails);
                    } while (rVal == eRET.RVAL_BUSY && !_abort);
                    if (!CheckRval(rVal, "MeteorSwathGetSwathDetails")) { return; }

                    // IsSwathWithinRange should always be 1; 0 means that we're asking for the details of
                    // a swath which is outside the valid range for the job (implying that swathsInJob is
                    // wrong)
                    //
                    if (swathDetails.IsSwathWithinRange == 0) {
                        Console.WriteLine($"*** Swath {swathIndex} out of range: end of job reached");
                        break;
                    }

                    // Check that the partial buffers are full for each plane
                    //
                    bool canProcessSwath = true;
                    for (int plane = 0; plane < MeteorConsts.MAX_PLANES; plane++) {
                        if (_planeUsed[plane]) {
                            if (swathDetails.IsPartialBufferFull[plane] == 0) {
                                canProcessSwath = false;
                                break;
                            }
                        }
                    }

                    // Process the swath if possible
                    //
                    if (canProcessSwath) {
                        do {
                            rVal = MeteorSwathInterface.MeteorSwathProcessOneSwath(swathIndex);
                        } while (rVal == eRET.RVAL_FULL && !_abort);
                        if (!CheckRval(rVal, "MeteorSwathProcessOneSwath")) { return; }
                        swathIndex++;
                        Console.WriteLine($"Swath {swathIndex} processed");
                    }
                }

                // Track the swaths which have been loaded into the hardware (the count will also
                // increase here after swaths have been processed even if we are running without 
                // hardware - if SimPrint is enabled, a SimFile for the swath will have been created)
                //
                if (swathsLoaded < swathsInJob ) {
                    TSwathDetails swathDetails;
                    do {
                        rVal = MeteorSwathInterface.MeteorSwathGetSwathDetails(swathsLoaded, out swathDetails);
                    } while (rVal == eRET.RVAL_BUSY && !_abort);
                    if (swathDetails.SwathLoadedCount != 0) {
                        swathsLoaded++;
                        Console.WriteLine($"Swath {swathsLoaded} loaded");
                    }
                }

                // Track how many swaths have been printed
                //
                TAppStatus appStatus;
                do {
                    rVal = PrinterInterfaceCLS.PiGetPrnStatus(out appStatus);
                } while (rVal == eRET.RVAL_BUSY && !_abort);

                if (swathsPrinted != appStatus.PrintCount) {
                    swathsPrinted = appStatus.PrintCount;
                    Console.WriteLine($"Swath {swathsPrinted} printed");
                }

                if (swathsLoaded == swathsInJob && (!_runningWithHardware || swathsPrinted == swathsInJob)) {
                    Console.WriteLine($"Swath print job complete");
                    Console.WriteLine("Press any key to continue ...");
                    Console.ReadKey();
                    return;
                }
            }
        }

        /// <summary>
        /// Run the application, once the Meteor components have been found and the 
        /// command line arguments have been successfully parsed.
        /// </summary>
        static void RunApplication() {
            
            // Start the Meteor PrintEngine; null means select the last used config file
            //
            // (Alternatively, the full path to the config file can be sent)
            //
            eRET rVal = PrinterInterfaceCLS.PiStartPrintEngine(null);
            if (!CheckRval(rVal, "PiStartPrintEngine")) { return; }

            // Connect to MeteorSwath
            //
            // (To use MeteorSwath, MeteorSwathConnect must be used instead of PiOpenPrinter)
            //
            rVal = MeteorSwathInterface.MeteorSwathConnect();
            if (!CheckRval(rVal, "MeteorSwathConnect")) { return; }

            // Check the Meteor plane configuration and note how many colour planes are configured
            rVal = PrinterInterfaceCLS.PiGetPlaneConfig(out MeteorPlaneConfig planeConfig);
            if (!CheckRval(rVal, "PiGetPlaneConfig")) { return; }
            for (int plane = 0; plane < MeteorConsts.MAX_PLANES; plane++) {
                _planeUsed[plane] = planeConfig.HeadCount[plane] != 0;
            }

            Console.WriteLine("-----------");

            // Check that the Meteor bits per pixel matches the files we're going to print
            //
            // (We need this check because this sample is using the PrintEngine's native TIFF 
            //  loading via MeteorSwathSendTIFF; alternatively, an application can write print 
            //  data directly into a buffer and send it to MeteorSwath via MeteorSwathSendImage)
            //
            rVal = PrinterInterfaceCLS.PiGetPrnStatus(out TAppStatus appStatus);
            if (!CheckRval(rVal, "PiGetPrnStatus")) { return; }
            bool bppOk = true;
            foreach (TiffFileDetails fd in _tiffFileList) {
                if (fd.Bpp != appStatus.BitsPerPixel) {
                    Console.WriteLine($"*** TIFF file '{fd.FilePath} bits per pixel ({fd.Bpp}) does not match Meteor bits per pixel ({appStatus.BitsPerPixel})");
                    bppOk = false;
                }
            }

            // The configuration file must be set up with [System] Scanning = 1
            bool isScanning = (appStatus.Control & Bmcontrol.BM_SCANNING) == Bmcontrol.BM_SCANNING;
            if (!isScanning) {
                Console.WriteLine($"*** Config file must have [System] Scanning = 1");
            }

            // Check that the selected geometry is defined in the Meteor configuration file
            //
            rVal = MeteorSwathInterface.MeteorSwathGetGeometrySummary(_geometryIndex, out TSwathGeometrySummary summary);
            if (!CheckRval(rVal, "MeteorSwathGetGeometrySummary")) { return; }
            bool geometryDefined = summary.IsGeometryDefined != 0;
            if ( !geometryDefined ) {
                Console.WriteLine($"*** Geometry index {_geometryIndex} is not defined in the config file");
            }

            // Assuming all's OK, wait for hardware to connect (if necessary) and run the sample print job
            //
            if (geometryDefined && bppOk && isScanning && !_abort) {
                // This sample code can be used without Meteor hardware connected to check the data is 
                // being processed via SimPrint files.  
                //
                // If running with hardware, we need to wait here until it has connected before starting
                // the print job.  
                //
                // (Note that this sample application does not enable head power, for simplicity - it can
                //  be enabled via Monitor once the PCC hardware has connected if requred)
                //
                if (_runningWithHardware) {
                    Console.WriteLine("Waiting for hardware to connect");
                    const int HW_STABLE_COUNT = 5;
                    int hardwareConnected = 0;
                    while (hardwareConnected < HW_STABLE_COUNT && !_abort) {
                        rVal = PrinterInterfaceCLS.PiGetPrnStatus(out appStatus);
                        if (rVal == eRET.RVAL_OK && appStatus.PccsAttached == appStatus.PccsRequired) {
                            hardwareConnected++;
                        } else {
                            hardwareConnected = 0;
                        }
                        Thread.Sleep(150);
                    }
                } else {
                    Console.WriteLine("Running without hardware");
                }

                // Start a print job and cycle through the TIFF files _copiesPerImage times
                //
                RunJob();
            }

            Console.WriteLine("-----------");

            // Wind up Meteor
            //
            PrinterInterfaceCLS.PiAbort();
            while (!PrinterInterfaceCLS.PiCanClosePrinter()) {
                Thread.Sleep(100);
            }
            MeteorSwathInterface.MeteorSwathDisconnect();
            PrinterInterfaceCLS.PiStopPrintEngine(0);
        }

        /// <summary>
        /// Application entry point.  
        /// Note that we cannot make any calls directly into the Meteor assemblies from this
        /// method, as the framework then attempts an assembly load before we've had the
        /// chance to call LocatePrinterInterface.
        /// </summary>
        static void Main(string[] args) {
            // Hook into CTRL+C so we can clean up Meteor rather than just terminating the process
            //
            Console.CancelKeyPress += Console_CancelKeyPress;
            // Fix up the environment so we can load the PrintEngine and MeteorSwath components
            //
            MeteorInkjet.MeteorPath.LocatePrinterInterface();
            // Process the command line arguments
            if (!ProcessArgs(args)) {
                return;
            }
            // Connect to Meteor and send the print job
            RunApplication();
        }

        /// <summary>
        /// Handler for a control+C key press.  Sets the _abort flag so that the main loop can exit cleanly.
        /// Note that this event is called asynchronously.
        /// </summary>
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            _abort = true;      // -- Set flag to tell the main loop to exit
            e.Cancel = true;    // -- Set to true, so the process will resume
        }
    }
}
