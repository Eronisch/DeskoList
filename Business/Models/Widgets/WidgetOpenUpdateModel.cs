using Core.Models.Update;

namespace Core.Models.Widgets
{
    public class WidgetOpenUpdateModel : OpenUpdateModel
    {
        public int WidgetId { get; set; }

        public WidgetOpenUpdateModel(string name, string description, string version, string downloadUrl, string deskoVersion, int widgetId) : base(name, description, version, downloadUrl, deskoVersion)
        {
            WidgetId = widgetId;
        }
    }
}
