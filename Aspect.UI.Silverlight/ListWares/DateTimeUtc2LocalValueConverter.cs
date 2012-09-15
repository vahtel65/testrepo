using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace ListWares
{
    public class DateTimeUtc2LocalValueConverter : IValueConverter
    {

        public DateTimeUtc2LocalValueConverter()
        {

        }

        public DateTimeUtc2LocalValueConverter(string dateFormat)
        {
            DateFormat = dateFormat;
        }

        public string DateFormat { get; set; }

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime valueUtc;
            if (value == null)
            {
                return null;
            }
            else if (value is string && !DateTime.TryParse((string)value, out valueUtc))
            {
                return null;
            }
            else if (!(value is DateTime || value is DateTime?))
            {
                return null;
            }

            DateTime valueLocal = ((DateTime)value).ToLocalTime();

            if (targetType == typeof(string))
            {
                return DateFormat == null ? valueLocal.ToString() : valueLocal.ToString(DateFormat);
            }
            else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                return valueLocal;
            }
            return null;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            DateTime valueLocal;

            if (value == null)
            {
                return null;
            }
            else if (value is string && !DateTime.TryParse((string)value, out valueLocal))
            {
                return null;
            }
            else if (!(value is DateTime || value is DateTime?))
            {
                return null;
            }

            DateTime valueUtc = ((DateTime)value).ToUniversalTime();

            if (targetType == typeof(string))
            {
                return DateFormat == null ? valueUtc.ToString() : valueUtc.ToString(DateFormat);
            }
            else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                return valueUtc;
            }

            return null;
        }

        #endregion
    }
}
