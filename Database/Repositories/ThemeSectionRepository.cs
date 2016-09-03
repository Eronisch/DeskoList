using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ThemeSectionRepository : IRepository<WidgetsThemeSection>
    {
        private readonly DbSet<WidgetsThemeSection> _query;

        public ThemeSectionRepository(DbSet<WidgetsThemeSection> settings)
        {
            _query = settings;
        }

        public WidgetsThemeSection GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WidgetsThemeSection> GetByThemeId(int themeId)
        {
            return _query.Where(x => x.ThemeId == themeId);
        }

        public IQueryable<WidgetsThemeSection> GetAll()
        {
            return _query;
        }

        public IQueryable<WidgetsThemeSection> GetBySection(int themeId, string sectionName)
        {
            return _query.Where(x => x.ThemeId == themeId && x.CodeName.Equals(sectionName, StringComparison.CurrentCultureIgnoreCase));
        }

        public void RemoveThemeSection(WidgetsThemeSection widgetsThemeSection)
        {
            _query.Remove(widgetsThemeSection);
        }

        public void AddThemeSection(WidgetsThemeSection widgetsThemeSection)
        {
            _query.Add(widgetsThemeSection);
        }
    }
}
