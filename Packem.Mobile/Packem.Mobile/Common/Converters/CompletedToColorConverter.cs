using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Converters
{
    public class CompletedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var completed = (bool)value;

            if (completed)
            {
                return Color.FromHex("#32CD32");
            }
            else
            {
                return Color.FromHex("#000000");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
