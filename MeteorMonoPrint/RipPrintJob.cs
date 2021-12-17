namespace Ttp.Meteor.MeteorMonoPrint {
    /// <summary>
    /// Sample class to demonstrate how to RIP-and-print a PDF or RGB TIFF
    /// </summary>
    class RipPrintJob {
        /// <summary>
        /// Full path to the file to processing using the PrintEngine's embedded HHR RIP
        /// </summary>
        private readonly string _fileToRip;
        /// <summary>
        /// Name of the RIP configuration file in the RIP's SW\TestConfig directory, or a full path to the RIP configuration
        /// </summary>
        private readonly string _ripConfig;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileToRip">Full path to the file to processing using the PrintEngine's embedded HHR RIP</param>
        /// <param name="ripConfig">Name of the RIP configuration file in the RIP's SW\TestConfig directory, or a full path to the RIP configuration</param>
        public RipPrintJob(string fileToRip, string ripConfig) {
            _fileToRip = fileToRip;
            _ripConfig = ripConfig;
        }

        /// <summary>
        /// Run the print job
        /// </summary>
        public eRET RunJob() {
            eRET rVal;

            //
            // The document width is used for (a) setting the product detect lockout, and (b) positioning images when
            // the media is moving from left-to-right with respect to the print heads (because the right hand side of
            // the image is printed first).  Alternatively, we can use the right align flag.
            //
            int jobType = (int)eJOBTYPE.JT_FIFO | (int)MeteorConsts.JT_FLAG_RIGHTALIGN;
            
            //
            // Meteor command to start a print job
            //
            int[] startJobCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTJOB,  // Command Id
                4,                              // Number of DWORD parameters
                999,                            // Job Id
                jobType,                        // This job uses the preload data path
                (int)eRES.RES_HIGH,             // Print at full resolution
                0                               // Document width (not used here)
            };

            //
            // A start job command can fail if there is an existing print job
            // ready or printing in Meteor, or if a previous print job is still
            // aborting.  The sequencing of the main form's control enables should 
            // guarantee that this never happens in this application.
            //
            rVal = PrinterInterfaceCLS.PiSendCommand(startJobCmd);
            if (rVal != eRET.RVAL_OK) {
                return rVal;
            }

            //
            // Fill in the PCMD_RIPIMAGE parameters
            //
            int[] ripCmd = new int[32];
            ripCmd[0] = (int)CtrlCmdIds.PCMD_RIPIMAGE;  // Command Id
            ripCmd[1] = 32;                             // Number of DWORD parameters
            // Indexes 2-5 are filled in by PiSendRipImageCommandHelper
            ripCmd[6] = 1;                              // Colour plane for the first channel of the RIPPed image
            ripCmd[7] = 1;                              // Xleft (for Right-to-Left printing) or XRight (for Left-to-Right printing)
            ripCmd[8] = 0;                              // YTop
            ripCmd[9] = 0;                              // Gap between images for continuous printing (of a multi-page PDF)
            ripCmd[10] = 0;                             // First page ...
            ripCmd[11] = 0;                             // ... and last page.  Zero means print all pages
            ripCmd[12] = 0;                             // Control flags (e.g. to mirror the image, whether this is FIFO or Preload document)
            ripCmd[13] = 1000;                          // Doc Id for the first page (incremented for subsequent pages if required)
            ripCmd[14] = 0;                             // Number of copies (for preload mode)
            // All other parameters are reserved for future use

            //
            // Start doc / end doc commands are not needed when PCMD_RIPIMAGE is being used; they are created automatically.
            // This allows a multi-page PDF to be RIPPed to individual documents with the one call.
            // 
            rVal = PrinterInterfaceCLS.PiSendRipImageCommandHelper(ripCmd, _fileToRip, _ripConfig);
            if (rVal != eRET.RVAL_OK) {
                return rVal;
            }

            //
            // End the print job
            //
            int[] endJobCmd = { (int)CtrlCmdIds.PCMD_ENDJOB, 0 };
            rVal = PrinterInterfaceCLS.PiSendCommand(endJobCmd);

            return rVal;
        }
    }
}
