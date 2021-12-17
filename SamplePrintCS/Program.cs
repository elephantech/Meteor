/* SamplePrintCS / Program.cs
 *
 * Copyright (c) 2010-2013, The Technology Partnership Plc
 */

// Simple example program for printing using Meteor, written in C#
// 
// For instructions on usage, run the program with no arguments
//
// This code demonstrates printing a single document using the FIFO or
// Preload path. It can also print two swaths with scanning printing.
//
// By default, this program will print a single image on the Preload path.
// The user can specify the file to be printed, and override the default options,
// from the command line.
//
// Many of the commands in this application are followed by explanations of
// what to look  for in the Monitor debug window, when the command executes

namespace SamplePrintCS {
    class Program {
        static int Main(string[] args) {// Main entry point for program
            // Assembly dependencies used during a method are loaded at the point
            // where the method is called.
            //
            // So we don't call SamplePrintCS directly from Main, to give us the 
            // opportunity to locate the Meteor assemblies first.
            //
            MeteorInkjet.MeteorPath.LocatePrinterInterface();
            return DoPrint(args);
        }

        static int DoPrint(string[] args) {
            return new SamplePrintCS().Print(args);
        }
    }
}
