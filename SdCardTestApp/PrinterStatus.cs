using System;
using System.Threading;

using Ttp.Meteor;

namespace MeteorInkJet.SdCardTestApp {
    public enum PRINTER_STATUS {
        /// <summary>
        /// Failed to start the Meteor PrintEngine.  This will happen if the Meteor PrintEngine
        /// is being hosted in another process.
        /// </summary>
        HOSTING_FAILED,
        /// <summary>
        /// Meteor is not connected.  This will happen if another application is currently connected 
        /// to the Meteor PrintEngine.
        /// </summary>
        DISCONNECTED,
        /// <summary>
        /// Meteor is connected and waiting for hardware
        /// </summary>
        WAITING_FOR_PCC,
        /// <summary>
        /// Meteor is initialising the PCC hardware
        /// </summary>
        INITIALISING,
        /// <summary>
        /// Meteor is inactive
        /// </summary>
        IDLE,
        /// <summary>
        /// The print data for the next print job is being loaded into the hardware buffers
        /// </summary>
        LOADING,
        /// <summary>
        /// The current print job is aborting
        /// </summary>
        ABORTING,
        /// <summary>
        /// The print data for the next print job is loaded and Meteor is waiting for an external
        /// product detect signal to start printing
        /// </summary>
        READY,
        /// <summary>
        /// Meteor is printing
        /// </summary>
        PRINTING,
        /// <summary>
        /// Meteor is in an error condition
        /// </summary>
        ERROR
    }

    /// <summary>
    /// Object to deal with connecting to Meteor and returning the current Meteor
    /// status.  For simplicity this object is designed to be called periodically 
    /// from a timer running in the main application form.
    /// 
    /// [ For more complex print applications it may be better to deal with the status 
    ///   polling in an asynchronous thread. ]
    ///   
    /// </summary>
    class PrinterStatus {
        /// <summary>
        /// Number of pccs required for the system as defined in the Meteor .cfg file
        /// </summary>
        public int PccsRequired { get; private set; }
        /// <summary>
        /// Is the Meteor PrintEngine started in this application process
        /// </summary>
        public bool IsHosting { get; private set; }
        /// <summary>
        /// Is the connection to the Meteor Printer Interface currently open
        /// </summary>
        public bool Connected { get; private set; }
        /// <summary>
        /// Bitmask indicating the bits per pixel values that Meteor can use for
        /// the current head type.  BIT1 (0x2) for 1bpp, BIT2 (0x4) for 2bpp, etc.
        /// </summary>
        public Int32 SupportedBppBitmask { get; private set; }
        /// <summary>
        /// The current value of the Meteor bits per pixel
        /// </summary>
        public Int32 MeteorBitsPerPixel { get; private set; }
        /// <summary>
        /// False if Meteor is in the process of turning head power on or off
        /// </summary>
        public bool HeadPowerIdle { get; private set; }
        /// <summary>
        /// Is head power enabled for at least one head in the system
        /// </summary>
        public bool HeadPowerEnabled { get; private set; }
        /// <summary>
        /// The size in 512 byte blocks of the UHS SD card if present; zero if the cards have not been detected.
        /// </summary>
        public UInt32 SdCardSizeBlocks { get; private set; }
        /// <summary>
        /// Is simulated flash memory being used; this is a PC-disk backed store of the data which is normally written
        /// to the PCC flash card, which allows SimPrint files to be created when an image is printed.
        /// </summary>
        public bool IsSimFlash { get; private set; }
        /// <summary>
        /// Is a write in progress on any of the SD cards 
        /// </summary>
        public bool SdCardWriteBusy { get; private set; }

        /// <summary>
        /// Tick count when we last attempted to start the Meteor PrintEngine / open the 
        /// Meteor Printer Interface
        /// </summary>
        private int _tickLastConnectAttempt = Environment.TickCount - CONNECT_INTERVAL_MS;
        /// <summary>
        /// Interval between attempts to start the Meteor PrintEngine and open the Printer Interface
        /// </summary>
        private const int CONNECT_INTERVAL_MS = 3000;

