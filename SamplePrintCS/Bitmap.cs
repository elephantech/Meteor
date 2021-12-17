using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Ttp.Meteor;

namespace SamplePrintCS {

static class SampleBitmap {


    /// <returns> true if 'aBpp' value is valid </returns>
    private static bool ValidateBpp(int aBpp) {
        if (aBpp == 1 || aBpp == 2 || aBpp == 4)
            return true;
        else
            return false;
    }


    /// <summary>
    /// Makes an image command buffer to print a checkerboard pattern
    /// </summary>
    /// <param name="width">Checkerboard width</param>
    /// <param name="height">Checkerboard height</param>
    /// <returns>Image command buffer</returns>
    public static int[] MakeCheckerboardCommand(uint width, uint height) {
        // Check the head type, and make an appropriate bitmap.
        // Different head types required different image formats, as
        // they have differing numbers of greyscale levels.
        int[] imageCommand = null;
        TAppStatus status;

        eRET rval = PrinterInterfaceCLS.PiGetPrnStatus(out status);
        if(eRET.RVAL_OK == rval) {

                if (ValidateBpp(status.BitsPerPixel)) {
                    imageCommand = MakeCheckerboard(width, height, (uint)status.BitsPerPixel);
                } else {
                    Console.WriteLine("Unsupported BPP value!");
                }

        } else {
            Console.WriteLine("Unable to read head type from Printer Interface {0}", rval);
        }
        return imageCommand;
    }

    /// <summary>
    /// Draws supplied bitmap into a buffer in command format for printer
    /// </summary>
    /// <param name="filename">Bitmap filename</param>
    /// <returns>Image command buffer</returns>
    public static int[] MakeBitmap(String filename) {
        // Check the head type, and make an appropriate bitmap.
        // Different head types required different image formats, as
        // they have differing numbers of greyscale levels.
        int[] imageCommand = null;
        TAppStatus status;

        eRET rval = PrinterInterfaceCLS.PiGetPrnStatus(out status);

        if(eRET.RVAL_OK == rval) {


                if (ValidateBpp(status.BitsPerPixel)) {
                    imageCommand = MakeBitmap(filename, status.BitsPerPixel);
                } else {
                    Console.WriteLine("Unsupported BPP value!");
                }
        } else {
            Console.WriteLine("Unable to read head type from Printer Interface {0}", rval);
        }
        return imageCommand;
    }

    /// <summary>
    /// Draws supplied bitmap into a buffer in command format for printer
    /// </summary>
    /// <param name="filename">Bitmap filename</param>
    /// <param name="bpp">True bits per pixel for head type</param>
    /// <returns>Image command buffer</returns>
    static int[] MakeBitmap(String filename, int bpp) {
        try {
            Bitmap Bmp = new Bitmap(filename);
            return LoadBitmap(Bmp, bpp);
        } catch(ArgumentException) {
            return null;
        }
    }

