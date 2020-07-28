using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GW2BuildLibrary
{
    public class MultiEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object value = values[0];
            if (value != null)
            {
                bool areEqual = true;
                for (int i = 1; i < values.Length; i++)
                    areEqual &= value.Equals(values[i]);
                return areEqual;
            }
            else
                return values.All(v => v == null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
