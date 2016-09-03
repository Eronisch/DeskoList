using Topsite.Areas.Administration.Models.Settings;

namespace Topsite.Areas.Administration.Models.Updates
{
    public class UpdateTableModel : SoftwareUpdateTableModel
    {
        public string DeskoVersion { get; set; }
        public bool IsVersionIncorrect { get; set; }
    }
}