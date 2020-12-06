using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GW2BuildLibrary.UI.Converters
{
    /// <summary>
    /// Converter used to convert a <see cref="bool"/> into a <see cref="Visibility"/>.
    /// </summary>
    /// <remarks>An input of <c>true</c> produces <c>Visible</c>, <c>false</c> produces <c>Collapsed</c>.</remarks>
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        #endregion Methods
    }
}