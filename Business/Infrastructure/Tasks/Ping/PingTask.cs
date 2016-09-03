using Core.Business.Ping;
using Quartz;

namespace Core.Infrastructure.Tasks.Ping
{
    public class PingTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
           new PingService().Start();
        }
    }
}