        /// Protect against brief transitions to MPS_IDLE
        private ePRINTERSTATE _lastMeteorState = ePRINTERSTATE.MPS_DISCONNECTED;
        private const int IDLE_CONF = 4;
        private int _idleConfCount = 0;

        /// <summary>
        /// Disconnect from the Meteor Printer Interface if currently connected.
        /// Stops the Meteor Print Engine if currently running.
        /// Should be called before the application exits.
        /// </summary>
        public void Disconnect() {
            if (Connected) {
                PrinterInterfaceCLS.PiAbort();
                WaitNotBusy();
                PrinterInterfaceCLS.PiClosePrinter();
                Connected = false;
            }
            if (IsHosting) {
                // Sending a 1 as the 'force' parameter forces a clean exit of the PrintEngine
                // even if there is an open connection to the PrintEngine (e.g. if the call
                // to PiClosePrinter above was missing).
                //
                // Normally 0 should be sent, but 1 can be useful during development.
                //
                PrinterInterfaceCLS.PiStopPrintEngine(1);
                IsHosting = false;
            }
        }
    
        /// <summary>
        /// Attempts to start the Meteor PrintEngine and establish a connection every 2 seconds, if the 
        /// application is not currently connected.
        /// Then returns the Meteor status.
        /// </summary>
        /// <returns></returns>
        public PRINTER_STATUS GetStatus() {
            TAppStatus AppStatus;
            eRET rVal;

            if (!Connected || !IsHosting) {
                // Only attempt connection every three seconds
                if (Environment.TickCount < _tickLastConnectAttempt + CONNECT_INTERVAL_MS) {
                    return !IsHosting ? PRINTER_STATUS.HOSTING_FAILED : PRINTER_STATUS.DISCONNECTED;
                }
                _tickLastConnectAttempt = Environment.TickCount;

                if (!IsHosting) {
                    // Attempt to start the Meteor PrintEngine.
                    //
                    // Sending null as the configFile parameter tells the PrintEngine to look in the
                    // registry for the path to the last configuration file which it used.
                    //
                    // Alternatively, a full path to a configuration file can be passed.
                    //
                    // If this fails with RVAL_ENGINE_RUNNING, it means that there is another application
                    // running which is already hosting the PrintEngine, such as Monitor.
                    //
                    // Monitor hosts the PrintEngine by default; this behaviour can be changed via the
                    // following registry keys:
                    //
                    // [HKEY_CURRENT_USER\Software\TTP\Meteor\PrintEngine\HostingMode]
                    //     "Monitor"=dword:00000000
                    //     "ShowMonitorSelect"=dword:00000001
                    //
                    rVal = PrinterInterfaceCLS.PiStartPrintEngine(null);
                    if (rVal != eRET.RVAL_OK) {
                        return PRINTER_STATUS.HOSTING_FAILED;
                    }
                    IsHosting = true;
                }
                // 
                rVal = PrinterInterfaceCLS.PiOpenPrinter();
                // RVAL_OK means the Meteor Printer Interface was successfully opened.
                //
                // RVAL_EXISTS means that the Printer Interface is already open in this
                // process.
                //
                // All other return values mean we have failed to open the printer 
                // interface - the most likely reason is that the Meteor Printer Interface 
                // is already open in another print application.
                //
                if (rVal != eRET.RVAL_OK && rVal != eRET.RVAL_EXISTS) {
                    return PRINTER_STATUS.DISCONNECTED;
                }
                Connected = true;
                // Send an abort command in case the application which previously used
                // Meteor left a print job running.  This is not done if Meteor is
                // currently initialising the hardware, as in this case it should 
                // not be necessary, and can take a significant amount of time to complete.
                rVal = PrinterInterfaceCLS.PiGetPrnStatus(out AppStatus);
                if (rVal == eRET.RVAL_OK && AppStatus.PrinterState != ePRINTERSTATE.MPS_INITIALIZING) {
                    PrinterInterfaceCLS.PiAbort();
                }
                // Wait until the abort has completed.  While an abort is in progress
                // the printer interface is busy, and status requests will fail.
                WaitNotBusy();
            }

            // Request the Meteor printer status.  In rare situations it is possible
            // for this to return RVAL_BUSY, in which case the application should
            // retry.
            //
            do {
                rVal = PrinterInterfaceCLS.PiGetPrnStatus(out AppStatus);
            } while ( rVal == eRET.RVAL_BUSY );

            // A fatal error implies the PrintEngine has stopped (which can't happen in
            // this application, as we are hosting).
            //
            if ( rVal != eRET.RVAL_OK ) {
                if (Connected) {
                    PrinterInterfaceCLS.PiClosePrinter();
                    Connected = false;
                }
                return PRINTER_STATUS.DISCONNECTED;
            }

            // Store the bitmask indicating which bits-per-pixel are valid for the current head type
            SupportedBppBitmask = AppStatus.SupportedBppBitmask;
            // The current bits per pixel value
            MeteorBitsPerPixel = AppStatus.BitsPerPixel;

            
            // Find out if the SD cards are fitted, or if we're using simulated flash memory (to allow SimPrint files), which
            // doesn't required PCC hardware to be connected
            GetSdCardStatus();

            // The Meteor Print Engine state goes into IDLE after all connected PCCs have finished 
            // initialising.
            //
            // Here we trap the case of the Meteor configuration file listing more PCCs than are
            // currently attached.
            //
            PccsRequired = AppStatus.PccsRequired;
            if (PccsRequired != AppStatus.PccsAttached) {
                return PRINTER_STATUS.WAITING_FOR_PCC;
            }
            // Protect against the application acting on a brief status transition to IDLE
            ePRINTERSTATE PrinterState = AppStatus.PrinterState;
            if (AppStatus.PrinterState == ePRINTERSTATE.MPS_IDLE) {
                if (_idleConfCount < IDLE_CONF) {
                    PrinterState = _lastMeteorState;
                    _idleConfCount++;
                }
            } else {
                _idleConfCount = 0;
            }
            // Track last known stable printer state
            _lastMeteorState = PrinterState;
            // Find out whether Meteor is currently turning head power on or off
            CheckHeadPowerIdle();
            // Is at least one head powered on
            HeadPowerEnabled = (AppStatus.HeadPowerState == eHEADPOWERSTATE.HPS_HEAD);
            // Convert from the Meteor printer status into the application status
            return GetPrinterStatus(PrinterState);
        }

