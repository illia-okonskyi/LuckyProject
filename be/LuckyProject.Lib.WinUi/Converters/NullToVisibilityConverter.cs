using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;

namespace LuckyProject.Lib.WinUi.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var visibleIfNull = (bool?)parameter;
            var condition = visibleIfNull == true ? value == null : value != null;
            return condition ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
