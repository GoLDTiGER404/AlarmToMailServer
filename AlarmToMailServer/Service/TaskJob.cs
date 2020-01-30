using AlarmToMailServer.Model;
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
                ///启动报警服务
                //ServerCenter serverCenter = new ServerCenter();
                //serverCenter.Handle();
                ///启动拆炉服务
                if (DBClusterDictionary.GetValue().Count > 0)
                {
                    RFServerCenter rFServerCenter = new RFServerCenter();
                    foreach (var d in DBClusterDictionary.GetValue())
                    {
                        rFServerCenter.Handle(d.Key, d.Value);
                    }
                }
                else
                {
                    Console.WriteLine($"没有配置数据库集群!无法启动定时任务,请停止服务检查配置后再启动!");
                }
            });
        }
    }
}
