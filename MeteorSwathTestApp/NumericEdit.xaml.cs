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
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Markup;
using System.ComponentModel;

namespace MeteorSwathTestApp
{
    public delegate void ValueChangedEventHandler(decimal newValue);

    /// <summary>
    /// Interaction logic for NumericEdit.xaml
    /// </summary>
    public partial class NumericEdit : UserControl, IValueConverter
    {
        /// <summary>
        /// Not ideal - this is just to make it easier to replace the event-driven
        /// ControlNumericEdit. Use binding instead, where possible.
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Value of the NumericEdit
        /// </summary>
        [Category("Number")]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(NumericEdit), new FrameworkPropertyMetadata(0m, null, CoerceCurrentValue) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Amount the value is incremented or decremented by the up / down buttons
        /// </summary>
        [Category("Number")]
        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(decimal), typeof(NumericEdit), new FrameworkPropertyMetadata(1m) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Maximum value
        /// </summary>
        [Category("Number")]
        public decimal Max
        {
            get { return (decimal)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(decimal), typeof(NumericEdit), new FrameworkPropertyMetadata(150m, null, CoerceMinMax) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Minimum value
        /// </summary>
        [Category("Number")]
        public decimal Min
        {
            get { return (decimal)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(decimal), typeof(NumericEdit), new FrameworkPropertyMetadata(0m, null, CoerceMinMax) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Number of decimal places displayed
        /// </summary>
        [Category("Number")]
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericEdit));

        /// <summary>
        /// Text to display after value.
        /// </summary>
        [Category("Number")]
        public string DisplaySuffix
        {
            get { return (string)GetValue(DisplaySuffixProperty); }
            set { SetValue(DisplaySuffixProperty, value); }
        }
        public static readonly DependencyProperty DisplaySuffixProperty =
            DependencyProperty.Register("DisplaySuffix", typeof(string), typeof(NumericEdit));

        /// <summary>
        /// Text to display before value.
        /// </summary>
        [Category("Number")]
        public string DisplayPrefix
        {
            get { return (string)GetValue(DisplayPrefixProperty); }
            set { SetValue(DisplayPrefixProperty, value); }
        }
        public static readonly DependencyProperty DisplayPrefixProperty =
            DependencyProperty.Register("DisplayPrefix", typeof(string), typeof(NumericEdit));

        /// <summary>
        /// Optional multiplier to apply to the value before it is displayed.
        /// If the scale can not be displayed as a decimal (e.g. 1/3, sqrt(2)),
        /// then ensure you put more digits than the user will see.
        /// </summary>
        [Category("Number")]
        public decimal DisplayScale
        {
            get { return (decimal)GetValue(DisplayScaleProperty); }
            set { SetValue(DisplayScaleProperty, value); }
        }
        public static readonly DependencyProperty DisplayScaleProperty =
            DependencyProperty.Register("DisplayScale", typeof(decimal), typeof(NumericEdit), new UIPropertyMetadata(1m));

        Timer ClickHoldTicker;
        DateTime lastClick = DateTime.Now;
        /// <summary>
        /// Called by the timer when either button is held down.
        /// </summary>
        /// <param name="state"></param>
        private void ClickHoldTimerCallback(object state)
        {
            Dispatcher.Invoke((Action)(() => AutoTick()));
        }

        /// <summary>
        /// 
        /// </summary>
        private void AutoTick()
        {
            if (buttonUp.IsPressed)
            {
                int steps = (int)Math.Pow(5, (int)DateTime.Now.Subtract(lastClick).TotalSeconds / 2);
                Increment(steps);
            }
            else if (buttonDown.IsPressed)
            {
                int steps = (int)Math.Pow(5, (int)DateTime.Now.Subtract(lastClick).TotalSeconds / 2);
                Decrement(steps);
            }
            else
            {
                lastClick = DateTime.Now;
            }
        }

        /// <summary>
        /// Coerce the CurrentValue to within Min and Max.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceCurrentValue(DependencyObject d, object value)
        {
            NumericEdit edit = d as NumericEdit;
            decimal dec = (decimal)value;

            if (dec < edit.Min)
            {
                return edit.Min;
            }

            if (dec > edit.Max)
            {
                return edit.Max;
            }

            return value;
        }

        /// <summary>
        /// Truncate either Min or max to the correct number of decimal places.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceMinMax(DependencyObject d, object value)
        {
            NumericEdit edit = d as NumericEdit;
            decimal decVal = (decimal)value;
            decimal scale = (decimal)Math.Pow(10, edit.DecimalPlaces);
            decVal = Math.Truncate(decVal * scale) / scale;
            return decVal;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NumericEdit()
        {
            InitializeComponent();

            textBoxValue.SetBinding(TextBox.TextProperty, new Binding("Value") { Converter = this, Mode = BindingMode.OneWay });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            Increment();
            ClickHoldTicker = new Timer(ClickHoldTimerCallback, null, 500, 100);
            lastClick = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="steps"></param>
        private void Increment(int steps = 1)
        {
            // Go to the next step. eg Increasing 17.6 to the next step, where Step == 1, 
            // the next step is 18, not 18.6
            decimal delta = Step * steps * DisplayScale;
            decimal newValue = (Value + delta);
            decimal clippedValue = newValue - (newValue % delta);
            Value = AdjustValue(clippedValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            Decrement();
            ClickHoldTicker = new Timer(ClickHoldTimerCallback, null, 500, 100);
            lastClick = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="steps"></param>
        private void Decrement(int steps = 1)
        {
            // Go to the next step. eg Increasing 17.6 to the next step, where Step == 1, 
            // the next step is 18, not 18.6
            decimal delta = Step * steps * DisplayScale;
            decimal newValue = (Value - delta);
            if (newValue % delta > 0)
            {
                newValue += (delta - newValue % delta);
            }
            Value = AdjustValue(newValue);
        }


        /// <summary>
        /// Adjusts the value to be within Min/Max and to the correct number
        /// of decimals place.
        /// </summary>
        /// <param name="value"></param>
        private decimal AdjustValue(decimal value)
        {
            value = value > Max ? Max : value;
            value = value < Min ? Min : value;
            decimal scale = (decimal)Math.Pow(10, DecimalPlaces);
            return value;

            // Use this line to fix the stored value to multiples of Step. Should Step be for display use only?
            // return Math.Truncate(value * scale) / scale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == MinProperty || e.Property == MaxProperty || e.Property == DecimalPlacesProperty
                || e.Property == DisplaySuffixProperty || e.Property == DisplayPrefixProperty
                || e.Property == DisplayScaleProperty)
            {
                InvalidateProperty(ValueProperty);
                textBoxValue.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }

            if (e.Property == ValueProperty)
            {
                var handler = ValueChanged;
                if (handler != null)
                {
                    handler((decimal)e.NewValue);
                }
            }

            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ClickHoldTicker != null)
            {
                ClickHoldTicker.Dispose();
                ClickHoldTicker = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (ClickHoldTicker != null)
            {
                ClickHoldTicker.Dispose();
                ClickHoldTicker = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (ClickHoldTicker != null)
            {
                ClickHoldTicker.Dispose();
                ClickHoldTicker = null;
            }
        }

        private void textBoxValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                TextBoxTextToValue();
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                Increment();
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                Decrement();
                e.Handled = true;
            }
        }

        /// <summary>
        /// For a 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxValue_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBoxTextToValue();
        }

        private void TextBoxTextToValue()
        {
            IValueConverter c = this;

            object converted = c.ConvertBack(textBoxValue.Text, typeof(decimal), null, null);
            if (converted != DependencyProperty.UnsetValue)
            {
                Value = AdjustValue((decimal)converted);
            }

            BindingExpression be = textBoxValue.GetBindingExpression(TextBox.TextProperty);
            be.UpdateTarget();
        }

        private string TextWithoutPrefixAndSuffix(string input)
        {
            if (input == null) { return ""; }

            if (DisplayPrefix != null && input.StartsWith(DisplayPrefix))
            {
                input = input.Remove(0, DisplayPrefix.Length);
            }

            if (DisplaySuffix != null && input.EndsWith(DisplaySuffix))
            {
                input = input.Substring(0, input.Length - DisplaySuffix.Length);
            }

            return input;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                decimal dec = (decimal)value;
                dec /= DisplayScale;
                // Force the correct number of decimal places.
                string rString = dec.ToString("N" + DecimalPlaces);
                rString = string.Format("{0}{1}{2}", DisplayPrefix, rString, DisplaySuffix);
                return rString;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string entryString = TextWithoutPrefixAndSuffix(value as string);

                decimal decVal = System.Convert.ToDecimal(entryString);
                return decVal * DisplayScale;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

    }
}

