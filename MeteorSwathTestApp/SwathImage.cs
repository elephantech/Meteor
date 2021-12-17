using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// This class contains code which loads an image file using the .NET
    /// BitmapImage class. The image data is then read from the file and 
    /// converted into a Meteor image buffer. 
    /// 
    /// See the Meteor Software Manual for more information on Meteor
    /// image buffers.
    /// </summary>
    class SwathImage
    {
        /// <summary>
        /// Data loaded from the bitmap file
        /// </summary>
        private BitmapSource bitmap;

        /// <summary>
        /// Constructor
        /// </summary>
        public SwathImage()
        {
        }

        public bool Load(string Path)
        {
            try
            {
                bitmap = new BitmapImage(new Uri(Path));
                if (bitmap.Format.BitsPerPixel != 32) 
                {
                    bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, BitmapPalettes.Gray256, 0);
                }

                try 
                {
                    bitmap.Freeze();
                }
                catch (Exception)
                {
                }

                StartLine = 0;
                EndLine = bitmap.PixelHeight;
                return true;
            }
            catch (Exception)
            {
                // If this is a TIFF file check that the file is strip based, and that its pixel
                // resolution matches the bits per pixel which is currently selected in Meteor.
                // It should then be possible to handle the file directly via MeteorSwathSendTIFF.
                MessageBox.Show("Failed to convert image file.\n\nPlease check that the image file's bits-per-pixel matches the Meteor setting.", "File format mismatch");
                return false;
            }
        }

        /// <summary>The image can be cropped. This is the first line that will be used.</summary>
        public int StartLine { get; set; }
        public int EndLine { get; set; }

        public int DocWidth { get {return bitmap.PixelWidth; }}
        public int DocHeight { get { return bitmap.PixelHeight; } }

        public int CroppedHeight { get { return EndLine - StartLine; } }

        public int[] GetImageBuffer(int yTop, int trueBpp)
        {
            // Meteor sends print data to the hardware as 1,2 or 4bpp.
            // Some heads accept 3bpp data; some accept 4bpp.
            // For 3bpp heads we need to use the least significant 3 bits in the 4bpp data.
            int outbpp = (trueBpp == 3) ? 4 : trueBpp;
            // Image dimensions
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            // The width of the image data buffer sent to Meteor must be a multiple
            // of DWORDs
            int bufwidthDWORDs = (((width * outbpp) + 31) >> 5);
            // Meteor image buffer size in DWORDs
            int isize = bufwidthDWORDs * height;
            // Allocate memory for image.  Note that this buffer will be
            // initialised to zero by the framework.
            int[] buff = new int[isize];
            if (null == buff)
            {
                return null;
            }
            int dp = 0; // Destination pointer (offset).

            byte[] lineData = new byte[bufwidthDWORDs * (32 / outbpp) * 4];

            for (int y = StartLine; y < EndLine; y++)
            {
                bitmap.CopyPixels(new System.Windows.Int32Rect(0, y, width, 1), lineData, width * 4, 0);

                if (trueBpp == 1)
                {
                    for (int x = 0; x < width; x += 4)
                    {
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

                        if ((x >= width - 4) || ((byteoff & 7) == 7))
                        {
                            dp++;
                        }
                    }
                }
                else if (trueBpp == 2)
                {
                    for (int x = 0; x < width; x += 2)
                    {
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
                        if ((x >= width - 2) || ((byteoff & 7) == 7))
                        {
                            dp++;
                        }
                    }
                }
                else
                {	// trueBpp ==  3 or 4
                    byte bppMask = (byte)((1 << trueBpp) - 1);  // Make to suit the true bpp
                    for (int x = 0; x < width; x += 1)
                    {
                        int byteoff = x;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, x * 4);
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 4 msbs

                        if (trueBpp == 3)
                        { //we are dealing with a smaller amount of bits at the head
                            b >>= 1;
                        }

                        b &= bppMask;      // Mask upper nibble
                        buff[dp] |= ((int)b) << shift;          // Write both pixels
                        if ((x >= width - 1) || ((byteoff & 7) == 7))
                        {
                            dp++;
                        }
                    }
                }
            }
            return buff;
        }

        /// <summary>
        /// Converts an ARGB value to greyscale
        /// </summary>
        /// <param name="image">Byte array of pixel data</param>
        /// <param name="offset">Byte offset of ARGB value</param>
        /// <returns>8-bit greyscale value</returns>
        private byte ArgbToGrey(byte[] image, int offset)
        {
            return (byte)((float)image[offset + 2] * 0.3 + (float)image[offset + 1] * 0.59 + (float)image[offset + 0] * 0.11);
        }
    }
}

