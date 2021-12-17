using System;
using System.Windows.Forms;
using System.IO;

namespace Ttp.Meteor.RAMPrintTestApp {
    public partial class ImageControl : UserControl {
        /// <summary>
        /// Current print image
        /// </summary>
        private IMeteorImageData _image;
        /// <summary>
        /// Has the image been loaded to PCCE DRAM
        /// </summary>
        private bool _imageLoadedToPccRAM;
        /// <summary>
        /// Is the loaded image for a circular print
        /// </summary>
        private bool _circularImageLoaded;
        /// <summary>
        /// Interface to retrieve global print job details, such as bits per pixel
        /// </summary>
        private IPrintDetails _printDetails;
        
        /// <summary>
        /// Zero based index of this image control.  This is used for the Meteor image reference index.
        /// </summary>
        public int ImageIndex { get; set; }
               
        /// <summary>
        /// Constructor
        /// </summary>
        public ImageControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Set the interface which the control can use to find out print job details, such as bits per pixel
        /// </summary>
        public void SetPrintDetails(IPrintDetails PrintDetails) { _printDetails = PrintDetails; }

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
        /// Predicate for doctype equal to FIFO read from the UI
        /// </summary>
        private bool IsDoctypeFIFO {
            get {
                return "fifo" == comboBoxDocType.Text.ToLower();
            }
        }

        /// <summary>
        /// Predicate for doctype equal to Preload read from the UI
        /// </summary>
        private bool IsDoctypePreload {
            get {
                return "preload" == comboBoxDocType.Text.ToLower();
            }
        }