        private PRINTER_STATUS GetPrinterStatus(ePRINTERSTATE State) {
            switch (State) {
                // There is no hardware connected.
                case ePRINTERSTATE.MPS_DISCONNECTED:
                    return PRINTER_STATUS.WAITING_FOR_PCC;
                // The PCC hardware has connected to the Print Engine but has not yet been
                // initialised
                case ePRINTERSTATE.MPS_CONNECTED:
                    return PRINTER_STATUS.INITIALISING;
                // The Print Engine is initialised and waiting for a print job to start
                case ePRINTERSTATE.MPS_IDLE:
                    return PRINTER_STATUS.IDLE;
                // The next print job is ready to start
                case ePRINTERSTATE.MPS_READY:
                    return PRINTER_STATUS.READY;
                // Meteor is actively printing
                case ePRINTERSTATE.MPS_PRINTING:
                    return PRINTER_STATUS.PRINTING;
                // The Print Engine is downloading FPGA code to the PCCs    
                case ePRINTERSTATE.MPS_INITIALIZING:
                    return PRINTER_STATUS.INITIALISING;
                // The PCC FPGA has completed programming and is starting up
                case ePRINTERSTATE.MPS_STARTUP:
                    return PRINTER_STATUS.INITIALISING;
                // Fault condition - more information can be found by looking at the
                // values in the status registers.  For the purposes of this application
                // we just report the generic error; the register values can be viewed
                // in Monitor
                case ePRINTERSTATE.MPS_FAULT:
                    return PRINTER_STATUS.ERROR;
                // Image data is being loaded
                case ePRINTERSTATE.MPS_LOADING:
                    return PRINTER_STATUS.LOADING;
                // Should never get here
                default:
                    return PRINTER_STATUS.ERROR;
            }
        }

