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
using System.Windows.Shapes;
using Ttp.Meteor.MeteorSwath;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// Interaction logic for AutoGeometryWindow.xaml
    /// </summary>
    public partial class AutoGeometryWindow : Window
    {
        public AutoGeometryModel GeometryModel { get; set; }

        private AutoGeometryWindow()
        {
            InitializeComponent();

            GeometryModel = new AutoGeometryModel();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            GeometryModel.Geometry = new AutoSwathGeometry()
            {
                ExactlyEqualSteps = checkBoxExactlyEqual.IsChecked == true,
                Overprints = (double)numericEditOverprints.Value,
                Passes = (int)numericEditPasses.Value,
                StitchbandWidth = (int)numericEditStitchband.Value,
                XFields = (int)numericEditXInterlace.Value,
                YFields = (int)numericEditYInterlace.Value,
            };

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Lets the user set up an AutoSwathGeomtry
        /// </summary>
        /// <param name="geometryModel"></param>
        /// <returns></returns>
        internal static bool ConfigureAutoGeometry(out AutoGeometryModel geometryModel)
        {
            AutoGeometryWindow agw = new AutoGeometryWindow();
            agw.Owner = Application.Current.MainWindow;
            agw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            agw.ShowDialog();


            if (agw.DialogResult == true)
            {
                geometryModel = agw.GeometryModel;
                return true;
            }
            else
            {
                geometryModel = null;
                return false;
            }
        }

    }
}
