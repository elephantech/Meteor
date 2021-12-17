namespace MeteorInkJet.SdCardTestApp {
    /// <summary>
    /// Object which calculates SD card block addresses
    /// The addresses depend on 
    ///   (a) The size of the SD card
    ///   (b) The maximum image size selected by the user
    ///   (c) The bit depth of the print data (1,2 or 4 bits per pixel)
    /// </summary>
    class SdCardSlot {
        /// <summary>
        /// Global instance of the SdCardSlot object 
        /// </summary>
        public static SdCardSlot Instance { get { return _instance; } }
        private static SdCardSlot _instance = new SdCardSlot();

        private int _sdCardSizeBlocks;
        private int _maxImageXLengthPixels;
        private int _bitsPerPixel;

        /// <summary>
        /// The total number of blocks available on the PCCE SD cards.
        /// </summary>
        public int SdCardSizeBlocks {
            get { return _sdCardSizeBlocks; }
            set { _sdCardSizeBlocks = value; }
        }

        /// <summary>
        /// The maximum X length of an image which will be written to the SD card
        /// </summary>
        public int MaxImageXLengthPixels {
            get { return _maxImageXLengthPixels; }
            set { _maxImageXLengthPixels = value; }
        }

        /// <summary>
        /// Print data resolution
        /// </summary>
        public int BitsPerPixel {
            get { return _bitsPerPixel; }
            set { _bitsPerPixel = value; }
        }

        /// <summary>
        /// The amount of memory on the SD card reserved per image per head
        /// </summary>
        private int SlotSizeBlocks {
            get {
                // The size which needs to be reserved for an image depends on the head
                // type.  
                //
                // Here we are assuming that we are running on the Dimatix Starfire.
                //

                // X span of the Starfire head in pixels at the native print-head resolution (400DPI).
                // This must be increased accordingly if printing at higher resolutions.
                //
                // Note that the calculation becomes more complicated for HDCs which drive 
                // more than one print head, as it must then take into account the entire 
                // X span of the HDC, which depends on the relative X positions of the heads.
                //
                int HeadXLengthPixels = 416;

                // Total X length of the slot
                //
                int SlotXLengthPixels = MaxImageXLengthPixels + HeadXLengthPixels;

                // The Starfire has 1024 nozzles.  
                //
                // For HDCs which drive more than one head, this is the total number of pixels 
                // in one print line over all heads.  For heads which include inactive nozzles 
                // (such as the Xaar 1001), we need the amount of data sent to the HDC, which 
                // includes the inactive nozzles.
                //
                int DataWidthPixels = 1024;

                // Multiply up by the number of bits in each print line.
                //
                int MaxHdImageBits = SlotXLengthPixels * DataWidthPixels * BitsPerPixel;
                
                // An SD card block is 512 bytes
                //
                int BitsPerBlock = 512 * 8;

                // Round the slot size up to a whole number of blocks
                //
                return (MaxHdImageBits + BitsPerBlock - 1) / BitsPerBlock;
            }
        }

        /// <summary>
        /// Calculates the block address of the start of the image data
        /// The UHS SD card is divided into image slots - each slot is the same size
        /// Slots on every PCC take the same address, so we only need a head index here - not a PCC index
        /// </summary>
        /// <param name="slot">Index of the slot (0-N)</param>
        /// <param name="hdc">Hdc index on the PCC (0-7)</param>
        /// <returns>Block address for the slot</returns>
        public int GetHdcBlockAddress(int slot, int hdc) {
            // We assume here that all 8 HDCs on the PCC are in use.
            //
            // If less heads are in use, the SD card can be divided up accordingly, to avoid wasting memory.
            //
            int blocksPerHead = SdCardSizeBlocks / 8;
            return (slot * SlotSizeBlocks) + (hdc * blocksPerHead);
        }
    }
}
