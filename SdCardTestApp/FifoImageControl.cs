using System;
using System.Windows.Forms;

using Ttp.Meteor;

namespace MeteorInkJet.SdCardTestApp {
    /// <summary>
    /// Control which allows a FIFO image to be selected for use with mixed mode
    /// </summary>
    public partial class FifoImageControl : UserControl {
        /// <summary>
        /// ID of the Meteor Image Buffer we have loaded the data to.
        /// Normally this mechanism would not be used for FIFO data (as each image is different); it is used
        /// in this sample code for simplicity.
        /// </summary>
        private UInt32 _imageBufferId = MeteorConsts.IMG_BUF_UNSET;
        /// <summary>
        /// Interface to retrieve global print job details, such as bits per pixel
        /// </summary>
        private IPrintDetails _printDetails;

        /// <summary>
        /// Constructor
        /// </summary>
        public FifoImageControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Is mixed mode enabled
        /// </summary>
        public bool EnableMixedMode { get { return checkBoxEnableMixedMode.Checked; } }

        /// <summary>
        /// Set the interface which the control can use to find out print job details, such as bits per pixel
        /// </summary>
        public void SetPrintDetails(IPrintDetails PrintDetails) { _printDetails = PrintDetails; }

        /// <summary>
        /// Y top position of the image.
        /// </summary>
        private int UserYTop {
            get {
                return (int)numericUpDownYTop.Value;
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
        /// Send the FIFO image
        /// </summary>
        public void SendImage() {
            if (!EnableMixedMode) {
                return;
            }
            if (_imageBufferId == MeteorConsts.IMG_BUF_UNSET) {
                _imageBufferId = imageFileControl.Image.FillMeteorImageBuffer(_printDetails.UserBitsPerPixel);
            }

            //
            // For simplicity in the sample code, we are using an application filled image buffer and sending the
            // same image for each document.
            //
            // In a real application, it is more likely that a PCMD_IMAGE command containing the image
            // data itself would be used, because the mixed mode image sent from the PC will probably be different
            // for each document.
            //
            int[] imgBufCmd = new int[] { 
                (int)CtrlCmdIds.PCMD_IMAGE_BUFFER, 
                5,              // Count of following Dwords
                1,              // Colour plane.  For this sample, the plane is hard-wired to 1.
                UserXStart, 
                UserYTop, 
                imageFileControl.Image.GetImageWidth(), 
                (int)_imageBufferId
            };

            eRET rVal;
            do {
                rVal = PrinterInterfaceCLS.PiSendCommand(imgBufCmd);
            } while (rVal == eRET.RVAL_FULL);
        }

        /// <summary>
        /// Restore user settings
        /// </summary>
        public void LoadSettings() {
            imageFileControl.RestoreImage(Properties.Settings.Default.FifoImageFileName);
            checkBoxEnableMixedMode.Checked = Properties.Settings.Default.EnableMixedMode;
        }

        /// <summary>
        /// Save user settings
        /// </summary>
        public void SaveSettings() {
            Properties.Settings.Default.FifoImageFileName = imageFileControl.FileName;
            Properties.Settings.Default.EnableMixedMode = EnableMixedMode;
        }

        /// <summary>
        /// Enable / disable user controls, e.g. depending on whether a print job is currently active
        /// </summary>
        public void EnableControls() {
            checkBoxEnableMixedMode.Enabled = (imageFileControl.Image != null) && !_printDetails.JobActive;
        }

        /// <summary>
        /// Select the image file which is sent (to every document) via the FIFO data path
        /// </summary>
        private void ButtonSelectFile_Click(object sender, EventArgs e) {
            imageFileControl.SelectImage();
            checkBoxEnableMixedMode.Enabled = (imageFileControl.Image != null);
            if (_imageBufferId != MeteorConsts.IMG_BUF_UNSET) {

                //
                // Free the PrintEngine memory used to hold the previous image
                //
                int[] imgBufCmd = new int[] { 
                    (int)CtrlCmdIds.PCMD_IMAGE_BUFFER, 
                    5,              // Count of following Dwords
                    0,              // Set colour plane to zero to free the image buffer.
                    0,              // Unused here
                    0,              // Unused here
                    0,              // Unused here
                    (int)_imageBufferId
                };

                eRET rVal;
                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(imgBufCmd);
                } while (rVal == eRET.RVAL_FULL);

                _imageBufferId = MeteorConsts.IMG_BUF_UNSET;
            }
        }
    }
}
