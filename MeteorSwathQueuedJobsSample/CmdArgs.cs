using System;
using System.Collections.Generic;

namespace MeteorSwathQueuedJobsSample
{
    /// <summary>
    /// Parsing and validation of the command line arguments passed to MeteorSwathQueuedJobsSample.exe
    /// </summary>
    class CmdArgs
    {
        /// <summary>
        /// Number of queued jobs requested
        /// </summary>
        public Int32 JobCount => _geometryIndexList.Count;
        /// <summary>
        /// Geometry index for the job
        /// </summary>
        /// <param name="job">Must be less than <see cref="JobCount"/></param>
        public UInt32 GeometryIndex(Int32 job) => _geometryIndexList[job];
        /// <summary>
        /// TIFF file details for the job
        /// </summary>
        /// <param name="job">Must be less than <see cref="JobCount"/></param>
        public TiffFileDetails TiffDetails(Int32 job) => _tiffFileList[job];

        /// <summary>
        /// Print frequency for the simulated print.  
        /// Defaults to 500Hz, giving a relatively slow clock which makes it easier to see what's going on. 
        /// </summary>
        public Int32 PrintFrequency { get; private set; } = 500;
        /// <summary>
        /// List of geometry indexes passed on the command line
        /// </summary>
        private List<UInt32> _geometryIndexList = new List<uint>();
        /// <summary>
        /// List of TIFF files passed on the command line
        /// </summary>
        private List<TiffFileDetails> _tiffFileList = new List<TiffFileDetails>();

        /// <summary>
        /// Process and validate the command line arguments.
        /// This sample application expects a set of [geometry index, tiff filename] pairs.
        /// </summary>
        public bool ProcessArgs(string[] args) {
            bool ok = true;
            int jobParamIdx = 0;
            for (int i = 0; i < args.Length; i++) {
                if (args[i].StartsWith("-f=")) {
                    if (Int32.TryParse(args[i].Substring(3), out Int32 printFrequency)) {
                        PrintFrequency = printFrequency;
                    } else {
                        Logger.WriteLine(ConsoleColor.Red, $">!! Invalid print frequency '{args[i]}'");
                        ok = false;
                    }
                } else {
                    if ((jobParamIdx++ % 2) == 0) {
                        if (!UInt32.TryParse(args[i], out UInt32 geometryIndex)) {
                            Logger.WriteLine(ConsoleColor.Red, $">!! Invalid geometry '{args[i]}', must be an index from 1 to max geometry index");
                            ok = false;
                        } else {
                            _geometryIndexList.Add(geometryIndex);
                        }
                    } else {
                        _tiffFileList.Add(new TiffFileDetails(args[i]));
                    }
                }
            }
            if (_tiffFileList.Count == 0) {
                Logger.WriteLine(ConsoleColor.Red, $">!! No TIFF files are listed");
                ok = false;
            }
            if (_geometryIndexList.Count != _tiffFileList.Count) {
                Logger.WriteLine(ConsoleColor.Red, $">!! Geomeotry count does not match TIFF file count");
                ok = false;
            }
            if (ok) {
                foreach (TiffFileDetails t in _tiffFileList) {
                    if (!t.SetDetails()) {
                        ok = false;
                    }
                }
            }
            if (ok) {
                foreach (TiffFileDetails t in _tiffFileList) {
                    if (t.Bpp != _tiffFileList[0].Bpp) {
                        Logger.WriteLine(ConsoleColor.Red, ">!! All TIFF files must have the same bit depth");
                        ok = false;
                        break;
                    }
                }
            }
            if (!ok) {
                Logger.WriteLine(">>>");
                Logger.WriteLine(">>> MeteorSwathQueuedJobsSample usage:");
                Logger.WriteLine(">>> MeteorSwathQueuedJobsSample GeometryIndex1 TiffFile1.tif");
                Logger.WriteLine(">>>                             [GeometryIndex2 TiffFile2.tif]");
                Logger.WriteLine(">>>                             [GeometryIndex3 TiffFile3.tif]");
                Logger.WriteLine(">>>                             [ ... ]");
                Logger.WriteLine(">>>                             [-f=Frequency] ");
                Logger.WriteLine(">>>");
            }
            return ok;
        }
    }
}
