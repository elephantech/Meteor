using System;
using Ttp.Meteor;

namespace MeteorSwathQueuedJobsSample
{
    /// <summary>
    /// Object which holds the details of a TIFF file to be printed
    /// </summary>
    class TiffFileDetails
    {
        public string FilePath { get; private set; }
        public UInt32 Width { get; private set; }
        public UInt32 Height { get; private set; }
        public UInt32 Bpp { get; private set; }
        public TiffFileDetails(string filePath) { FilePath = filePath; }
        public bool SetDetails() {
            eRET rVal = PrinterInterfaceCLS.PiGetImageFileDetails(FilePath, out GenericImageFileDetails fileDetails);
            if (rVal != eRET.RVAL_OK) {
                Logger.WriteLine($">!! Failed to get details for file '{FilePath}': {rVal}");
                return false;
            }
            Height = fileDetails.Height;
            Width = fileDetails.Width;
            Bpp = fileDetails.BitsPerSample;
            return true;
        }
    }
}
