using System;
using System.Collections.Specialized;
using Core.Business.Settings;
using Quartz;
using Quartz.Impl;

namespace Core.Business.Schedule
{
    public static class ScheduleService
    {
        private const string GroupName = "Desko";

        public static void Schedule<TQuartzTask>(string identity, string cron) where TQuartzTask : IJob
        {
            ValidateCron(identity, cron);

            var schedule = GetScheduler(identity);

            DeleteOldJobIfExists(schedule, identity);

            var job = CreateJob<TQuartzTask>(identity);

            var trigger = CreateTrigger(identity, cron);

            ScheduleTask(schedule, job, trigger);
        }

        public static void UnSchedule(string identity)
        {
            var schedule = GetScheduler(identity);

            if (schedule != null)
            {
                DeleteOldJobIfExists(schedule, identity);
            }
        }

        public static void UpdateSchedule(string identity, string cronjob)
        {
            var schedule = GetScheduler(identity);

            if (schedule != null)
            {
                var newTrigger = CreateTrigger(identity, cronjob);

                schedule.RescheduleJob(new TriggerKey(identity, GroupName), newTrigger);
            }
        }

        private static void ScheduleTask(IScheduler schedule, IJobDetail job, ITrigger trigger)
        {
            schedule.ScheduleJob(job, trigger);
            schedule.Start();
        }

        private static void DeleteOldJobIfExists(IScheduler schedule, string identity)
        {
            if (schedule.CheckExists(new JobKey(identity, GroupName)))
            {
                schedule.DeleteJob(new JobKey(identity, GroupName));
            }
        }

        private static void ValidateCron(string identity, string cron)
        {
            if (!CronExpression.IsValidExpression(cron))
            {
                throw new Exception("Invalid cron for task: " + identity);
            }
        }

        private static ICronTrigger CreateTrigger(string identity, string cron)
        {
            var settingsService = new SettingsService();

            return (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity(identity, GroupName)
                .WithSchedule(CronScheduleBuilder
                .CronSchedule(cron)
                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(settingsService.GetActiveTimeZone())))
                .StartAt(DateTime.UtcNow)
                .Build();
        }

        private static IJobDetail CreateJob<TQuartzTask>(string identity) where TQuartzTask : IJob
        {
            return JobBuilder.Create<TQuartzTask>()
               .WithIdentity(identity, GroupName)
               .RequestRecovery()
               .Build();
        }

        private static IScheduler GetScheduler(string identity)
        {
            var properties = new NameValueCollection
            {
                {"quartz.scheduler.instanceName", identity}
            };

            var sf = new StdSchedulerFactory(properties);

            return sf.GetScheduler();
        }
    }
}
