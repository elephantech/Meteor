/* SamplePrint.cpp
*
* Copyright (c) 2007-2012, The Technology Partnership Plc
*/

// Simple example program for printing using Meteor
// 
// For instructions on usage, run the programme with no arguments.
// PrinterInterface.dll must be present in the same directory as SamplePrint.exe
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

#include "stdafx.h"

// Standard libraries
#include <stdio.h>
#include <time.h>

// Meteor specific includes
#include "PrinterInterface.h"	// Contains definitions for all the Meteor library functions
#include "Meteor.h"				// Definition of common datatypes and constants

// Utility functions for handling bitmaps and generating image commands in the correct format
#include "Bitmaps.h"

// Function declarations
void InitialisePrinter(int scanning);
bool StartJob(DWORD jobid, DWORD jobtype, DWORD res, DWORD docwidth);
void Start(int scanning, DWORD jobtype, DWORD ncopies, DWORD docid);
void SendImage(uint32* buffer);
void ScanSendSecondDoc(uint32* SecondBuffer);
void EndDoc(void);
void EndJob(void);
void SendAbort(void);

void ParseArguments(int argc, _TCHAR* argv[], DWORD* jobtype,
                    DWORD* ncopies, int* scanning, _TCHAR** bitmap2);

int CheckScan(int scanning, DWORD jobtype, _TCHAR* bitmap2, uint32** SecondBuffer);
void PrintUsage(void);

//---------------------------------------------------------------------------
int _tmain(int argc, _TCHAR* argv[]) {

    _TCHAR* bitmap1 = NULL; _TCHAR* bitmap2 = NULL;
    uint32* ImageBuffer = NULL; uint32* SecondBuffer = NULL;

    eRET retval; // For return values from function calls

    DWORD jobid = 0;					// Default to 0
    DWORD jobtype = JT_PRELOAD;			// Default to Preload path
    DWORD res = RES_HIGH;				// Default to high resolution
    DWORD docwidth = 3508;				// Default to A4 length @ 300dpi
    DWORD ncopies = 1;					// Default to 1 copy of the document
    DWORD docid = 1;					// Default to a document ID of 1
    int scanning = 0;					// Default to not scanning

    // Parse the input arguments

    // The first argument is the filename of the bitmap to print
    if(argc < 2) {
        PrintUsage();
    } else {
        bitmap1 = argv[1];

        ParseArguments(argc, argv, &jobtype, &ncopies, &scanning, &bitmap2);

        // Connect to the printer first
        retval = PiOpenPrinter();	// Open the printer interface
        // Look out for Monitor log message:  "Application Attached"

        if(RVAL_OK != retval) {
            puts("Error, unable to open printer interface...is Monitor.exe running?");
        } else {

            // Printer interface opened ok
            puts("Printer interface opened");

            // Attempt to open the file and create the image command, for later use.
            ImageBuffer = MakeBitmap(bitmap1);

            if (ImageBuffer != NULL) {

                if(CheckScan(scanning,jobtype,bitmap2,&SecondBuffer)) {
                    // Everything ok, so start printing now

                    // Reset the printer
                    InitialisePrinter(scanning);

                    // Specify the parameters of the job
                    if ( StartJob(jobid, jobtype, res, docwidth) ) {

                        //
                        // Number of documents to send.  For FIFO, each copy of the
                        // document is sent by the application; FIFO is typically used for
                        // variable data, although in this example all documents are identical.  
                        // For preload, the document is repeated from PCC memory, so only
                        // needs to be sent once
                        //
                        int cycle = (JT_PRELOAD == jobtype) ? 1 : ncopies;

                        for (int i = 0; i < cycle; i++ ) {

                            // Start the document
                            Start(scanning,jobtype,ncopies, docid);

                            // Send the image.  We already created the header when we generated the ImageBuffer,
                            // so all we need to do here is send the ImageBuffer
                            puts("Sending Image");
                            SendImage(ImageBuffer);			

                            // Finished sending images, send the end doc command
                            EndDoc();

                            // If using scanning printing, send the second document
                            if(scanning) {
                                ScanSendSecondDoc(SecondBuffer);
                            }

                        }

                        // Finished sending the document, send the end job command
                        EndJob();

                    }

                    if(SecondBuffer) {
                        free(SecondBuffer);
                    }
                }
                free(ImageBuffer);
            }

            // All done now, close the printer interface, by calling PiClosePrinter until the interface closes
            puts("Closing Printer Interface");
            while ( !PiCanClosePrinter() ) { 
                Sleep(100);
            }
            while (RVAL_OK != PiClosePrinter()) {}
            // Look out for Monitor log message: "Application Detached"
        }
    }
    puts("Press Enter to continue...");
    getchar();
    return 0;
}

