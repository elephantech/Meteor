To build and run SamplePrint you must first:

(1) Add the path to the Meteor header file folder to the project.

    In Visual Studio 2010 this can be done through the Project Properties, 
    in the "Additional Include Directories" field in the 
    "Configuration Properties | C/C++ | General" page.

    The header files are found in the Api folder of your Meteor installation.

    Typically this is within your program files folder, e.g. "C:\Program Files\Meteor Inkjet\Meteor\Api".

(2) Add the path to the Meteor library files folder to the project.

    In Visual Studio 2010 this can be done through the Project Properties,
    in the "Additional Library Directories" field 
    in the "Configuration Properties | Linker | General" page.

    The 32 bit library files are found in the Api\x86 folder of your Meteor installation.

    Typically this is within your program files folder, e.g. "C:\Program Files\Meteor Inkjet\Meteor\Api\x86".

( SamplePrint can also be built as a 64 bit application by adding an "x64" project platform
  through the configuration manager.  The 64 bit library files are found in the
  Api\amd64 folder of your Meteor installation. )

(3) Add an executable path to the 32 bit or 64 bit Meteor library files folder

    This can be done by adding the path to the PC's environment.

    Alternatively, for debugging through Visual Studio 2010, the path can be added
    to the debug environment by adding the path the Project Properties,
    in the "Configuration Properties | Debugging" page "Environment" fields.

    e.g. PATH=%PATH%;C:\Program Files\Meteor Inkjet\Meteor\Api\x86

    The SamplePrint Command Arguments can also be set in the "Configuration Properties | Debugging"
    page

( For a 64 bit build the equivalent is PATH=%PATH%;C:\Program Files\Meteor Inkjet\Meteor\Api\amd64 )


