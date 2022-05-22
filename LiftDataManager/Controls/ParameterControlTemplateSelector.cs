using LiftDataManager.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Controls
{
    public class ParameterControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate NumberOnlyTemplate { get; set; }
        public DataTemplate DateTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate DropDownList { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SelectTemplateCore(item, null);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not null)
            {
                Parameter parameter = (Parameter)item;

                switch (parameter.ParameterTyp)
                {
                    case Parameter.ParameterTypValue.String:
                        return StringTemplate;
                    case Parameter.ParameterTypValue.NumberOnly:
                        return NumberOnlyTemplate;
                    case Parameter.ParameterTypValue.Date:
                        return DateTemplate;
                    case Parameter.ParameterTypValue.Boolean:
                        return BooleanTemplate;
                    case Parameter.ParameterTypValue.DropDownList:
                        return DropDownList;

                    default:
                        return DefaultTemplate;
                }
            }
            else
            {
                return DefaultTemplate;
            }
        }
    }
}
