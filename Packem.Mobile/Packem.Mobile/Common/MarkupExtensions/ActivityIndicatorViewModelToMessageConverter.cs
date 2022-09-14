using Packem.Mobile.Models.Enums;
using Packem.Mobile.Modules.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.MarkupExtensions
{
    public class ActivityIndicatorViewModelToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ActivityIndicatorViewModel vm)
            {
                if (vm.ActivityIndicatorType == ActivityIndicatorType.Default)
                    return null;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.TransparentBackground)
                    return null;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.DefaultWithMessage)
                    return vm.Message;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.TransparentBackgroundWithMessage)
                    return vm.Message;
                else
                    return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
