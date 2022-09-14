using Packem.Mobile.Models.Enums;
using Packem.Mobile.Modules.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.MarkupExtensions
{
    public class ActivityIndicatorViewModelToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ActivityIndicatorViewModel vm)
            {
                if (vm.ActivityIndicatorType == ActivityIndicatorType.Default)
                    return 1;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.TransparentBackground)
                    return 0.8;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.DefaultWithMessage)
                    return 1;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.TransparentBackgroundWithMessage)
                    return 0.8;
                else
                    return 1;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
