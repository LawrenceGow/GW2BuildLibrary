using System;
using System.Globalization;
using System.Windows.Data;

namespace GW2BuildLibrary
{
    public class ProfessionToIconUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Profession profession = (Profession)value;
            return $"..\\Icons\\{profession}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
