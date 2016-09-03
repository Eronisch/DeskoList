using System.Linq;
using Core.Business.Plugin;
using Core.Business.Widgets;
using Database;
using Database.Entities;

namespace Core.Business.Dll
{
    /// <summary>
    /// Manager for dlls in the database
    /// </summary>
    public class DllService
    {
        private readonly WidgetService _widgetService;
        private readonly PluginService _pluginService;
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public DllService()
        {
            _widgetService = new WidgetService();
            _unitOfWorkRepository = new UnitOfWorkRepository();
            _pluginService = new PluginService();
        }

        /// <summary>
        /// Check if the dll is being other third parties
        /// </summary>
        /// <param name="ignoreWidgetId">Widget to exclude to search in</param>
        /// <param name="dllName">dll filename</param>
        /// <returns></returns>
        public bool IsDllBeingUsedByWidgetId(int ignoreWidgetId, string dllName)
        {
            return _unitOfWorkRepository.ActiveDllRepository.GetActiveDllsByName(dllName).ToList()
                .Any(w => w.Dlls.Name == dllName && w.WidgetId != ignoreWidgetId);
        }

        /// <summary>
        /// Check if the dll is being other third parties
        /// </summary>
        /// <param name="ignorePluginId">Plugin to exclude to search in</param>
        /// <param name="dllName">dll filename</param>
        /// <returns></returns>
        public bool IsDllBeingUsedByPluginId(int ignorePluginId, string dllName)
        {
            return _unitOfWorkRepository.ActiveDllRepository.GetActiveDllsByName(dllName).ToList()
               .Any(w => w.Dlls.Name == dllName && w.PluginId != ignorePluginId);
        }

        /// <summary>
        /// Check if the dll is being used by other third parties
        /// </summary>
        /// <param name="ignoreThemeId">Theme to exclude to search in</param>
        /// <param name="dllName">dll filename</param>
        /// <returns></returns>
        public bool IsDllBeingUsedByThemeId(int ignoreThemeId, string dllName)
        {
            return _unitOfWorkRepository.ActiveDllRepository.GetActiveDllsByName(dllName).ToList()
               .Any(w => w.Dlls.Name == dllName && w.ThemeId != ignoreThemeId);
        }

        /// <summary>
        /// Checks if the dll is being used by anyone
        /// </summary>
        /// <param name="dllName">dll filename</param>
        /// <returns></returns>
        public bool IsDllBeingUsed(string dllName)
        {
            return _unitOfWorkRepository.ActiveDllRepository.GetActiveDllsByName(dllName).Any();
        }

        /// <summary>
        /// Removes a dll from the database
        /// </summary>
        /// <param name="dllName">dll filename</param>
        public void RemoveDll(string dllName)
        {
            var dll = GetDllByName(dllName);

            if (dll != null)
            {
                _unitOfWorkRepository.DllRepository.Remove(dll);

                _unitOfWorkRepository.SaveChanges();    
            }
        }

        /// <summary>
        /// Add a dll to a widget
        /// </summary>
        /// <param name="dllName">Existing or new dll file name</param>
        /// <param name="widgetId"></param>
        public void AddWidgetDll(string dllName, int widgetId)
        {
            AddDllToWidget(AddDll(dllName), widgetId);
        }

        /// <summary>
        /// Add a dll to a plugin
        /// </summary>
        /// <param name="dllName">Existing or new dll file name</param>
        /// <param name="pluginId"></param>
        public void AddPluginDll(string dllName, int pluginId)
        {
            AddDllToPlugin(AddDll(dllName), pluginId);
        }

        /// <summary>
        /// Add a dll to a theme
        /// </summary>
        /// <param name="dllName">Existing or new dll file name</param>
        /// <param name="themeId"></param>
        public void AddThemeDll(string dllName, int themeId)
        {
            AddDllToTheme(AddDll(dllName), themeId);
        }

        /// <summary>
        /// Add the dll
        /// </summary>
        /// <param name="dllName">dll filename</param>
        /// <returns>Returns id from the dll record</returns>
        public int AddDll(string dllName)
        {
            var dll = GetDllByName(dllName);

            if (dll == null)
            {
                dll = new Dlls
                {
                    Name = dllName
                };

                _unitOfWorkRepository.DllRepository.Add(dll);

                _unitOfWorkRepository.SaveChanges();
            }

            return dll.Id;
        }

        /// <summary>
        /// Add a specific dll to the widget
        /// </summary>
        /// <param name="dllId"></param>
        /// <param name="widgetId"></param>
        public void AddDllToWidget(int dllId, int widgetId)
        {
            _unitOfWorkRepository.ActiveDllRepository.Add(new ActiveDlls
            {
                DllId = dllId,
                WidgetId = widgetId
            });

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Add a specific dll to the widget
        /// </summary>
        /// <param name="dllId"></param>
        /// <param name="pluginId"></param>
        public void AddDllToPlugin(int dllId, int pluginId)
        {
            _unitOfWorkRepository.ActiveDllRepository.Add(new ActiveDlls
            {
                DllId = dllId,
                PluginId = pluginId
            });

            _unitOfWorkRepository.SaveChanges();
        }

        public void AddDllToTheme(int dllId, int themeId)
        {
            _unitOfWorkRepository.ActiveDllRepository.Add(new ActiveDlls
            {
                DllId = dllId,
                ThemeId = themeId
            });

            _unitOfWorkRepository.SaveChanges();
        }

        private Dlls GetDllByName(string dllName)
        {
            return _unitOfWorkRepository.DllRepository.GetDllByName(dllName);
        }
    }
}
