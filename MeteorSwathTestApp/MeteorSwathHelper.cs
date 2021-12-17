using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ttp.Meteor;
using Ttp.Meteor.MeteorSwath;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// 
    /// </summary>
    class MeteorSwathHelper
    {
        public int Bpp { get; private set; }
        public TAppStatus? LastAppStatus { get; private set; }
        public TSwathSeparatorStatus? LastSwathSeparatorStatus { get; private set; }
        public event EventHandler StatusesUpdated;
        public List<TSwathDetails> SwathDetails = new List<TSwathDetails>();

        private DateTime LastPowerUpRequest = new DateTime(0);
        private bool bPrintEngineRunning = false;

        private Thread MeteorPollingThread;
        int PollingPeriodMs { get; set; }
        volatile bool AbortThread = false;
        volatile bool IsConnected = false;

        /// <summary>
        /// Start an asynchronous thread to poll the Meteor and Meteor Swath status
        /// </summary>
        /// <param name="pollingPeriodMs">Interval in milliseconds between status polls</param>
        public void StartPolling(int pollingPeriodMs)
        {
            AbortThread = false;
            PollingPeriodMs = pollingPeriodMs;
            if (MeteorPollingThread == null)
            {
                MeteorPollingThread = new Thread(MeteorSwathHelperMain);
                MeteorPollingThread.Start();
            }
        }

        /// <summary>
        /// Cancel the asynchronous status polling and wait for the MeteorSwathHelperMain thread to exit
        /// Once we know that there will be no further calls to any of the PrinterInterfaceCLS or
        /// MeteorSwathInterface status methods, it is safe to discconect from and stop the PrintEngine
        /// </summary>
        public void CancelPolling()
        {
            AbortThread = true;
            if (MeteorPollingThread != null) {
                if (!MeteorPollingThread.Join(TimeSpan.FromMilliseconds(5 * PollingPeriodMs)))
                {
                    Console.WriteLine("*** MeteorPollingThread.Join timeout !!!");
                }
                MeteorPollingThread = null;
            }
        }

        /// <summary>
        /// Main function for the MeteorSwath Helper thread.
        /// </summary>
        private void MeteorSwathHelperMain()
        {
            while (!AbortThread)
            {
                Thread.Sleep(PollingPeriodMs);

                if (AbortThread) 
                {
                    return;
                }

                TAppStatus appStatus;
                eRET rvalApp = PrinterInterfaceCLS.PiGetPrnStatus(out appStatus);

                if (IsConnected && rvalApp == eRET.RVAL_OK)
                {
                    // Keep the TAppStatus we just read.
                    LastAppStatus = appStatus;

                    // Read the PCC Status for PCC number one.  
                    // [ The status is not used below - this is included to demonstrate how to query the PCC Status ]
                    TAppPccStatus pccStatus;
                    eRET rvalPcc = PrinterInterfaceCLS.PiGetPccStatus(1 /* Pcc number */, out pccStatus); // Get the PCC status.
                    if (rvalPcc == eRET.RVAL_OK)
                    {
                        // If the status was read successfully, get the PCC state from bmStatusBits.
                        ePCCSTATE pccstate = (ePCCSTATE)((pccStatus.bmStatusBits & Bmps.BMPS_PCC_STATE) >> Bmps.SH_PCC_STATE);
                    }

                    // Read the Swath Separator status.
                    TSwathSeparatorStatus swathstatus;
                    eRET rvalSwath = MeteorSwathInterface.MeteorSwathGetStatus(out swathstatus);

                    // Read the details of each swath.
                    if (rvalSwath == eRET.RVAL_OK)
                    {
                        List<TSwathDetails> newList = new List<TSwathDetails>();

                        LastSwathSeparatorStatus = swathstatus;

                        for (int i = 0; i < LastSwathSeparatorStatus.Value.SwathsRequired; i++)
                        {
                            TSwathDetails details;
                            MeteorSwathInterface.MeteorSwathGetSwathDetails(i, out details);
                            newList.Add(details);
                        }

                        SwathDetails = newList;
                    }
                    else
                    {
                        LastSwathSeparatorStatus = null;
                    }

                    // Send a request to power on the heads. Only do this once every 10 seconds while the heads are off.
                    // This just stops us having to do it manually.
                    // 
                    // We also check that the required number of PCCs are connected to Meteor before sending the head 
                    // power command. This step could be omitted if required, for instance to print with just one
                    // colour plane.  Head power can also be applied by clicking on the "Head Power" button in Monitor.exe
                    double SecondsSinceLastPowerUp = DateTime.Now.Subtract(LastPowerUpRequest).TotalSeconds;
                    if (appStatus.HeadPowerState == eHEADPOWERSTATE.HPS_OFF && SecondsSinceLastPowerUp > 10)
                    {
                        if (appStatus.PccsAttached == appStatus.PccsRequired) 
                        {
                            PrinterInterfaceCLS.PiSetHeadPower(1);
                        }
                        LastPowerUpRequest = DateTime.Now;
                    }

                }
                else
                {
                    if (!IsConnected)
                    {
                        LastAppStatus = null;
                    }
                    else if (IsConnected && MeteorSwathInterface.MeteorSwathConnect() == eRET.RVAL_OK)
                    {
                        // We thought we were connected, but MeteorSwathConnect said otherwise.
                        LastAppStatus = null;
                        IsConnected = false;

                        // We called connect when testing the connection - we disconnect here
                        // to go back to our original state.
                        MeteorSwathInterface.MeteorSwathDisconnect(); 
                    }
                }
                var handler = StatusesUpdated;
                if (handler != null) { handler(this, EventArgs.Empty); }
            }

            AbortThread = false;
        }

        /// <summary>
        /// Start the Meteor Print Engine within the current process.
        /// An application *must* host the Meteor Print Engine in order to
        /// use Meteor Swath.  If the Meteor Print Engine is already running
        /// in another process (e.g. if it is being hosted by Meteor Monitor) 
        /// then PiStartPrintEngine returns RVAL_BUSY.
        /// </summary>
        /// <returns>Success / failure</returns>
        public bool StartPrintEngine() 
        {
            // Check whether Monitor is set up to host the Print Engine.
            // If it is, give the option of changing the registry key so that
            // Monitor can be used alongside MeteorSwath.
            // After this, Monitor will need to be restarted.
            RegistryKey basekey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            RegistryKey hostingkey = basekey.CreateSubKey(@"Software\TTP\Meteor\PrintEngine\HostingMode");
            object monitorhosting = hostingkey.GetValue("Monitor");
            if ( monitorhosting == null || (Int32)monitorhosting != 0 ) 
            {
                MessageBoxResult res =  MessageBox.Show(
                                "An application must host the Meteor PrintEngine in order to use MeteorSwath\n\n" +
                                "Monitor is currently set to host the Meteor PrintEngine\n\n" +
                                "Select 'yes' to change Monitor to run in non-hosting mode\n\n" +
                                "If Monitor is currently running it must be restarted", 
                                "MeteorSwathHelper", 
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Asterisk);
                if (res == MessageBoxResult.Yes) 
                {
                    hostingkey.SetValue("Monitor", (Int32)0);
                } 
                else
                {
                    return false;
                }
            }

            eRET rval = PrinterInterfaceCLS.PiStartPrintEngine(null /* use last set config file */);
            DisplayError("PrinterInterfaceCLS.PiStartPrintEngine", rval);
            if (rval == eRET.RVAL_OK)
            {
                bPrintEngineRunning = true;
            }
            return rval == eRET.RVAL_OK;
        }

        /// <summary>
        /// Stop the Meteor Print Engine from running within the current process.
        /// Normally forceExit should be false.  PiStopPrintEngine will return RVAL_BUSY
        /// if it is not possible to cleanly stop the Meteor Print Engine.  Typically
        /// this will happen if there is still a print job in progress.  If forceExit is 
        /// true the Meteor Print Engine will exit regardless.
        /// </summary>
        /// <param name="forceExit">Set to true to force the Meteor Print Engine to exit even if there is a job in progress</param>
        /// <returns>Success / failure</returns>
        public bool StopPrintEngine(bool forceExit) 
        {
            eRET rval = eRET.RVAL_OK;
            if (bPrintEngineRunning)
            {
                rval = PrinterInterfaceCLS.PiStopPrintEngine(forceExit ? 1 : 0);
                DisplayError("PrinterInterfaceCLS.PiStopPrintEngine", rval);
                bPrintEngineRunning = false;
            }
            return rval == eRET.RVAL_OK;
        }

        /// <summary>
        /// Connect to Meteor Swath and the Meteor PrinterInterface
        /// Applications which using the MeteorSwath API should use MeteorSwathConnect,
        /// rather than the standard SDK method PiOpenPrinter, when connecting to the 
        /// Meter Print Engine
        /// </summary>
        /// <returns>Success (connected) / failure (not connected)</returns>
        public bool Connect()
        {
            eRET rval = MeteorSwathInterface.MeteorSwathConnect();
            DisplayError("MeteorSwathInterface.MeteorSwathConnect", rval);
            IsConnected = (eRET.RVAL_OK == rval || eRET.RVAL_EXISTS == rval);
            return IsConnected;
        }

        /// <summary>
        /// Disconnect from Meteor Swath and the Meteor PrinterInterface
        /// </summary>
        /// <returns>Success (no longer connected) / failure (still connected)</returns>
        public bool Disconnect()
        {
            if (!IsConnected)
            {
                return false;
            }
            eRET rval = MeteorSwathInterface.MeteorSwathDisconnect();
            DisplayError("MeteorSwathInterface.MeteorSwathDisconnect", rval);
            IsConnected = !(eRET.RVAL_OK == rval);
            return !IsConnected;
        }

        /// <summary>
        /// Start a Meteor Swath print job.  An index selects which pre-configured swath geometry to
        /// use, which specifies the interlace pattern.  The width and height of the image to print
        /// allows Meteor Swath to calculate the number of swaths required for the job and the Y
        /// positions for each swath.  A typical print application will then retrieve this information
        /// (using MeteorSwathGetSwathDetails) and pass it on to the motion control sub-system so it can 
        /// determine the sequence of axis movements required for printing.
        /// </summary>
        /// <param name="geomIndex">Index (1-20) of the swath geometry to use for this job</param>
        /// <param name="width">Width of the print job in pixels</param>
        /// <param name="height">Height of the print job in pixels</param>
        /// <param name="bidi">Is the print job unidirectional or bidirectional</param>
        /// <param name="firstRev">Is the first scan in the job </param>
        /// <param name="jobid">Arbitrary value to identify the job.  Used for debug as part of the SimPrint filename</param>
        /// <param name="partial">Whether to use partial buffers for this job</param>
        /// <returns>Success / failure</returns>
        public bool StartOnlineJob(uint geomIndex, int width, int height, bool bidi, bool firstRev, UInt32 jobid, bool partial)
        {
            if (!IsConnected)
            {
                return false;
            }
            eRET rval = MeteorSwathInterface.MeteorSwathStartOnlineJob(geomIndex, partial, (uint)width, (uint)height, bidi, firstRev, jobid);
            DisplayError("MeteorSwathInterface.MeteorSwathStartOnlineJob", rval);
            return (eRET.RVAL_OK == rval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bidi"></param>
        /// <param name="firstRev"></param>
        /// <param name="jobid"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public bool StartAutoJob(AutoSwathGeometry geometry, int width, int height, bool bidi, bool firstRev, UInt32 jobid, bool partial)
        {
            if (!IsConnected)
            {
                return false;
            }
            eRET rval = MeteorSwathInterface.MeteorSwathStartAutoJob(geometry, partial, (uint)width, (uint)height, bidi, firstRev, jobid);
            DisplayError("MeteorSwathInterface.MeteorSwathStartAutoJob", rval);
            return (eRET.RVAL_OK == rval);
        }

        /// <summary>
        /// Find out how many swaths are required to print the current Meteor Swath print job
        /// </summary>
        /// <param name="count">Number of swaths required for the current print job, or -1 if the status request failed</param>
        /// <returns>Success / failure</returns>
        public bool GetSwathCount(out int count)
        {
            count = -1;
            if (!IsConnected)
            {
                return false;
            }
            TSwathSeparatorStatus temp;
            eRET rval = MeteorSwathInterface.MeteorSwathGetStatus(out temp);
            if (rval == eRET.RVAL_OK)
            {
                count = temp.SwathsRequired;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Send the image data for the plane to MeteorSwath
        /// </summary>
        /// <param name="image">Image data for the plane, held in a System.Windows.Media.Imaging.BitmapSource object</param>
        /// <param name="plane">Colour plane to send the data to (indexed from 1)</param>
        /// <param name="xstart">
        /// X position of the image.  Assuming the encoder count increases as the printer carriage moves from left to right, 
        /// this is the left-hand side of the image.
        /// </param>
        /// <returns>Success / failure</returns>
        public bool SendImage(SwathImage image, int plane, int xstart, int ystart = 0)
        {
            if (!IsConnected)
            {
                return false;
            }
            // Find out the Meteor bits per pixel, and use this to
            // format the image data appropriately
            TAppStatus status;
            if (PrinterInterfaceCLS.PiGetPrnStatus(out status) == eRET.RVAL_OK)
            {    
                Bpp = status.BitsPerPixel;
            }
            int[] buffer = image.GetImageBuffer(0, Bpp);
            eRET rval = MeteorSwathInterface.MeteorSwathSendImage(plane, xstart, ystart, image.DocWidth, image.CroppedHeight, false, buffer.Length, buffer);
            DisplayError("MeteorSwathInterface.MeteorSwathSendImage", rval);
            return (eRET.RVAL_OK == rval);
        }

        /// <summary>
        /// Send the same image data for muliple planes to MeteorSwath
        /// </summary>
        /// <param name="image">Image data, held in a System.Windows.Media.Imaging.BitmapSource object</param>
        /// <param name="planebitmask">Colour planes to send the data to.  bit0 set for plane 1 etc.</param>
        /// <param name="xstart">
        /// X position of the image.  Assuming the encoder count increases as the printer carriage moves from left to right, 
        /// this is the left-hand side of the image.
        /// </param>
        /// <returns>Success / failure</returns>
        public bool SendImageMultiplePlanes(SwathImage image, int planebitmask, int xstart, int ystart = 0) 
        {
            if (!IsConnected) 
            {
                return false;
            }
            // Find out the Meteor bits per pixel, and use this to
            // format the image data appropriately
            TAppStatus status;
            if (PrinterInterfaceCLS.PiGetPrnStatus(out status) == eRET.RVAL_OK) 
            {
                Bpp = status.BitsPerPixel;
            }
            int[] buffer = image.GetImageBuffer(0, Bpp);
            eRET rval = MeteorSwathInterface.MeteorSwathSendImageMultiplePlanes(planebitmask, xstart, ystart, image.DocWidth, image.CroppedHeight, false, buffer.Length, buffer);
            DisplayError("MeteorSwathInterface.MeteorSwathSendImage", rval);
            return (eRET.RVAL_OK == rval);
        }


        /// <summary>
        /// Attempt to load a TIFF file directly into MeteorSwath, via the
        /// MeteorSwathSendTIFF method
        /// 
        /// This mechanism is appropriate for applications which:
        /// 
        ///   (1) print the contents of TIFF files directly, without any post-RIP processing, and 
        ///   (2) can fit all TIFF files for a print job in PC memory simultaneously, and
        ///   (3) use strip-based TIFF files at 1,2 or 4 bits-per-pixel (matching the Meteor print resolution)
        ///   
        /// For more complex applications this is not suitable and the MeteorSwathSendImage
        /// mechanism should be used.  For example, partial buffers are necessary where the
        /// image files are too large to fit completely in pc memory.
        /// </summary>
        public bool SendTIFF(string path, int plane, int xstart, int ystart = 0) 
        {
            if (!IsConnected) 
            {
                return false;
            }
            eRET rval = MeteorSwathInterface.MeteorSwathSendTIFF(plane, xstart, ystart, path);
            return rval == eRET.RVAL_OK;
        }

        /// <summary>
        /// As <see cref="SendTIFF"/> but allows the same file to be sent to multiple planes
        /// This helps to optimises buffer memory and disk load times if the same image needs to be
        /// printed by more than one plane in the printer
        /// </summary>
        public bool SendTIFFMultplePlanes(string path, int planebitmask, int xstart, int ystart = 0) 
        {
            if (!IsConnected) 
            {
                return false;
            }
            eRET rval = MeteorSwathInterface.MeteorSwathSendTIFFMultiplePlanes(planebitmask, xstart, ystart, path);
            return rval == eRET.RVAL_OK;
        }

        
        /// <summary>
        /// Called to close a Meteor Swath print job, after the image data has been sent
        /// </summary>
        /// <returns>Success / failure</returns>
        public bool EndJob()
        {
            if (!IsConnected)
            {
                return false;
            }
            eRET rval = MeteorSwathInterface.MeteorSwathEndJob();
            DisplayError("MeteorSwathInterface.MeteorSwathEndJob", rval);
            return (eRET.RVAL_OK == rval);
        }

        /// <summary>
        /// Call into Meteor to set the resolution of print data in bits per pixel 
        /// </summary>
        /// <param name="bpp">Bits per pixel</param>
        /// <returns>Success / failure</returns>
        public bool SetBpp(int bpp)
        {
            if (!IsConnected)
            {
                return false;
            }
            Bpp = bpp;
            eRET rval = PrinterInterfaceCLS.PiSetParam((int)eCFGPARAM.CCP_BITS_PER_PIXEL, bpp);
            DisplayError("PrinterInterfaceCLS.PiSetParam", rval);
            return rval == eRET.RVAL_OK;
        }

        /// <summary>
        /// Send an abort command to Meteor.  This halts printing of the current print
        /// job (if any), and clears out the hardware print data buffers.
        /// </summary>
        /// <returns></returns>
        public bool Abort() 
        {
            if ( !IsConnected )
            {
                return false;
            }
            eRET rval = PrinterInterfaceCLS.PiAbort();
            DisplayError("PrinterInterfaceCLS.PiAbort", rval);
            return rval == eRET.RVAL_OK;
        }

        /// <summary>
        /// Set the meteor home position.  This is typically called when the printer carriage
        /// is in its homing position.  The home position is Meteor's reference point for
        /// printing; all X co-ordinates are relative to the home position.
        /// </summary>
        /// <returns>Success / failure</returns>
        public bool SetMeteorHomePosition() 
        {
            if (!IsConnected) 
            {
                return false;
            }
            eRET rval = PrinterInterfaceCLS.PiSetHome();
            DisplayError("PrinterInterfaceCLS.PiSetHome", rval);
            return rval == eRET.RVAL_OK;
        }

        /// <summary>
        /// Puts up a message box if rval is not RVAL_OK - i.e. a call into Meteor Swath or the
        /// Meteor Printer Interface failed
        /// </summary>
        /// <param name="msg">Message to display, typically the name of the last API method called</param>
        /// <param name="rval">Meteor / Meteor Swath error code</param>
        private void DisplayError(string msg, eRET rval) 
        {
            if (rval != eRET.RVAL_OK) 
            {
                MessageBox.Show(msg + " failed:\n\n" + rval.ToString(), "MeteorSwathHelper");
            }
        }
    }
}
