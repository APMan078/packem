using Packem.Domain.Common.ExtensionMethods;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Converters
{
    public class StringNullOrEmptyToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;

            if (str.HasValue())
            {
                return str;
            }
            else
            {
                return (string)parameter;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}