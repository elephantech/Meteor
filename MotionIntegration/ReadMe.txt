-----------------------------------------
Meteor Motion Integration (MMI) Framework
-----------------------------------------
The MMI framework allows a motion control plugin module to be developed for use with Meteor MetScan


----------------
Package contents
----------------
The zip file contains the following folders and files:

\---MotionIntegration                                       | Parent folder
    |   JobSequenceStatus.txt                               | Technical note describing the JobSequence start/stop/pause/resume status timing
    |   Licence.txt                                         | Third party module licence information
    |   MotionIntegrationHelp.chm                           | MMI API documentation
    |   ReadMe.txt                                          | This file
    |   
    +---bin                                                 | Folders for binary files
    |   +---x64                                             | 64 bit plugin output folder and server application
    |   |       MotionIntegrationCLS.dll
    |   |       MotionIntegrationCLS.xml                    | The MotionIntegrationHelp.chm information formatted for Visual Studio Intellisense
    |   |       SwathIPCServer.exe
    |   |       Newtonsoft.Json.dll                         | MIT licensed JSON parser (see Licence.txt)
    |   |       Newtonsoft.Json.xml
    |   |       
    |   \---x86                                             | 32 bit plugin output folder and server application
    |           MotionIntegrationCLS.dll
    |           MotionIntegrationCLS.xml                    | The MotionIntegrationHelp.chm information formatted for Visual Studio Intellisense
    |           SwathIPCServer.exe
    |           Newtonsoft.Json.dll                         | MIT licensed JSON parser (see Licence.txt)
    |           Newtonsoft.Json.xml
    |           
    +---MotionController                                    | Unmanaged (native) plugin framework sample source code
    |       MotionController.sln                            | Solution file used to build MotionController.dll
    |       MotionController.vcxproj
    |       MotionController.vcxproj.filters
    |       MotionController.vcxproj.user
    |       CustomSettingsExample.cpp
    |       CustomSettingsExample.h
    |       JobGroupSizeCommand.cpp
    |       JobGroupSizeCommand.h
    |       json11.cpp                                      | MIT licensed JSON parser (see Licence.txt)
    |       json11.hpp                                      | MIT licensed JSON parser (see Licence.txt)
    |       MotionControllerDll.cpp
    |       MotionControllerDll.h
    |       MotionResult.h
    |       ProgressInfo.cpp
    |       ProgressInfo.h
    |       TemperatureInfo.cpp
    |       TemperatureInfo.h
    |       TemperatureInfos.cpp
    |       TemperatureInfos.h
    |       UvLampSwathCommand.cpp
    |       UvLampSwathCommand.h
    |           
    +---MotionControllerCLS                                 | Managed plugin framework sample source code
    |   |   MMI_Example.cs
    |   |   MotionControllerCLS.csproj
    |   |   MotionControllerCLS.csproj.user
    |   |   MotionControllerCLS.sln                         | Solution file used to build MotionControllerCLS.dll
    |   |   
    |   \---Properties
    |           AssemblyInfo.cs
    |           
    \---StartApplication                                    | Test application and supporting assemblies
            ManualMotionControl.exe
            MotionIntegrationCLS.dll
            SwathIPCClient.dll
            AppKit.Translations.dll
            AppKit.Utilities.dll
            MeteorWpf.dll
            System.IO.Abstractions.dll                      | Third party MIT licenced assembly (see Licence.txt)
            System.Windows.Interactivity.dll                | Microsoft Expression Blend redistributable component
            

---------------------
MotionIntegration\bin
---------------------
An MMI plugin can be written either as a 32 bit or as a 64 bit plugin.

This is completely independent of whether the controlling application is 32 bit or 64 bit.

SwathIPCServer.exe is an application which is used to launch the MMI plugin.

"SwathIPCServer.exe -m" looks for a managed MMI plugin in the folder
"SwathIPCServer.exe -0" attempts to run the unmanaged plugin in the folder
"SwathIPCServer.exe -1" is used to launch the driver for the Meteor MPC (Motor & Pump Control) hardware


