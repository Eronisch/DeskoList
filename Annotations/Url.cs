using System;
using System.ComponentModel.DataAnnotations;

namespace Topsite.Annotations
{
    public class Url : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string strUrl = value.ToString();
                Uri Url;
                return Uri.TryCreate(strUrl, UriKind.Absolute, out Url);
            }
            return false;
        }
    }
}