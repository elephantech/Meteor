﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Ttp.Meteor;

namespace MeteorInkJet.SdCardTestApp {
    /// <summary>
    /// Interface used by the image controls to find out about the printer setup
    /// </summary>
    public interface IPrintDetails {
        /// <summary>
        /// Selected print resolution
        /// </summary>
        int UserBitsPerPixel { get; }
        /// <summary>
        /// Is a Meteor print job active
        /// </summary>
        bool JobActive { get; }
        /// <summary>
        /// Are the SD cards (or other image store type) present
        /// </summary>
        bool SdCardsPresent { get; }
    }

    /// <summary>
    /// Main application form
    /// </summary>
    public partial class FormSdCardTestApp : Form, IPrintDetails {
        /// <summary>
        /// Object handles Meteor connection and status
        /// </summary>
        private PrinterStatus _status = new PrinterStatus();
        /// <summary>
        /// Last known printer status
        /// </summary>
        private PRINTER_STATUS _latestPrinterStatus = PRINTER_STATUS.DISCONNECTED;
        /// <summary>
        /// Prevent re-entrancy problems in the timers - can happen if 
        /// a message box is displayed.
        /// </summary>
        private bool _inTimer;
        /// <summary>
        /// Prevent re-entrancy if PiSetHeadPower fails and the check box
        /// value is reset from within the check changed handler; also used
        /// if we change the head power check box state based on the Meteor
        /// status
        /// </summary>
        private bool _inSetHeadPower;
        /// <summary>
        /// The last known value of the bits-per-pixel modes supported by Meteor
        /// This can change if the Meteor head type is changed
        /// </summary>
        private int _lastSupportedBppBitmask = 0;
        /// <summary>
        /// ID of the latest meteor print job.  Incremented for each job.
        /// </summary>
        private int _jobid = 500;
        /// <summary>
        /// Set after we abort a print job and cleared when the Meteor status 
        /// changes to idle.
        /// </summary>
        private bool _jobAborting = false;
        /// <summary>
        /// Set if a print job is active
        /// </summary>
        private bool _jobActive = false;
        /// <summary>
        /// Array of the image control tab pages within the form
        /// </summary>
        private TabPage[] _pages;
        /// <summary>
        /// List of the image controls, one for each tab page
        /// </summary>
        private List<ImageControl> _listImageControls = new List<ImageControl>();

