namespace Ttp.Meteor.MeteorMonoPrint {
    public enum REPEAT_MODE {
        /// <summary>
        /// Multiple copies of the image will be joined with a zero pixel gap
        /// 
        /// One product detect signal will initiate the printing of all copies of
        /// the image within the print job
        /// </summary>
        SEAMLESS = 0,
        /// <summary>
        /// Each copy of the image will require its own product detect signal
        /// 
        /// The minimum gap between product detects must leave sufficient margin
        /// for the image plus head X-direction span
        /// </summary>
        DISCRETE = 1
    }

    /// <summary>
    /// Sample class to start a pre-load print job comprising a number of repeats of a single
    /// image
    /// 
    /// The repeats are either continuous with zero-pixel gap (which requires a single product
    /// detect to print all copies of the image) or discrete, where a product signal is required
    /// for each copy of the image.
    /// 
    /// In discrete mode there is a minimum gap between image copies of the head Y direction span
    /// 
    /// This example uses the Meteor PCMD_BIGIMAGE command.  This is similar to the standard
    /// PCMD_IMAGE; PCMD_BIGIMAGE must be used if the application needs to pass in more than
    /// 60MB of image data in one command.  
    /// 
    /// The only difference in the parameters is that PCMD_BIGIMAGE includes the image height.
    /// </summary>
    class PreLoadPrintJob {
        private readonly int _bpp;
        private IMeteorImageData _image;
        private readonly int _ytop;
        private readonly int _copies;
        private readonly REPEAT_MODE _repeatmode;
        private readonly int _jobid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bpp">Bits per pixel</param>
        /// <param name="image">Image to print</param>
        /// <param name="yTop">Y position for the image</param>
        /// <param name="copies">Number of copies of the image to be printed</param>
        /// <param name="repeatMode">Repeat images seamlessly (zero gap) or on demand (individual product detects)</param>
        /// <param name="jobId">Meteor job ID</param>
        public PreLoadPrintJob(int bpp, IMeteorImageData image, int yTop, int copies, REPEAT_MODE repeatMode, int jobId) {
            _bpp        = bpp;
            _image      = image;
            _ytop       = yTop;
            _copies     = copies;
            _repeatmode = repeatMode;
            _jobid      = jobId;
        }

        /// <summary>
        /// Send the print job to Meteor.  When the method returns, all print data has been
        /// sent to Meteor - however, it has not necessarily all been sent to the hardware.
        /// </summary>
        /// <returns>Success / failure</returns>
        public eRET Start() {
            eRET rVal;
            // Meteor command to start a print job
            int[] startJobCmd = new int[] { 
                (int)CtrlCmdIds.PCMD_STARTJOB,  // Command ID
                4,                              // Number of DWORD parameters
                _jobid,                         // Job ID
                (int)eJOBTYPE.JT_PRELOAD,       // This job uses the preload data path
                (int)eRES.RES_HIGH,             // Print at full resolution
                _image.GetDocWidth()+2          // Needed for Left-To-Right printing only
            };

            // A start job command can fail if there is an existing print job
            // ready or printing in Meteor, or if a previous print job is still
            // aborting.  The sequencing of the main form's control enables should 
            // guarantee that this never happens in this application.
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(startJobCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            // The start document command specifies the number of discrete copies
            // of the image which are required
            //
            int[] startDocCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTPDOC, // Command ID
                1,                              // DWORD parameter count
                _repeatmode == (REPEAT_MODE.DISCRETE) ? _copies : 1 
            };
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(startDocCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            // For seamless image repeats using the preload data path, PCMD_REPEAT
            // must be sent after PCMD_STARTPDOC and before the image data.
            //
            if (_copies > 1 && _repeatmode == REPEAT_MODE.SEAMLESS) {
                int[] RepeatCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_REPEAT,    // Command ID
                    1,                              // DWORD parameter count
                    _copies
                };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(RepeatCmd)) != eRET.RVAL_OK) {
                    return rVal;
                }
            }
            // PCMD_BIGIMAGE must be used if the application needs to pass images 
            // which exceed 60MB in size to Meteor as one buffer.  (An alternative
            // is for the application to split up the data into smaller images,
            // each of which can used PCMD_IMAGE).
            //
            // The image data is sent through the Printer Interface to the Meteor
            // Print Engine in chunks.  The application must continually call 
            // PiSendCommand with the same buffer while the Print Engine
            // returns RVAL_FULL.
            //
            // Note that it is necessary to fix the location of the image command
            // in memory while carrying out this sequence, to prevent the garbage
            // collector from relocating the buffer (theoretically possible, but 
            // highly unlikely) between successive PiSendCommand calls.
            //
            int[] imageCmd = _image.GetBigImageCommand(_ytop, _bpp);
            unsafe {
                fixed (int* pImageCmd = imageCmd) {
                    do {
                        rVal = PrinterInterfaceCLS.PiSendCommand(imageCmd);
                    } while (rVal == eRET.RVAL_FULL);
                    if (rVal != eRET.RVAL_OK) {
                        return rVal;
                    }
                }
            }
            int[] endDocCmd = new int[] {(int)CtrlCmdIds.PCMD_ENDDOC, 0};
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(endDocCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            int[] endJobCmd = new int[] {(int)CtrlCmdIds.PCMD_ENDJOB, 0};
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(endJobCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            return eRET.RVAL_OK;
        }
    }
}
