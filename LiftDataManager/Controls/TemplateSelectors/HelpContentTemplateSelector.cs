namespace LiftDataManager.Controls.TemplateSelectors
{

    class HelpContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? MainTemplate { get; set; }
        public DataTemplate? SubTemplate { get; set; }
        public DataTemplate? Sub2Template { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            return SelectTemplateCore(item, null);
        }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject? container)
        {
            if (item is not null and HelpContent)
            {
                var helpContentCenterEntry = (HelpContent)item;

                return helpContentCenterEntry.Level switch
                {
                    HelpContentLevel.Main => MainTemplate,
                    HelpContentLevel.Sub => SubTemplate,
                    HelpContentLevel.Sub2 => Sub2Template,
                    _ => MainTemplate,
                };
            }
            else
            {
                return MainTemplate;
            }
        }
    }
}
