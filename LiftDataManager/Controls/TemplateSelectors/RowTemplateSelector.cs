namespace LiftDataManager.Controls.TemplateSelectors;
internal class RowTemplateSelector : DataTemplateSelector
{
    public DataTemplate Standard
    {
        get; set;
    }
    public DataTemplate Selected
    {
        get; set;
    }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        return SelectTemplateCore(item, null);
    }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (((TableRowIntDouble)item).IsSelected)
        {
            return Selected;
        }
        else
        {
            return Standard;
        }
    }
}
