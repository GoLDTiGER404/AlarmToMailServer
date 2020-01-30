using AlarmToMailServer.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.TimeTask
{
    public class EventToDayServerSchedule
    {

        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public EventToDayServerSchedule(ISchedulerFactory schedulerFactory)
        {
            this._schedulerFactory = schedulerFactory;
        }
        public async Task TaskRun(string TaskTime)
        {
            //1、通过调度工厂获得调度器
            _scheduler = await _schedulerFactory.GetScheduler();
            //2、开启调度器
            await _scheduler.Start();
            //3、创建一个触发器
            var trigger = TriggerBuilder.Create()
                            .WithCronSchedule(TaskTime)
                            .Build();
            //4、创建任务
            var jobDetail = JobBuilder.Create<TaskEventToDay>()
                            .WithIdentity("jobeventtoday", "group")
                            .Build();
            //5、将触发器和任务器绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);
            
        }
    }
}