        /// <summary>
        /// Get the image control from the tab page.  We expect this to be the
        /// only control within the page
        /// </summary>
        private ImageControl GetImageControl(int i) {
            if ( _pages == null ) {
                return null;
            }
            if ( i >= _pages.Length ) {
                return null;
            }
            TabPage tab = _pages[i];
            if (tab.Controls.Count != 1) {
                return null;
            }
            return tab.Controls[0] as ImageControl;
        }
        /// <summary>
        /// Number of image control tabs in the form
        /// </summary>
        private int ImageControlCount { get { return _listImageControls.Count; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public FormSdCardTestApp() {
            InitializeComponent();
            _pages = new TabPage[] { tabPageImage0, tabPageImage1, tabPageImage2, tabPageImage3 };
            for (int i = 0; i < _pages.Length; i++ ) {
                _listImageControls.Add(GetImageControl(i));
            }
            _listImageControls.ForEach(x => x.SetPrintDetails(this, fifoImageControl.SendImage));
            fifoImageControl.SetPrintDetails(this);
            LoadSettings();
        }

        /// <summary>
        /// Start a new print job
        /// </summary>
        private void StartPrintJob() {
            eRET rVal;
            if (!SetupPrinter(out rVal)) {
                MessageBox.Show("Failed to setup printer: " + rVal);
                return;
            }
            // The document width is optional when printing from the SD card.
            // It is used to set the product detect lockout distance if 
            // [ProductDetect] Lockout = 1 is set in the configuration file
            //
            int docWidth = (int)numericUpDownDocWidth.Value;

            // If all images in the print job will come from the SD card, use JT_FIFO.
            //
            // For a mixed mode where SD card images can be combined with images sent from
            // the PC, use JT_DUAL_FIFO.
            //
            eJOBTYPE jobType = fifoImageControl.EnableMixedMode ? eJOBTYPE.JT_DUAL_FIFO : eJOBTYPE.JT_FIFO;

            // Each HDC print lane has 128MB of memory on the PCCE.  This is split into two parts when
            // running a dual FIFO print job, splitting the memory between datapath "A" and datapath "B".
            // Memory assigned to datapath "A" is used for buffering data from the SD card; memory assigned 
            // to datapath "B" is used for the print data sent from the PC.
            //
            // It is the responsibility of the application to decide how much memory to allocate to each
            // datapath and set this in Meteor using PiSetParam with CCP_MEM_BOUNDARY.
            //
            // Here we simply split the memory in half.
            //
            if (jobType == eJOBTYPE.JT_DUAL_FIFO) {
                int pccDatapathDwords = 128 * 1024 * 128;
                PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_MEM_BOUNDARY, pccDatapathDwords / 2);
            }

            // Meteor command to start a print job
            //
            int[] StartJobCmd = new int[] { 
                (int)CtrlCmdIds.PCMD_STARTJOB,      // Command ID
                4,                                  // Number of DWORD parameters
                _jobid++,                           // Job ID
                (int)jobType,                       // Print job type
                (int)eRES.RES_HIGH,                 // Print at full resolution
                docWidth
            };

            // A start job command can fail if there is an existing print job
            // ready or printing in Meteor, or if a previous print job is still
            // aborting.  The sequencing of the main form's control enables should 
            // guarantee that this never happens in this application.
            //
            rVal = PrinterInterfaceCLS.PiSendCommand(StartJobCmd);
            _jobActive = (rVal == eRET.RVAL_OK);
            if (!_jobActive) {
                string Err = string.Format("Failed to start print job\n\n{0}", rVal);
                MessageBox.Show(Err, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxStatus.Text = _latestPrinterStatus.ToString();
            }

            // Enable / disable form controls according to whether a print job is active
            //
            EnableControls();
        }

        /// <summary>
        /// Set the enabled state of the various controls.  This depends on
        /// whether we are currently connected to Meteor, and whether there
        /// is a print job starting or in progress.
        /// </summary>
        private void EnableControls() {
            bool Connected = (_latestPrinterStatus != PRINTER_STATUS.DISCONNECTED && _latestPrinterStatus != PRINTER_STATUS.HOSTING_FAILED);

            groupBoxControl.Enabled = Connected;
            groupBoxResolution.Enabled = Connected && !_jobActive;
            groupBoxPrintClock.Enabled = Connected && !_jobActive;

            buttonStartJob.Enabled = !_jobActive;
            buttonStopJob.Enabled = _jobActive && !_jobAborting;
            numericUpDownDocWidth.Enabled = !_jobActive;

            numericUpDownFrequency.Enabled = radioButtonInternalEncoder.Checked;

            _listImageControls.ForEach(x => x.EnableControls());
            fifoImageControl.EnableControls();
        }

        /// <summary>
        /// Helper method for EnableBppRadioButtons.  Called for each of the three bpp radio
        /// buttons in sequence.
        /// </summary>
        /// <param name="enable">Zero to disable, non-zero to enable</param>
        /// <param name="button">Radio button to enable/disable</param>
        /// <param name="firstEnabled">Set with button if this is the first enabled control</param>
        /// <returns>true if the currently set bpp value is now unavailable</returns>
        private static bool RadioButtonCheck(Int32 enable, RadioButton button, ref RadioButton firstEnabled) {
            bool retval = false;
            if (enable != 0) {
                button.Enabled = true;
                if (firstEnabled == null) {
                    firstEnabled = button;
                }
            } else {
                button.Enabled = false;
                if (button.Checked) {
                    retval = true;
                }
            }
            return retval;
        }

        /// <summary>
        /// Act on a change in the valid bits-per-pixel bitmask returned by Meteor, to
        /// (1) enable/disable the appropriate 1,2 or 4bpp radio buttons and (2) make
        /// sure that the currently selected bpp value is valid.  
        /// </summary>
        private void EnableBppRadioButtons() {
            RadioButton firstEnabled = null;
            bool changeSelection = RadioButtonCheck(_lastSupportedBppBitmask & 0x02, radioButton1bpp, ref firstEnabled);
            changeSelection |= RadioButtonCheck(_lastSupportedBppBitmask & 0x04, radioButton2bpp, ref firstEnabled);
            changeSelection |= RadioButtonCheck(_lastSupportedBppBitmask & 0x10, radioButton4bpp, ref firstEnabled);
            if (changeSelection && firstEnabled != null) {
                firstEnabled.Checked = true;
            }
        }
        
        /// <summary>
        /// Set up the printer prior to starting a print job.
        ///
        /// PiSetAndValidateParam blocks until the parameters have been successfully set (or have failed to set
        /// - e.g. if there is an out of range value).  
        /// 
        /// This must be used here in preference to the asynchronous method PiSetParam to guarantee that the
        /// values are set in Meteor before the print job is started.
        ///
        /// </summary>
        /// <param name="rVal">Meteor error code, or RVAL_OK on success</param>
        /// <returns>Success / failure</returns>
        private bool SetupPrinter(out eRET rVal) {
            rVal = PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_PRINT_CLOCK_HZ, UserPrintClock);
            if (rVal != eRET.RVAL_OK) {
                return false;
            }
            rVal = PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_BITS_PER_PIXEL, UserBitsPerPixel);
            if (rVal != eRET.RVAL_OK) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Abort any in-progress print job
        /// </summary>
        private void AbortPrintJob() {
            _jobAborting = true;
            // Update display status and refresh, as the abort can take a few seconds
            buttonStopJob.Enabled = false;
            buttonStopJob.Refresh();
            textBoxStatus.Text = PRINTER_STATUS.ABORTING.ToString();
            textBoxStatus.Refresh();
            // Send the abort command to Meteor.  This will halt any in-progress
            // print, and clear out all print buffers
            PrinterInterfaceCLS.PiAbort();
            // Wait until the abort has completed
            Cursor.Current = Cursors.WaitCursor;
            _status.WaitNotBusy();
            Cursor.Current = Cursors.Default;
            _jobActive = false;
            _jobAborting = false;
        }

        // -- Timers for handling Meteor connection and status --
        #region StatusHandlers
        /// <summary>
        /// Called periodically to (a) connect to Meteor (if the connection is
        /// not already open); (b) retrieve the status from Meteor
        /// </summary>
        private void timerMeteorStatus_Tick(object sender, EventArgs e) {
            if (_inTimer) {
                return;
            }
            _inTimer = true;
            try {
                HandleMeteorStatus();
            }
            // If an exception is thrown above, display it to assist trouble shooting
            catch (Exception Exc) {
                string Msg = "Exception thrown:\r\n\r\n" + Exc.ToString();
                MessageBox.Show(Msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            _inTimer = false;
        }

        /// <summary>
        /// Called periodically to handle Meteor initialisation and status
        /// </summary>
        private void HandleMeteorStatus() {
            // Get the Meteor status / open a connection to Meteor if one doesn't 
            // already exist
            PRINTER_STATUS Status = _status.GetStatus();
            _latestPrinterStatus = Status;
            if (_status.Connected) {
                if (_lastSupportedBppBitmask != _status.SupportedBppBitmask) {
                    _lastSupportedBppBitmask = _status.SupportedBppBitmask;
                    EnableBppRadioButtons();
                }
                // Meteor will reject a PiSetHeadPower command if any of the PCCs are still in the process
                // of changing the head power status
                if (checkBoxEnableHeadPower.Enabled != _status.HeadPowerIdle) {
                    checkBoxEnableHeadPower.Enabled = _status.HeadPowerIdle;
                }
                // If the Meteor head power state is stable, check that we're displaying the correct state
                if (_status.HeadPowerIdle) {
                    if (_status.HeadPowerEnabled != checkBoxEnableHeadPower.Checked) {
                        _inSetHeadPower = true;
                        checkBoxEnableHeadPower.Checked = _status.HeadPowerEnabled;
                        _inSetHeadPower = false;
                    }
                }
                if (_status.MeteorBitsPerPixel != UserBitsPerPixel) {
                    eRET rVal;
                    SetupPrinter(out rVal);
                }
            }
            // Update the enabled state of the controls to reflect the Meteor status
            EnableControls();
            // Update the status text
            if ( _jobAborting ) {
                textBoxStatus.Text = PRINTER_STATUS.ABORTING.ToString();
            } else {
                textBoxStatus.Text = Status.ToString();
            }
            if (_status.SdCardSizeBlocks == 0) {
                textBoxSdSize.Text = "Missing";
            } else {
                // One block is 512 bytes.
                // So 1 gigabyte is 1024 * 1024 * 2 blocks
                double gb = (double)_status.SdCardSizeBlocks / (1024 * 1024 * 2);
                textBoxSdSize.Text = (_status.IsSimFlash ? "SimFlash: " : "") + string.Format("{0:f2}GB", gb);
            }
            checkBoxSDWriteBusy.Checked = _status.SdCardWriteBusy;

            // If the image store details have changed - e.g. the user has changed the slot size
            // or the print resolution - then the images will need to be reloaded.
            //
            if ( SdCardSlot.Instance.SdCardSizeBlocks != (int)_status.SdCardSizeBlocks ||
                 SdCardSlot.Instance.BitsPerPixel != UserBitsPerPixel ||
                 SdCardSlot.Instance.MaxImageXLengthPixels != (int)numericUpDownSlotWidth.Value ) {
                
                SdCardSlot.Instance.SdCardSizeBlocks = (int)_status.SdCardSizeBlocks;
                SdCardSlot.Instance.BitsPerPixel = UserBitsPerPixel;
                SdCardSlot.Instance.MaxImageXLengthPixels = (int)numericUpDownSlotWidth.Value;

                _listImageControls.ForEach(x => x.InvalidateSdCardImages());
            }
        }
        #endregion

        // -- Save and load of parameters set by the user --
        #region Settings
        void SaveSettings() {
            try {
                Properties.Settings.Default.PrintFrequency = numericUpDownFrequency.Value;
                Properties.Settings.Default.PrintResolution = UserBitsPerPixel;
                Properties.Settings.Default.ExternalEncoder = UserExternalEncoder;
                Properties.Settings.Default.DocWidth = numericUpDownDocWidth.Value;
                Properties.Settings.Default.SlotWidth = numericUpDownSlotWidth.Value;

                Properties.Settings.Default.ImageFileName = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Copies = new decimal[ImageControlCount];
                Properties.Settings.Default.YTop = new int[ImageControlCount];
                Properties.Settings.Default.XStart = new int[ImageControlCount];

                _listImageControls.ForEach(x => x.SaveSettings());
                fifoImageControl.SaveSettings();
                
                Properties.Settings.Default.Save();
            }
            catch (Exception e) {
                MessageBox.Show("SaveSettings exception: \r\n" + e.Message);
            }
        }

        void LoadSettings() {
            try {
                numericUpDownFrequency.Value = Properties.Settings.Default.PrintFrequency;
                UserBitsPerPixel = Properties.Settings.Default.PrintResolution;
                UserExternalEncoder = Properties.Settings.Default.ExternalEncoder;
                numericUpDownDocWidth.Value = Properties.Settings.Default.DocWidth;
                numericUpDownSlotWidth.Value = Properties.Settings.Default.SlotWidth;
                _listImageControls.ForEach(x => x.LoadSettings());
                fifoImageControl.LoadSettings();
            }
            catch (Exception) {// Ignore any load exception and fall back on default form values
                               // (can happen if valid ranges in the up/down controls are changed)
            }
        }
        #endregion

        // -- Form properties set by the user --
        #region UserProperties
        /// <summary>
        /// Value for the Meteor CCP_PRINT_CLOCK_HZ parameter.
        /// Zero means use external encoder.  
        /// A non zero value sets the master internal print clock frequency.
        /// </summary>
        private int UserPrintClock {
            get {
                if (radioButtonExternalEncoder.Checked) {
                    return 0;
                } else {
                    return (int)(numericUpDownFrequency.Value * 1000);
                }
            }
        }
        private bool UserExternalEncoder {
            get {
                return radioButtonExternalEncoder.Checked;
            }
            set {
                radioButtonInternalEncoder.Checked = !value;
                radioButtonExternalEncoder.Checked = value;
            }
        }
        public int UserBitsPerPixel {
            get {
                if (radioButton1bpp.Checked) {
                    return 1;
                } else if (radioButton2bpp.Checked) {
                    return 2;
                } else {
                    return 4;
                }
            }
            private set {
                switch (value) {
                    case 1:
                        radioButton1bpp.Checked = true;
                        radioButton2bpp.Checked = false;
                        radioButton4bpp.Checked = false;
                        break;
                    case 2:
                        radioButton1bpp.Checked = false;
                        radioButton2bpp.Checked = true;
                        radioButton4bpp.Checked = false;
                        break;
                    default:
                        radioButton1bpp.Checked = false;
                        radioButton2bpp.Checked = false;
                        radioButton4bpp.Checked = true;
                        break;
                }
            }
        }
        public bool JobActive { get { return _jobActive; } }
        public bool SdCardsPresent { get { return _status.SdCardSizeBlocks != 0; } }
        #endregion

        // -- Handlers for user control interaction --
        #region UserInteraction

        private void buttonStartJob_Click(object sender, EventArgs e) {
            StartPrintJob();
        }

        private void buttonStopPrint_Click(object sender, EventArgs e) {
            AbortPrintJob();
        }

        private void FormSdCardTestApp_FormClosing(object sender, FormClosingEventArgs e) {
            SaveSettings();
            _status.Disconnect();
            timerMeteorStatus.Enabled = false;
        }

        private void checkBoxEnableHeadPower_CheckedChanged(object sender, EventArgs e) {
            if (_inSetHeadPower) {
                return;
            }
            _inSetHeadPower = true;
            if (_status.Connected) {
                eRET rVal = PrinterInterfaceCLS.PiSetHeadPower(checkBoxEnableHeadPower.Checked ? 1 : 0);
                if (rVal != eRET.RVAL_OK) {
                    MessageBox.Show("PiSetHeadPower failed with " + rVal.ToString() +
                                    "\n\nPlease check that all PCC cards are connected and have completed initialisation",
                                    Application.ProductName,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Asterisk);
                    checkBoxEnableHeadPower.Checked = false;
                } else {
                    // Prevent further PiSetHeadPower commands being sent until the status reports HeadPowerIdle
                    checkBoxEnableHeadPower.Enabled = false;
                }
            }
            _inSetHeadPower = false;
        }
        #endregion

    }
}
