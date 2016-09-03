using System.Collections.Generic;
using System.Linq;
using Database;
using Database.Entities;

namespace Core.Business.Plugin
{
    public class PluginService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public PluginService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        /// <summary>
        /// Adds the plugin and returns the record id
        /// Also enables the plugin
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="area"></param>
        /// <param name="version"></param>
        /// <param name="author"></param>
        /// <param name="authorUrl"></param>
        /// <param name="namespace"></param>
        /// <param name="updateUrl"></param>
        /// <returns></returns>
        public int AddToDatabase(string name, string description, string area, string version, string author,
            string authorUrl, string @namespace, string updateUrl)
        {
            var plugin = new Database.Entities.Plugins
            {
                Name = name,
                Description = description,
                Author = author,
                AuthorUrl = authorUrl,
                Version = version,
                UpdateUrl = updateUrl,
                Area = area,
                Namespace = @namespace,
                Enabled = true
            };

            _unitOfWorkRepository.PluginRepository.Add(plugin);

            _unitOfWorkRepository.SaveChanges();

            return plugin.Id;
        }

        public IEnumerable<Database.Entities.Plugins> GetPlugins()
        {
            return _unitOfWorkRepository.PluginRepository.GetAll().ToList();
        }

        public IEnumerable<Database.Entities.Plugins> GetAll()
        {
            return _unitOfWorkRepository.PluginRepository.GetAll().OrderBy(x => x.Name);
        }

        public Plugins Get(int pluginId)
        {
            return _unitOfWorkRepository.PluginRepository.GetById(pluginId);
        }

        public void SavePlugin(Database.Entities.Plugins plugin)
        {
            _unitOfWorkRepository.UpdateEntity(plugin);
            _unitOfWorkRepository.SaveChanges();
        }

        public Plugins GetByNameSpace(string callingAssembly)
        {
            return _unitOfWorkRepository.PluginRepository.GetByNameSpace(callingAssembly);
        }

        public IList<Plugins> GetActivePlugins()
        {
            return _unitOfWorkRepository.PluginRepository.GetActivePlugins().ToList();
        }

        public IList<Plugins> GetInActivePlugins()
        {
            return _unitOfWorkRepository.PluginRepository.GetInActivePlugins().ToList();
        } 
    }
}
