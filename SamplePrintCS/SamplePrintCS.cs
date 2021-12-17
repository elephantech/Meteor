/* SamplePrintCS.cs
 *
 * Copyright (c) 2010-2013, The Technology Partnership Plc
 */

// Simple example program for printing using Meteor, written in C#
// 
// For instructions on usage, run the program with no arguments.
//
// This code demonstrates printing a single document using the FIFO or
// Preload path. It can also print two swaths with scanning printing.
//
// By default, this program will print a single image on the Preload path.
// The user can specify the file to be printed, and override the default options,
// from the command line.
//
// Many of the commands in this file are followed by explanations of what to look 
// for in the Monitor debug window, when the command executes
//

using System;
using System.Threading;
using System.IO;
using Ttp.Meteor;

namespace SamplePrintCS {

    class SamplePrintCS {

        string bitmapPath1, bitmapPath2;    //bitmap filenames (two required when in scanning mode)
        bool bCheckerboard;                 //Print checkerboard?
        eJOBTYPE jobType;                   //Type of print data path
        uint printCopies;                   //Number of copies to print
        int bpp;                            //Bits per pixel resolution
        bool bScanning;                     //Scanning mode,
        int[] imageBuffer1, imageBuffer2;   //Buffers for holding image data
        int docId, jobId;                   //Document and job id
        int res;                            //Print resolution
        int docWidth;                       //Document Width

        public SamplePrintCS() {
            bitmapPath1 = null;
            bitmapPath2 = null;
            bCheckerboard = false;             //Default to print files
            jobType = eJOBTYPE.JT_PRELOAD;     //Default to preload data path
            printCopies = 1;                   //Default to 1 copy of the document
            bpp = 0;                           //Default to existing print engine setting
            bScanning = false;                 //Default to not scanning
            docId = 1;                         //Default to a document id of 1
            jobId = 0;                         //Default to job id of 0
            res = (int)eRES.RES_HIGH;          //Default to highest resolution
            docWidth = 3508;                   //Default to A4length @300dpi
        }

        public int Print(string[] args) {
            // First argument is filename of bitmap to print
            if(args.Length < 1) {
                PrintUsage();
            } else {
                ParseArguments(args);
                try {
                    SendPrinterCommands(args);
                } catch(BadImageFormatException ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("This occurs when PrinterInterface.dll mismatches your platform target");
                }
            }
            Console.WriteLine("\nPress Enter to Continue...");
            Console.ReadLine();
            return 0;
        }

        private void SendPrinterCommands(string[] args) {
            // Connect to the printer first
            eRET retval = PrinterInterfaceCLS.PiOpenPrinter(); //Open the printer interface
            // Look out for debug message:  "Application Attached"

            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, unable to open Printer Interface...is Monitor.exe running?");
            } else {
                //Printer interface opened OK
                Console.WriteLine("Printer Interface opened");

                //Attempt to open bitmap file and create the image command, for later use
                if(!bCheckerboard) {
                    this.bitmapPath1 = args[0];
                    Console.WriteLine("Using bitmap1 = " + bitmapPath1);

                    // Default option is to print the file specified
                    imageBuffer1 = SampleBitmap.MakeBitmap(bitmapPath1);
                    if(null == imageBuffer1) {
                        Console.WriteLine("Error, unable to open input file: " + bitmapPath1);
                    } else {
                        Console.WriteLine("Opened bitmap 1 OK");
                    }
                } else {
                    // Otherwise print a fixed checkerboard pattern
                    Console.WriteLine("Bitmap 1 is fixed checkerboard pattern");
                    imageBuffer1 = SampleBitmap.MakeCheckerboardCommand(2000, 2656);
                    if(null == imageBuffer1) {
                        Console.WriteLine("Error, failed to make checkerboard command");
                    }
                }

                if(null != imageBuffer1) {
                    if(CheckScan()) {
                        //Everything ok, so start printing now

                        //Reset the printer
                        InitialisePrinter();

                        //Specify parameters of the job
                        StartJob();

                        //Start the document
                        Start();

                        // Send the image.  We already created the header when we generated the ImageBuffer,
                        // so all we need to do here is send the ImageBuffer
                        Console.WriteLine("Sending Image");
                        SendImage();

                        // Finished sending images, send the end doc command
                        EndDoc();

                        // If using scanning printing, send the second document
                        if(bScanning) {
                            SendSecondImage();
                        }

                        // Finished sending the document, send the end job command
                        EndJob();

                        // Check the printer status
                        CheckStatus();
                    }
                }

                // All done now, close the printer interface, by calling PiClosePrinter until the interface closes
                Console.WriteLine("Closing Printer Interface");
                while(eRET.RVAL_OK != PrinterInterfaceCLS.PiClosePrinter()) {
                    Thread.Sleep(1000);
                }
                // Look out for debug message: "Application Detached"
            }
        }

