﻿using System;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using Quartz;
using Quartz.Impl;
using System.Web;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Purchasing.Web.App_Start.CreateIndexes), "ScheduleJobs")]
namespace Purchasing.Web.App_Start
{
    public static class CreateIndexes
    {
        private static void ScheduleJobs()
        {
            var indexRoot = HttpContext.Current.Server.MapPath("~/App_Data/Indexes");

            // create job
            var jobDetail = JobBuilder.Create<CreateHistoricalOrderIndexJob>().UsingJobData("indexRoot", indexRoot).Build();

            // create trigger
            var everyFifteenMinutes = TriggerBuilder.Create().ForJob(jobDetail).WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(15)).StartNow().Build();

            // get reference to scheduler (remote or local) and schedule job
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(jobDetail, everyFifteenMinutes);
            sched.Start();
        }
    }
   
    public class CreateHistoricalOrderIndexJob : Quartz.IJob
    {
        private readonly IIndexService _indexService;

        public CreateHistoricalOrderIndexJob()
        {
            _indexService = ServiceLocator.Current.GetInstance<IIndexService>();
        }
        public void Execute(IJobExecutionContext context)
        {
            var indexRoot = context.MergedJobDataMap["indexRoot"] as string;
            _indexService.SetIndexRoot(indexRoot);

            try
            {
                //_indexService.CreateHistoricalOrderIndex();
                //new System.Net.Mail.SmtpClient("smtp.ucdavis.edu").Send("srkirkland@ucdavis.edu", "srkirkland@ucdavis.edu", "it worked!", "bender is the greatest");
            }
            catch (Exception ex)
            {
                new System.Net.Mail.SmtpClient("smtp.ucdavis.edu").Send("srkirkland@ucdavis.edu", "srkirkland@ucdavis.edu", "error", ex.ToString());
            }
        }
    }
}