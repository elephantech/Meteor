using System;
using System.Windows.Forms;

using Ttp.Meteor;

namespace MeteorInkJet.SdCardTestApp {
    public partial class ImageControl : UserControl {
        /// <summary>
        /// Has the image been loaded to PCCE DRAM
        /// </summary>
        private bool _imageLoadedToSdCard;
        /// <summary>
        /// Interface to retrieve global print job details, such as bits per pixel
        /// </summary>
        private IPrintDetails _printDetails;
        /// <summary>
        /// Delegate for sending a standard FIFO image from the PC, when mixed mode is enabled.
        /// Calling this delegate has no effect when running in standard SD card mode.
        /// </summary>
        private Action _sendFifoImage;
        /// <summary>
        /// Incrementing document ID
        /// </summary>
        private int _lastDocID = 1;
        
        /// <summary>
        /// Zero based index of this image control.  This is used to select the SD card slot.
        /// </summary>
        public int ImageIndex { get; set; }
               
        /// <summary>
        /// Constructor
        /// </summary>
        public ImageControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Set the interface which the control can use to find out print job details, such as bits per pixel,
        /// and a delegate which can be used to send variable data in a mixed mode (dual FIFO) job
        /// </summary>
        public void SetPrintDetails(IPrintDetails PrintDetails, Action sendFifoImage) {
            _printDetails = PrintDetails;
            _sendFifoImage = sendFifoImage;
        }

        /// <summary>
        /// Y top position of the image.  This is fixed at the point where the image is loaded to the PCCE.
        /// </summary>
        private int UserYTop {
            get {
                return (int)numericUpDownYTop.Value;
            }
        }

        /// <summary>
        /// Number of copies of the image requested
        /// </summary>
        private int UserImageCopies {
            get {
                return (int)numericUpDownCopies.Value;
            }
        }

        /// <summary>
        /// X start position of the image
        /// </summary>
        private int UserXStart {
            get {
                return (int)numericUpDownXStart.Value;
            }
        }

        /// <summary>
        /// Load the user application settings for this control
        /// </summary>
        public void LoadSettings() {
            Properties.Settings s = Properties.Settings.Default;

            if (s.YTop != null && s.YTop.Length > ImageIndex) {
                numericUpDownYTop.Value = s.YTop[ImageIndex];
            }
            if (s.Copies != null && s.Copies.Length > ImageIndex) {
                numericUpDownCopies.Value = s.Copies[ImageIndex];
            }
            if (s.ImageFileName != null && s.ImageFileName.Count > ImageIndex) {
                imageFileControl.RestoreImage(s.ImageFileName[ImageIndex]);                
            }
            if (s.XStart != null && s.XStart.Length > ImageIndex) {
                numericUpDownXStart.Value = s.XStart[ImageIndex];
            }
        }

        /// <summary>
        /// Save the user application settings for this control
        /// </summary>
        public void SaveSettings() {
            // This assumes that the caller has cleared the StringCollection, and is calling the
            // ImageControl objects in order
            Properties.Settings.Default.ImageFileName.Add(imageFileControl.FileName);
            // This assumes that the caller has created a new array of the correct size
            Properties.Settings.Default.YTop[ImageIndex] = (int)numericUpDownYTop.Value;
            Properties.Settings.Default.Copies[ImageIndex] = numericUpDownCopies.Value;
            Properties.Settings.Default.XStart[ImageIndex] = (int)numericUpDownXStart.Value;
        }

        public void EnableControls() {
            groupBoxLoad.Enabled = _printDetails.SdCardsPresent && imageFileControl.Image != null;
            groupBoxPrint.Enabled = _imageLoadedToSdCard && _printDetails.JobActive;
        }

        public void InvalidateSdCardImages() {
            _imageLoadedToSdCard = false;
            EnableControls();
        }

        private void ButtonSelectFile_Click(object sender, EventArgs e) {
            imageFileControl.SelectImage();
            EnableControls();
        }