        /// <summary>
        /// Parses command line arguments to SamplePrint.exe and stores valid arguments in class members
        /// </summary>
        void ParseArguments(string[] args) { // Parse remaining arguments to check for options

            for(int i = 0; i < args.Length; i++) {
                int index = 0;
                bool bMatched = false;
                string value;

                index = args[i].IndexOf("-path="); // Check for -path option
                if(index >= 0) {
                    bMatched = true;
                    value = args[i].Substring(index + "-path=".Length); //extract path from string
                    if(value.Equals("fifo")) {
                        jobType = eJOBTYPE.JT_FIFO;
                        Console.WriteLine("Using path = Fifo");
                    } else if(value.Equals("preload")) {
                        jobType = eJOBTYPE.JT_PRELOAD;
                        Console.WriteLine("Using path = Preload");
                    } else {
                        Console.WriteLine("Warning: Invalid path, defaulting to Preload");
                    }
                }

                index = args[i].IndexOf("-copies="); // Check for -copies option
                if(index >= 0) {
                    bMatched = true;
                    value = args[i].Substring(index + "-copies=".Length); //extract argument from string
                    try {
                        printCopies = Convert.ToUInt32(value);
                        Console.WriteLine("Using copies = " + printCopies.ToString());
                    } catch {
                        printCopies = 1;
                        Console.WriteLine("Warning: Invalid number of copies, defaulting to 1");
                    }
                }

                index = args[i].IndexOf("-scan="); // Check for -scan option
                if(index >= 0) {
                    bMatched = true;
                    value = args[i].Substring(index + "-scan=".Length); //extract argument from string

                    bitmapPath2 = value;
                    bScanning = true;
                    Console.WriteLine("Using Scanning mode, bitmap = " + bitmapPath2);
                }

                index = args[i].IndexOf("-bpp="); // Check for -bpp option
                if(index >= 0) {
                    bMatched = true;
                    value = args[i].Substring(index + "-bpp=".Length); //extract argument from string
                    try {
                        bpp = Convert.ToInt32(value);
                        Console.WriteLine("Using bits per pixel = " + bpp.ToString());
                    } catch {
                        bpp = 1;
                        Console.WriteLine("Warning: Invalid bits per pixel, defaulting to 0");
                    }
                }

                index = args[i].IndexOf("-checkerboard"); // Check for checkerboard option
                if(index >= 0) {
                    bMatched = true;
                    bCheckerboard = true;
                }

                if(!bMatched) {
                    if (i != 0 || !File.Exists(args[i])) {
                        Console.WriteLine("Invalid argument: \"" + args[i] + "\" ignored");
                    }
                }
            }
        }

        /// <summary>
        /// Print description of valid command line arguments to SamplePrint
        /// </summary>
        static void PrintUsage() {
            Console.Write("\nUsage: SamplePrintCLS <bitmap1> [options]\n\n");
            Console.Write("Options:\n\n");
            Console.Write("-path=<fifo|preload> Use the fifo or the preload path.  Default is preload.\n");
            Console.Write("-copies=<ncopies>    Number of copies to print on the preload path.\n");
            Console.Write("                     Default is 1.\n");
            Console.Write("-scan=<bitmap2>      Scanning printing.  Prints two swaths, with the\n");
            Console.Write("                     second swath being taken from <bitmap2>.\n");
            Console.Write("                     Only available with fifo path.\n");
            Console.Write("-bpp=<bitsPerPixel>  Bits per pixel, up to maximum supported by head type.\n");
            Console.Write("-checkerboard        Print a checkerboard pattern instead of a file.\n");
        }

