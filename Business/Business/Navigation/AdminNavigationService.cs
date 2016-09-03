using Database;
using Database.Entities;

namespace Core.Business.Navigation
{
    /// <summary>
    /// Manager for navigation items (left) in admin pages
    /// </summary>
    public class AdminNavigationService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public AdminNavigationService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();    
        }

        /// <summary>
        /// Add a widget navigation item
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationName"></param>
        public void AddWidgetNavigation(int widgetId, string controller, string action, string icon,
            string localizationBase, string localizationName)
        {
            var adminNavigation = CreateAdminNavigation(controller, action, icon, localizationBase,
                localizationName);

            adminNavigation.WidgetId = widgetId;

            _unitOfWorkRepository.AdminNavigationRepository.Add(adminNavigation);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Add a plugin admin navigation item
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationName"></param>
        public void AddPluginNavigation(int pluginId, string controller, string action, string icon,
          string localizationBase, string localizationName)
        {
            var adminNavigation = CreateAdminNavigation(controller, action, icon, localizationBase,
                localizationName);

            adminNavigation.PluginId = pluginId;

            _unitOfWorkRepository.AdminNavigationRepository.Add(adminNavigation);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Add a theme admin navigation item
        /// </summary>
        /// <param name="themeId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="icon"></param>
        /// <param name="localizationBase"></param>
        /// <param name="localizationName"></param>
        public void AddThemeNavigation(int themeId, string controller, string action, string icon,
          string localizationBase, string localizationName)
        {
            var adminNavigation = CreateAdminNavigation(controller, action, icon, localizationBase,
                localizationName);

            adminNavigation.ThemeId = themeId;

            _unitOfWorkRepository.AdminNavigationRepository.Add(adminNavigation);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Clear widget navigation items
        /// </summary>
        /// <param name="widgetId"></param>
        public void ClearWidgetNavigation(int widgetId)
        {
            _unitOfWorkRepository.AdminNavigationRepository.ClearWidgetNavigation(widgetId);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Clear theme navigation items
        /// </summary>
        /// <param name="themeId"></param>
        public void ClearThemeNavigation(int themeId)
        {
            _unitOfWorkRepository.AdminNavigationRepository.ClearThemeNavigation(themeId);

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Clear plugin navigation items
        /// </summary>
        /// <param name="pluginId"></param>
        public void ClearPluginNavigation(int pluginId)
        {
            _unitOfWorkRepository.AdminNavigationRepository.ClearPluginNavigation(pluginId);

            _unitOfWorkRepository.SaveChanges();
        }

        private AdminNavigation CreateAdminNavigation(string controller, string action, string icon, string localizationBase, string localizationName)
        {
            return new AdminNavigation
            {
                Action = action,
                Controller = controller,
                Icon = icon,
                LocalizedBase = localizationBase,
                LocalizedName = localizationName
            };
        }
    }
}
