using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class PluginRepository : IRepository<Plugins>
    {
        private readonly DbSet<Plugins> _query;

        public PluginRepository(DbSet<Plugins> settings)
        {
            _query = settings;
        }

        public Plugins GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Plugins> GetAll()
        {
            return _query;
        }

        public IQueryable<Plugins> GetPluginsWithUpdatesAvailable()
        {
            return _query.Include(w => w.PluginOpenUpdates).Where(w => w.PluginOpenUpdates.Any());
        }

        public Plugins GetByNameAndAuthor(string name, string author)
        {
            return _query.FirstOrDefault(x => x.Author.Equals(author, StringComparison.CurrentCultureIgnoreCase) &&
                                              x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void Add(Plugins plugin)
        {
            _query.Add(plugin);
        }

        public Plugins GetByNameSpace(string @namespace)
        {
            return _query.FirstOrDefault(x => x.Namespace == @namespace);
        }

        public IQueryable<Plugins> GetActivePlugins()
        {
            return _query.Where(x => x.Enabled);
        }

        public void Remove(int pluginId)
        {
            _query.Remove(GetById(pluginId));
        }

        public IQueryable<Plugins> GetInActivePlugins()
        {
            return _query.Where(x => !x.Enabled);
        }
    }
}
