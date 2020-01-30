using AlarmToMailServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    /// <summary>
    /// 发送当天报警集合
    /// </summary>
    public class SendTodayAlarm
    {
        public void Handle()
        {
            AlarmRecordHelp alarmRecordHelp = new AlarmRecordHelp();
            var alarmlist = alarmRecordHelp.GetByDate(DateTime.Now.ToString("yyyy-MM-dd"));
            if (alarmlist == null) return;
            if (alarmlist.Count < 1) return;
            List<SendMailEntity> sendMailEntities = new List<SendMailEntity>();
            foreach (var alarm in alarmlist)
            {
                SendMailEntity s = new SendMailEntity();
                s.AlarmDate = alarm.AlarmDate;
                s.DeviceCode = alarm.DeviceCode;
                sendMailEntities.Add(s);
            }
            SendMail send = new SendMail(sendMailEntities);
            try
            {
                send.SendMailMain();//发送邮件
            }
            catch(Exception ex) {
                Console.WriteLine("邮件发送失败！");
                Console.WriteLine(ex.Message.ToString());
            
            }
            }

        public void RemoveHandle()//拆清报警
        {
            RemoveRecordHelp removeRecordHelp = new RemoveRecordHelp();
            var removelist = removeRecordHelp.GetAll();
            if (removelist == null) { Console.WriteLine($"预定任务结束--{DateTime.Now.ToString("yyyy-MM-dd")}当日无拆清列表!未发送邮件！"); return; }
            var list = removelist.Where(r => r.StopDate.Value.Date == DateTime.Now.Date);
            if (list == null) { Console.WriteLine($"预定任务结束--{DateTime.Now.ToString("yyyy-MM-dd")}当日无拆清列表!未发送邮件！"); return; }
            if (list.Count() < 1) { Console.WriteLine($"预定任务结束--{DateTime.Now.ToString("yyyy-MM-dd")}当日无拆清列表!未发送邮件！"); return; }
            SendMail send = new SendMail(list.ToList());
            try
            {
                send.SendMailMain();//发送邮件
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>预定任务结束--{DateTime.Now}--邮件已发送!<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
            catch (Exception ex)
            {
                Console.WriteLine("邮件发送失败！");
                Console.WriteLine(ex.Message.ToString());

            }
        }

    }
}
