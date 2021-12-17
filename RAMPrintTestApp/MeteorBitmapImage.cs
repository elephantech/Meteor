using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ttp.Meteor.RAMPrintTestApp {
    /// <summary>
    /// Object which loads a .bmp file and converts it into an image command for Meteor
    /// The resolution is reduced to the required bits-per-pixel using an arbitrary
    /// set of boundaries, so that an attempt can be made to print images from any
    /// source.  This approach is suitable for demonstration purposes only.
    /// </summary>
    public class MeteorBitmapImage : IMeteorImageData {
        /// <summary>
        /// Data loaded from the bitmap file
        /// </summary>
        private Bitmap _bitmap;
        /// <summary>
        /// Name of the source bitmap file with the path removed
        /// </summary>
        private string _basename;

        /// <summary>
        /// Constructor
        /// </summary>
        public MeteorBitmapImage() {
        }

        #region IMeterImageData
        public bool Load(string path) {
            try {
                _bitmap = new Bitmap(path);
                _basename = path.Substring(path.LastIndexOf('\\') + 1);
                return true;
            }
            catch (Exception e) {
                MessageBox.Show("Failed to open file " + path + "\r\n\r\n" + e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Standard image command
        /// </summary>
        public int[] GetImageCommand(int yTop, int trueBpp) {
            return GetImageCommand(yTop, trueBpp, CtrlCmdIds.PCMD_IMAGE);
        }

        /// <summary>
        /// Big image command (for images larger than ~60MB)
        /// </summary>
        public int[] GetBigImageCommand(int yTop, int trueBpp) {
            return GetImageCommand(yTop, trueBpp, CtrlCmdIds.PCMD_BIGIMAGE);
        }

        /// <summary>
        /// Write RAM image command (PCCE only)
        /// </summary>
        public int[] GetWriteRAMImageCommand(int yTop, int trueBpp, int imgRef, bool circular, bool reverse, int plane, bool sendToHardware) {
            return GetImageCommand(yTop, trueBpp, CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX, imgRef, circular, reverse, plane, sendToHardware);
        }

        public string GetBaseName()         { return _basename; }
        public Bitmap GetPreviewBitmap()    { return _bitmap; }
        public int GetDocWidth()            { return _bitmap.Width; } 
        public int GetDocHeight()           { return _bitmap.Height; }
        #endregion

        /// <summary>
        /// Allocates and fills an image command buffer to be sent to Meteor via PiSendCommand.  
        /// The image data in the buffer comes from the previously loaded bitmap.
        /// 
        /// The standard command for sending image data to Meteor is PCMD_IMAGE.
        /// If a very large image (> 60MB) needs to be sent in one command then PCMD_BIGIMAGE command can be used.
        /// Alternatively, the size of the PrinterInterface / PrintEngine shared memory can be increased using the [System] CmdBufSizeDwords
        /// parameter in this situation.
        /// </summary>
        /// <param name="yTop">Y position in pixels of the image</param>
        /// <param name="trueBpp">Bits per pixel</param>
        /// <param name="imgRef">Image reference for CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX</param>
        /// <param name="cmd">The image command to use: should be CtrlCmdIds.PCMD_BIGIMAGE, CtrlCmdIds.PCMD_IMAGE, or CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX</param>
        /// <param name="bCircular">Circular printing flag for CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX</param>
        /// <param name="bReversed">Reverse scan image for CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX</param>
        /// <param name="plane">Colour plane index (1-N)</param>
        /// <param name="bSendToHardware">For CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX, true if the data for the reference should be sent to the hardware; 
        ///                              false if there is more data (for other colour planes) to follow</param>
        /// <param name="useBigImage">Use the standard PCMD_IMAGE or PCMD_BIGIMAGE</param>
        /// <returns>Image command to send to Meteor.  null if memory allocation fails</returns>
        private int[] GetImageCommand(int yTop, int trueBpp, CtrlCmdIds cmd, int imgRef = 0, bool bCircular = false, bool bReversed = false,
                                      int plane = 1, bool bSendToHardware = true) {
            if ( cmd != CtrlCmdIds.PCMD_BIGIMAGE &&
                 cmd != CtrlCmdIds.PCMD_IMAGE &&
                 cmd != CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX ) {
                throw new Exception("Invalid image command type");
            }
            // Meteor sends print data to the hardware as 1,2 or 4bpp.
            // Some heads accept 3bpp data; some accept 4bpp.
            // For 3bpp heads we need to use the least significant 3 bits in the 4bpp data.
            int outbpp = (trueBpp == 3) ? 4 : trueBpp;
            // Image dimensions
            int width = _bitmap.Width;
            int height = _bitmap.Height;
            // The width of the image data buffer sent to Meteor must be a multiple
            // of DWORDs
            int bufwidthDWORDs = (((width * outbpp) + 31) >> 5);
            // Meteor image buffer size in DWORDs
            int isize = bufwidthDWORDs * height;
            // Header size depends on the command.  This is the size of the command
            // header in addition to the first two DWORDS in the command (command type
            // and size) which are present in every command.
            int hdrsize = 0;
            switch (cmd) {
                case CtrlCmdIds.PCMD_BIGIMAGE :           hdrsize = 5; break;
                case CtrlCmdIds.PCMD_IMAGE :              hdrsize = 4; break;
                case CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX : hdrsize = 6; break;
            }

            // Allocate memory for image + header.  Note that this buffer will be
            // initialised to zero by the framework.
            int[] buff = new int[isize + hdrsize + 2];
            if(null == buff) {
                return null;
            }

            // Fill in the command header
            buff[0] = (int)cmd;                     // Command
            buff[1] = isize + hdrsize;              // Dword count
            if(cmd == CtrlCmdIds.PCMD_WRITE_RAM_IMAGE_EX) {
                buff[2] = 5;                            // Number of command parameters
                buff[3] = imgRef;                       // Image reference
                buff[4] = plane;                        // Plane
                if (bCircular) {
                    // Top bit set to prepare the image for circular printing
                    buff[4] |= unchecked((int)0x80000000);
                }
                if (!bSendToHardware) {
                    // If there will be more images to send to other colour planes for this image reference, the "more images" flag should
                    // be set.  It should be clear for the final image for the reference, at which point the print data is downloaded to the
                    // PCCEs.
                    buff[4] |= unchecked((int)0x40000000);
                }
                if (bReversed) {
                    // Bit 29 set indicates reversed scan direction (clear = forwards)
                    buff[4] |= unchecked((int)0x20000000);
                }
                buff[5] = yTop;                         // Ytop
                buff[6] = width;                        // Width
                buff[7] = 0;                            // Reserve width.  Can be used to reserve additional memory for the slot.
            } else {
                buff[2] = plane;                        // Plane
                buff[3] = 1;                            // Xleft
                buff[4] = yTop;                         // Ytop
                buff[5] = width;                        // Width
                if (cmd == CtrlCmdIds.PCMD_BIGIMAGE) {
                    buff[6] = height;                   // Height (PCMD_BIGIMAGE only)
                }
            }
            int dp = 2 + hdrsize;                   // Index of first data DWORD

            // Copy the image data line by line from the bitmap and write it into the Meteor 
            // command buffer at the requested resolution
            BitmapData bmpData = _bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int srcstrideBytes = bmpData.Stride;
            byte[] lineData = new byte[4 * bufwidthDWORDs * (32 / outbpp)]; // 32 bits per pixel for the maximum Meteor buffer line length
                                                                            // to protect against falling off the end of actual pixel data
                                                                            // in the below loop
            //
            // The below code performs an arbitrary mapping to the grey levels
            // for the selected bits per pixel, so that an attempt can be made
            // to print the image regardless of its format.
            //
            for(int y = 0; y < height; y++) {
                Marshal.Copy(new IntPtr(bmpData.Scan0.ToInt64() + y * srcstrideBytes), lineData, 0, srcstrideBytes);
                if(trueBpp == 1) {
                    for(int x = 0; x < width; x += 4) {
                        int byteoff = x >> 2;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, x * 4);
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 1 msbs

                        // Get second source pixel
                        byte b2 = ArgbToGrey(lineData, (x + 1) * 4);
                        b2 ^= 0xFF;     // Invert for printer
                        b2 >>= 5;       // Only interested in 1 msbs

                        // Get third source pixel
                        byte b3 = ArgbToGrey(lineData, (x + 2) * 4);
                        b3 ^= 0xFF;     // Invert for printer
                        b3 >>= 6;       // Only interested in 1 msbs

                        // Get fourth source pixel
                        byte b4 = ArgbToGrey(lineData, (x + 3) * 4);
                        b4 ^= 0xFF;     // Invert for printer
                        b4 >>= 7;       // Only interested in 1 msbs

                        b = (byte)((b & 0x08) | (b2 & 0x4) | (b3 & 0x02) | (b4 & 0x01));    // Align pixels

                        buff[dp] |= ((int)b) << shift;          // Write both pixels

                        if((x >= width - 4) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                } else if(trueBpp == 2) {
                    int lbuff = bmpData.Stride * y;
                    for(int x = 0; x < width; x += 2) {
                        int byteoff = x >> 1;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, x * 4);
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 2 msbs

                        // Get second source pixel
                        byte b2 = ArgbToGrey(lineData, (x + 1) * 4);
                        b2 ^= 0xFF;     // Invert for printer
                        b2 >>= 6;       // Only interested in 2 msbs

                        b = (byte)((b & 0x0C) | (b2 & 0x3));    // Align pixels
                        buff[dp] |= ((int)b) << shift;          // Write both pixels
                        if((x >= width - 2) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                } else {	// trueBpp ==  3 or 4
                    byte bppMask = (byte)((1 << trueBpp) - 1);  // Make to suit the true bpp
                    int lbuff = bmpData.Stride * y;
                    for(int x = 0; x < width; x += 1) {
                        int byteoff = x;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, x * 4);
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 4 msbs

                        if (trueBpp == 3) { //we are dealing with a smaller amount of bits at the head
                            b >>= 1;
                        }

                        b &= bppMask;      // Mask upper nibble
                        buff[dp] |= ((int)b) << shift;          // Write both pixels
                        if((x >= width - 1) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                }
            }
            _bitmap.UnlockBits(bmpData);
            return buff;
        }

        /// <summary>
        /// Converts an ARGB value to greyscale
        /// </summary>
        /// <param name="image">Byte array of pixel data</param>
        /// <param name="offset">Byte offset of ARGB value</param>
        /// <returns>8-bit greyscale value</returns>
        private byte ArgbToGrey(byte[] image, int offset) {
            return (byte)((float)image[offset + 2] * 0.3 + (float)image[offset + 1] * 0.59 + (float)image[offset + 0] * 0.11);
        }

        #region IDisposable
        public void Dispose()
        {
            if (_bitmap != null) { _bitmap.Dispose(); }
        }
        #endregion
    }
}
