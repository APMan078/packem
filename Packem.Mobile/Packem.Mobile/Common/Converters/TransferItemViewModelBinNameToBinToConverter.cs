using Packem.Domain.Common.ExtensionMethods;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.Converters
{
    public class TransferItemViewModelBinNameToBinToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bin = (string)value;

            if (bin.HasValue())
            {
                return bin;
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
