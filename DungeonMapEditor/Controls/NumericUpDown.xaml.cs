using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DungeonMapEditor.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : Border
    {
        public event EventHandler<EventArgs> ValueChanged;

        private DispatcherTimer valueDelayTimer;
        private DispatcherTimer valueChangeTimer;
        private bool isValueUp;

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericUpDown), new PropertyMetadata(double.MaxValue));
        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0d));
        #endregion

        #region DefaultValue
        public double DefaultValue
        {
            get { return (double)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0d));
        #endregion

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NumericUpDown), new PropertyMetadata("0"));
        #endregion

        #region Foreground
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(NumericUpDown), new PropertyMetadata(Brushes.White));
        #endregion

        #region FontWeight
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(NumericUpDown), new PropertyMetadata(FontWeights.Normal));
        #endregion

        #region Value
        public double Value
        {
            get 
            {
                return (double)GetValue(ValueProperty); 
            }
            set
            {
                if (DecimalPlaces > 0)
                {
                    value = Math.Round((double)GetValue(ValueProperty), DecimalPlaces);
                }
                else
                {
                    value = Math.Round((double)GetValue(ValueProperty), 0);
                }

                value = Math.Max(value, Minimum);
                value = Math.Min(value, Maximum);
                SetValue(ValueProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0d));
        #endregion

        #region DecimalPlaces
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecimalPlaces.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericUpDown), new PropertyMetadata(-1));
        #endregion

        #region ChangeAmount
        public double ChangeAmount
        {
            get { return (double)GetValue(ChangeAmountProperty); }
            set { SetValue(ChangeAmountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeAmount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeAmountProperty =
            DependencyProperty.Register("ChangeAmount", typeof(double), typeof(NumericUpDown), new PropertyMetadata(.1d));
        #endregion

        public NumericUpDown()
        {
            InitializeComponent();

            valueChangeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            valueChangeTimer.Tick += ValueChangeTimer_Tick;

            valueDelayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(750)
            };
            valueDelayTimer.Tick += ValueDelayTimer_Tick;
        }

        private void ValueChangeTimer_Tick(object sender, EventArgs e)
        {
            if (isValueUp)
            {
                ValueUp_Click(sender, new RoutedEventArgs());
            }
            else
            {
                ValueDown_Click(sender, new RoutedEventArgs());
            }
        }

        private void ValueDelayTimer_Tick(object sender, EventArgs e)
        {
            valueDelayTimer.Stop();
            valueChangeTimer.Start();
        }

        private void ValueUp_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Text, out double value))
            {
                value += ChangeAmount;
                if (value <= Maximum)
                {
                    //Value = value;
                    input.Text = value.ToString();
                }
            }
            else
            {
                //Value = 1;
                input.Text = "1";
            }
            RefreshValue();
        }

        private void ValueDown_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Text, out double value))
            {
                value -= ChangeAmount;
                if (value >= Minimum)
                {
                    //Value = value;
                    input.Text = value.ToString();
                }
            }
            else
            {
                //Value = -1;
                input.Text = "-1";
            }
            RefreshValue();
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void NumericTextBox_ValueChanged(object sender, TextChangedEventArgs e)
        {
            RefreshValue();
        }

        private void RefreshValue()
        {
            Text = input.Text;
            Value = double.Parse(Text.Replace('.', ','));
            OnValueChanged(EventArgs.Empty);
        }

        private void ValueUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartRaiseValue(sender, e.RoutedEvent);
        }

        private void ValueDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartLowerValue(sender, e.RoutedEvent);
        }

        private void ValueButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            StopTimers();
        }

        private void control_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                StartRaiseValue(sender, e.RoutedEvent);
            }
            else if (e.Key == Key.Down)
            {
                StartLowerValue(sender, e.RoutedEvent);
            }
        }

        private void control_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            StopTimers();
        }

        private void StartRaiseValue(object sender, RoutedEvent routedEvent)
        {
            isValueUp = true;
            ValueUp_Click(sender, new RoutedEventArgs(routedEvent));
            valueDelayTimer.Start();
        }

        private void StartLowerValue(object sender, RoutedEvent routedEvent)
        {
            isValueUp = false;
            ValueDown_Click(sender, new RoutedEventArgs(routedEvent));
            valueDelayTimer.Start();
        }

        private void StopTimers()
        {
            valueDelayTimer.Stop();
            valueChangeTimer.Stop();
        }
    }
}