----------------------------------
MotionIntegration\MotionController
----------------------------------
Framework sample source code for an unmanaged (native) MMI plugin, which can be written in C or C++.

The project is built using Visual Studio 2010 or above, using the MotionController.sln solution.

The output file MotionController.dll is built to 
- the MotionIntegration\bin\x86 folder, if the 32 bit build configuration is selected
- the MotionIntegration\bin\x64 folder, if the 64 bit build configuration is selected


-------------------------------------
MotionIntegration\MotionControllerCLS
-------------------------------------
Framework sample source code for a managed (.NET) MMI plugin, written in C#.

The project is built using Visual Studio 2010 or above, using the MotionControllerCLS.sln solution.

The output file MotionControllerCLS.dll is built to 
- the MotionIntegration\bin\x86 folder, if the 32 bit build configuration is selected
- the MotionIntegration\bin\x64 folder, if the 64 bit build configuration is selected


----------------------------------
MotionIntegration\StartApplication
----------------------------------
Test application allowing commands to be manually sent to the plugin from the user interface.

It allows all of the commands which are used by MetScan to be tested.


---------------
Getting started
---------------


(1) Decide whether the plugin will be written as unmanaged or managed code

    Open the appropriate solution in Microsoft Visual Studio

    -- MotionIntegration\MotionControllerCLS\MotionControllerCLS.sln for a managed plugin
    -- MotionIntegration\MotionController\MotionController.sln for an unmanaged plugin



(2) Decide whether the plugin will be 32 bit or 64 bit

    Select the corresponding "Active solution platform" in the Visual Studio "Configuration Manager"

    (The Configuration Manager can be found on the Build menu in Visual Studio)



(3) Rebuild the project

    The output files should appear in either MotionIntegration\bin\x86 or MotionIntegration\bin\x64



(4) Run StartApplication\ManualMotionControl.exe

    In the initial dialog box, select the configuration which correponds to the choices made in (1) and (2) above

    Server Platform
    ---------------

    - "32 bit (x86) using SwathIPCServer.exe" for 32 bit
    - "64 bit (x86) using SwathIPCServer.exe" for 64 bit

    Plugin
    ------

    - MotionController.dll for an unmanaged plugin
    - IMotion for a managed plugin

    Make sure "Show console" is ticked

    Press OK

    This should launch a Windows Console which should display text similar to the below:

        ----------------------------------------------------------------

        Starting Swath IPC Server 32 bit
        IMotion type is Ttp.Meteor.MotionIntegration.MotionController
        SwathIPCClient connected

        ----------------------------------------------------------------

    An error message will be displayed if the plugin has not been built or if the configuration does not match the built plugin.



(5) Press the "Initialise" button in the application

    You should see text like the following appear in the Windows Console:

        
        For the Unmanaged plugin .... ----------------------------------

        IPC Cmd: MMI_Initialise
        ---MotionDLL---
        Initialise ''
        ---------------
        IPC Resp: MMI_Initialise MRES_UNIMPL

        ----------------------------------------------------------------

        For the Managed plugin .... ------------------------------------

        IPC Cmd: MMI_Initialise
        --- MMI_Example ---

        MMI_Initialise

        -------------------
        IPC Resp: MMI_Initialise MRES_UNIMPL

        ----------------------------------------------------------------


    This confirms that the test application is communicating with the plugin



----------------------------------------
Steps for debugging the framework plugin
----------------------------------------

The sample visual studio projects are set up to launch both the client and the server side for ease of debuuging.

To confirm this:

(1) Set a breakpoint in the Initialise function

    This can be found in MMI_Example.cs for the managed plugin, or MotionControllerDll.cpp for the native plugin

(2) Start Debugging (e.g. press f5)

    The console and the test application should both be launched

(3) Press the application's "Initialise" button

    The breakpoint should be hit


-------------------------------
Developing the framework plugin
-------------------------------

The comments in each empty function, and the markup comments in MotionIntegrationCLS, describe what each function should do

