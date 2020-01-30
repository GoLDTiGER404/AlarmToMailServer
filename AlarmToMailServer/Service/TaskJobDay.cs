using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    public class TaskJobDay:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>预定任务启动--{DateTime.Now}<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                SendTodayAlarm sendTodayAlarm = new SendTodayAlarm();
                ///报警日计划
                //sendTodayAlarm.Handle();
                ///拆炉日计划
                sendTodayAlarm.RemoveHandle();
            });
        }
    }
}
