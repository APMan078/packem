using Packem.Mobile.Modules.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.MarkupExtensions
{
    public class ActivityIndicatorViewModelToIsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ActivityIndicatorViewModel vm)
            {
                if (vm.IsRunning)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
