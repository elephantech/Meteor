using System;
using System.Windows.Forms;
using System.IO;

namespace Ttp.Meteor.MeteorMonoPrint {
    public partial class FormMeteorMonoPrint : Form {
        /// <summary>
        /// Object handles Meteor connection and status
        /// </summary>
        private PrinterStatus _status = new PrinterStatus();
        /// <summary>
        /// Last known printer status
        /// </summary>
        private PRINTER_STATUS _latestPrinterStatus = PRINTER_STATUS.DISCONNECTED;
        /// <summary>
        /// Currently loaded print image
        /// </summary>
        private IMeteorImageData _image;
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
        /// The last target head temperature value which was sent to Meteor
        /// </summary>
        private int _lastSetHeadTemperature = -1;
        /// <summary>
        /// The last target auxiliary temperature value which was sent to Meteor
        /// </summary>
        private int _lastSetAuxTemperature = -1;
        /// <summary>
        /// The last known value of the bits-per-pixel modes supported by Meteor
        /// This can change if the Meteor head type is changed
        /// </summary>
        private int _lastSupportedBppBitmask = 0;
        /// <summary>
        /// ID of the latest meteor print job.  Incremented for each job.
        /// </summary>
        private int _jobid = 1;
        /// <summary>
        /// Set after a job has been successfully started when there is Meteor
        /// hardware connected, and cleared when the Meteor status changes to
        /// ready or printing.
        /// The flag is not set if a job is started without hardware (e.g.
        /// to allow the SimPrint output to be checked)
        /// </summary>
        bool _jobStarting = false;
        /// <summary>
        /// Set after we abort a print job and cleared when the Meteor status 
        /// changes to idle.
        /// </summary>
        bool _jobAborting = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public FormMeteorMonoPrint() {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Load the data for a new print image
        /// </summary>
        private void LoadImage() {
            // Attempt to avoid memory allocation problems with large images by
            // cleaning up the currently loading image.  For very large images
            // the 64 bit build of the application should be used.
            if (_image != null) {
                pictureBoxPrintData.Image = null;
                _image.Dispose();
                _image = null;
                pictureBoxPrintData.Refresh();
            }
            GC.Collect();
            // Load the new image
            Cursor.Current = Cursors.WaitCursor;
            string FileName = openFileDialogLoadImage.FileName;
            _image = ImageDataFactory.Create(FileName);

            if (_image != null) {
                pictureBoxPrintData.Image = _image.GetPreviewBitmap();
                Cursor.Current = Cursors.Default;
                toolStripStatusFilename.Text = _image.GetBaseName();
                toolStripStatusFilename.ToolTipText = FileName;
                toolStripStatusImageDimensions.Text = string.Format("{0} x {1} pixels", _image.GetDocWidth(), _image.GetDocHeight());
            } else {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Failed to load image " + openFileDialogLoadImage.FileName,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                
                openFileDialogLoadImage.FileName = "";
                toolStripStatusFilename.Text = "No image loaded";
                toolStripStatusFilename.ToolTipText = "";
                toolStripStatusImageDimensions.Text = "";
            }
            EnableControls();
        }

        /// <summary>
        /// Start a new print job
        /// </summary>
        private void StartPrintJob() {
            if (_image == null) {
                return;
            }
            if (!SetupPrinter()) {
                MessageBox.Show("Failed to setup printer");
                return;
            }
            PreLoadPrintJob test = new PreLoadPrintJob(UserBitsPerPixel, _image, UserYTop, UserCopies, UserRepeatMode, _jobid++);
            // Move status to LOADING if Meteor has hardware connected - refresh the display 
            // immediately because sending the print data to Meteor in PreLoadPrintJob.Start
            // can take a significant amount of time for a large image
            if (_latestPrinterStatus == PRINTER_STATUS.IDLE) {
                textBoxStatus.Text = PRINTER_STATUS.LOADING.ToString();
                _jobStarting = true;
                textBoxStatus.Refresh();
            }
            Cursor.Current = Cursors.WaitCursor;
            eRET rVal = test.Start();
            Cursor.Current = Cursors.Default;
            if (rVal != eRET.RVAL_OK) {
                string Err = string.Format("Failed to start print job\n\n{0}", rVal);
                MessageBox.Show(Err, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxStatus.Text = _latestPrinterStatus.ToString();
            }
            EnableControls();
        }

        /// <summary>
        /// Set the enabled state of the various controls.  This depends on
        /// whether we are currently connected to Meteor, and whether there
        /// is a print job starting or in progress.
        /// </summary>
        private void EnableControls() {
            bool JobInProgress = _latestPrinterStatus == PRINTER_STATUS.READY ||
                                 _latestPrinterStatus == PRINTER_STATUS.PRINTING ||
                                 _jobStarting;
            bool ImageLoaded = _image != null;
            bool Connected = (_latestPrinterStatus != PRINTER_STATUS.DISCONNECTED);

            groupBoxControl.Enabled = Connected;
            groupBoxTemperatures.Enabled = Connected;
            groupBoxSetup.Enabled = Connected && !JobInProgress;
            groupBoxResolution.Enabled = Connected && !JobInProgress;
            groupBoxPrintClock.Enabled = Connected && !JobInProgress;

            buttonStartPrint.Enabled = (_image != null) && !JobInProgress;
            buttonLoadImage.Enabled = !JobInProgress;
            buttonStopPrint.Enabled = JobInProgress && !_jobAborting;
            numericUpDownYTop.Enabled = !JobInProgress;

            numericUpDownFrequency.Enabled = radioButtonInternalEncoder.Checked;
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
        /// <returns>Success / failure</returns>
        private bool SetupPrinter() {
            if (PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_PRINT_CLOCK_HZ, UserPrintClock) != eRET.RVAL_OK) {
                return false;
            }
            if (PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_BITS_PER_PIXEL, UserBitsPerPixel) != eRET.RVAL_OK) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Abort any in-progress print job
        /// </summary>
        private void AbortPrintJob() {
            // No longer starting a job
            _jobStarting = false;
            _jobAborting = true;
            // Update display status and refresh, as the abort can take a few seconds
            buttonStopPrint.Enabled = false;
            buttonStopPrint.Refresh();
            textBoxStatus.Text = PRINTER_STATUS.ABORTING.ToString();
            textBoxStatus.Refresh();
            // Send the abort command to Meteor.  This will halt any in-progress
            // print, and clear out all print buffers
            PrinterInterfaceCLS.PiAbort();
            // Wait until the abort has completed
            Cursor.Current = Cursors.WaitCursor;
            _status.WaitNotBusy();
            Cursor.Current = Cursors.Default;
        }

        // -- Timers for handling Meteor connection and status --
        #region StatusHandlers
        /// <summary>
        /// Called periodically to (a) connect to Meteor (if the connection is
        /// not already open); (b) retrieve the status from Meteor; (c) update
        /// the temperature set points if required.
        /// </summary>
        private void TimerMeteorStatus_Tick(object sender, EventArgs e) {
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
        /// Lists the different Meteor API mechanisms for setting target head temperatures
        /// </summary>
        private enum HeadTempType {
            /// <summary>
            /// Head temperature control is not available
            /// </summary>
            NONE,
            /// <summary>
            /// Use PiSetParam with CCP_HEAD_TEMP
            /// </summary>
            CCP_HEAD_TEMP,
            /// <summary>
            /// Use PiSetParamEx with CPEX_TargetTemp
            /// </summary>
            CPEX_TargetTemp,
            /// <summary>
            /// Use PiSetParamEx with CPEX_TemperatureUpperLimit and CPEX_TemperatureLowerLimit
            /// </summary>
            CPEX_TemperatureLimits
        }

        /// <summary>
        /// Find out the API commands to use to set the head temperature target for a particular Meteor HDC type
        /// </summary>
        private HeadTempType GetHeadTemperatureType(eHEADTYPE ht) {
            switch (ht) {
                case eHEADTYPE.HT_SPECTRA:      return HeadTempType.CCP_HEAD_TEMP;
                case eHEADTYPE.HT_KJ4:          return HeadTempType.CCP_HEAD_TEMP;
                case eHEADTYPE.HT_SPT_508GS:    return HeadTempType.CCP_HEAD_TEMP;
                case eHEADTYPE.HT_RG4:          return HeadTempType.CCP_HEAD_TEMP;
                case eHEADTYPE.HT_RG5:          return HeadTempType.CCP_HEAD_TEMP;
                case eHEADTYPE.HT_STARFIRE:     return HeadTempType.CCP_HEAD_TEMP;

                case eHEADTYPE.HT_K150:         return HeadTempType.CPEX_TemperatureLimits;
                case eHEADTYPE.HT_K300:         return HeadTempType.CPEX_TemperatureLimits;
                case eHEADTYPE.HT_K600:         return HeadTempType.CPEX_TemperatureLimits;
                case eHEADTYPE.HT_K1200:        return HeadTempType.CPEX_TemperatureLimits;

                case eHEADTYPE.HT_KM130:        return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_KM_M600:      return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_KM1024I:      return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_KM1800I:      return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_KM1800I_SH:   return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_KM1024:       return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_SPT_1024GS:   return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_SPT_RC1536:   return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_SEIKO_SRC1800:return HeadTempType.CPEX_TargetTemp;
                case eHEADTYPE.HT_SG600:        return HeadTempType.CPEX_TargetTemp;

                default:                        return HeadTempType.NONE;
            }
        }

        /// <summary>
        /// The minimum temperature allowed by the Meteor CPEX_TargetTemp parameter for the head type, in tenths of a degree
        /// </summary>
        private int MinTemperature(eHEADTYPE ht) {
            switch (ht) {
                case eHEADTYPE.HT_SPT_1024GS:       return 150;
                case eHEADTYPE.HT_SPT_RC1536:       return 150;
                case eHEADTYPE.HT_SEIKO_SRC1800:    return 150;
                case eHEADTYPE.HT_SG600:            return 100;
                case eHEADTYPE.HT_K150:             return 150;
                case eHEADTYPE.HT_K300:             return 150;
                case eHEADTYPE.HT_K600:             return 150;
                case eHEADTYPE.HT_K1200:            return 150;
                default:                            return 0;
            }
        }

        /// <summary>
        /// Find out if the current HDC type has a secondary (auxiliary) temperature control loop
        /// </summary>
        private bool SupportsAuxTemperatureControl(eHEADTYPE ht) {
            switch (ht) {
                case eHEADTYPE.HT_STARFIRE: return true;
                case eHEADTYPE.HT_KJ4:      return true;
                default:                    return false;
            }
        }

        /// <summary>
        /// Called periodically to handle Meteor initialisation and status
        /// </summary>
        private void HandleMeteorStatus() {
            // Get the Meteor status / open a connection to Meteor if one doesn't 
            // already exist
            PRINTER_STATUS Status = _status.GetStatus();
            if (Status == PRINTER_STATUS.READY || 
                Status == PRINTER_STATUS.PRINTING || 
                Status == PRINTER_STATUS.DISCONNECTED) {
                _jobStarting = false;
            } else {
                _jobAborting = false;
            }
            _latestPrinterStatus = Status;
            // Update the head/aux temperature setpoints if the values in the interface have been changed
            // Enable / disable the temperature controls depending on what the current HDC type supports
            if (_status.Connected) {
                HeadTempType headTempType = GetHeadTemperatureType(_status.HeadType);
                if (headTempType == HeadTempType.NONE && checkBoxHeadTemperatureControl.Checked) {
                    checkBoxHeadTemperatureControl.Checked = false;
                }
                checkBoxHeadTemperatureControl.Enabled = (headTempType != HeadTempType.NONE);

                if (_lastSetHeadTemperature != UserHeadTemperature) {
                    //
                    // The mechanism for setting the target head temperature depends on the head type.
                    // Older head types uses PrinterInterfaceCLS.PiSetParam with the eCFGPARAM.CCP_HEAD_TEMP
                    // parameter.
                    //
                    // More recent head types use PrinterInterfaceCLS.PiSetParamEx, which also provides more
                    // flexibility in head addressing for e.g. dual colour heads, or HDC types which drive
                    // more than one print head
                    //
                    // Note that for simplicity in this sample code, the value sent to the PrintEngine is *not* fully 
                    // range checked .  An out of range value will result in a log error.  The valid range for a
                    // Meteor temperature parameter can be found in the Meteor manuals or the distributed technical notes.
                    //
                    switch (headTempType) {

                        case HeadTempType.CCP_HEAD_TEMP:
                            // Set target head temperature globally (PiSetParam with pcc = 0, head = 0)
                            if (PrinterInterfaceCLS.PiSetParam((int)eCFGPARAM.CCP_HEAD_TEMP, UserHeadTemperature) == eRET.RVAL_OK) {
                                _lastSetHeadTemperature = UserHeadTemperature;
                            }
                            break;

                        case HeadTempType.CPEX_TargetTemp: {
                            // Set target head temperature globally (PiSetParamEx with pcc, hdc, head and ja all set to zero)
                            int peAddr = PrinterInterfaceCLS.MakePEAddress(0, 0, 0, 0);
                            // Limit the minimum temperature to the valid parameter range (otherwise we can't disable temperature control)
                            int target = Math.Max(MinTemperature(_status.HeadType), UserHeadTemperature);
                            // The value is already in tenths of a degree, so we must set a divisor of 10 in the upper 32 bits of
                            // the value 
                            // (Alternatively, use MakePEParam or PiSetParamEx with a decimal for the scaling to be done automatically)
                            Int64 val = ((UInt32)target) | ((Int64)10 << 32);

                            if (PrinterInterfaceCLS.PiSetParamEx(peAddr, eCFGPARAMEx.CPEX_TargetTemp, val) == eRET.RVAL_OK) {
                                _lastSetHeadTemperature = UserHeadTemperature;
                            }
                            break;
                        }

                        case HeadTempType.CPEX_TemperatureLimits: {
                            // Set target head temperature globally (PiSetParamEx with pcc, hdc, head and ja all set to zero)
                            int peAddr = PrinterInterfaceCLS.MakePEAddress(0, 0, 0, 0);
                            // Set the upper and lower bounds on temperature 1 degree either side of the setpoint.
                            // Limit the minimum temperature to the valid parameter range (otherwise we can't disable temperature control)
                            // N.B. Temperatures are in tenths of a degree here
                            int targetLower = Math.Max(MinTemperature(_status.HeadType), UserHeadTemperature - 10);
                            int targetUpper = Math.Max(MinTemperature(_status.HeadType), UserHeadTemperature + 10);
                            // The value is already in tenths of a degree, so we must set a divisor of 10 in the upper 32 bits of
                            // the value 
                            // (Alternatively, use MakePEParam or PiSetParamEx with a decimal for the scaling to be done automatically)
                            Int64 valLower = ((UInt32)targetLower) | ((Int64)10 << 32);
                            Int64 valUpper = ((UInt32)targetUpper) | ((Int64)10 << 32);

                            if (PrinterInterfaceCLS.PiSetParamEx(peAddr, eCFGPARAMEx.CPEX_TemperatureLowerLimit, valLower) == eRET.RVAL_OK
                                && PrinterInterfaceCLS.PiSetParamEx(peAddr, eCFGPARAMEx.CPEX_TemperatureUpperLimit, valUpper) == eRET.RVAL_OK ) {
                                _lastSetHeadTemperature = UserHeadTemperature;
                            }
                            break;
                        }

                    }
                }

                if (_lastSetAuxTemperature != UserAuxTemperature) {
                    bool supportsAuxTemp = SupportsAuxTemperatureControl(_status.HeadType);
                    if (!supportsAuxTemp) {
                        if (checkBoxAuxTemperatureControl.Checked) {
                            checkBoxAuxTemperatureControl.Checked = false;
                        }
                        checkBoxAuxTemperatureControl.Enabled = false;
                    } else {
                        checkBoxAuxTemperatureControl.Enabled = true;
                        // Set target auxiliary temperature globally (PiSetParam with pcc = 0, head = 0)
                        if (PrinterInterfaceCLS.PiSetParam((int)eCFGPARAM.CCP_AUX_TEMP, UserAuxTemperature) == eRET.RVAL_OK) {  // Set globally
                            _lastSetAuxTemperature = UserAuxTemperature;
                        }
                    }
                }
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
            } else {
                _lastSetHeadTemperature = -1;
                _lastSetAuxTemperature  = -1;
            }
            // Update the enabled state of the controls to reflect the Meteor status
            EnableControls();
            // Update the status text
            if ( _jobStarting ) {
                textBoxStatus.Text = PRINTER_STATUS.LOADING.ToString();
            } else if ( _jobAborting ) {
                textBoxStatus.Text = PRINTER_STATUS.ABORTING.ToString();
            } else {
                textBoxStatus.Text = Status.ToString();
            }
        }
        #endregion

        // -- Save and load of parameters set by the user --
        #region Settings
        void SaveSettings() {
            try {
                Properties.Settings.Default.YTop = (int)numericUpDownYTop.Value;
                Properties.Settings.Default.PrintFrequency = numericUpDownFrequency.Value;
                Properties.Settings.Default.Copies = numericUpDownCopies.Value;
                Properties.Settings.Default.RepeatMode = (int)UserRepeatMode;
                Properties.Settings.Default.PrintResolution = UserBitsPerPixel;
                Properties.Settings.Default.HeadTemperature = numericUpDownHeadTemperatureSetPoint.Value;
                Properties.Settings.Default.HeadTemperatureEnabled = checkBoxHeadTemperatureControl.Checked;
                Properties.Settings.Default.AuxTemperature = numericUpDownAuxTemperatureSetPoint.Value;
                Properties.Settings.Default.AuxTemperatureEnabled = checkBoxAuxTemperatureControl.Checked;
                Properties.Settings.Default.ImageFileName = openFileDialogLoadImage.FileName;
                Properties.Settings.Default.ExternalEncoder = UserExternalEncoder;
                Properties.Settings.Default.Save();
            }
            catch (Exception e) {
                MessageBox.Show("SaveSettings exception: \r\n" + e.Message);
            }
        }

        void LoadSettings() {
            try {
                numericUpDownYTop.Value = Properties.Settings.Default.YTop;
                numericUpDownFrequency.Value = Properties.Settings.Default.PrintFrequency;
                numericUpDownCopies.Value = Properties.Settings.Default.Copies;
                UserRepeatMode = (REPEAT_MODE)Properties.Settings.Default.RepeatMode;
                UserBitsPerPixel = Properties.Settings.Default.PrintResolution;
                numericUpDownHeadTemperatureSetPoint.Value = Properties.Settings.Default.HeadTemperature;
                checkBoxHeadTemperatureControl.Checked = Properties.Settings.Default.HeadTemperatureEnabled;
                numericUpDownAuxTemperatureSetPoint.Value = Properties.Settings.Default.AuxTemperature;
                checkBoxAuxTemperatureControl.Checked = Properties.Settings.Default.AuxTemperatureEnabled;
                openFileDialogLoadImage.FileName = Properties.Settings.Default.ImageFileName;
                UserExternalEncoder = Properties.Settings.Default.ExternalEncoder;
                if (File.Exists(openFileDialogLoadImage.FileName)) {
                    LoadImage();
                } else {
                    openFileDialogLoadImage.FileName = "";
                }
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
        private int UserYTop {
            get {
                return (int)(numericUpDownYTop.Value);
            }
        }
        private int UserCopies {
            get {
                return (int)numericUpDownCopies.Value;
            }
        }
        private REPEAT_MODE UserRepeatMode {
            get {
                return radioButtonSeamless.Checked ? REPEAT_MODE.SEAMLESS : REPEAT_MODE.DISCRETE;
            }
            set {
                radioButtonSeamless.Checked = (value == REPEAT_MODE.SEAMLESS);
                radioButtonDiscrete.Checked = (value == REPEAT_MODE.DISCRETE);
            }
        }
        private int UserBitsPerPixel {
            get {
                if (radioButton1bpp.Checked) {
                    return 1;
                } else if (radioButton2bpp.Checked) {
                    return 2;
                } else {
                    return 4;
                }
            }
            set {
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
        /// <summary>
        /// Target head temperature in tenths of a degree
        /// </summary>
        private int UserHeadTemperature {
            get {
                return checkBoxHeadTemperatureControl.Checked ? (int)(numericUpDownHeadTemperatureSetPoint.Value * 10) : 0;
            }
        }
        private int UserAuxTemperature {
            get {
                return checkBoxAuxTemperatureControl.Checked ? (int)(numericUpDownAuxTemperatureSetPoint.Value * 10) : 0;
            }
        }
        #endregion

        // -- Handlers for user control interaction --
        #region UserInteraction
        private void ButtonLoadImage_Click(object sender, EventArgs e) {
            if (openFileDialogLoadImage.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                LoadImage();
            }
        }

        private void ButtonStartPrint_Click(object sender, EventArgs e) {
            StartPrintJob();
        }

        private void ButtonStopPrint_Click(object sender, EventArgs e) {
            AbortPrintJob();
        }

        private void FormMeteorMonoPrint_FormClosing(object sender, FormClosingEventArgs e) {
            SaveSettings();
            _status.Disconnect();
            timerMeteorStatus.Enabled = false;
        }

        private void CheckBoxHeadTemperatureControl_CheckedChanged(object sender, EventArgs e) {
            numericUpDownHeadTemperatureSetPoint.Enabled = checkBoxHeadTemperatureControl.Checked;
        }

        private void CheckBoxAuxTemperatureControl_CheckedChanged(object sender, EventArgs e) {
            numericUpDownAuxTemperatureSetPoint.Enabled = checkBoxAuxTemperatureControl.Checked;
        }

        private void CheckBoxEnableHeadPower_CheckedChanged(object sender, EventArgs e) {
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
