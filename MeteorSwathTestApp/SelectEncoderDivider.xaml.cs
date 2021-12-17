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
    /// Interaction logic for SelectEncoderDivider.xaml
    /// </summary>
    public partial class SelectEncoderDividerWindow : Window
    {
        public int EncoderDivider { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max"></param>
        private SelectEncoderDividerWindow(int max)
        {
            InitializeComponent();

            EncoderDivider = 0; // 0 is "none selected". 

            for (int i = 0; i < max; ++i)
            {
                comboBoxValues.Items.Add(i + 1); // Show the user a list starting with 1.
            }

            if (comboBoxValues.Items.Count > 0)
            {
                comboBoxValues.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Asks the user for a number between 1 and max (inclusive).
        /// </summary>
        /// <param name="max">Max encoder divider</param>
        /// <returns>The new encoder divider, or 0 if user cancelled</returns>
        public static int GetEncoderDivider(int max)
        {
            SelectEncoderDividerWindow ssw = new SelectEncoderDividerWindow(max);
            ssw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ssw.ShowDialog();

            return ssw.EncoderDivider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            EncoderDivider = (int)comboBoxValues.SelectedItem;
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
