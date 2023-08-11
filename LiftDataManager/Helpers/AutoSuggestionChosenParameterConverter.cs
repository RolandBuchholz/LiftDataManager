namespace LiftDataManager.Helpers
{
    class AutoSuggestionChosenParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (AutoSuggestBoxSuggestionChosenEventArgs)value;
            if (string.IsNullOrWhiteSpace((string)args.SelectedItem))
                    return string.Empty;
            return (string)args.SelectedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
