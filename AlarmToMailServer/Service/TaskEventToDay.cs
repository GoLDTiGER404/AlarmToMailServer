using AlarmToMailServer.TimeTask;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    public class TaskEventToDay:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>触发定时间隔任务--{DateTime.Now}<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                var services = ATMSHttpContext.ServiceProvider;
                var dayservice = services.GetService(typeof(DayServerSchedule)) as DayServerSchedule;
                await dayservice.TaskRun(TaskTime);
            });
        }
    }
}
