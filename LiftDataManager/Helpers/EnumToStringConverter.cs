namespace LiftDataManager.Helpers
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string EnumString;
            try
            {
                EnumString = Enum.GetName(value.GetType(), value);
                return EnumString;
            }
            catch
            {
                return string.Empty;
            }
        }

        //// No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