        /// <summary>
        /// Check that if in scan mode, two valid bitmaps have been supplied
        /// </summary>
        bool CheckScan() {
            bool ok = true;
            if(this.bScanning) {
                if(this.jobType != eJOBTYPE.JT_FIFO) {
                    Console.WriteLine("Error, Scanning printing requires using fifo path");
                    Console.WriteLine("Use the option -path=fifo");
                    ok = false;
                } else {
                    this.imageBuffer2 = SampleBitmap.MakeBitmap(this.bitmapPath2);
                    if(null == this.imageBuffer2) {
                        Console.WriteLine("Error, unable to open input file: " + this.bitmapPath2);
                        ok = false;
                    }
                }
            }
            return ok;
        }

        /// <summary>
        /// Zero the Xcounters, turn on head power and set the bits-per-pixel value 
        /// </summary>
        void InitialisePrinter() {
            eRET retval;
            // Send an abort to clear out any old data
            SendAbort();
            // Look out for debug message: "Output: Abort"

            // In scanning mode, we need to zero the X-position at the home position.
            // In a real application, the transport would be moved to the home position
            // before calling this command
            if(this.bScanning) {
                Console.WriteLine("Setting Home");
                retval = PrinterInterfaceCLS.PiSetHome();
                if(eRET.RVAL_OK != retval) {
                    Console.WriteLine("Error, Set Home failed");
                }
                // Look out for debug message: "PCCx X-counters cleared"
                // Note this may cause an error message if the printer is not configured
                // for scanning mode.  Set Scanning = 1 in the config file.
            }

            // Before sending any print commands, turn on the head power
            // A real application should turn off the head power when printing has finished. 
            // For simplicity this is ignored here, and the heads are left on.
            Console.WriteLine("Turning on head power");
            retval = PrinterInterfaceCLS.PiSetHeadPower(1);
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, Set Head Power failed");
            }
            // Look out for debug message: "Switching HDC and Head Power ON"

