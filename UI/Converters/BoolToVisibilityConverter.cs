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

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The boolean value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An input of <c>true</c> produces <c>Visible</c>, <c>false</c> produces <c>Collapsed</c>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The type to convert to.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Nothing.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        #endregion Methods
    }
}