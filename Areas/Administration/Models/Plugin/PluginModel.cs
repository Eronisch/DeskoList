using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Database.Entities;
using Topsite.Annotations;

namespace Topsite.Areas.Administration.Models.Plugin
{
    public class PluginModel
    {
        public PluginModel(IEnumerable<Plugins> plugins)
        {
            Plugins = plugins.ToList();
            AmountPlugins = Plugins.Count();
            AmountActivatePlugins = Plugins.Count(p => p.Enabled);;
            AmountInActivatePlugins = AmountPlugins - AmountActivatePlugins;
        }

        public PluginModel() { }

        public readonly IEnumerable<Plugins> Plugins;

        public readonly int AmountPlugins;

        public readonly int AmountActivatePlugins;

        public readonly int AmountInActivatePlugins;


        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.PluginModel), Name = "Plugin")]
        [Required(ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.PluginModel), ErrorMessageResourceName = "NoFileSelected")]
        [HttpPostedFileExtensions(Extensions = ".zip", ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.PluginModel), ErrorMessageResourceName = "InvalidFile", ErrorMessage = null)]
        public HttpPostedFileBase PluginFile { get; set; }
    }
}