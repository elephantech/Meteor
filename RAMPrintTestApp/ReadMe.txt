RAMPrintTestApp.csproj.user assumes that Meteor has been installed to its default location in c:\program files.

If Meteor is installed to a different location, the project's "Reference Paths" setting should be changed accordingly.

RAMPrintTestApp demonstrates how to use PCMD_WRITE_RAM_IMAGE_EX and PCMD_PRINT_RAM_IMAGE.

In this mode, the application takes over managing when images are loaded into PCC RAM.

- ImageControl.buttonLoadImage_Click shows how to use PCMD_WRITE_RAM_IMAGE_EX
 
- ImageControl.buttonPrint_Click shows how to use PCMD_PRINT_RAM_IMAGE

PCMD_WRITE_RAM_IMAGE_EX can be used to reserve additional memory for an image reference slot the first time a slot is written during a print job

N.B. PCMD_WRITE_RAM_IMAGE is deprecated and should no longer be used 