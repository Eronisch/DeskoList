using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ThemesRepository : IRepository<Themes>
    {
        private readonly DbSet<Themes> _query;

        public ThemesRepository(DbSet<Themes> settings)
        {
            _query = settings;
        }

        public Themes GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Themes> GetAll()
        {
            return _query;
        }

        public Themes AddTheme(Themes theme)
        {
            _query.Add(theme);

            return theme;
        }

        public Themes GetByNameAndAuthor(string themeName, string author)
        {
            return
                _query.FirstOrDefault(
                    x =>
                        x.AuthorName.Equals(author, StringComparison.CurrentCultureIgnoreCase) &&
                        x.ThemeName.Equals(themeName, StringComparison.CurrentCultureIgnoreCase));
        }

        public void RemoveTheme(int themeId)
        {
            var theme = _query.FirstOrDefault(x => x.Id == themeId);

            if (theme != null)
            {
                _query.Remove(theme);    
            }
        }

        public IQueryable<Themes> GetThemesWithUpdatesAvailable()
        {
            return _query.Include(w => w.ThemeOpenUpdates).Where(w => w.ThemeOpenUpdates.Any());
        }
    }
}
