/* SamplePrintCLS.cpp
 *
 * Copyright (c) 2008-2012, The Technology Partnership Plc
 */

// Simple example program for printing using Meteor
// 
// For instructions on usage, run the program with no arguments.
// PrinterInterfaceCLS.dll must be present in the same directory as SamplePrintCLS.exe
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
#include <stdio.h>
#include <time.h>

// Utility functions for handling bitmaps and generating image commands in the correct format
#include "Bitmaps.h"

using namespace Ttp;
using namespace Meteor;
using namespace System;

// Function declarations
void InitialisePrinter(int scanning, DWORD bitsPerPixel);
void StartJob(DWORD jobid, eJOBTYPE jobtype, DWORD res, DWORD docwidth);
void Start(int scanning, eJOBTYPE jobtype, DWORD ncopies, DWORD docid);
void SendImage(DWORD *buffer);
void ScanSendSecondDoc(DWORD *SecondBuffer);
void EndDoc(void);
void EndJob(void);
void CheckStatus(void);
void SendAbort(void);

void ParseArguments(int argc, 
					_TCHAR* argv[], 
					eJOBTYPE *jobtype, 
					DWORD *ncopies, 
					int *scanning,
					_TCHAR **bitmap2,
					DWORD* pBitsPerPixel);

int CheckScan(int scanning,eJOBTYPE jobtype, _TCHAR *bitmap2, DWORD **SecondBuffer);
void PrintUsage(void);

int _tmain(int argc, _TCHAR* argv[])
{

	_TCHAR *bitmap1 = NULL, *bitmap2 = NULL;
	DWORD *ImageBuffer = NULL, *SecondBuffer = NULL;

	eRET retval; // For return values from function calls

	DWORD jobid = 0;					// Default to 0
	eJOBTYPE jobtype = eJOBTYPE::JT_PRELOAD;			// Default to Preload path
	DWORD res = (DWORD) eRES::RES_HIGH;	// Default to high resolution
	DWORD docwidth = 3508;				// Default to A4 length @ 300dpi
	DWORD ncopies = 1;					// Default to 1 copy of the document
	DWORD docid = 1;					// Default to a document ID of 1
	int scanning = 0;					// Default to not scanning
	DWORD bitsPerPixel = 0;				// Default to existing print engine setting

	// Parse the input arguments

	// The first argument is the filename of the bitmap to print
	if(argc < 2) {
		PrintUsage();
	}
	else {
		bitmap1 = argv[1];


		ParseArguments(argc, 
						argv, 
						&jobtype, 
						&ncopies, 
						&scanning,
						&bitmap2,
						&bitsPerPixel);
						
		// Connect to the printer first
		retval = PrinterInterfaceCLS::PiOpenPrinter();	// Open the printer interface
		// Look out for debug message:  "Application Attached"

		if(eRET::RVAL_OK != retval) {
			printf("Error, unable to open printer interface...is Monitor.exe running?\n");
		}
		else {

			// Printer interface opened ok
			printf("Printer interface opened\n");

			// Attempt to open the file and create the image command, for later use.
			ImageBuffer = MakeBitmap(bitmap1);

			if (NULL == ImageBuffer) {
				printf("Error, unable to open input file: \"%S\"\n",bitmap1);
			}
			else {
				if(CheckScan(scanning,jobtype,bitmap2,&SecondBuffer)) {
					// Everything ok, so start printing now

					// Reset the printer
					InitialisePrinter(scanning, bitsPerPixel);

					// Specify the parameters of the job
					StartJob(jobid, jobtype, res, docwidth);

					// Start the document
					Start(scanning,jobtype,ncopies, docid);

					// Send the image.  We already created the header when we generated the ImageBuffer,
					// so all we need to do here is send the ImageBuffer
					printf("Sending Image\n");
					SendImage(ImageBuffer);			

					// Finished sending images, send the end doc command
					EndDoc();

					// If using scanning printing, send the second document
					if(scanning) {
						ScanSendSecondDoc(SecondBuffer);
					}
				
					// Finished sending the document, send the end job command
					EndJob();

					// Check the printer status
					CheckStatus();

					if(SecondBuffer) free(SecondBuffer);
				}
				free(ImageBuffer);
			}
			
			// All done now, close the printer interface, by calling PiClosePrinter until the interface closes
			printf("Closing Printer Interface\n");
			while (eRET::RVAL_OK != PrinterInterfaceCLS::PiClosePrinter()) {}
			// Look out for debug message: "Application Detatched"
		}
	}
	printf("Press Enter to continue...\n");
	getchar();
	return 0;
}


