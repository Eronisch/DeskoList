using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Core.Business.File;

namespace Core.Business.Languages
{
    /// <summary>
    /// Service used for comparing the default language dll with the given language dll
    /// </summary>
    internal class ResourceManagerCompareService
    {
        private readonly string _filePath;
        private readonly string _languageAbbreviation;
        private readonly IEnumerable<TypeInfo> _typeInfos;

        public ResourceManagerCompareService(string path, string languageAbbreviation, IEnumerable<TypeInfo> typeInfos)
        {
            if (!FileService.FileExists(path)) { throw new FileNotFoundException("File not found for the resource manager compare service!", path); }

            _filePath = path;
            _languageAbbreviation = languageAbbreviation;
            _typeInfos = typeInfos;
        }

        public IList<TypeInfo> Compare()
        {
            var missingItems = new Collection<TypeInfo>();

            var assembly = GetAssembly(_filePath);

            foreach (var typeInfo in _typeInfos)
            {
                var resourceManager = GetResourceManager(typeInfo.RelativePath, _languageAbbreviation, assembly);

                var missingProperties = new Collection<string>();

                foreach (var property in typeInfo.PropertyNames)
                {
                    try
                    {
                        resourceManager.GetString(property);
                    }
                    catch (MissingManifestResourceException)
                    {
                        missingProperties.Add(property);
                    }
                }

                if (missingProperties.Any())
                {
                    missingItems.Add(new TypeInfo(typeInfo.ClassName, typeInfo.RelativePath, missingProperties));    
                }
                
            }

            return missingItems;
        }

        private Assembly GetAssembly(string filePath)
        {
            return Assembly.ReflectionOnlyLoadFrom(filePath);
        }

        private ResourceManager GetResourceManager(string fullName, string languageAbbreviation, Assembly assembly)
        {
            return new ResourceManager(string.Format("{0}.{1}", fullName, languageAbbreviation), assembly);
        }
    }
}