//---------------------------------------------------------------------------
// Parse the input arguments to extract the options 
void ParseArguments(int argc, _TCHAR* argv[], DWORD *jobtype,
                    DWORD *ncopies, int *scanning, _TCHAR **bitmap2) {

    // Parse the remaining input arguments to check for options
    for(int i = 2; i < argc; i++) {
        _TCHAR *value;
        int matched = 0;

        // Check for the presence of the "-path" option
        value = wcsstr(argv[i],L"-path=");
        if(NULL != value) {
            matched = 1;
            value += wcslen(L"-path=");	// Move the pointer to the start of the argument
            if(0 == wcscmp(value,L"fifo")) {
                *jobtype = JT_FIFO;
            } else if(0 == wcscmp(value,L"preload")) {
                *jobtype = JT_PRELOAD;
            } else {
                puts("Warning: Invalid path, defaulting to Preload");
            }
        }

        // Check for the presence of the "-copies" option
        value = wcsstr(argv[i],L"-copies=");
        if(NULL != value) {
            matched = 1;
            value += wcslen(L"-copies=");	// Move the pointer to the start of the argument
            int argument = _wtoi(value);
            if(argument <= 0) {
                puts("Warning, invalid number of copies, defaulting to 1");
            } else {
                *ncopies = argument;
            }
        }

        // Check for the presence of the "-scan" option
        value = wcsstr(argv[i],L"-scan=");
        if(NULL != value) {
            matched = 1;
            value += wcslen(L"-scan=");	// Move the pointer to the start of the argument
            *bitmap2 = value;
            *scanning = 1;
        }


        // Check if none of the commands matched a valid option
        if(!matched) {
            printf("Invalid argument \"%S\" ignored\n",argv[i]);
        }
    }
}

//---------------------------------------------------------------------------
// Clear out any old data, and setup the printer to a known state in order to 
// be ready to print
void InitialisePrinter(int scanning) {
    eRET retval;

    // Send an abort to clear out any old data
    SendAbort();
    // Look out for Monitor log message: "Rx:Abort"

    // In scanning mode, we need to zero the X-position at the home position.
    // In a real application, the transport would be moved to the home position
    // before calling this command
    if(scanning) {
        puts("Setting Home");
        retval = PiSetHome();
        if(RVAL_OK != retval) {
            puts("Error, Set Home failed");
        }
        // Look out for Monitor log message: "PCCx X-counters cleared"
        // Note this may cause an error message if the printer is not configured
        // for scanning mode.  Set Scanning = 1 in the config file.
    }


    // Before sending any print commands, turn on the head power
    // A real application should turn off the head power when printing has finished. 
    // For simplicity this is ignored here, and the heads are left on.
    puts("Turning on head power");
    retval = PiSetHeadPower(1);
    if(RVAL_OK != retval) {
        puts("Error, Set Head Power failed");
    }
    // Look out for Monitor log message: "Switching HDC and Head Power ON"
}

//---------------------------------------------------------------------------
bool StartJob(DWORD jobid, DWORD jobtype, DWORD res, DWORD docwidth) {
    uint32 CommandBuffer[6];
    eRET retval;

    // Setup the command buffer with the STARTJOB command
    CommandBuffer[0] = PCMD_STARTJOB;	// Command ID 
    CommandBuffer[1] = 4;				// 4 more parameters for STARTJOB command
    CommandBuffer[2] = jobid;			// Job ID for tracking purposes
    CommandBuffer[3] = jobtype;			// Preload path used
    CommandBuffer[4] = res;				// Print Resolution
    CommandBuffer[5] = docwidth;		// Width of document, only used when printing right-to-left

    puts("Sending StartJob");

    //
    // Commands and print data are buffered in several places - in the hardware, in the Print Engine, 
    // and in the Printer Interface.  
    // If print data is not being consumed by the print heads, eventually these buffers will fill up.
    // When this happens, the application should retry the command.
    // Normally an application would also have a mechanism for breaking out of this loop if it decides 
    // to abort the print job; this is not present in this example for simplicity.
    //
    do {
        retval = PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
    } while (RVAL_FULL == retval) ;

    if(RVAL_OK != retval) {
        printf("Error %d (0x%x), StartJob failed\n", retval, retval);
    }

    // Look out for Monitor log message:  "Rx:StartJob"

    return RVAL_OK == retval;
}

//---------------------------------------------------------------------------
void Start(int scanning, DWORD jobtype, DWORD ncopies, DWORD docid) {
    uint32 CommandBuffer[6];
    eRET retval;

    // Send the StartDoc or StartScan Command.  
    // For scanning printing, we use StartScan.  Otherwise we use a STARTPDOC command or a
    // STARTFDOC command, depending on whether we are using preload or fifo mode.

    if(scanning) {
        CommandBuffer[0] = PCMD_STARTSCAN;	// Command ID
        CommandBuffer[2] = SD_FWD;			// Print the first swath in the forward direction
    } else {
        // Setup the command buffer with the STARTDOC command
        if(JT_PRELOAD == jobtype) {
            CommandBuffer[0] = PCMD_STARTPDOC;	// Command ID
            CommandBuffer[2] = ncopies;			// Number of copies of the document to print
        } else {
            CommandBuffer[0] = PCMD_STARTFDOC;	// Command ID
            CommandBuffer[2] = docid;			// ID of this document
        }
    }
    CommandBuffer[1] = 1;				// 1 more parameter for any of these commands

    puts("Starting Document");
    do {
        retval = PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
    } while (RVAL_FULL == retval) ;

    if(RVAL_OK != retval) {
        puts("Error, StartDoc failed");
    }
    // Look out for Monitor log message:  "Rx:StartPreloadDoc" or "Rx:StartFifoDoc" or "Rx:StartScan" 
}

