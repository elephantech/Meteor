using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Ttp.Meteor;

namespace MeteorInkJet.SdCardTestApp {
    /// <summary>
    /// Object which loads a .bmp file and converts it into an image command for Meteor
    /// The resolution is reduced to the required bits-per-pixel using an arbitrary
    /// set of boundaries, so that an attempt can be made to print images from any
    /// source.  This approach is suitable for demonstration purposes only.
    /// </summary>
    public class MeteorImage : IDisposable {
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
        public MeteorImage() {
        }

        public bool Load(string Path) {
            try {
                _bitmap = new Bitmap(Path);
                _basename = Path.Substring(Path.LastIndexOf('\\') + 1);
                return true;
            }
            catch (Exception e) {
                MessageBox.Show("Failed to open file " + Path + "\r\n\r\n" + e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public string GetBaseName()         { return _basename; }
        public Bitmap GetPreviewBitmap()    { return _bitmap; }
        public int GetImageWidth()          { return _bitmap.Width; } 
        public int GetImageHeight()         { return _bitmap.Height; }

        /// <summary>
        /// Create a Meteor image buffer, and load the print data into it
        /// </summary>
        public UInt32 FillMeteorImageBuffer(int trueBpp) {
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
            UInt32 imageSizeDwords = (UInt32)(bufwidthDWORDs * height);
            // Allocate memory for the image, including end of line padding.  
            // Note that this buffer will be initialised to zero by the framework.
            UInt32[] buff = new UInt32[imageSizeDwords];
            if (null == buff) {
                return UInt32.MaxValue;
            }
            // Start writing at the beginning of the buffer
            int dp = 0;

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

                        buff[dp] |= ((UInt32)b) << shift;          // Write both pixels

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
                        buff[dp] |= ((UInt32)b) << shift;          // Write both pixels
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
                        buff[dp] |= ((UInt32)b) << shift;          // Write both pixels
                        if((x >= width - 1) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                }
            }
            _bitmap.UnlockBits(bmpData);

            // Structure describing the buffer we want to allocate
            ImageBufferAllocParams allocParams = new ImageBufferAllocParams();
            allocParams.StructureSizeBytes = (UInt32)Marshal.SizeOf(allocParams);
            allocParams.SizeDwords = imageSizeDwords;
            allocParams.BitsPerPixel = (UInt32)outbpp;
            allocParams.WidthPixels = (UInt32)width;
            allocParams.HeightPixels = (UInt32)height;

            // Allocate memory in Meteor for the image
            eRET rVal = PrinterInterfaceCLS.PiAllocateImageBufferEx(ref allocParams);
            if (rVal != eRET.RVAL_OK) {
                return UInt32.MaxValue;
            }
            UInt32 bufId = allocParams.ImageBufferID;
            
            // And copy in the data
            rVal = PrinterInterfaceCLS.PiFillImageBuffer(bufId, 0, imageSizeDwords, buff);
            if (rVal != eRET.RVAL_OK) {
                return UInt32.MaxValue;
            }

            // I.D. of the image buffer which is now ready to be sent to the SD card
            return bufId;
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
