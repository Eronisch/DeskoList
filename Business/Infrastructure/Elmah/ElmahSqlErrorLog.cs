using System.Collections;
using DatabaseXML;
using Elmah;

namespace Core.Infrastructure.Elmah
{
    public class ElmahSqlErrorLog : SqlErrorLog
    {
        public override string ConnectionString
        {
            get { return LocalDatabaseSettingsService.Manager.GetConnectionString(); }
        }

        public ElmahSqlErrorLog(IDictionary config) : base(config)
        {
        }

        public ElmahSqlErrorLog(string connectionString) : base(connectionString)
        {
        }
    }
}
