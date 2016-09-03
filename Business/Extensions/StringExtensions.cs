using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
   public  static class StringExtensions
    {
       public static string ReplaceText(this string value, Dictionary<string, string> replaceDictionairy)
       {
           return replaceDictionairy.Aggregate(value, (current, item) => current.Replace(item.Key, item.Value));
       }

       public static string UppercaseFirstLettter(this string value)
       {
           if (string.IsNullOrEmpty(value))
               return string.Empty;

           return (char.ToUpper(value[0]) + value.Substring(0));
       }
    }
}
