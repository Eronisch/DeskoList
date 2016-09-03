using Core.Models.Settings;

namespace Core.Models.Update
{
    public class OpenUpdateModel : SoftwareOpenUpdateModel
    {
        public string DeskoVersion { get; private set; }

        public OpenUpdateModel(string name, string description, string version, string downloadUrl, string deskoVersion) : base(name, description, version, downloadUrl)
        {
            DeskoVersion = deskoVersion;
        }
    }
}
