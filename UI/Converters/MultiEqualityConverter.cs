using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GW2BuildLibrary.UI.Converters
{
    /// <summary>
    /// Converter to check if all values are equal.
    /// </summary>
    public class MultiEqualityConverter : IMultiValueConverter
    {
        #region Methods

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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        #endregion Methods
    }
}