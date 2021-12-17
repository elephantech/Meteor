SdCardTestApp.csproj.user assumes that Meteor has been installed to its default location in c:\program files.

If Meteor is installed to a different location, the project's "Reference Paths" setting should be changed accordingly.

SdCardTestApp demonstrates how to use the PCMD_CF_TRANSLATEIMAGE, PCMD_CF_STOREIMAGE and PCMD_CF_HDIMAGE_BLK commands
to store image data on, and print data from, the PCCE's UHS SD card.

To enable the UHS SD card, the following configuration file parameter should be present:

[UhsSd]
EnableUhsSdCard          = 1

- ImageControl.buttonLoadImage_Click demonstrates how to use PCMD_CF_TRANSLATEIMAGE and PCMD_CF_STOREIMAGE
 
- ImageControl.buttonPrint_Click shows how to use PCMD_CF_HDIMAGE_BLK

The UHS SD card contains no file system; block allocation is the application's responsibility.

SdCardTestApp divides the available memory into 8 head driver lanes, with an image slot size determined by the
value of numericUpDownSlotWidth.

The block addresses are calculated by the SdCardSlot object.

Note that the SdCardTestApp slot address calculations assume that the head type is Dimatix Starfire.

It is also possible to use a mixed mode (dual fifo) to combine print data stored on the SD card and print data
sent from the PC during the print job.  The code which demonstrates this mode can be found in 
class FifoImageControl and by looking for JT_DUAL_FIFO in FormSdCardTestApp.cs.

---------------------------------------------------------------------------------------------------------------

A test mode is available which can store the translated print data on the local hard drive instead of
on the PCCE UHS SD card.  This mode is referred to as the "Virtual Compact Flash (CF)", or "Simulated CF".
A file per PCC named "PccN.bin" is used for the data which is normally written to flash memory.

When PCMD_CF_HDIMAGE_BLK is used to print image data from the Virtual CF, internally to the Meteor PrintEngine
it is effectively sent through the standard FIFO datapath.

This means that the PC to PCC bandwidth can be a limiting factor when printing with this mode, making it
inappropriate for a production printer environment.  However, the Virtual CF mode can be useful during 
development because it allows SimPrint (.sim) files to be generated during a print job and without
the need for PCC hardware to be physically connected.  When using the actual UHS SD cards it is not 
possible to generate .sim files because the print data has already been transferred from the PC to 
the PCC before the PCMD_CF_HDIMAGE_BLK command is sent.

The Virtual CF is enabled using the following configuration file parameters:

[SimFlash]
SimFlashFolder           = "C:\ProgramData\MeteorInkjet"       ; Directory where the PccN.bin files are stored
SimFlashSizeBlocks       = 2097152                             ; The size of each simulated CF card in 512-byte blocks

---------------------------------------------------------------------------------------------------------------
