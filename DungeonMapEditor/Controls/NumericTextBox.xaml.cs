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

namespace DungeonMapEditor.Controls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : TextBox
    {
        public event EventHandler<TextChangedEventArgs> ValueChanged;

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MaxValue));
        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MinValue));
        #endregion

        #region DefaultValue
        public double DefaultValue
        {
            get { return (double)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(double), typeof(NumericTextBox), new PropertyMetadata(0d));
        #endregion

        public NumericTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInputAllowed((sender as TextBox).Text);
            if (!e.Handled)
            {
                e.Handled = !IsInputAllowed(e.Text);
            }
        }

        private bool IsInputAllowed(string text)
        {
            if (double.TryParse(text, out double value))
            {
                return value <= Maximum && value >= Minimum;
            }

            return false;
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsInputAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox target = sender as TextBox;
            if (int.TryParse(target.Text, out int inputNumber))
            {
                if (inputNumber < Minimum)
                {
                    Text = Minimum.ToString();
                }
                else if (inputNumber > Maximum)
                {
                    Text = Maximum.ToString();
                }
            }
            else
            {
                Text = DefaultValue.ToString();
            }
            OnValueChanged(new TextChangedEventArgs(e.RoutedEvent, UndoAction.None));
        }

        protected virtual void OnValueChanged(TextChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnValueChanged(e);
        }

        private void input_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}