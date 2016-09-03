using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WidgetsThemeRepository : IRepository<WidgetsTheme>
    {
        private readonly DbSet<WidgetsTheme> _query;

        public WidgetsThemeRepository(DbSet<WidgetsTheme> settings)
        {
            _query = settings;
        }

        public WidgetsTheme GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WidgetsTheme> GetAll()
        {
            return _query;
        }

        public WidgetsTheme GetByWidgetAndSectionId(int widgetId, int sectionId)
        {
            return _query.FirstOrDefault(w => w.WidgetId == widgetId && w.ThemeSectionId == sectionId);
        }
    }
}