            if(this.bpp > 0) {
                Console.WriteLine("Setting BitsPerPixel to " + this.bpp);
                eRET rval = PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_BITS_PER_PIXEL, this.bpp);
                if(eRET.RVAL_OK == rval) {
                    Console.WriteLine("");
                } else {
                    Console.WriteLine(" ... FAILED");
                }
            }
        }

        /// <summary>
        /// Send an abort, and wait for up to 5 seconds for the printer to become idle again 
        /// </summary>
        void SendAbort() {
            eRET retval;

            Console.WriteLine("Sending Abort");
            retval = PrinterInterfaceCLS.PiAbort();
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, Abort failed");
            }

            // Now check the printer status, and wait until it is idle
            DateTime end = DateTime.Now.AddSeconds(5);

            bool busy = true;
            while(DateTime.Now < end) {
                if(!PrinterInterfaceCLS.PiIsBusy()) {
                    busy = false;
                    break;
                }
            }
            if(busy) {
                Console.WriteLine("Error, Too long to become idle after abort");
            }
        }

        /// <summary>
        /// Send the start job command to print engine, retry until successful
        /// </summary>
        void StartJob() {
            int[] CommandBuffer = new int[6];
            eRET retval;

            // Setup the command buffer with the STARTJOB command
            CommandBuffer[0] = (int)CtrlCmdIds.PCMD_STARTJOB;   //Command ID
            CommandBuffer[1] = 4;                           //4 more parameters for STARTJOB
            CommandBuffer[2] = this.jobId;                  //Job ID for tracking purposes
            CommandBuffer[3] = (int)this.jobType;          //Path to be used
            CommandBuffer[4] = this.res;                    //Print Resolution
            CommandBuffer[5] = this.docWidth;               //Width of document, only used when printing right to left

            Console.WriteLine("Sending StartJob");

            // Here we send the StartJob command repeatedly, until the Printer Interface
            // is not busy, and accepts the command.  Normally the return value 
            // should be checked for every command, but we do not do it every time in this
            // example to keep the example simple.
            do {
                retval = PrinterInterfaceCLS.PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
            } while(retval == eRET.RVAL_BUSY);
            // Look out for debug message:  "StartJob"
        }

        /// <summary>
        /// Send the StartDoc or StartScan Command.  
        /// </summary>
        void Start() {
            int[] CommandBuffer = new int[3];
            eRET retval;

            // For scanning printing, we use StartScan.  Otherwise we use a STARTPDOC command or a
            // STARTFDOC command, depending on whether we are using preload or fifo mode.

            if(this.bScanning) {
                CommandBuffer[0] = (int)CtrlCmdIds.PCMD_STARTSCAN;	// Command ID
                CommandBuffer[2] = (int)eSCANDIR.SD_FWD;			// Print the first swath in the forward direction
            } else {
                // Setup the command buffer with the STARTDOC command
                if(eJOBTYPE.JT_PRELOAD == this.jobType) {
                    CommandBuffer[0] = (int)CtrlCmdIds.PCMD_STARTPDOC;	// Command ID
                    CommandBuffer[2] = (int)this.printCopies;			// Number of copies of the document to print
                } else {
                    CommandBuffer[0] = (int)CtrlCmdIds.PCMD_STARTFDOC;	// Command ID
                    CommandBuffer[2] = this.docId;			// ID of this document
                }
            }
            CommandBuffer[1] = 1;				// 1 more parameter for any of these commands command

            Console.WriteLine("Starting Document");
            retval = PrinterInterfaceCLS.PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, Start failed");
            }
            // Look out for debug message:  "StartPreloadDoc" or "StartFifoDoc" or "StartScan" 
        }

        /// <summary>
        /// Send image data to print engine
        /// </summary>
        void SendImage() {
            eRET retval;

            retval = PrinterInterfaceCLS.PiSendCommand(this.imageBuffer1);
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, Send Image Failed");
            }
            // Look out for debug message:  "Image"
        }

        /// <summary>
        /// Send second document and print data to print engine (for use in scan mode)
        /// </summary>
        void SendSecondImage() {
            int[] CommandBuffer = new int[3];
            eRET retval;

            CommandBuffer[0] = (int)CtrlCmdIds.PCMD_STARTSCAN;  //Command ID
            CommandBuffer[1] = 1;           // 1 more parameter for StartScan command
            CommandBuffer[2] = (int)eSCANDIR.SD_REV;    // print second swath in the reverse direction

            Console.WriteLine("Starting Second Swath");
            retval = PrinterInterfaceCLS.PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, Start failed");
            }

            //Send the image
            Console.WriteLine("Sending Second Swath");
            retval = PrinterInterfaceCLS.PiSendCommand(this.imageBuffer2);
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, Send Second Image Failed");
            }

            //Finished sending images, send the end doc command
            EndDoc();
            // Look out for debug message:  "Image"
        }

        /// <summary>
        /// Send the End Document command to print engine
        /// </summary>
        void EndDoc() {
            int[] CommandBuffer = new int[2];
            eRET retval;

            CommandBuffer[0] = (Int32)CtrlCmdIds.PCMD_ENDDOC;		// Command ID
            CommandBuffer[1] = 0;						// No parameters for ENDDOC command

            Console.WriteLine("Sending EndDoc");
            retval = PrinterInterfaceCLS.PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, EndDoc failed");
            }
            // Look out for debug message:  "EndDoc"
        }

        /// <summary>
        /// Send the End Job command to print engine
        /// </summary>
        void EndJob() {
            int[] CommandBuffer = new int[2];
            eRET retval;

            CommandBuffer[0] = (int)CtrlCmdIds.PCMD_ENDJOB;		// Command ID
            CommandBuffer[1] = 0;				// No parameters for ENDJOB command

            Console.WriteLine("Sending EndJob");
            retval = PrinterInterfaceCLS.PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
            if(eRET.RVAL_OK != retval) {
                Console.WriteLine("Error, EndJob failed");
            }
            // Look out for debug message:  "EndJob"
        }

        /// <summary>
        /// Placeholder for your application to request print status from print engine
        /// </summary>
        void CheckStatus() {
            TAppStatus AppStatus;
            TAppPccStatus PccStatus;
            TAppHeadStatus HeadStatus;

            eRET retVal;

            // These calls do not do anything in this sample application.  They are provided only
            // to show how they can be used to request status information from the Print Engine.
            // Using a debugger, it's possible to see what status information was returned.  
            retVal = PrinterInterfaceCLS.PiGetPrnStatus(out AppStatus);
            retVal = PrinterInterfaceCLS.PiGetPccStatus(1, out PccStatus);
            retVal = PrinterInterfaceCLS.PiGetHeadStatus(1, 1, out HeadStatus);
        }

    }//end of class SamplePrintCS
}//end of namespace SamplePrintCS
