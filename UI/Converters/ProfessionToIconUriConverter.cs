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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Profession profession = (Profession)value;

            // Show no icon for a blank profession
            if (profession == Profession.None)
                return null;

            return $"pack://application:,,,/Icons/{profession}_Profession.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
