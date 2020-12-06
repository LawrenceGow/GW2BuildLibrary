using System;
using System.Globalization;
using System.Windows.Data;

namespace GW2BuildLibrary.UI.Converters
{
    /// <summary>
    /// Converter used to convert a given <see cref="Specialization"/> member into its icon location.
    /// </summary>
    public class SpecializationToIconUriConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Specialization specialization = (Specialization)value;

            // Show no icon for a blank profession
            if (specialization == Specialization.None)
                return null;

            return $"pack://application:,,,/Icons/{specialization}_Specialization.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        #endregion Methods
    }
}