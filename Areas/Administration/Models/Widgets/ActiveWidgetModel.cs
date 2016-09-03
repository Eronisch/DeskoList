namespace Topsite.Areas.Administration.Models.Widgets
{
    public class ActiveWidgetModel
    {
        public int WidgetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string AreaName { get; set; }
        public bool IsWidgetEnabled { get; set; }

        public string ThemeSection { get; set; }
    }
}