using Core.Infrastructure.Tasks;
using Core.Infrastructure.Tasks.Daily_Reset;
using Core.Infrastructure.Tasks.Email;
using Core.Infrastructure.Tasks.Ping;
using Core.Infrastructure.Tasks.Reset;
using Core.Infrastructure.Tasks.Thumbnail;
using Core.Infrastructure.Tasks.Update;

namespace Core.Business.Schedule
{
    /// <summary>
    /// Schedules all the built in tasks
    /// Should only be run on startup
    /// </summary>
    public class BuiltInTasksScheduler
    {
        /// <summary>
        /// Starts all the tasks
        /// </summary>
        public void Schedule()
        {
            ScheduleTasks();
        }

        private void ScheduleTasks()
        {
            ScheduleDailyResetInAndOuts();

            ScheduleResetInAndOuts();

            ScheduleEmailUserWebsite();

            SchedulePingUserServers();

            ScheduleCreatUserWebsiteThumbnails();

            ScheduleUpdater();
        }

        private void ScheduleDailyResetInAndOuts()
        {
            ScheduleService.Schedule<DailyResetTask>(ScheduleConfig.GetResetDailyInAndOutsKey(), ScheduleConfig.GetResetDailyInAndOutsCronjob());
        }

        private void ScheduleUpdater()
        {
            if (ScheduleConfig.IsAutoUpdateEnabled())
            {
                ScheduleService.Schedule<UpdateTask>(ScheduleConfig.GetUpdateKey(), ScheduleConfig.GetCronjobUpdate());
            }
        }

        private void ScheduleResetInAndOuts()
        {
            if (ScheduleConfig.IsResetInAndOutsEnabled())
            {
                ScheduleService.Schedule<ResetTask>(ScheduleConfig.GetResetInAndOutsKey(), ScheduleConfig.GetCronjobResetInAndOuts());
            }
        }

        private void ScheduleEmailUserWebsite()
        {
            if (ScheduleConfig.IsEmailingUserStatisticsEnabled())
            {
                ScheduleService.Schedule<EmailTask>(ScheduleConfig.GetEmailUserWebsiteStatisticsKey(), ScheduleConfig.GetCronjobUserStatisticsEmail());
            }
        }

        private void SchedulePingUserServers()
        {
            if (ScheduleConfig.IsUserServerMonitoringEnabled())
            {
                ScheduleService.Schedule<PingTask>(ScheduleConfig.GetUserServerMonitoringKey(), ScheduleConfig.GetCronjobUserServerMonitoring());
            }
        }

        private void ScheduleCreatUserWebsiteThumbnails()
        {
            if (ScheduleConfig.IsCreateThumbnailsEnabled())
            {
                ScheduleService.Schedule<ThumbnailTask>(ScheduleConfig.GetCreateUserWebsitesThumbnailKey(), ScheduleConfig.GetCronjobCreateUserWebsiteThumbnails());
            }
        }
    }
}
