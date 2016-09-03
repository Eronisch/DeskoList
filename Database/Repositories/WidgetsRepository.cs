using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WidgetsRepository : IRepository<Widgets>
    {
        private readonly DbSet<Widgets> _query;

        public WidgetsRepository(DbSet<Widgets> settings)
        {
            _query = settings;
        }

        public Widgets GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public Widgets GetByNameSpace(string @namespace)
        {
            return _query.FirstOrDefault(x => x.Namespace == @namespace);
        }

        public bool IsWidgetByAreaAndNamespace(string area, string @namespace)
        {
            return
                _query.Any(
                    x => x.AreaName.Equals(area, StringComparison.CurrentCultureIgnoreCase) && x.Namespace == @namespace);
        }

        public IQueryable<Widgets> GetAll()
        {
            return _query;
        }

        public void AddWidget(Widgets widget)
        {
            _query.Add(widget);
        }

        public void Remove(int widgetId)
        {
            _query.Remove(_query.First(x=> x.Id == widgetId));
        }

        public Widgets GetByNameAndAuthor(string widgetName, string author)
        {
            return _query.FirstOrDefault(x => x.Author.Equals(author, StringComparison.CurrentCultureIgnoreCase) &&
                                              x.Name.Equals(widgetName, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<Widgets> GetWidgetsWithUpdatesAvailable()
        {
            return _query.Include(w => w.WidgetOpenUpdates).Where(w => w.WidgetOpenUpdates.Any());
        }
    }
}