    /// <summary>
    /// Allocates a buffer and draws bitmap in command format for printer
    /// </summary>
    /// <param name="bitmap">Source bitmap</param>
    /// <param name="trueBpp">True bits per pixel for head type</param>
    /// <returns>Image command buffer</returns>
    static int[] LoadBitmap(Bitmap bitmap, int trueBpp) {
        int x, y, shift, isize, width, height, byteoff;
        int dp;
        byte b = 0;
        byte bppMask;

        int outbpp = trueBpp;

        width = bitmap.Width;
        height = bitmap.Height;
        isize = ((width * outbpp + 31) >> 5) * height;
        int[] buff = new int[isize + 6];      // Room for image + header
        if(null == buff) {
            return null;
        }

        //bitmap.Save("c:\\temp\\meteor_test.bmp"); // For checking

        for(int i = 0; i < buff.Length; i++) {
            buff[i] = 0;
        }

        buff[0] = (int)CtrlCmdIds.PCMD_IMAGE;   // Command
        buff[1] = isize + 4;                    // Dword count
        buff[2] = 1;                            // Plane
        buff[3] = 0;                            // Xleft
        buff[4] = 0;                            // Ytop
        buff[5] = width;                        // Width
        dp = 6;                                 // Index of first data

        //Create rectangle with size of image to be printed
        Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

        //Pad width if necessary to be divisible by 4 - extra pixels will be not be printed
        int padWidth = bitmap.Width + (4 - bitmap.Width % 4);
        Bitmap bmpPadded = new Bitmap(padWidth, bitmap.Height);
        //keep the same X/Y resolution as in the un-padded bitmap
        bmpPadded.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

        Graphics g = Graphics.FromImage(bmpPadded);
        g.DrawImage(bitmap, 0, 0, rect, GraphicsUnit.Pixel);
        //bmpPadded.Save("c:\\temp\\meteor_test_padded.bmp");
        g.Dispose();

        Rectangle PadRect = new Rectangle(0, 0, bmpPadded.Width, bmpPadded.Height);
        BitmapData bmpData = bmpPadded.LockBits(PadRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


        int bytes = bmpData.Stride * bmpData.Height;
        byte[] imageData = new byte[bytes];
        Marshal.Copy(bmpData.Scan0, imageData, 0, bytes);

        if(trueBpp == 1) {
            // 1 bits-per-pixel, for Spectra ...
            for(y = 0; y < height; y++) {
                int lbuff = bmpData.Stride * y;
                for(x = 0; x < width; x += 4) {
                    byteoff = x >> 2;
                    shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                    // Get first source pixel
                    b = ArgbToGrey(imageData, lbuff + x * 4);
                    b ^= 0xFF;      // Invert for printer
                    b >>= 4;        // Only interested in 1 msbs

                    // Get second source pixel
                    byte b2 = ArgbToGrey(imageData, lbuff + (x + 1) * 4);
                    b2 ^= 0xFF;     // Invert for printer
                    b2 >>= 5;       // Only interested in 1 msbs

                    // Get third source pixel
                    byte b3 = ArgbToGrey(imageData, lbuff + (x + 2) * 4);
                    b3 ^= 0xFF;     // Invert for printer
                    b3 >>= 6;       // Only interested in 1 msbs

                    // Get fourth source pixel
                    byte b4 = ArgbToGrey(imageData, lbuff + (x + 3) * 4);
                    b4 ^= 0xFF;     // Invert for printer
                    b4 >>= 7;       // Only interested in 1 msbs


                    b = (byte)((b & 0x08) | (b2 & 0x4) | (b3 & 0x02) | (b4 & 0x01));    // Align pixels

                    buff[dp] |= ((int)b) << shift;          // Write both pixels

                    if((x >= width - 4) || ((byteoff & 7) == 7)) {
                        dp++;
                    }
                }
            }
        } else if(trueBpp == 2) {
            // 2 bits-per-pixel, for Q-Class, KJ4, ...
            for(y = 0; y < height; y++) {
                int lbuff = bmpData.Stride * y;
                for(x = 0; x < width; x += 2) {
                    byteoff = x >> 1;
                    shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                    // Get first source pixel
                    b = ArgbToGrey(imageData, lbuff + x * 4);
                    b ^= 0xFF;      // Invert for printer
                    b >>= 4;        // Only interested in 2 msbs

                    // Get second source pixel
                    byte b2 = ArgbToGrey(imageData, lbuff + (x + 1) * 4);
                    b2 ^= 0xFF;     // Invert for printer
                    b2 >>= 6;       // Only interested in 2 msbs

                    b = (byte)((b & 0x0C) | (b2 & 0x3));    // Align pixels
                    buff[dp] |= ((int)b) << shift;          // Write both pixels
                    if((x >= width - 2) || ((byteoff & 7) == 7)) {
                        dp++;
                    }
                }
            }
        } else {	// trueBpp ==  3 or 4
            // 4 bits-per-pixel, for ...
            bppMask = (byte)((1 << trueBpp) - 1);			// Make a mask to suit the true bpp
            for(y = 0; y < height; y++) {
                int lbuff = bmpData.Stride * y;
                for(x = 0; x < width; x += 1) {
                    byteoff = x;
                    shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                    // Get first source pixel
                    b = ArgbToGrey(imageData, lbuff + x * 4);
                    b ^= 0xFF;      // Invert for printer
                    b >>= 4;        // Only interested in 4 msbs

                    if(trueBpp == 3) b >>= 1;//we are dealing with a smaller amount of bits

                    b &= bppMask;      // Mask upper nibble
                    buff[dp] |= ((int)b) << shift;          // Write both pixels
                    if((x >= width - 1) || ((byteoff & 7) == 7)) {
                        dp++;
                    }
                }
            }
        }

        bmpPadded.UnlockBits(bmpData);
        return buff;
    }

    /// <summary>
    /// Converts an ARGB value to greyscale
    /// </summary>
    /// <param name="image">Byte array of pixel data</param>
    /// <param name="offset">Byte offset of ARGB value</param>
    /// <returns>8-bit greyscale value</returns>
    static byte ArgbToGrey(byte[] image, int offset) {
        return (byte)((float)image[offset + 2] * 0.3 + (float)image[offset + 1] * 0.59 + (float)image[offset + 0] * 0.11);
    }

    /// <summary>
    /// Make an image command of the specified size with black squares in a checkerboard pattern
    /// </summary>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="bpp">Bits per pixel for head type</param>
    /// <param name="greylevel"></param>
    /// <returns>Image command buffer</returns>

    static int[] MakeCheckerboard(uint width, uint height, uint bpp) {
        uint greylevel = (2*bpp) - 1;
        uint docWidthInts = ((width * bpp) + 31) >> 5;
        uint isize = docWidthInts * height;
        int[] CmdBuff = new int[isize + 6];

        if(null != CmdBuff) {

            for(int i = 0; i < CmdBuff.Length; i++) {
                CmdBuff[i] = 0;
            }

            CmdBuff[0] = (int)CtrlCmdIds.PCMD_IMAGE;
            CmdBuff[1] = (int)isize + 4; // n
            CmdBuff[2] = 1;          // plane
            CmdBuff[3] = 0;          // xleft
            CmdBuff[4] = 0;          // ytop
            CmdBuff[5] = (int)width; // image width

            uint iPixelsPerInt = 32 / bpp;
            uint iPixelData = 0;

            // Fill whole row with current grey level
            for(int i = 0; i < iPixelsPerInt; i++) {
                iPixelData <<= (int)bpp;
                iPixelData |= greylevel;
            }

            // Calculate size of a square, use 10 DWORDs as standard
            uint sizeDwords = 10;
            uint hPeriod = sizeDwords * 2;
            uint size = 32 / bpp * sizeDwords;
            uint vPeriod = size * 2;

            bool odd = false;
            for(uint y = 0; y < height; y++) {
                uint index = 6 + y * docWidthInts;

                // Is this an odd row or even?
                odd = (y % vPeriod) >= size;

                for(int x = 0; x < docWidthInts; x++) {
                    if(((x % hPeriod) >= sizeDwords) ^ odd) {
                        CmdBuff[index + x] = (Int32)iPixelData;
                    }
                }
            }
        }
        return CmdBuff;
    }
}
}
