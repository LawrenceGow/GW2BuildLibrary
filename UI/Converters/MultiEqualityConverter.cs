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

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it
        /// propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">
        /// The array of values that the source bindings in the System.Windows.Data.MultiBinding produces. The value
        /// <see cref="System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to
        /// provide for conversion.
        /// </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns><c>True</c> if all the inputs are equal to one another. Otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">
        /// The array of types to convert to. The array length indicates the number and types of values that are
        /// suggested for the method to return.
        /// </param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Nothing.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        #endregion Methods
    }
}