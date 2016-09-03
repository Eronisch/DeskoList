using System.Web.Hosting;
using Core.Business.Plugin;
using Core.Business.Software;
using Core.Business.Themes;
using Core.Business.Widgets;
using Quartz;

namespace Core.Infrastructure.Tasks.Update
{
    public class UpdateTask : IJob
    {
        private readonly SoftwareUpdateService _softwareUpdateService;
        private readonly ThemeUpdateService _themeUpdateService;
        private readonly WidgetUpdateService _widgetUpdateService;
        private readonly PluginUpdateService _pluginUpdateService;

        public UpdateTask()
        {
            _softwareUpdateService = new SoftwareUpdateService();
            _themeUpdateService = new ThemeUpdateService();
            _widgetUpdateService = new WidgetUpdateService();
            _pluginUpdateService = new PluginUpdateService();
        }

        public void Execute(IJobExecutionContext context)
        {
            HostingEnvironment.QueueBackgroundWorkItem(bi =>
            {
                DownloadUpdates();

                InstallUpdates();
            });
        }

        private bool IsUpdatingSoftware()
        {
            return _softwareUpdateService.IsUpdating();
        }

        private bool IsUpdatingThemes()
        {
            return _themeUpdateService.IsUpdating();
        }

        private bool IsUpdatingWidgets()
        {
            return _widgetUpdateService.IsUpdating();
        }

        private bool IsUpdatingPlugins()
        {
            return _pluginUpdateService.IsUpdating();
        }

        private void DownloadUpdates()
        {
            if (!IsUpdatingSoftware())
            {
                _softwareUpdateService.SearchAndDownloadUpdates();
            }

            if (!IsUpdatingPlugins())
            {
                _pluginUpdateService.SearchAndDownloadUpdates();
            }

            if (!IsUpdatingWidgets())
            {
                _widgetUpdateService.SearchAndDownloadUpdates();
            }

            if (!IsUpdatingThemes())
            {
                _themeUpdateService.SearchAndDownloadUpdates();
            }
        }

        private void InstallUpdates()
        {
            if (!IsUpdatingSoftware())
            {
                _softwareUpdateService.InstallDownloadedUpdates();
            }

            if (IsUpdatingPlugins())
            {
                _pluginUpdateService.InstallDownloadedUpdates();
            }

            if (!IsUpdatingWidgets())
            {
                _widgetUpdateService.InstallDownloadedUpdates();
            }

            if (!IsUpdatingThemes())
            {
                _themeUpdateService.InstallDownloadedUpdates();
            }
        }
    }
}

