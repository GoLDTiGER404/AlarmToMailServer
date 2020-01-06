using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    public class TaskJob:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));

            });
        }
    }
}
