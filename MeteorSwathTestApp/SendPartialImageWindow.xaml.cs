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
    /// Interaction logic for SendPartialImageWindow.xaml
    /// </summary>
    public partial class SendPartialImageWindow : Window
    {
        public int TopLine
        {
            get { return (int)GetValue(TopLineProperty); }
            set { SetValue(TopLineProperty, value); }
        }
        public static readonly DependencyProperty TopLineProperty =
            DependencyProperty.Register("TopLine", typeof(int), typeof(SendPartialImageWindow), new UIPropertyMetadata(1));

        public int BottomLine
        {
            get { return (int)GetValue(BottomLineProperty); }
            set { SetValue(BottomLineProperty, value); }
        }
        public static readonly DependencyProperty BottomLineProperty =
            DependencyProperty.Register("BottomLine", typeof(int), typeof(SendPartialImageWindow), new UIPropertyMetadata(1));

        public int NumberOfLines
        {
            get { return (int)GetValue(NumberOfLinesProperty); }
            set { SetValue(NumberOfLinesProperty, value); }
        }
        public static readonly DependencyProperty NumberOfLinesProperty =
            DependencyProperty.Register("NumberOfLines", typeof(int), typeof(SendPartialImageWindow));

        public bool ClickedOk { get; set; }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TopLineProperty)
            {
                if (Convert.ToInt32(e.NewValue) > NumberOfLines || Convert.ToInt32(e.NewValue) < 1)
                {
                    TopLine = Convert.ToInt32(e.OldValue);
                    return;
                }
                BottomLine = Math.Max(TopLine, BottomLine);
            }
            else if (e.Property == BottomLineProperty)
            {
                if (Convert.ToInt32(e.NewValue) > NumberOfLines|| Convert.ToInt32(e.NewValue) < 1)
                {
                    BottomLine = Convert.ToInt32(e.OldValue);
                    return;
                }
                TopLine = Math.Min(TopLine, BottomLine);
            }

            base.OnPropertyChanged(e);
        }

        private SendPartialImageWindow(int nLines)
        {
            NumberOfLines = nLines;
            BottomLine = nLines;
            TopLine = 1;

            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            ClickedOk = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            ClickedOk = false;
            Close();
        }

        public static bool GetImageSection(int ImageLines, out int TopLine, out int BottomLine) {
            SendPartialImageWindow window = new SendPartialImageWindow(ImageLines);
            window.ShowDialog();

            if (!window.ClickedOk)
            {
                TopLine = 0;
                BottomLine = 0;
                return false;
            }
            else
            {
                TopLine = window.TopLine - 1;
                BottomLine = window.BottomLine;
                return true;
            }
        }
    }
}
