using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO;
using Ttp.Meteor;
using Ttp.Meteor.MeteorSwath;
using System.Text.RegularExpressions;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// This is a small application for developers to learn about the 
    /// basic functionality of Meteor Swath. 
    /// 
    /// This application is designed to make it clear which commands are
    /// called in which order. This is done by activating and deactivating
    /// buttons to guide the user (developer) through the normal command 
    /// sequence.
    /// 
    /// This places some limitations on the application, which a normal
    /// application would not encounter.
    /// 
    /// For example, this application checks several statuses from Meteor
    /// before activating buttons. This significantly slows down the 
    /// process; normally an application would not need to check Meteor's
    /// status after every function call. 
    /// </summary>
    public partial class MainWindow : Window
    {
        // Only updated by PrinterInterface status, clicking buttons only affects that button.
        public bool IsPrinterInterfaceConnected
        {
            get { return (bool)GetValue(IsPrinterInterfaceConnectedProperty); }
            set { SetValue(IsPrinterInterfaceConnectedProperty, value); }
        }
        public static readonly DependencyProperty IsPrinterInterfaceConnectedProperty =
            DependencyProperty.Register("IsPrinterInterfaceConnected", typeof(bool), typeof(MainWindow));

        public bool IsInSwathJob
        {
            get { return (bool)GetValue(IsInSwathJobProperty); }
            set { SetValue(IsInSwathJobProperty, value); }
        }
        public static readonly DependencyProperty IsInSwathJobProperty =
            DependencyProperty.Register("IsInSwathJob", typeof(bool), typeof(bool));

        public int SwathRequired
        {
            get { return (int)GetValue(SwathRequiredProperty); }
            set { SetValue(SwathRequiredProperty, value); }
        }
        public static readonly DependencyProperty SwathRequiredProperty =
            DependencyProperty.Register("SwathRequired", typeof(int), typeof(MainWindow));

        public bool IsUsingPartialBuffers
        {
            get { return (bool)GetValue(IsUsingPartialBuffersProperty); }
            set { SetValue(IsUsingPartialBuffersProperty, value); }
        }
        public static readonly DependencyProperty IsUsingPartialBuffersProperty =
            DependencyProperty.Register("IsUsingPartialBuffers", typeof(bool), typeof(MainWindow));

        
        private MeteorSwathHelper MeteorSwath = new MeteorSwathHelper();
        private string fullPathToImage = "";
        private int selectedImageWidth;
        private int selectedImageHeight;
        private bool setMeteorBppOnChange;
        private UInt32 Jobid = 100;
        private AutoSwathGeometry autoGeometry = new AutoSwathGeometry();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            autoGeometry.XFields = 3;
            autoGeometry.YFields = 2;
            autoGeometry.Overprints = 1;
            autoGeometry.ExactlyEqualSteps = true;

            InitializeComponent();
            
            // Start with everything disabled. Buttons will become enabled when they
            // are relevent. 
            buttonConnect.IsEnabled = false;
            buttonDisconnect.IsEnabled = false;
            IsPrinterInterfaceConnected = false;
            groupBoxSeparation.IsEnabled = false;
            groupBoxControl.IsEnabled = false;


            MeteorSwath.StartPolling(1000);
            MeteorSwath.StatusesUpdated += OnStatusesRead;

            // One for each geometry, plus one for AutoGeometry.
            AutoGeometryModel agm = new AutoGeometryModel() { Geometry = autoGeometry };
            comboBoxGeoIndex.Items.Add(agm);
        }

        /// <summary>
        /// Handler for the MeteorSwathTestApp main window's "Connect" button
        /// </summary>
        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (MeteorSwath.StartPrintEngine() && MeteorSwath.Connect()) 
            {
                buttonConnect.IsEnabled = false;
                TAppStatus status;
                if (PrinterInterfaceCLS.PiGetPrnStatus(out status) == eRET.RVAL_OK) 
                {
                    setMeteorBppOnChange = false;
                    switch (status.BitsPerPixel)
                    {
                        case 1: comboBoxBitsPerPixel.SelectedIndex = 0; break;
                        case 2: comboBoxBitsPerPixel.SelectedIndex = 1; break;
                        default: comboBoxBitsPerPixel.SelectedIndex = 2; break;
                    }
                    setMeteorBppOnChange = true;
                }
            }
        }

        /// <summary>
        /// Handler for the MeteorSwathTestApp main window's "Disconnect" button
        /// </summary>
        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            MeteorSwath.Disconnect();
            MeteorSwath.StopPrintEngine(false);
            buttonDisconnect.IsEnabled = false;
        }

        /// <summary>
        /// Event handler which handles a status read by the MeteorSwathHelper
        /// This event is raised in the MeteorSwathHelper's asynchronous polling thread
        /// All "Invokes" in this method must be synchronous in order to protect against race conditions
        /// </summary>
        private void OnStatusesRead(object sender, EventArgs e)
        {
            // Keep reference on application in case it is closed during one of the below Invokes
            Application app = Application.Current; 
            if (app != null)
            {
                app.Dispatcher.Invoke((Action)(() => buttonConnect.IsEnabled = MeteorSwath.LastAppStatus == null));
                app.Dispatcher.Invoke((Action)(() => buttonDisconnect.IsEnabled = MeteorSwath.LastAppStatus != null));
                app.Dispatcher.Invoke((Action)(() => IsPrinterInterfaceConnected = MeteorSwath.LastAppStatus != null));
                app.Dispatcher.Invoke((Action)(() => groupBoxSeparation.IsEnabled = MeteorSwath.LastAppStatus != null));
                app.Dispatcher.Invoke((Action)(() => groupBoxControl.IsEnabled = MeteorSwath.LastAppStatus != null));

                TSwathSeparatorStatus? lastSwathStatus = MeteorSwath.LastSwathSeparatorStatus;
                TAppStatus? lastAppStatus = MeteorSwath.LastAppStatus;
                if (lastSwathStatus != null && lastAppStatus != null)
                {
                    if (lastSwathStatus.Value.IsBusy == 0)
                    {
                        // 
                        bool inJob = lastSwathStatus.Value.IsInJob == 1;
                        ePRINTERSTATE prnState = lastAppStatus == null ? ePRINTERSTATE.MPS_DISCONNECTED : lastAppStatus.Value.PrinterState;
                        app.Dispatcher.Invoke((Action)(() => UpdateControlsForJob(inJob, prnState)));
                    }
                    app.Dispatcher.Invoke((Action)(() => UpdateGeometryComboBox(lastSwathStatus.Value)));
                    app.Dispatcher.Invoke((Action)(() => SwathRequired = lastSwathStatus.Value.SwathsRequired));
                }

                app.Dispatcher.Invoke((Action)(() => UpdateSwathList()));
            }
        }

        /// <summary>
        /// Called each time a SwathSeparatorStatus is read. Several buttons and combos boxes 
        /// are either enabled or disabled, to reflect the options available when in or out of a
        /// Swath job.
        /// </summary>
        /// <param name="isInJob"></param>
        private void UpdateControlsForJob(bool isInJob, ePRINTERSTATE printerState)
        {
            buttonStartJob.IsEnabled = !isInJob && (printerState == ePRINTERSTATE.MPS_DISCONNECTED || printerState == ePRINTERSTATE.MPS_IDLE);
            buttonSendImage.IsEnabled = isInJob;
            buttonProcessOne.IsEnabled = isInJob;
            buttonProcessAll.IsEnabled = isInJob;
            buttonEndJob.IsEnabled = isInJob;
            comboBoxGeoIndex.IsEnabled = !isInJob;
            comboBoxBitsPerPixel.IsEnabled = !isInJob;
            checkBox_Bidirection.IsEnabled = !isInJob;
            checkBox_FirstRev.IsEnabled = !isInJob;
            comboBoxPlane.IsEnabled = isInJob;
            textBoxXPos.IsEnabled = isInJob;
            labelPlane.IsEnabled = isInJob;
            labelXpos.IsEnabled = isInJob;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        private void UpdateGeometryComboBox(TSwathSeparatorStatus status)
        {
            // Update number of items in the combo box until we reach the number of defined geometries.
            // The first item in the combo box is reserved for "automatic".
            while (comboBoxGeoIndex.Items.Count < status.AvailableGeometryCount + 1) {
                comboBoxGeoIndex.Items.Add(null);
            }
            // Originally only up to 20 swath geometries could be listed in the Meteor configuration file;
            // now the number is effectively unlimited.  
            // Note that there can be unset geometries in the list between 1 and status.AvailableGeometryCount.
            // - Swath geometries are defined using the MeteorSwathTookit application.
            // - Swath geometries are always indexed from 1 in the MeteorSwath API.
            // - Index 0 in the combo box is used to select an automatic geometry.
            for (int i = 1; i <= status.AvailableGeometryCount; ++i)
            {
                SwathGeometryModel sgm = new SwathGeometryModel();
                TSwathGeometrySummary swathGeometrySummary;
                MeteorSwathInterface.MeteorSwathGetGeometrySummary((uint)i, out swathGeometrySummary);
                sgm.Exists = swathGeometrySummary.IsGeometryDefined != 0;
                sgm.Index = i;
                sgm.Name = System.Text.Encoding.ASCII.GetString(swathGeometrySummary.GeometryName);
                if (sgm.Name.IndexOf('\0') != -1)
                {
                    sgm.Name = sgm.Name.Substring(0, sgm.Name.IndexOf('\0'));
                }

                if (comboBoxGeoIndex.Items[i] == null || comboBoxGeoIndex.Items[i].ToString() != sgm.ToString())
                {
                    comboBoxGeoIndex.Items[i] = sgm;
                }
            }

            if (comboBoxGeoIndex.SelectedItem == null)
            {
                comboBoxGeoIndex.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Called synchronously from the status read event to update the display of the swath 
        /// details for the current print job (if any)
        /// 
        /// A typical print application will need to pass these details onto a motion control 
        /// subsystem, so it can calculate the axis movements required for each swath
        /// 
        /// Note that as this method is Invoked directly from the MeteorSwathHelper StatusesUpdated
        /// event, there is no need to worry about a race condition when accessing the SwathDetails 
        /// list
        /// </summary>
        private void UpdateSwathList() 
        {
            listBoxSwaths.Items.Clear();
            listBoxSwaths.Items.Add("Index".PadRight(8) + "YPos".PadRight(8) + "Reverse".PadRight(8) + "Loaded".PadRight(0));

            List<TSwathDetails> source = MeteorSwath.SwathDetails;
            for (int i = 0; i < source.Count; i++)
            {
                string displaystring = i.ToString().PadRight(8);
                displaystring += source[i].YPrintPosition.ToString().PadRight(8);
                displaystring += source[i].IsReverse.ToString().PadRight(8);
                displaystring += source[i].SwathLoadedCount.ToString().PadRight(8);

                if (source[i].IsPartialBufferFull != null && source[i].IsPartialBufferFull[0] != 0)
                {
                    displaystring += "Partial buffer ready";
                } else {
                    ;
                }
                listBoxSwaths.Items.Add(displaystring);
            }
        }

        /// <summary>
        /// Handler for the "StartOnlineJob" button click
        /// </summary>
        private void buttonStartJob_Click(object sender, RoutedEventArgs e)
        {
            if (fullPathToImage == "" || fullPathToImage == null)
            {
                MessageBox.Show("The filename is empty. Please choose a file to start the job", "No filename", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (!File.Exists(fullPathToImage))
            {
                MessageBox.Show("The file could not be found. Please check: " + Environment.NewLine + fullPathToImage,
                    "File not found", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (comboBoxGeoIndex.SelectedItem is SwathGeometryModel)
            {
                // Geometry index is 1 - 20 inclusive.
                SwathGeometryModel sgm = comboBoxGeoIndex.SelectedItem as SwathGeometryModel;
                MeteorSwath.StartOnlineJob((uint)sgm.Index, selectedImageWidth, selectedImageHeight, (bool)checkBox_Bidirection.IsChecked, (bool)checkBox_FirstRev.IsChecked, Jobid++ /* arbitrary job ID */, IsUsingPartialBuffers);
                buttonStartJob.IsEnabled = false;
                comboBoxBitsPerPixel.IsEnabled = false;
                comboBoxGeoIndex.IsEnabled = false;
                checkBox_Bidirection.IsEnabled = false;
                checkBox_FirstRev.IsEnabled = false;
            } else if (comboBoxGeoIndex.SelectedItem is AutoGeometryModel) {
                AutoGeometryModel agm = (AutoGeometryModel)comboBoxGeoIndex.SelectedItem;

                // We let the user configure the AutoSwathGeometry just before startjob.
                // Alternatively, this could be moved up into the settings menu, or 
                // there could be a list of them available.
                if (AutoGeometryWindow.ConfigureAutoGeometry(out agm))
                {
                    MeteorSwath.StartAutoJob(agm.Geometry, selectedImageWidth, selectedImageHeight, (bool)checkBox_Bidirection.IsChecked, (bool)checkBox_FirstRev.IsChecked, Jobid++ /* arbitrary job ID */, IsUsingPartialBuffers);
                }
            }
        }

        /// <summary>
        /// Handler for the "SendImage" button click
        /// </summary>
        private void buttonSendImage_Click(object sender, RoutedEventArgs e)
        {
            if (fullPathToImage != "")
            {
                // Find out how many colour planes there are
                int NumPlanes = 1;
                TAppStatus status;
                if (PrinterInterfaceCLS.PiGetPrnStatus(out status) == eRET.RVAL_OK) {
                    NumPlanes = status.NumPlanes;
                }
                int Plane;
                if (comboBoxPlane.SelectedItem is string) { /* "All" is the only string in the combo box */
                    Plane = -1;
                } else {
                    Plane = (int)comboBoxPlane.SelectedItem; /* N.B. Meteor planes are indexed from 1 */
                }
                if (IsUsingPartialBuffers)
                {
                    SwathImage im = new SwathImage();
                    if (im.Load(fullPathToImage)) 
                    {
                        int startLine, endLine;
                        if (SendPartialImageWindow.GetImageSection(im.DocHeight, out startLine, out endLine))
                        {
                            im.StartLine = startLine;
                            im.EndLine = endLine;

                            int XStart = int.Parse(textBoxXPos.Text);
                            if (Plane == -1) {
                                for (int plane = 1; plane <= NumPlanes; plane++) {
                                    MeteorSwath.SendImage(im, plane, XStart, im.StartLine);
                                }
                            } else {
                                MeteorSwath.SendImage(im, Plane, XStart, im.StartLine);
                            }
                        }
                    }
                }
                else
                {
                    // X start position of the image.  This must be the same for
                    // each colour plane
                    int XStart = int.Parse(textBoxXPos.Text);
                    // If this is a TIFF file, see if MeteorSwath can handle it natively.
                    // The TIFF file must be strip based and match the Meteor print resolution.
                    // If this fails, load via an image buffer
                    bool bTiffLoaded = false;
                    if (fullPathToImage.ToLower().Contains(".tif")) 
                    {
                        if (Plane == -1) 
                        {
                            int planebitmask = (1 << NumPlanes) - 1;
                            bTiffLoaded = MeteorSwath.SendTIFFMultplePlanes(fullPathToImage, planebitmask, XStart);
                        } 
                        else 
                        {
                            bTiffLoaded = MeteorSwath.SendTIFF(fullPathToImage, Plane, XStart);
                        }
                    }
                    if (!bTiffLoaded) 
                    {
                        SwathImage im = new SwathImage();
                        if (im.Load(fullPathToImage)) 
                        {
                            if (Plane == -1) 
                            {
                                int planebitmask = (1 << NumPlanes) - 1;
                                MeteorSwath.SendImageMultiplePlanes(im, planebitmask, XStart);
                            } 
                            else 
                            {
                                MeteorSwath.SendImage(im, Plane, XStart);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handler for the "EndJob" button click
        /// </summary>
        private void buttonEndJob_Click(object sender, RoutedEventArgs e)
        {
            MeteorSwath.EndJob();
            buttonEndJob.IsEnabled = false;
            buttonProcessOne.IsEnabled = false;
            buttonProcessAll.IsEnabled = false;
            buttonSendImage.IsEnabled = false;
            comboBoxPlane.IsEnabled = false;
            textBoxXPos.IsEnabled = false;
            labelPlane.IsEnabled = false;
            labelXpos.IsEnabled = false;
        }

        /// <summary>
        /// Handler for the "Abort" button click
        /// </summary>
        private void buttonAbort_Click(object sender, RoutedEventArgs e) 
        {
            MeteorSwath.Abort();
        }

        /// <summary>
        /// Handler for the "Set Meteor Home Position" button click
        /// </summary>
        private void buttonSetHome_Click(object sender, RoutedEventArgs e) 
        {
            MeteorSwath.SetMeteorHomePosition();
        }

        /// <summary>
        /// Event handler called when the main window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e) 
        {
            LoadSettings();
        }

        /// <summary>
        /// Event handler called when the main window is about to close
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MeteorSwath.CancelPolling();
            MeteorSwath.Disconnect();
            MeteorSwath.StopPrintEngine(true);
            SaveSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayImage() 
        {
            if (fullPathToImage != null)
            {
                BitmapImage im = new BitmapImage(new Uri(fullPathToImage));
                image_Preview.Source = im;

                selectedImageWidth = im.PixelWidth;
                selectedImageHeight = im.PixelHeight;
            }
            else
            {
                image_Preview.Source = null;

                selectedImageWidth = 0;
                selectedImageHeight = 0;
            }

            textBox_Width.Text = selectedImageWidth.ToString();
            textBox_Height.Text = selectedImageHeight.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        private void LoadImage(string FileName) 
        {
            fullPathToImage = FileName;
            textBox_Filename.Text = System.IO.Path.GetFileName(fullPathToImage);
            DisplayImage();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseImage()
        {
            fullPathToImage = null;
            textBox_Filename.Text = "";
            DisplayImage();
        }

        /// <summary>
        /// Load the application settings, upgrading from a previous version if necessary
        /// </summary>
        private void LoadSettings() 
        {
            if (Properties.Settings.Default.NeedsUpgrade ) 
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.NeedsUpgrade = false;
            }
            if (File.Exists(Properties.Settings.Default.ImagePath)) 
            {
                LoadImage(Properties.Settings.Default.ImagePath);
            }
        }

        /// <summary>
        /// Save the application settings
        /// </summary>
        private void SaveSettings() 
        {
            Properties.Settings.Default.ImagePath = fullPathToImage;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handler for a change of selection in the "Bits per pixel" combo box.
        /// We call into the Meteor PrinterInterface to change the bits per pixel,
        /// unless setMeteorBppOnChange is false.  This happens when we're setting 
        /// the selection in the combo box on first connection to match the default 
        /// Meteor value.
        /// </summary>
        private void comboBoxBitsPerPixel_SelectionChanged(object sender, SelectionChangedEventArgs e)        
        {
            if (setMeteorBppOnChange) 
            {
                int val = (int)comboBoxBitsPerPixel.SelectedValue;
                MeteorSwath.SetBpp(val);
            }
        }

        /// <summary>
        /// Handler called when the user types into the X Position text box; filters out
        /// all non-numeric characters
        /// </summary>
        private void textBoxXPos_PreviewTextInput(object sender, TextCompositionEventArgs e) 
        {
            Regex regex = new Regex("[0-9]");
            e.Handled = !regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Handler for when the user clicks on Update Encoder. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUpdateEncoder_Click(object sender, RoutedEventArgs e)
        {
            SwathGeometryModel sgm = comboBoxGeoIndex.SelectedItem as SwathGeometryModel;

            int divider = SelectEncoderDividerWindow.GetEncoderDivider(10);
            if (divider != -1)
            {
                Ttp.Meteor.MeteorSwath.MeteorSwathInterface.MeteorSwathUpdateEncoderResolution((uint)divider);
            }
        }

        /// <summary>
        /// Handler when user clicks on the "Set Param" menu item.
        /// Opens the SetParam dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemConfigSetParam_Click(object sender, RoutedEventArgs e)
        {
            Window window = new SetParamWindow();
            window.ShowDialog();
        }

        /// <summary>
        /// Handler when user clicks File->Exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handler when user clicks File->Load. Prompts the user to choose and image
        /// and then loads the image data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFileLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TIFF Files (*.tif)|*.tif|Bitmap files (*.bmp)|*.bmp";
            dlg.Multiselect = false;
            dlg.ShowDialog();

            if (System.IO.File.Exists(dlg.FileName))
            {
                LoadImage(dlg.FileName);
            }
        }

        /// <summary>
        /// Handler when user clicks File->CloseImage. Closes the file handle to the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFileCloseImage_Click(object sender, RoutedEventArgs e)
        {
            CloseImage();
        }

        /// <summary>
        /// Handler when the user clicks the Process One button. Prompts the user
        /// for the index of the swath to process, then calls MeteorSwathProcessOneSwath.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonProcessOne_Click(object sender, RoutedEventArgs e)
        {
            int requestedIndex = SelectSwathWindow.GetSwathIndex(SwathRequired);
            if (requestedIndex != -1)
            {
                if (IsUsingPartialBuffers)
                {
                    List<TSwathDetails> allSwaths = MeteorSwath.SwathDetails;

                    if (requestedIndex < allSwaths.Count) 
                    {
                        TSwathDetails sw = allSwaths[requestedIndex];
                        Console.WriteLine(String.Format("Partial buffer state. FULL: {0}. GAPSTART: {1}. GAPCOUNT: {2}",
                            sw.IsPartialBufferFull[0], sw.PartialBufferEmptySectionStart[0] + 1, sw.PartialBufferEmptyLinesCount[0]));
                    }
                }

                MeteorSwathInterface.MeteorSwathProcessOneSwath(requestedIndex);
            }
        }

        /// <summary>
        /// Handler for when the user clicks Process All. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonProcessAll_Click(object sender, RoutedEventArgs e)
        {
            MeteorSwathInterface.MeteorSwathProcessAllSwaths();
        }
    }
}
