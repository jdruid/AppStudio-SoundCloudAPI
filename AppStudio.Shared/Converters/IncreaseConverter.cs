using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AppStudio.Controls
{
    public class IncreaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int increment = 0;
            if (parameter is int)
            {
                increment = (int)parameter;
            }
            else if (parameter is string)
            {
                int.TryParse(parameter as string, out increment);
            }

            if (value is int)
            {    
                value = (int)value + increment;
            }
            else if (value is decimal)
            {
                value = (decimal)value + increment;
            }
            else if (value is double)
            {
                value = (double)value + increment;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
