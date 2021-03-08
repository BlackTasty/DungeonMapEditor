using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DungeonMapEditor.Converter
{
    class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double valueDouble)
            {
                return valueDouble.ToString();
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string valueString)
            {
                if (double.TryParse(valueString, out double valueDouble))
                {
                    return valueDouble;
                }
            }

            return Binding.DoNothing;
        }
    }
}
