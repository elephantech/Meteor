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
using Ttp.Meteor;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// Interaction logic for SetParamWindow.xaml
    /// </summary>
    public partial class SetParamWindow : Window
    {
        public SetParamWindow()
        {
            InitializeComponent();
            comboBoxParamIds.ItemsSource = Enum.GetValues(typeof(eSWATHPARAM));
        }

        private void buttonSendParam_Click(object sender, RoutedEventArgs e)
        {
            object selected = comboBoxParamIds.SelectedItem;
            if (selected == null)
            {
                textBlockInfo.Text = String.Format("Error: No parameter selected");
                return;
            }

            eSWATHPARAM selectedParam = (eSWATHPARAM)selected;

            Int32 val;
            if (!Int32.TryParse(textBoxValueEntry.Text, out val)) {
                textBlockInfo.Text = String.Format("Error: Could not parse value as int: {0}", textBlockInfo.Text);
            }
            
            eRET rval = MeteorSwathInterface.MeteorSwathSetParam(selectedParam, val);
            if (rval == eRET.RVAL_OK)
            {
                textBlockInfo.Text = String.Format("SetParam success: {0} was set to {1}", selectedParam, val);
            }
            else
            {
                textBlockInfo.Text = String.Format("SetParam failed: {0} not set to {1}", selectedParam, val);
            }


        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
