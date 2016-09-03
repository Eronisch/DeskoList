using System.Data.Entity;
using DatabaseXML;

namespace Database
{
    public class DataContent : DbContext
    {
        public DataContent()
            : base(LocalDatabaseSettingsService.Manager.GetDataModelConnectionString())
        {
        }
    }
}
