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

namespace MeteorSwathTestApp
{
    /// <summary>
    /// Interaction logic for SelectSwathWindow.xaml
    /// </summary>
    public partial class SelectSwathWindow : Window
    {
        public int SelectedSwathIndex { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max"></param>
        private SelectSwathWindow(int max)
        {
            InitializeComponent();

            SelectedSwathIndex = 0; // 0 is "no swath". 

            for (int i = 0; i < max; ++i)
            {
                comboBoxSwaths.Items.Add(i + 1); // Show the user a list starting with 1.
            }

            if (comboBoxSwaths.Items.Count > 0)
            {
                comboBoxSwaths.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Asks the user for a number between 1 and max (inclusive).
        /// </summary>
        /// <param name="max">Max number of swaths</param>
        /// <returns>The index of the swath selected, or -1 if no swath selected</returns>
        public static int GetSwathIndex(int max)
        {
            SelectSwathWindow ssw = new SelectSwathWindow(max);
            ssw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ssw.ShowDialog();

            return ssw.SelectedSwathIndex - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedSwathIndex = (int)comboBoxSwaths.SelectedItem;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
