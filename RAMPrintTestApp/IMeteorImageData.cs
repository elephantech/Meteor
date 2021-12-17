using System;
using System.Drawing;

namespace Ttp.Meteor.RAMPrintTestApp {
    /// <summary>
    /// Interface hiding the details of a file type
    /// </summary>
    interface IMeteorImageData : IDisposable {
        /// <summary>
        /// Load the image file
        /// </summary>
        /// <param name="path">Full path to the source image file</param>
        /// <returns>success(true) or failure(false)</returns>
        bool Load(string path);
        /// <summary>
        /// Return a formatted command buffer to send the image to Meteor using
        /// PrinterInterfaceCLS.PiSendCommand.
        /// </summary>
        /// <param name="yTop">Y position if the image in pixels</param>
        /// <param name="trueBpp">Bits per pixel</param>
        /// <returns>Image command buffer or null if memory allocation fails</returns>
        int[] GetImageCommand(int yTop, int trueBpp);
        /// <summary>
        /// Return a formatted command buffer to send the image to Meteor using PrinterInterfaceCLS.PiSendCommand.
        /// </summary>
        /// <param name="yTop">Y position if the image in pixels</param>
        /// <param name="trueBpp">Bits per pixel</param>
        /// <returns>Image command buffer or null if memory allocation fails</returns>
        int[] GetBigImageCommand(int yTop, int trueBpp);
        /// <summary>
        /// Return a formatted command buffer to send the image to Meteor using PrinterInterfaceCLS.PiSendCommand.
        /// </summary>
        /// <param name="yTop">Y position if the image in pixels</param>
        /// <param name="trueBpp">Bits per pixel</param>
        /// <param name="imgRef"></param>
        /// <param name="circular"></param>
        /// <param name="reverse"></param>
        /// <param name="plane"></param>
        /// <param name="sendToHardware"></param>
        /// <returns>Image command buffer or null if memory allocation fails</returns>
        int[] GetWriteRAMImageCommand(int yTop, int trueBpp, int imgRef, bool circular, bool reverse, int plane, bool sendToHardware);
        /// <summary>
        /// Base name for display as determined from the image filename
        /// </summary>
        string GetBaseName();
        /// <summary>
        /// Gets the framework bitmap object to use for a preview display of the image
        /// </summary>
        Bitmap GetPreviewBitmap();
        /// <summary>
        /// Width of the image in pixels.  
        /// Note that this does **not** include any stride padding which may be necessary
        /// when the image data is sent to Meteor, where each line must be a whole number
        /// of DWORDs.
        /// </summary>
        int GetDocWidth();
        /// <summary>
        /// Height of the image in pixels.
        int GetDocHeight();
    }
}
