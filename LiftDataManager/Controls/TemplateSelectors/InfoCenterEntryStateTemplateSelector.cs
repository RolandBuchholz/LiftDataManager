namespace LiftDataManager.Controls.TemplateSelectors;

public class InfoCenterEntryStateTemplateSelector : DataTemplateSelector
{
    public DataTemplate? None
    {
        get; set;
    }
    public DataTemplate? InfoCenterMessage
    {
        get; set;
    }
    public DataTemplate? InfoCenterWarning
    {
        get; set;
    }
    public DataTemplate? InfoCenterError
    {
        get; set;
    }
    public DataTemplate? InfoCenterParameterChanged
    {
        get; set;
    }
    public DataTemplate? InfoCenterSaveParameter
    {
        get; set;
    }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return SelectTemplateCore(item, null);
    }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject? container)
    {
        if (item is not null and InfoCenterEntry)
        {
            var infoCenterEntry = (InfoCenterEntry)item;

            return infoCenterEntry.State.Value switch
            {
                0 => None,
                1 => InfoCenterMessage,
                2 => InfoCenterWarning,
                3 => InfoCenterError,
                4 => InfoCenterParameterChanged,
                5 => InfoCenterSaveParameter,
                _ => None,
            };
        }
        else
        {
            return None;
        }
    }
}
