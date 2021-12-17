MeteorSwathQueuedJobsSample
---------------------------

This sample code demonstrates how to use the MeteorSwath API queued jobs functionality

-------------------------------------------------------------------------------------------------

MeteorSwathQueuedJobsSample.exe is a command line application which takes a set of
<geometry index, tiff file path> parameter pairs on the command line.

e.g. the following queues 3 jobs using geometry indexes 1,2 and 3
    
    MeteorSwathQueuedJobsSample.exe 1 c:\File1.Tif 2 c:\File2.Tif 3 c:\File3.Tif

The same TIFF file is used for all colour planes.

Additional optional parameters are:

    -f=N        set the internal print clock frequency of the PCC to N Hz

The print job can be aborted by pressing CTRL+C

-------------------------------------------------------------------------------------------------

The project's user settings (MeteorSwathQueuedJobsSample.csproj.user) assume that the Meteor 
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
