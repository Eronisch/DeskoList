using Core.Business.Websites;
using Quartz;

namespace Core.Infrastructure.Tasks.Daily_Reset
{
    public class DailyResetTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var websiteOutService = new WebsiteOutService();
            var websiteInService = new WebsiteInService();

            websiteInService.CleanWebsiteInDaily();
            websiteOutService.CleanWebsiteOutDaily();
        }
    }
}
