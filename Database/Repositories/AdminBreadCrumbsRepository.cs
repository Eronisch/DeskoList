using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class AdminBreadCrumbsRepository : IRepository<AdminBreadcrumbs>
    {
        private readonly DbSet<AdminBreadcrumbs> _query;

        public AdminBreadCrumbsRepository(DbSet<AdminBreadcrumbs> settings)
        {
            _query = settings;
        }

        public AdminBreadcrumbs GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public AdminBreadcrumbs GetByControllerAndAction(string controller, string action)
        {
            return
                _query.FirstOrDefault(
                    n =>
                        n.Controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase) &&
                        n.Action.Equals(action, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<AdminBreadcrumbs> GetAll()
        {
            return _query;
        }

        public AdminBreadcrumbs GetBreadcrumbs(string controller, string action)
        {
            return _query.FirstOrDefault(x => x.Controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase)
                                              && x.Action.Equals(action, StringComparison.CurrentCultureIgnoreCase));
        }

        public AdminBreadcrumbs GetWidgetBreadcrumbs(string controller, string action, int widgetId)
        {
            return _query.FirstOrDefault(x => x.WidgetId == widgetId && x.Controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase)
                                              && x.Action.Equals(action, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<AdminBreadcrumbs> GetBreadcrumbsByWidgetId(int widgetId)
        {
            return _query.Where(b => b.WidgetId == widgetId);
        }

        public void ClearWidgetBreadcrumbs(int widgetId)
        {
            _query.RemoveRange(_query.Where(x => x.WidgetId == widgetId));
        }

        public void ClearPluginBreadcrumbs(int pluginId)
        {
            _query.RemoveRange(_query.Where(x => x.PluginId == pluginId));
        }

        public void ClearThemeBreadcrumbs(int themeId)
        {
            _query.RemoveRange(_query.Where(x => x.ThemeId == themeId));
        }

        public void Add(AdminBreadcrumbs breadcrumb)
        {
            _query.Add(breadcrumb);
        }

        public void Remove(AdminBreadcrumbs breadcrumb)
        {
            _query.Remove(breadcrumb);
        }
    }
}
