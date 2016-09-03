using Core.Business.Websites;
using Database;
using Quartz;

namespace Core.Infrastructure.Tasks.Reset
{
    public class ResetTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var websiteInService = new WebsiteInService();
            var websiteOutService = new WebsiteOutService();
            var databaseService = new DatabaseService();

            websiteInService.CleanWebsiteIn();
            websiteOutService.CleanWebsiteOut();

            databaseService.ResetIdentity("WebsiteIn");
            databaseService.ResetIdentity("WebsiteOut");
        }
    }
}
