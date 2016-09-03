using System.Collections.Generic;

namespace Topsite.Areas.Administration.Models.Updates
{
    public class TabbedUpdateTableModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<UpdateTableModel> Updates { get; set; }
    }
}