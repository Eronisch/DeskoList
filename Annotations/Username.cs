using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Topsite.Annotations
{
    public class UsernameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                return Regex.IsMatch(value.ToString(), "^[a-zA-Z0-9_.-]*$");
            }
            return false;
        }
    }
}