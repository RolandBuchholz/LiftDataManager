using LiftDataManager.Core.DataAccessLayer.Models;

namespace LiftDataManager.Helpers;

public class NavigationPropertyToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
        {
            return string.Empty;
        }
        var navigationProperty = value as dynamic;

        if (navigationProperty.GetType() == typeof(TypeExaminationCertificate))
        {
            return navigationProperty.CertificateNumber;
        }
        return navigationProperty.Name;
    }

    //// No need to implement converting back on a one-way binding 
    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
