using System.Collections.Generic;

namespace Topsite.Areas.Administration.Models.Report
{
    public class ReportBundleModel
    {
        public IEnumerable<ReportModel> Reports { get; set; }
        public int AmountReports { get; set; }
    }
}