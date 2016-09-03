using Core.Business.Thumbnail;
using Quartz;

namespace Core.Infrastructure.Tasks.Thumbnail
{
    public class ThumbnailTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
           new ThumbnailService().Start();
        }
    }
}