// Parse the input arguments to extract the options 
void ParseArguments(int argc, 
					_TCHAR* argv[], 
					eJOBTYPE *jobtype, 
					DWORD *ncopies, 
					int *scanning,
					_TCHAR **bitmap2,
					DWORD* pBitsPerPixel) {

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
				*jobtype = eJOBTYPE::JT_FIFO;
			}
			else if(0 == wcscmp(value,L"preload")) {
				*jobtype = eJOBTYPE::JT_PRELOAD;
			}
			else {
				printf("Warning: Invalid path, defaulting to Preload\n");
			}
		}

		// Check for the presence of the "-copies" option
		value = wcsstr(argv[i],L"-copies=");
		if(NULL != value) {
			matched = 1;
			value += wcslen(L"-copies=");	// Move the pointer to the start of the argument
			int argument = _wtoi(value);
			if(argument <= 0) {
				printf("Warning, invalid number of copies, defaulting to 1\n");
			}
			else {
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

		// Check for the presence of the "-bpp" option
		value = wcsstr(argv[i],L"-bpp=");
		if(NULL != value) {
			matched = 1;
			value += wcslen(L"-bpp=");	// Move the pointer to the start of the argument
			*pBitsPerPixel = _wtoi(value);
		}

		// Check if none of the commands matched a valid option
		if(!matched) {
			printf("Invalid argument \"%S\" ignored\n",argv[i]);
		}
	}
}

// Clear out any old data, and setup the printer to a known state in order to 
// be ready to print
void InitialisePrinter(int scanning, DWORD bitsPerPixel) {
	eRET retval;

	// Send an abort to clear out any old data
	SendAbort();
	// Look out for debug message: "Output: Abort"

	// In scanning mode, we need to zero the X-position at the home position.
	// In a real application, the transport would be moved to the home position
	// before calling this command
	if(scanning) {
		printf("Setting Home\n");
		retval = PrinterInterfaceCLS::PiSetHome();
		if(eRET::RVAL_OK != retval) {
			printf("Error, Set Home failed\n");
		}
		// Look out for debug message: "PCCx X-counters cleared"
		// Note this may cause an error message if the printer is not configured
		// for scanning mode.  Set Scanning = 1 in the config file.
	}


	// Before sending any print commands, turn on the head power
	// A real application should turn off the head power when printing has finished. 
	// For simplicity this is ignored here, and the heads are left on.
	printf("Turning on head power\n");
	retval = PrinterInterfaceCLS::PiSetHeadPower(1);
	if(eRET::RVAL_OK != retval) {
		printf("Error, Set Head Power failed\n");
	}
	// Look out for debug message: "Switching HDC and Head Power ON"

	if( bitsPerPixel > 0 ) {

		printf("Setting BitsPerPixel to %d", bitsPerPixel);
		eRET rval = PrinterInterfaceCLS::PiSetAndValidateParam((int)eCFGPARAM::CCP_BITS_PER_PIXEL, bitsPerPixel);
		if( eRET::RVAL_OK == rval ) {
			printf("\n");
		} else {
			printf(" ... FAILED\n");
		}
	}
}

