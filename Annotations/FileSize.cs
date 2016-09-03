using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Topsite.Annotations
{
    public class FileSize : ValidationAttribute
    {
        private int MaxMb { get; set; }

        public FileSize(int maxMb)
        {
            MaxMb = maxMb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;

            if (!ValidateFile((HttpPostedFileBase)value))
            {
                ErrorMessage = string.Format(ErrorMessage, validationContext.DisplayName, MaxMb);
            }

            return null;
        }

        private bool ValidateFile(HttpPostedFileBase postedFile)
        {
            const int mbBytes = 1048576;

            if (postedFile != null)
            {
                double maxBytes = MaxMb * mbBytes;

                return maxBytes >= postedFile.ContentLength;
            }

            return false;
        }
    }
}
