using System.Windows.Forms;

namespace Ttp.Meteor.RAMPrintTestApp {
    /// <summary>
    /// Factory for creating an object that implements the IMeteorImageData interface
    /// from a file path.  The type of object to create is based on the file
    /// extension.
    /// </summary>
    class ImageDataFactory {
        public static IMeteorImageData Create(string fileName) {
            IMeteorImageData image = null;
            int ExtensionIndex = fileName.LastIndexOf('.');
            if (ExtensionIndex != -1) {
                string FileExt = fileName.Substring(ExtensionIndex + 1).ToLower();
                switch (FileExt) {
                    case "bmp":
                    case "jpg":
                    case "tif":
                        image = new MeteorBitmapImage();
                        break;
                    default:
                        break;
                }
            }
            if (image != null) {
                if ( image.Load(fileName) ) {
                    return image;
                }
            } else {
                MessageBox.Show("Unrecognised file type " + fileName,
                                Application.ProductName, 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
            }
            return null;
        }
    }
}