void StartJob(DWORD jobid, eJOBTYPE jobtype, DWORD res, DWORD docwidth) {
	array<Int32,1>^CommandBuffer = gcnew array<Int32>(6);
	eRET retval;

	// Setup the command buffer with the STARTJOB command
	CommandBuffer[0] = (DWORD) CtrlCmdIds::PCMD_STARTJOB;	// Command ID 
	CommandBuffer[1] = 4;				// 4 more parameters for STARTJOB command
	CommandBuffer[2] = jobid;			// Job ID for tracking purposes
	CommandBuffer[3] = (Int32) jobtype; // Preload path used
	CommandBuffer[4] = res;				// Print Resolution
	CommandBuffer[5] = docwidth;		// Width of document, only used when printing right-to-left

	printf("Sending StartJob\n");

	// Here we send the StartJob command repeatedly, until the Printer Interface
	// is not busy, and accepts the command.  Normally the return value 
	// should be checked for every command, but we do not do it every time in this
	// example to keep the example simple.
	do {
		retval = PrinterInterfaceCLS::PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
	} while(retval == eRET::RVAL_BUSY);
	// Look out for debug message:  "StartJob"
}


void Start(int scanning, eJOBTYPE jobtype, DWORD ncopies, DWORD docid) {
	array<Int32,1>^CommandBuffer = gcnew array<Int32>(6);
	eRET retval;

	// Send the StartDoc or StartScan Command.  
	// For scanning printing, we use StartScan.  Otherwise we use a STARTPDOC command or a
	// STARTFDOC command, depending on whether we are using preload or fifo mode.

	if(scanning) {
			CommandBuffer[0] = (DWORD) CtrlCmdIds::PCMD_STARTSCAN;	// Command ID
			CommandBuffer[2] = (DWORD) eSCANDIR::SD_FWD;			// Print the first swath in the forward direction
	}
	else {
		// Setup the command buffer with the STARTDOC command
		if(eJOBTYPE::JT_PRELOAD == jobtype) {
			CommandBuffer[0] = (DWORD) CtrlCmdIds::PCMD_STARTPDOC;	// Command ID
			CommandBuffer[2] = ncopies;			// Number of copies of the document to print
		}
		else {
			CommandBuffer[0] = (DWORD) CtrlCmdIds::PCMD_STARTFDOC;	// Command ID
			CommandBuffer[2] = docid;			// ID of this document
		}
	}
	CommandBuffer[1] = 1;				// 1 more parameter for any of these commands command

	printf("Starting Document\n");
	retval = PrinterInterfaceCLS::PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
	if(eRET::RVAL_OK != retval) {
		printf("Error, Start failed\n");
	}
	// Look out for debug message:  "StartPreloadDoc" or "StartFifoDoc" or "StartScan" 
}

void SendImage(DWORD *buffer) {
	eRET retval;

	// Copy the data into a managed buffer.  Required to interface with
	// the CLI
	int length = buffer[1] + 6;	// Allow for the size of the header as well as the payload
	array<Int32>^ ManagedBuffer = gcnew array<Int32>(length);
	System::Runtime::InteropServices::Marshal::Copy((IntPtr)buffer, ManagedBuffer, 0, length);

	retval = PrinterInterfaceCLS::PiSendCommand(ManagedBuffer);
	if(eRET::RVAL_OK != retval) {
		printf("Error, Send Image Failed\n");
	}
	// Look out for debug message:  "Image"
}

void ScanSendSecondDoc(DWORD *SecondBuffer) {
	array<Int32,1>^CommandBuffer = gcnew array<Int32>(6);
	eRET retval;

		CommandBuffer[0] = (DWORD) CtrlCmdIds::PCMD_STARTSCAN;	// Command ID
	CommandBuffer[1] = 1;				// 1 more parameter for StartScan command
	CommandBuffer[2] = (DWORD) eSCANDIR::SD_REV;			// Print the second swath in the reverse direction

	printf("Starting Second Swath\n");
	retval = PrinterInterfaceCLS::PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
	if(eRET::RVAL_OK != retval) {
		printf("Error, Start failed\n");
	}

	// Send the image.  
	printf("Sending Second Swath\n");
	SendImage(SecondBuffer);

	// Finished sending images, send the end doc command
	EndDoc();
}

