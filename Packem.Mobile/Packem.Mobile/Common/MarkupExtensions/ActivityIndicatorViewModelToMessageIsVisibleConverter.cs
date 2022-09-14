using Packem.Mobile.Models.Enums;
using Packem.Mobile.Modules.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Packem.Mobile.Common.MarkupExtensions
{
    public class ActivityIndicatorViewModelToMessageIsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ActivityIndicatorViewModel vm)
            {
                if (vm.ActivityIndicatorType == ActivityIndicatorType.Default)
                    return false;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.TransparentBackground)
                    return false;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.DefaultWithMessage)
                    return true;
                else if (vm.ActivityIndicatorType == ActivityIndicatorType.TransparentBackgroundWithMessage)
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