        /// <summary>
        /// Polls the Printer Interface until it finishes the previous control command
        /// Use timeoutms to specify how long to wait in ms, default is 10000ms.
        /// For example, the Printer Interface will remain busy after a
        /// PiAbort until the abort has completed in Meteor.
        /// Note that the interface does not become busy as a result of a call to
        /// PiSendCommand.
        /// </summary>
        /// <param name="timeoutms">Timeout in milliseconds.  Defaults to ten seconds.</param>
        public bool WaitNotBusy(int timeoutms = 10000) {
            while (PrinterInterfaceCLS.PiIsBusy()) {
                Thread.Sleep(100);
                if ((timeoutms -= 100) <= 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether any of the PCCs in the system are in the process of turning head
        /// power on or off.  Should only be called when all required PCCs are connected.
        /// Sets the value of HeadPowerIdle.
        /// </summary>
        private void CheckHeadPowerIdle() {
            for (int pccNum = 1; pccNum <= PccsRequired; pccNum++) {
                TAppPccStatus pccStatus;
                eRET rVal = PrinterInterfaceCLS.PiGetPccStatus(pccNum, out pccStatus);
                if (rVal != eRET.RVAL_OK) {
                    HeadPowerIdle = false;
                    return;
                }
                if ((pccStatus.bmStatusBits2 & Bmps.BMPS2_HEAD_POWER_IN_PROGRESS) != 0) {
                    HeadPowerIdle = false;
                    return;
                }
            }
            HeadPowerIdle = true;
        }

        private void GetSdCardStatus() {
            // There are several "image store" mechanisms.  The first one was the
            // Compact Flash card on the PCC8 (hence the name).  This has subsequently
            // been extended to cover other means of storing images, including the
            // PCCE UHS SD card.  
            //
            // There is also the PC disk based virtual / simulated Compact Flash,
            // which can be useful for development (e.g. it can generate SimPrint
            // files, unlike the PCC based image stores), although for larger
            // systems its performance can be quite slow.
            //
            TCompactFlashStatus cfStatus;
            eRET rVal;
            do {
                rVal = PrinterInterfaceCLS.PiGetCompactFlashStatus(out cfStatus);
            } while ( rVal == eRET.RVAL_BUSY );

            if (rVal == eRET.RVAL_OK) {
                bool allPresent = true;
                bool anyBusy = false;
                for (int pcc = 0; pcc < PccsRequired; pcc++) {
                    int pccStatus = cfStatus.Status[pcc];
                    if ((pccStatus & MeteorConsts.CFPRESENT) == 0) {
                        allPresent = false;
                    }
                    if (cfStatus.CfTranslateDocumentRxCount != 0) {
                        if (cfStatus.CfTranslateProcessedPercent != 100 ||
                            cfStatus.CfStoreImageLoadedPercent != 100) {
                            anyBusy = true;
                        }
                    }
                    if ((pccStatus & MeteorConsts.CFWRITE_IN_PROGRESS) != 0) {
                        anyBusy = true;
                    }
                }
                if (allPresent || cfStatus.SimFlash != 0) {
                    SdCardSizeBlocks = cfStatus.MinSizeBlocks;
                    IsSimFlash = (cfStatus.SimFlash != 0);
                } else {
                    SdCardSizeBlocks = 0;
                    IsSimFlash = false;
                }
                SdCardWriteBusy = anyBusy;
            } else {
                SdCardSizeBlocks = 0;
                SdCardWriteBusy = false;
            }
        }
    }
}
