using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace LuckyProject.Lib.WinUi.Converters
{
    public class ElementThemeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is not string enumString)
            {
                throw new ArgumentException(nameof(parameter),  "Parameter Must Be An Enum Name");
            }

            if (!Enum.IsDefined(typeof(ElementTheme), value))
            {
                throw new ArgumentException(nameof(value), "Value Must Be An ElementTheme");
            }

            return Enum.Parse(typeof(ElementTheme), enumString).Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(typeof(ElementTheme), enumString);
            }

            throw new ArgumentException(nameof(parameter), "Parameter Must Be An Enum Name");
        }
    }
}
