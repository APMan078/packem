using Packem.Domain.Common.ExtensionMethods;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Converters
{
    public class TransferItemViewModelZoneNameToZoneToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var zone = (string)value;

            if (zone.HasValue())
            {
                return zone;
            }
            else
            {
                return "Any";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