        private void ButtonLoadImage_Click(object sender, EventArgs e) {
            if (imageFileControl.Image == null) {
                return;
            }

            eRET rVal; 

            // Find out the number of configured colour planes
            //
            // Normally an application will not need to do this; it's done here so that the
            // sample code doesn't have to assume how many planes there are in the
            // configuration file
            //
            MeteorPlaneConfig planeConfig;
            do {
                rVal = PrinterInterfaceCLS.PiGetPlaneConfig(out planeConfig);
            } while ( rVal == eRET.RVAL_BUSY );

            if ( rVal != eRET.RVAL_OK ) {
                MessageBox.Show("PiGetPlaneConfig failed: " + rVal);
                return;
            }

            do {
                // Send the same image to each colour plane in the system.  Note that here
                // we assume that the mapped plane indexes are contiguous (which is not
                // necessarily the case)
                //
                for (int planeId = 0; planeId < planeConfig.UsedPlaneCount && rVal == eRET.RVAL_OK; planeId++) {
                    // Load the image into a Meteor buffer ID.
                    //
                    // This loads the print data into memory from the source file, performs an (arbitrary) 
                    // grey level scaling, and copies the data into the Meteor print buffer.  This
                    // mechanism will attempt to print any file which can be loaded by .NET, but may
                    // lose resolution, and is intended to demonstrate how to fill an image buffer.
                    // A real application will probably already have the print data at the correct
                    // bit depth.
                    //
                    // If TIFF files are being used, PiAllocateTiffImageBuffer can be used instead.
                    //
                    // Note that MetCal features such as NozzleFix can alter the contents of the image buffer.
                    // As we're sending the same image to each plane, we therefore need to reload the image data for each plane in turn.
                    //
                    UInt32 bufId = imageFileControl.Image.FillMeteorImageBuffer(_printDetails.UserBitsPerPixel);

                    // Meteor planes are indexed from one
                    //
                    int meteorPlane = 1 + planeId;
                    
                    // The top bit of meteorPlane can be set to free the buffer memory
                    // automatically after use.
                    //
                    meteorPlane |= unchecked((int)0x80000000);

                    // The width in pixels of the image
                    //
                    int width = imageFileControl.Image.GetImageWidth();
                    
                    // Fill width; normally this is the same as the image width.
                    // It can be larger to add padding whitespace after the image, but
                    // normally this is not necessary, as the print width in the 
                    // PCMD_CF_HDIMAGE_BLK command should match the written image size.
                    //
                    int fillWidth = imageFileControl.Image.GetImageWidth();

                    // Command to load and translate the image into the form needed by the hardware
                    //
                    int[] translateCmd = new int[] { 
                        (int)CtrlCmdIds.PCMD_CF_TRANSLATEIMAGE, 
                        5, 
                        meteorPlane, 
                        UserYTop, 
                        width, 
                        (int)bufId, 
                        fillWidth 
                    };

                    // Send the command to Meteor
                    //
                    // [ Note that the RVAL_FULL condition is very unlikely here, as the image data 
                    //  has already been loaded synchronously into an image buffer, but it's good 
                    //  practice to check for it anyway ]
                    //
                    do {
                        rVal = PrinterInterfaceCLS.PiSendCommand(translateCmd);
                    } while (rVal == eRET.RVAL_FULL);
                }
                
                if (rVal != eRET.RVAL_OK) {
                    break;
                }

                // If the buffer(s) aren't being freed as we go along, a dedicated PCMD_CF_TRANSLATEIMAGE
                // must be used to free the buffer memory.  Normally this is done by setting the
                // top bit of the 'plane' parameter when sending the image data above.
                //
                /*
                int[] freeMemCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_CF_TRANSLATEIMAGE,
                    5,
                    0,          // Zero plane means free buffer memory
                    0,
                    0,
                    (int)bufId, // Buffer ID to free
                    0
                };

                // Send the free memory command to the PrintEngine
                //
                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(freeMemCmd);
                } while (rVal == eRET.RVAL_FULL);

                if (rVal != eRET.RVAL_OK) {
                    break;
                }
                */

                // Now that the print data for each plane has been loaded into PC memory, it can be stored
                // to the PCCE SD card (or PC-disk backed "virtual" flash memory for SimPrint testing)
                //
                int[] storeCmd = new int[] { 
                    (int)CtrlCmdIds.PCMD_CF_STOREIMAGE, 
                    8, 
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 0),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 1),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 2),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 3),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 4),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 5),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 6),
                    SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, 7),
                    };

                // Send the store command to the PrintEngine
                //
                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(storeCmd);
                } while (rVal == eRET.RVAL_FULL);

                if (rVal != eRET.RVAL_OK) {
                    break;
                }

            } while (false) ;

            if (rVal != eRET.RVAL_OK) {
                MessageBox.Show("Failed to send print commands: " + rVal);
            } else {
                _imageLoadedToSdCard = true;
            }
        }

        private void ButtonPrint_Click(object sender, EventArgs e) {
            if (!_imageLoadedToSdCard) {
                return;
            }
            eRET rVal = eRET.RVAL_OK;

            //
            // Send a START DOC / IMAGE COMMANDs / END DOC sequence for each copy of the image
            //
            int loopCount = UserImageCopies;
            for (int i = 0; i < loopCount; i++) {

                // ----------------------------------------------------------- //
                //
                // Start document command.  Use an arbitrary document ID.
                // 
                int docID = _lastDocID + (ImageIndex * 1000);
                _lastDocID++;
                int[] StartDocCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_STARTFDOC,
                    1,
                    docID};

                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(StartDocCmd);
                } while (rVal == eRET.RVAL_FULL);

                if (rVal != eRET.RVAL_OK) {
                    break;
                }
                // ----------------------------------------------------------- //

                // ----------------------------------------------------------- //
                // 

                // Find out how many PCC cards are in use.  Normally an application
                // will already know this, so can avoid asking for the status at this
                // point.
                //
                TAppStatus appStatus;
                do {
                    rVal = PrinterInterfaceCLS.PiGetPrnStatus(out appStatus);
                } while ( rVal == eRET.RVAL_BUSY );

                // The width we're printing should match the width of the image written to
                // the SD card.  This means there is no need to fill the remainder of the
                // SD card slot with zeros.
                //
                int WidthPixels = imageFileControl.Image.GetImageWidth();

                // Print CF image commands.  
                //
                // We need to send a command for each head on each PCC.
                //
                for ( int pcc = 0; pcc < appStatus.PccsRequired && rVal == eRET.RVAL_OK; pcc++ ) {
                    for (int hdc = 0; hdc < MeteorConsts.MAX_HDCS_PER_PCC && rVal == eRET.RVAL_OK; hdc++) {

                        // Here we ask Meteor whether each head is in use.  Normally
                        // an application will already know which heads are being used,
                        // so can avoid this step.
                        //
                        TAppHeadStatus headStatus;
                        do {
                            rVal = PrinterInterfaceCLS.PiGetHeadStatus(pcc+1, hdc+1, out headStatus);
                        } while ( rVal == eRET.RVAL_BUSY );

                        if ( rVal != eRET.RVAL_OK || (headStatus.bmStatusBits & Bmhs.BMHS_HEAD_REQUIRED) == 0 ) {
                            continue;
                        }
                        
                        // Form the print command for the HDC
                        //
                        int[] PrintCmd = new int[] {
                            (int)CtrlCmdIds.PCMD_CF_HDIMAGE_BLK,
                            5, 
                            pcc+1,
                            hdc+1,
                            UserXStart,
                            WidthPixels,
                            SdCardSlot.Instance.GetHdcBlockAddress(ImageIndex, hdc)
                        };

                        // Send the print command to the PrintEngine
                        //
                        do {
                            rVal = PrinterInterfaceCLS.PiSendCommand(PrintCmd);
                        } while (rVal == eRET.RVAL_FULL);

                        // In mixed (dual-fifo) mode, image data can be sent directly from the PC and
                        // combined with the SD card data when the document is printed.  Calling this
                        // delegate has no effected if mixed mode is not enabled.
                        //
                        if (_sendFifoImage != null) {
                            _sendFifoImage();
                        }
                    }
                }
                // ----------------------------------------------------------- //
                if (rVal != eRET.RVAL_OK) {
                    break;
                }

                // ----------------------------------------------------------- //
                //
                // End document command
                //
                int[] EndDocCmd = { (int)CtrlCmdIds.PCMD_ENDDOC, 0 };
                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(EndDocCmd);
                } while (rVal == eRET.RVAL_FULL);

                if (rVal != eRET.RVAL_OK) {
                    break;
                }
                // ----------------------------------------------------------- //
            }
            
            if (rVal != eRET.RVAL_OK) {
                MessageBox.Show("Failed to send print commands: " + rVal);
            }

        }

    }
}