        /// <summary>
        /// Predicate for doctype equal to Scan read from the UI
        /// </summary>
        private bool IsDoctypeScan {
            get {
                return "scan" == comboBoxDocType.Text.ToLower();
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
            if (s.XOffset != null && s.XOffset.Length > ImageIndex) {
                numericUpDownXOffset.Value = s.XOffset[ImageIndex];
            }
            if (s.CircularPrint != null && s.CircularPrint.Length > ImageIndex) {
                checkBoxCircular.Checked = s.CircularPrint[ImageIndex];
            }
            if (s.ImageFileName != null && s.ImageFileName.Count > ImageIndex) {
                openFileDialogReadImage.FileName = s.ImageFileName[ImageIndex];
            }
            if (s.XStart != null && s.XStart.Length > ImageIndex) {
                numericUpDownXStart.Value = s.XStart[ImageIndex];
            }
            if (s.DocType != null && s.DocType.Length > ImageIndex) {
                comboBoxDocType.SelectedIndex = s.DocType[ImageIndex];
            }
            if (s.ReverseDirection != null && s.ReverseDirection.Length > ImageIndex) {
                radioButtonReverse.Checked = s.ReverseDirection[ImageIndex];
            }
            if (File.Exists(openFileDialogReadImage.FileName)) {
                ReadImage();
            } else {
                openFileDialogReadImage.FileName = "";
            }
        }

        /// <summary>
        /// Save the user application settings for this control
        /// </summary>
        public void SaveSettings() {
            // This assumes that the caller has cleared the StringCollection, and is calling the
            // ImageControl objects in order
            Properties.Settings.Default.ImageFileName.Add(openFileDialogReadImage.FileName);
            // This assumes that the caller has created a new array of the correct size
            Properties.Settings.Default.YTop[ImageIndex] = (int)numericUpDownYTop.Value;
            Properties.Settings.Default.Copies[ImageIndex] = numericUpDownCopies.Value;
            Properties.Settings.Default.XOffset[ImageIndex] = (int)numericUpDownXOffset.Value;
            Properties.Settings.Default.CircularPrint[ImageIndex] = checkBoxCircular.Checked;
            Properties.Settings.Default.XStart[ImageIndex] = (int)numericUpDownXStart.Value;
            Properties.Settings.Default.DocType[ImageIndex] = comboBoxDocType.SelectedIndex;
            Properties.Settings.Default.ReverseDirection[ImageIndex] = radioButtonReverse.Checked;
        }

        /// <summary>
        /// Read the data for a new print image from disk into PC memory
        /// </summary>
        private void ReadImage() {
            // Attempt to avoid memory allocation problems with large images by
            // cleaning up the currently loading image.  For very large images
            // the 64 bit build of the application should be used.
            if (_image != null) {
                pictureBoxPrintData.Image = null;
                _image.Dispose();
                _image = null;
                pictureBoxPrintData.Refresh();
            }
            GC.Collect();
            // Load the new image
            Cursor.Current = Cursors.WaitCursor;
            string FileName = openFileDialogReadImage.FileName;
            _image = ImageDataFactory.Create(FileName);

            if (_image != null) {
                pictureBoxPrintData.Image = _image.GetPreviewBitmap();
                Cursor.Current = Cursors.Default;
                toolStripStatusFilename.Text = _image.GetBaseName();
                toolStripStatusFilename.ToolTipText = FileName;
                toolStripStatusImageDimensions.Text = string.Format("{0} x {1} pixels", _image.GetDocWidth(), _image.GetDocHeight());
            } else {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Failed to load image " + openFileDialogReadImage.FileName,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                openFileDialogReadImage.FileName = "";
                toolStripStatusFilename.Text = "No image loaded";
                toolStripStatusFilename.ToolTipText = "";
                toolStripStatusImageDimensions.Text = "";
            }
            EnableControls();
        }

        public void EnableControls() {
            if (!_printDetails.JobActive) {
                _imageLoadedToPccRAM = false;
            }

            groupBoxLoad.Enabled = _printDetails.JobActive && _image != null;
            groupBoxPrint.Enabled = _imageLoadedToPccRAM;
            numericUpDownXOffset.Enabled = _imageLoadedToPccRAM && _circularImageLoaded;
        }

        private void ButtonSelectFile_Click(object sender, EventArgs e) {
            if (openFileDialogReadImage.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                ReadImage();
            }
        }

        private void ButtonLoadImage_Click(object sender, EventArgs e) {
            if (_image == null) {
                return;
            }
            
            //
            // Circular printing is a special feature which allows the start and the end of
            // the image to join up.  Printing can start at any offset within the image.
            //
            bool circular = checkBoxCircular.Checked;

            //
            // The reverse flag allows images for forwards or reverse scans to be written to RAM for a scanning printer
            //
            bool reverse = radioButtonReverse.Checked;

            //
            // Find out how many colour planes there are in the current Meteor configuration.
            // For simplicity, we assume that the indexes of the configured colour planes start at 1 and are contiguous.
            //
            eRET rVal = PrinterInterfaceCLS.PiGetPlaneConfig(out MeteorPlaneConfig planeCfg);
            if (rVal != eRET.RVAL_OK) {
                MessageBox.Show("PiGetPlaneConfig failed: " + rVal);
            }

            //
            // Send the same image to each colour plane for simplicity in this example.
            // 
            for (int plane = 1; plane <= planeCfg.UsedPlaneCount; plane++) {

                //
                // Form the WRITE_RAM_IMAGE command for the plane.
                // The "send to hardware" flag should be set only for the final plane which is sent to the reference
                // Prior to this point, the images are held in PC memory within the PrintEngine
                // Any colour planes which do not have any data in PC memory at the point where "send to hardware" is set have their memory
                // contents set to zero
                //
                bool sendToHardware = plane == planeCfg.UsedPlaneCount;
                int[] imageCmd = _image.GetWriteRAMImageCommand(UserYTop, _printDetails.UserBitsPerPixel, ImageIndex, circular, reverse, plane, sendToHardware);

                //
                // Send to Meteor
                //
                unsafe {
                    fixed (int* pImageCmd = imageCmd) {
                        do {
                            rVal = PrinterInterfaceCLS.PiSendCommand(imageCmd);
                        } while (rVal == eRET.RVAL_FULL);
                        if (rVal != eRET.RVAL_OK) {
                            MessageBox.Show("Failed to load image to PCC DRAM: " + rVal);
                        } else {
                            _imageLoadedToPccRAM = true;
                            _circularImageLoaded = circular;
                        }
                    }
                }
            }
        }

        private void ButtonPrint_Click(object sender, EventArgs e) {
            if (!_imageLoadedToPccRAM) {
                return;
            }
            eRET rVal = eRET.RVAL_OK;

            //
            // For Doctype == FIFO|Scan, send a START DOC / IMAGE / END DOC sequence for each copy of the image
            // For Doctype == Preload, send one START DOC / IMAGE / END DOC sequence with a copy count
            //
            int loopCount = IsDoctypePreload ? 1 : UserImageCopies;
            for (int i = 0; i < loopCount; i++) {

                // ----------------------------------------------------------- //
                //
                // Start document command
                // 
                int[] startDocCmd;
                if (IsDoctypeScan) {
                    startDocCmd = new int[] {
                        (int)CtrlCmdIds.PCMD_STARTSCAN,
                        1,
                        radioButtonReverse.Checked ? 0x00000001 : 0x00000000}; // LSB specifies scan direction.
                } else if (IsDoctypePreload) {
                    startDocCmd = new int[] {
                        (int)CtrlCmdIds.PCMD_STARTPDOC,
                        2,  // Can also be 1 if the PCC number parameter is not required - i.e. documents are always printed by every PCC.
                        UserImageCopies,
                        // For most applications, the PCC number parameter can be omitted.
                        // When running with multiple master PCCs, it allows a document to be printed by a single PCC.
                        (int)numericUpDownPcc.Value };
                } else if (IsDoctypeFIFO) {
                    int docId = loopCount + (ImageIndex * 1000);
                    startDocCmd = new int[] {
                        (int)CtrlCmdIds.PCMD_STARTFDOC,
                        2,  // Can also be 1 if the PCC number parameter is not required - i.e. documents are always printed by every PCC.
                        docId,
                        // For most applications, the PCC number parameter can be omitted.
                        // When running with multiple master PCCs, it allows a document to be printed by a single PCC.
                        (int)numericUpDownPcc.Value };
                } else {
                    startDocCmd = new int[] {
                        (int)CtrlCmdIds.PCMD_NONE};
                }

                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(startDocCmd);
                } while (rVal == eRET.RVAL_FULL);

                if (rVal != eRET.RVAL_OK) {
                    break;
                }
                // ----------------------------------------------------------- //

                // ----------------------------------------------------------- //
                // 
                // Print RAM image command.  The command can be used with 3, 4 or 5 parameters; normally
                // just the first 3 are required.
                //
                int WidthPixels = _image.GetDocWidth();
                int[] printCmd;
                if (_circularImageLoaded) {
                    printCmd = new int[] {
                        (int)CtrlCmdIds.PCMD_PRINT_RAM_IMAGE, 
                        4, 
                        ImageIndex, 
                        UserXStart, 
                        WidthPixels, 
                        (int)numericUpDownXOffset.Value};
                } else {
                    printCmd = new int[] {
                        (int)CtrlCmdIds.PCMD_PRINT_RAM_IMAGE,
                        5,
                        ImageIndex,
                        UserXStart,
                        WidthPixels,
                        0,
                        // If the PCC index is 0 (the default value if the parameter is not used), the print command is sent 
                        // to all PCCs.  A PCC index of 1 sends the print command to PCC 1 only; 2 sends the print command to
                        // PCC2 only, etc.  This must match the PCC sent in the start and end document commands.
                        (int)numericUpDownPcc.Value};
                }

                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(printCmd);
                } while (rVal == eRET.RVAL_FULL);

                if (rVal != eRET.RVAL_OK) {
                    break;
                }
                // ----------------------------------------------------------- //

                // ----------------------------------------------------------- //
                //
                // End document command.  Normally this takes no parameters.  It only needs the pcc number
                // parameter if the document is being printed by a single PCC, in which case it must match
                // the PCC index sent in the start document and print image commands.
                //
                int[] endDocCmd = { (int)CtrlCmdIds.PCMD_ENDDOC, 1, (int)numericUpDownPcc.Value };
                do {
                    rVal = PrinterInterfaceCLS.PiSendCommand(endDocCmd);
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
