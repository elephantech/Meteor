MeteorSwathPartialBuffersSample
-------------------------------

This sample code demonstrates how to use the partial buffers functionality of the MeteorSwath API
to achieve a seamless print of multiple images in the Y direction.

-------------------------------------------------------------------------------------------------

MeteorSwathPartialBuffersSample.exe is a command line application which takes the following
parameters:

[Optional] -g=N               selects swath geometry index N from the configuration file.  Default is 1
[Optional] -h=false           tells the application whether Meteor PCCs are going to be connected.  Default is true
[Mandatory] TIFFPATH1.tif     path to a TIFF file to print
[Optional] TIFFPATH2.tif etc. path to additional TIFF files to print
[Optional] -p                 operate in per plane mode. Requires a tif file per plane of the form 
                              TIFFPATH1_Clr1.tif, TIFFPATH1_Clr2.tif, ... in the same folder.
                              The command line image input should be TIFFPATH1 (without ".tif" or "_Clr1", etc.).

The TIFF files are printed in sequence, 20 times

The print job can be aborted by pressing CTRL+C

-------------------------------------------------------------------------------------------------

The project's user settings (MeteorSwathPartialBuffersSample.csproj.user) assume that the Meteor 
ScanEngine SDK has been installed to its default location in c:\program files\meteor inkjet\meteor.

If the installation is in a different folder, the reference path should be changed accordingly.

Note that the reference path 'c:\program files\meteor inkjet\meteor\api\x86' is used for both the
32 bit and 64 bit build.  

This is fine for compiling the code, because the CLS assemblies which are referenced are
compiled as "ANY CPU", so the CLS files in \x86 and \amd64 are identical.

However, the correct \x86 or \amd64 folder MUST be selected for running the application, 
depending on whether the application is 32 bit or 64 bit, so that the correct unmanaged
Meteor componenets are found.  This is done by the MeteorPath object, based on values which
the ScanEngine installer writes into the registry.
