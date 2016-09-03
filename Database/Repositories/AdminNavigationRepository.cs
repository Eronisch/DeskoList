using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class AdminNavigationRepository : IRepository<AdminNavigation>
    {
        private readonly DbSet<AdminNavigation> _query;

        public AdminNavigationRepository(DbSet<AdminNavigation> settings)
        {
            _query = settings;
        }

        public AdminNavigation GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<AdminNavigation> GetAllByWidgetId(int widgetId)
        {
            return _query.Where(x => x.WidgetId == widgetId);
        }

        public AdminNavigation GetByControllerAndAction(string controller, string action)
        {
            return
                _query.FirstOrDefault(
                    n =>
                        n.Controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase) &&
                        n.Action.Equals(action, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<AdminNavigation> GetAll()
        {
            return _query;
        }

        public void Add(AdminNavigation widgetNavigation)
        {
            _query.Add(widgetNavigation);
        }

        public void ClearWidgetNavigation(int widgetId)
        {
            _query.Where(x => x.WidgetId == widgetId).ToList().ForEach(widget =>
            {
                _query.Remove(widget);
            });
        }

        public void ClearPluginNavigation(int pluginId)
        {
            _query.Where(x => x.PluginId == pluginId).ToList().ForEach(plugin =>
            {
                _query.Remove(plugin);
            });
        }

        public void RemoveNavigation(AdminNavigation widgetsNavigation)
        {
            _query.Remove(widgetsNavigation);
        }

        public void ClearThemeNavigation(int themeId)
        {
            _query.Where(x => x.ThemeId == themeId).ToList().ForEach(theme =>
            {
                _query.Remove(theme);
            });
        }
    }
}
