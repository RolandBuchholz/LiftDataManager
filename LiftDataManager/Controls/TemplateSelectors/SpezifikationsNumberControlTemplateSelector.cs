namespace LiftDataManager.Controls.TemplateSelectors;

public class SpezifikationsNumberControlTemplateSelector : DataTemplateSelector
{
    public DataTemplate? OrderNumberTemplate
    {
        get; set;
    }
    public DataTemplate? OfferNumberTemplate
    {
        get; set;
    }
    public DataTemplate? PlanningNumberTemplate
    {
        get; set;
    }

    public DataTemplate? RequestTemplate
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
                //ParameterTypValue.Text => StringTemplate,
                //ParameterTypValue.NumberOnly => NumberOnlyTemplate,
                //ParameterTypValue.Date => DateTemplate,
                //ParameterTypValue.Boolean => BooleanTemplate,
                //ParameterTypValue.DropDownList => DropDownList,
                _ => OrderNumberTemplate,
            };
        }
        else
        {
            return OrderNumberTemplate;
        }
    }
}
