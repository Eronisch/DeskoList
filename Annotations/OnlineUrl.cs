using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;

namespace Topsite.Annotations
{
    public class OnlineUrl : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                try
                {
                    WebRequest request = WebRequest.Create(value.ToString());
                    request.Timeout = 5000;

                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}