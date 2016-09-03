using Database;
using Database.Entities;

namespace Core.Business.Breadcrumbs
{
    /// <summary>
    /// Manager that is used for admin breadcrumbs
    /// </summary>
    public class AdminBreadCrumbsService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public AdminBreadCrumbsService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        /// <summary>
        /// Get breadcrumbs
        /// </summary>
        /// <param name="controller">Case insensitive controller</param>
        /// <param name="action">Case insensitive action</param>
        /// <returns></returns>
        public AdminBreadcrumbs GetBreadCrumbs(string controller, string action)
        {
            return _unitOfWorkRepository.AdminBreadCrumbsRepository.GetBreadcrumbs(controller, action);
        }

        /// <summary>
        /// Get widget breadcrumbs
        /// </summary>
        /// <param name="controller">Case insensitive controller</param>
        /// <param name="action">Case insensitive action</param>
        /// <param name="widgetId">Widget id</param>
        /// <returns></returns>
        public AdminBreadcrumbs GetWidgetBreadCrumbs(string controller, string action, int widgetId)
        {
            return _unitOfWorkRepository.AdminBreadCrumbsRepository.GetWidgetBreadcrumbs(controller, action, widgetId);
        }

        /// <summary>
        /// Add widget breadcrumb
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationTitle"></param>
        /// <param name="localizedDescription"></param>
        /// <param name="localizedControllerFriendlyName"></param>
        /// <param name="localizedActionFriendlyName"></param>
        public void AddWidgetBreadCrumbs(int widgetId, string controller, string action, string icon, string localizationBase,
            string localizationTitle, string localizedDescription, string localizedControllerFriendlyName,
            string localizedActionFriendlyName)
        {
            var breadCrumbs = CreateAdminBreadCrumbs(controller, action, icon, localizationBase, localizationTitle,
                localizedDescription, localizedControllerFriendlyName, localizedActionFriendlyName);

            breadCrumbs.WidgetId = widgetId;

            AddBreadCrumb(breadCrumbs);
        }

        /// <summary>
        /// Add plugin breadcrumb
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationTitle"></param>
        /// <param name="localizedDescription"></param>
        /// <param name="localizedControllerFriendlyName"></param>
        /// <param name="localizedActionFriendlyName"></param>
        public void AddPluginBreadCrumbs(int pluginId, string controller, string action, string icon, string localizationBase,
           string localizationTitle, string localizedDescription, string localizedControllerFriendlyName,
           string localizedActionFriendlyName)
        {
            var breadCrumbs = CreateAdminBreadCrumbs(controller, action, icon, localizationBase, localizationTitle,
                localizedDescription, localizedControllerFriendlyName, localizedActionFriendlyName);

            breadCrumbs.PluginId = pluginId;

            AddBreadCrumb(breadCrumbs);
        }

        /// <summary>
        /// Add breadcrumb
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationTitle"></param>
        /// <param name="localizedDescription"></param>
        /// <param name="localizedControllerFriendlyName"></param>
        /// <param name="localizedActionFriendlyName"></param>
        public void AddBreadCrumbs(string controller, string action, string icon, string localizationBase,
           string localizationTitle, string localizedDescription, string localizedControllerFriendlyName,
           string localizedActionFriendlyName)
        {
            var breadCrumbs = CreateAdminBreadCrumbs(controller, action, icon, localizationBase, localizationTitle,
                localizedDescription, localizedControllerFriendlyName, localizedActionFriendlyName);

            AddBreadCrumb(breadCrumbs);
        }

        /// <summary>
        /// Add theme breadcrumb
        /// </summary>
        /// <param name="themeId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationTitle"></param>
        /// <param name="localizedDescription"></param>
        /// <param name="localizedControllerFriendlyName"></param>
        /// <param name="localizedActionFriendlyName"></param>
        public void AddThemeBreadCrumbs(int themeId, string controller, string action, string icon, string localizationBase,
            string localizationTitle, string localizedDescription, string localizedControllerFriendlyName,
            string localizedActionFriendlyName)
        {
            var breadCrumbs = CreateAdminBreadCrumbs(controller, action, icon, localizationBase, localizationTitle,
                localizedDescription, localizedControllerFriendlyName, localizedActionFriendlyName);

            breadCrumbs.ThemeId = themeId;

            AddBreadCrumb(breadCrumbs);
        }

        /// <summary>
        /// Add breadcrumb
        /// </summary>
        /// <param name="themeId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationTitle"></param>
        /// <param name="localizedDescription"></param>
        /// <param name="localizedControllerFriendlyName"></param>
        /// <param name="localizedActionFriendlyName"></param>
        public void AddThemeBreadCrumbs(string controller, string action, string icon, string localizationBase,
            string localizationTitle, string localizedDescription, string localizedControllerFriendlyName,
            string localizedActionFriendlyName)
        {
            var breadCrumbs = CreateAdminBreadCrumbs(controller, action, icon, localizationBase, localizationTitle,
                localizedDescription, localizedControllerFriendlyName, localizedActionFriendlyName);

            AddBreadCrumb(breadCrumbs);
        }

        /// <summary>
        /// Removes all the breadcrumbs from a specific widget
        /// </summary>
        /// <param name="widgetId"></param>
        public void ClearWidgetBreadCrumbs(int widgetId)
        {
            _unitOfWorkRepository.AdminBreadCrumbsRepository.ClearWidgetBreadcrumbs(widgetId);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Removes all the breadcrumbs from a specific plugin
        /// </summary>
        /// <param name="pluginId"></param>
        public void ClearPluginBreadCrumbs(int pluginId)
        {
            _unitOfWorkRepository.AdminBreadCrumbsRepository.ClearPluginBreadcrumbs(pluginId);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Removes all the breadcrumbs from a specific widget
        /// </summary>
        /// <param name="themeId"></param>
        public void ClearThemeBreadCrumbs(int themeId)
        {
            _unitOfWorkRepository.AdminBreadCrumbsRepository.ClearThemeBreadcrumbs(themeId);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Create breadcrumb model
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationTitle"></param>
        /// <param name="localizedDescription"></param>
        /// <param name="localizedControllerFriendlyName"></param>
        /// <param name="localizedActionFriendlyName"></param>
        /// <returns></returns>
        private AdminBreadcrumbs CreateAdminBreadCrumbs(string controller, string action, string icon, string localizationBase,
           string localizationTitle, string localizedDescription, string localizedControllerFriendlyName,
           string localizedActionFriendlyName)
        {
            return new AdminBreadcrumbs
            {
                Action = action,
                Controller = controller,
                LocalizationBase = localizationBase,
                LocalizedTitle = localizationTitle,
                LocalizedActionFriendlyName = localizedActionFriendlyName,
                LocalizedControllerFriendlyName = localizedControllerFriendlyName,
                LocalizedDescription = localizedDescription,
                Icon = icon
            };
        }

        private void AddBreadCrumb(AdminBreadcrumbs breadCrumbs)
        {
            _unitOfWorkRepository.AdminBreadCrumbsRepository.Add(breadCrumbs);

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
