using System;
using System.Globalization;
using System.Windows.Data;

namespace GW2BuildLibrary.UI.Converters
{
    /// <summary>
    /// Converter used to convert a given <see cref="Profession"/> member into its icon location.
    /// </summary>
    public class ProfessionToIconUriConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The boolean value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An Uri to the icon for the input <see cref="Profession"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Profession profession = (Profession)value;

            // Show no icon for a blank profession
            if (profession == Profession.None)
                return null;

            return $"pack://application:,,,/Icons/{profession}_Profession.png";
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