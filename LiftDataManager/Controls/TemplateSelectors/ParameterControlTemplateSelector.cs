namespace LiftDataManager.Controls.TemplateSelectors;

public class ParameterControlTemplateSelector : DataTemplateSelector
{
    public DataTemplate? StringTemplate
    {
        get; set;
    }
    public DataTemplate? NumberOnlyTemplate
    {
        get; set;
    }
    public DataTemplate? DateTemplate
    {
        get; set;
    }
    public DataTemplate? BooleanTemplate
    {
        get; set;
    }
    public DataTemplate? DropDownList
    {
        get; set;
    }
    public DataTemplate? DefaultTemplate
    {
        get; set;
    }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return SelectTemplateCore(item, null);
    }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject? container)
    {
        if (item is not null)
        {
            var parameter = (Parameter)item;

            return parameter.ParameterTyp switch
            {
                Parameter.ParameterTypValue.Text => StringTemplate,
                Parameter.ParameterTypValue.NumberOnly => NumberOnlyTemplate,
                Parameter.ParameterTypValue.Date => DateTemplate,
                Parameter.ParameterTypValue.Boolean => BooleanTemplate,
                Parameter.ParameterTypValue.DropDownList => DropDownList,
                _ => DefaultTemplate,
            };
        }
        else
        {
            return DefaultTemplate;
        }
    }
}
