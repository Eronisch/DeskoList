using System.Collections.Generic;

namespace Core.Business.Languages
{
    public class TypeInfo
    {
        internal TypeInfo(string className, string relativePath, IList<string> properties)
        {
            ClassName = className;
            RelativePath = relativePath;
            PropertyNames = properties;
        }

        public string ClassName { get; private set; }
        public string RelativePath { get; private set; }
        public IList<string> PropertyNames { get; private set; }
    }
}
