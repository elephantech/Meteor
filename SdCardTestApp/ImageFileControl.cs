using System;
using System.Windows.Forms;
using System.IO;

namespace MeteorInkJet.SdCardTestApp {
    public partial class ImageFileControl : UserControl {
        /// <summary>
        /// Current print image
        /// </summary>
        public MeteorImage Image { get; private set; }
        /// <summary>
        /// Filename of the current print image
        /// </summary>
        public string FileName { get { return openFileDialogReadImage.FileName; } }

        public ImageFileControl() {
            InitializeComponent();
        }

        public void RestoreImage(string fileName) {
            openFileDialogReadImage.FileName = fileName;
            if (File.Exists(openFileDialogReadImage.FileName)) {
                ReadImage();
            } else {
                openFileDialogReadImage.FileName = "";
            }
        }

        public void SelectImage() {
            if (openFileDialogReadImage.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                ReadImage();
            }
        }

        /// <summary>
        /// Read the data for a new print image from disk into PC memory
        /// </summary>
        private void ReadImage() {
            // Attempt to avoid memory allocation problems with large images by
            // cleaning up the currently loading image.  For very large images
            // the 64 bit build of the application should be used.
            if (Image != null) {
                pictureBoxPrintData.Image = null;
                Image.Dispose();
                Image = null;
                pictureBoxPrintData.Refresh();
            }
            GC.Collect();
            // Load the new image
            Cursor.Current = Cursors.WaitCursor;
            string FileName = openFileDialogReadImage.FileName;
            Image = new MeteorImage();
            Image.Load(FileName);

            if (Image != null) {
                pictureBoxPrintData.Image = Image.GetPreviewBitmap();
                Cursor.Current = Cursors.Default;
                toolStripStatusFilename.Text = Image.GetBaseName();
                toolStripStatusFilename.ToolTipText = FileName;
                toolStripStatusImageDimensions.Text = string.Format("{0} x {1} pixels", Image.GetImageWidth(), Image.GetImageHeight());
            } else {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Failed to load image " + openFileDialogReadImage.FileName,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                openFileDialogReadImage.FileName = "";
                toolStripStatusFilename.Text = "No image loaded";
                toolStripStatusFilename.ToolTipText = "";
                toolStripStatusImageDimensions.Text = "";
            }            
        }
    }
}
