namespace Web.Infrastructure.Factory
{
    public class ControllerModel
    {
        public System.Web.Mvc.Controller Controller { get; set; }
        public ControllerSoftwareType Type { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
    }
}