using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using LuckyProject.Lib.WinUi.Models;

namespace LuckyProject.Lib.WinUi.Converters
{
    public class LpAppMessageTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is not string enumString)
            {
                throw new ArgumentException(nameof(parameter), "Parameter Must Be An Enum Name");
            }

            if (!Enum.IsDefined(typeof(LpAppMessageType), value))
            {
                throw new ArgumentException(nameof(value), "Value Must Be An LpAppMessageType");
            }

            return Enum.Parse(typeof(LpAppMessageType), enumString).Equals(value)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
