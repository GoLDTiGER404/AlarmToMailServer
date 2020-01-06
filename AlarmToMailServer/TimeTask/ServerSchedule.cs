using AlarmToMailServer.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.TimeTask
{
    public class ServerSchedule
    {

        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public ServerSchedule(ISchedulerFactory schedulerFactory)
        {
            this._schedulerFactory = schedulerFactory;
        }
        public async Task TaskRun()
        {
            //1、通过调度工厂获得调度器
            _scheduler = await _schedulerFactory.GetScheduler();
            //2、开启调度器
            await _scheduler.Start();
            //3、创建一个触发器
            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(2).RepeatForever())//每两秒执行一次
                            .Build();
            //4、创建任务
            var jobDetail = JobBuilder.Create<TaskJob>()
                            .WithIdentity("job", "group")
                            .Build();
            //5、将触发器和任务器绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);
            
        }
    }
}