void EndDoc(void) {
	array<Int32,1>^CommandBuffer = gcnew array<Int32>(6);
	eRET retval;
	
	CommandBuffer[0] = (Int32) CtrlCmdIds::PCMD_ENDDOC;		// Command ID
	CommandBuffer[1] = 0;						// No parameters for ENDDOC command
	
	printf("Sending EndDoc\n");
	retval = PrinterInterfaceCLS::PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
	if(eRET::RVAL_OK != retval) {
		printf("Error, EndDoc failed\n");
	}
	// Look out for debug message:  "EndDoc"
}

void EndJob(void) {
	array<Int32,1>^CommandBuffer = gcnew array<Int32>(6);
	eRET retval;

	CommandBuffer[0] = (DWORD) CtrlCmdIds::PCMD_ENDJOB;		// Command ID
	CommandBuffer[1] = 0;				// No parameters for ENDJOB command

	printf("Sending EndJob\n");
	retval = PrinterInterfaceCLS::PiSendCommand(CommandBuffer);		// Send the command to the Printer Interface
	if(eRET::RVAL_OK != retval) {
		printf("Error, EndJob failed\n");
	}
	// Look out for debug message:  "EndJob"
}

// Check if the jobtype is compatible with scanning.  Either path can be used for
// unidirectional printing, but scanning mode requires that the fifo path is
// used.
int CheckScan(int scanning,eJOBTYPE jobtype, _TCHAR *bitmap2, DWORD **SecondBuffer) {

	int ok = 1;

	if(scanning) {
		if(eJOBTYPE::JT_FIFO != jobtype) {
			printf("Error: Scanning printing requires using fifo path:\n");
			printf("Use the option -path=fifo\n");
			ok = 0;
		}
		else {
			*SecondBuffer = MakeBitmap(bitmap2);
			if(NULL == *SecondBuffer) {
				printf("Error, unable to open input file: \"%S\"\n",bitmap2);
				ok = 0;
			}
		}
	}
	return ok;
}

void CheckStatus(void) {
	TAppStatus AppStatus;
	TAppPccStatus  PccStatus;
	TAppHeadStatus HeadStatus;

	eRET retVal;

	// These calls do not do anything in this sample application.  They are provided only
	// to show how they can be used to request status information from the Print Engine.
	// Using a debugger, it's possible to see what status information was returned.  
	retVal = PrinterInterfaceCLS::PiGetPrnStatus(AppStatus);
	retVal = PrinterInterfaceCLS::PiGetPccStatus(1,PccStatus);
	retVal = PrinterInterfaceCLS::PiGetHeadStatus(1,1,HeadStatus);
}

// Send an abort, and wait for up to 5 seconds for the printer to become
// idle again
void SendAbort(void) {


	eRET retval;

	printf("Sending Abort\n");
	retval = PrinterInterfaceCLS::PiAbort();
	if(eRET::RVAL_OK != retval) {
		printf("Error, Abort failed\n");
	}

	// Now check the printer status, and wait until it is idle
	int starttime = clock();
	bool busy = true;
	while((clock() - starttime) < 5000) {
		if(!PrinterInterfaceCLS::PiIsBusy()) {
			busy = false;
			break;
		}
	}
	if(busy) {
		printf("Error, Too long to become idle after abort\n");
	}

}

void PrintUsage(void) {
	printf("Usage: SamplePrintCLS <bitmap1> [options]\n\n");
	printf("Options:\n\n");
	printf("-path=<fifo|preload> Use the fifo or the preload path.  Default is preload.\n");
	printf("-copies=<ncopies>    Number of copies to print on the preload path.\n");
	printf("                     Default is 1.\n");
	printf("-scan=<bitmap2>      Scanning printing.  Prints two swaths, with the\n");
	printf("                     second swath being taken from <bitmap2>.\n");
	printf("                     Only available with fifo path.\n");
	printf("-bpp=<bitsPerPixel>  Bits per pixel, up to maximum supported by head type.\n");
}

