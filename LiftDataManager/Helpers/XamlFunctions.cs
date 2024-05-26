namespace LiftDataManager.Helpers;
public static class XamlFunctions
{
    public static Visibility TrueToVisible(bool value)
    {
        return value is true
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public static Visibility NotEmptyStringToVisible(string value)
    {
        return string.IsNullOrEmpty(value) is not true
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public static Visibility DoubleNot0ToVisible(double value)
    {
        return value != 0d
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
