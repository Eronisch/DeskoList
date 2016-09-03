using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Business.File;

namespace Core.Business.Languages
{
    /// <summary>
    /// Service used for loading the default language resource files
    /// </summary>
    internal class ReflectionLanguageService
    {
        private readonly string _filepath;

        public ReflectionLanguageService(string filepath)
        {
            _filepath = filepath;
        }

        public IList<TypeInfo> Get()
        {
            if (!FileService.FileExists(_filepath)) { throw new FileNotFoundException("File not found for reflection!", _filepath); }

            return GetAllTypeInfo(_filepath);
        }

        private IList<TypeInfo> GetAllTypeInfo(string filepath)
        {
            // Only in the folder that starts with Languages.
            return (from type in GetReflectionTypes(filepath)
                   let typeInfo = GetTypeInfo(type)
                   where typeInfo.RelativePath.StartsWith("Localization.Languages")
                   select typeInfo).ToList();
        }

        private TypeInfo GetTypeInfo(Type type)
        {
            return new TypeInfo(type.Name, type.FullName, GetStringProperties(type));
        }

        private IList<Type> GetReflectionTypes(string filePath)
        {
            return Assembly.ReflectionOnlyLoadFrom(filePath).GetTypes().Where(t => t.IsClass
                     && !t.Assembly.FullName.StartsWith("System")
                     && !t.Assembly.FullName.StartsWith("Microsoft")
                     && !t.IsAbstract
                     && !t.IsSealed).ToList();
        }

        private IList<string> GetStringProperties(Type type)
        {
            return type.GetProperties().Where(p => p.PropertyType == typeof(string)).Select(x => x.Name).ToList();
        }
    }
}
