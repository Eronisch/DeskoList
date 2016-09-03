using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Net;

namespace Topsite.Annotations
{
    public class ImageUrlAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                WebRequest request = null;
                try
                {
                    request = WebRequest.Create(value.ToString());
                    request.Timeout = 5000;
                    WebResponse response = request.GetResponse();
                    // Max size of image is 500kb - 0.5mb
                    if (response.ContentLength < 500000)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                var image = Image.FromStream(stream);
                                if (image.Width <= 510 && image.Height <= 125)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (request != null)
                    {
                        request.Abort();
                    }
                }
            }
            return false;
        }
    }
}