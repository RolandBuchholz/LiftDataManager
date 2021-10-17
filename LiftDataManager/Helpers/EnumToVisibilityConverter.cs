using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace LiftDataManager.Helpers
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public Visibility NonVisibleVisibility { get; set; }

        public EnumToVisibilityConverter()
        {
            NonVisibleVisibility = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.ToString() == parameter.ToString())
            {
                return Visibility.Visible;
            }
            else
            {
                return NonVisibleVisibility;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
