namespace Core.Business.Plugin
{
    public class PluginHtml
    {
        public string Controller { get; private set; }
        public string Action { get; private set; }
        public string ViewName { get; private set; }
        public string Area { get; private set; }

        public PluginHtml(string controller, string action, string area, string viewName)
        {
            Controller = controller;
            Action = action;
            Area = area;
            ViewName = viewName;
        }
    }
}