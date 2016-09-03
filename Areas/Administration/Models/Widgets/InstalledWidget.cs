namespace Topsite.Areas.Administration.Models.Widgets
{
    public class InstalledWidget
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string AreaName { get; set; }
        public ActivateWidgetModel ActivateWidget { get; set; }
        public bool IsUsedInAnyOfTheThemes { get; set; }
        public bool IsUsedInAllThemes { get; set; }
        public bool IsEnabledInActiveTheme { get; set; }

        public InstalledWidget(int widgetId)
        {
            ActivateWidget = new ActivateWidgetModel(widgetId);
        }

        // Parameterless constructor for controller action
        public InstalledWidget(){}

    }
}