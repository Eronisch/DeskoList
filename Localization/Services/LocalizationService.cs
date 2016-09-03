using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Localization.Services
{
    public static class LocalizationService
    {
        private static readonly Dictionary<string, ResourceManager> ResourceManagerDictionary = new Dictionary<string, ResourceManager>();
        private const string ReturnValueNotFound = "??";

        public static string GetValue(string baseName, string name)
        {
            ResourceManager resourceManager;

            if (ResourceManagerDictionary.ContainsKey(baseName))
            {
                resourceManager = ResourceManagerDictionary[baseName];
            }
            else
            {
                resourceManager = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
                ResourceManagerDictionary.Add(baseName, resourceManager); 
            }

            try
            {
                string foundResourceString = resourceManager.GetString(name);
                return string.IsNullOrEmpty(foundResourceString) ? ReturnValueNotFound : foundResourceString;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is MissingManifestResourceException ||
                    ex is MissingSatelliteAssemblyException)
                {
                    return ReturnValueNotFound;
                }

                throw;
            }
        }
    }
}