//---------------------------------------------------------------------------
void SendImage(uint32* buffer) {
    eRET retval;

    //
    // Commands and print data are buffered in several places - in the hardware, in the Print Engine, 
    // and in the Printer Interface.  
    // If print data is not being consumed by the print heads, eventually these buffers will fill up.
    // When this happens, the application should retry the command.
    // Normally an application would also have a mechanism for breaking out of this loop if it decides 
    // to abort the print job; this is not present in this example for simplicity.
    //
    do {
        retval = PiSendCommand(buffer);
    } while (RVAL_FULL == retval) ;

    if(RVAL_OK != retval) {
        puts("Error, Send Image Failed");
    }
    // Look out for Monitor log message:  "Rx:Image"
}

//---------------------------------------------------------------------------
void ScanSendSecondDoc(uint32* SecondBuffer) {
    uint32 CommandBuffer[6];
    eRET retval;

    CommandBuffer[0] = PCMD_STARTSCAN;	// Command ID
    CommandBuffer[1] = 1;				// 1 more parameter for StartScan command
    CommandBuffer[2] = SD_REV;			// Print the second swath in the reverse direction

    puts("Starting Second Swath");
    do {
        retval = PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
    } while ( RVAL_FULL == retval ) ;

    if(RVAL_OK != retval) {
        puts("Error, Start failed");
    }

    // Send the image.  
    puts("Sending Second Swath");
    SendImage(SecondBuffer);

    // Finished sending images, send the end doc command
    EndDoc();
}

//---------------------------------------------------------------------------
void EndDoc(void) {
    uint32 CommandBuffer[6];
    eRET retval;

    CommandBuffer[0] = PCMD_ENDDOC;		// Command ID
    CommandBuffer[1] = 0;				// No parameters for ENDDOC command

    puts("Sending EndDoc");
    do {
        retval = PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
    } while ( RVAL_FULL == retval ) ;

    if(RVAL_OK != retval) {
        puts("Error, EndDoc failed");
    }
    // Look out for Monitor log message:  "Rx:EndDoc"
}

//---------------------------------------------------------------------------
void EndJob(void) {
    uint32 CommandBuffer[6];
    eRET retval;

    CommandBuffer[0] = PCMD_ENDJOB;		// Command ID
    CommandBuffer[1] = 0;				// No parameters for ENDJOB command

    puts("Sending EndJob");
    do {
        retval = PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
    } while ( RVAL_FULL == retval ) ;

    if(RVAL_OK != retval) {
        puts("Error, EndJob failed");
    }
    // Look out for Monitor log message:  "Rx:EndJob"
}

//---------------------------------------------------------------------------
// Check if the jobtype is compatible with scanning.  Either path can be used for
// unidirectional printing, but scanning mode requires that the fifo path is
// used.
int CheckScan(int scanning,DWORD jobtype, _TCHAR *bitmap2, uint32** SecondBuffer) {

    int ok = 1;

    if(scanning) {
        if(JT_FIFO != jobtype) {
            puts("Error: Scanning printing requires using fifo path:");
            puts("Use the option -path=fifo");
            ok = 0;
        } else {
            *SecondBuffer = MakeBitmap(bitmap2);
            if(NULL == SecondBuffer) {
                printf("Error, unable to open input file: \"%S\"\n",bitmap2);
                ok = 0;
            }
        }
    }
    return ok;
}

//---------------------------------------------------------------------------
// Send an abort, and wait for up to 5 seconds for the printer to become
// idle again
void SendAbort(void) {

    puts("Sending Abort");
    eRET retval = PiAbort();
    if(RVAL_OK != retval) {
        puts("Error, Abort failed");
    }

    // Now check the printer status, and wait until it is idle
    int starttime = clock();
    bool busy = true;
    while((clock() - starttime) < 5000) {
        if(!PiIsBusy()) {
            busy = false;
            break;
        }
    }
    if(busy) {
        puts("Error, Too long to become idle after abort");
    }

}

//---------------------------------------------------------------------------
void PrintUsage(void) {
    puts("Usage: SamplePrint <bitmap1> [options]\n");
    puts("Options:\n");
    puts("-path=<fifo|preload> Use the fifo or the preload path.  Default is preload.");
    puts("-copies=<ncopies>    Number of copies to print from PCC memory on the preload path.");
    puts("                     Number of copies to send from the application on the fifo path.");
    puts("                     Default is 1.");
    puts("-scan=<bitmap2>      Scanning printing.  Prints two swaths, with the");
    puts("                     second swath being taken from <bitmap2>.");
    puts("                     Only available with fifo path.");
}

