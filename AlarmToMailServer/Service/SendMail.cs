using AlarmToMailServer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    public class SendMail
    {
        private static IConfiguration Configuration { get; set; }
        private SendMailEntity _sendMailEntity { get; set; }
        private List<SendMailEntity> _sendMailEntities { get; set; }

        private List<RemoveRecord> _removeRecords { get; set; }

        public SendMail(SendMailEntity sendMailEntity)
        {
            _sendMailEntity = sendMailEntity;
        }

        public SendMail(List<SendMailEntity> sendMailEntities)
        {
            _sendMailEntities = sendMailEntities;
        }

        public SendMail(List<RemoveRecord> removeRecords)
        {
            _removeRecords = removeRecords;
        }

        public void SendMailMain()
        {
            bool html = false;
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
            try
            {
                string mailProfile = Configuration.GetSection("MailConfig").GetSection("mailProfile").Value;
                string receivers = Configuration.GetSection("MailConfig").GetSection("receivers").Value;
                string copyreceivers = Configuration.GetSection("MailConfig").GetSection("copy_receivers").Value; 
                string connectionString = Configuration.GetSection("Connection").Value;
                string subject = Configuration.GetSection("MailConfig").GetSection("subject").Value; 
                string fileAttachments = string.Empty;
                string body = string.Empty;
               
                if (_sendMailEntities!=null)
                {
                    if(string.IsNullOrEmpty(subject))
                    subject = $"{DateTime.Now.ToString("yyyy-MM-dd")}报警信息统计";
                    body = $"{DateTime.Now.ToString("yyyy-MM-dd")}报警信息发生统计:\r\n\r\n ";
                    foreach (var s in _sendMailEntities)
                    {
                        body = body + $"\r\n\r\n {s.DeviceCode}炉台，在{s.AlarmDate} 时，出现主加热器功率等于零的故障!";
                    }
                }
                if (_sendMailEntity != null)
                {
                    if (string.IsNullOrEmpty(subject))
                        subject = $"{_sendMailEntity.DeviceCode}炉台报警！";
                    body = "警报!\r\n\r\n ";
                    body = body + $"\r\n\r\n {_sendMailEntity.DeviceCode}炉台，在{_sendMailEntity.AlarmDate} 时，出现主加热器功率等于零的故障!";
                    body = body + "\r\n\r\n 请及时检查!";
                }
                if (_removeRecords != null)
                {
                    if (string.IsNullOrEmpty(subject))
                        subject = $"{DateTime.Now.ToString("yyyy-MM-dd")}拆清炉台报表";
                    body = "<table border='1' align='center' valian='middle'><tr><td align='center'>片区</td><td align='center'>炉号</td><td align='center'>停炉时间</td><td align='center'>拆炉时间</td><td align='center'>备注</td></tr>";                                                      
                    foreach (var r in _removeRecords)
                    {
                        body = body + $"<tr><td>{r.ZoneCode}</dt><td>{r.DeviceCode}</dt><td>{r.StopDate}</dt><td>{r.RemoveDate}</dt><td>{r.Remark}</dt></tr>";
                    }
                    body = body + "</table>";
                    html = true;
                }
                //初始化存储过程的参数
                var paramMailProfile = new SqlParameter("@profile_name", SqlDbType.NVarChar, 128) { Value = mailProfile };//程序名称
                var paramReceivers = new SqlParameter("@recipients", SqlDbType.NVarChar, -1) { Value = receivers };//接收邮箱
                var paramCopyReceivers = new SqlParameter("@copy_recipients ", SqlDbType.NVarChar, 255) { Value = copyreceivers };//抄送
                var paramBody = new SqlParameter("@body", SqlDbType.NVarChar, -1) { Value = body };//邮件内容
                var paramSubject = new SqlParameter("@subject", SqlDbType.NVarChar, 255) { Value = subject };//标题
                var paramFileAttachments = new SqlParameter("@file_attachments", SqlDbType.NVarChar, -1) { Value = fileAttachments };
                SqlParameter paramBodyFormat = null;
                if(html) paramBodyFormat=new SqlParameter("@body_format", SqlDbType.NVarChar, 20) { Value = "HTML"};//邮件内容

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("msdb.dbo.sp_send_dbmail", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 500,
                })
                {
                    if(html) cmd.Parameters.AddRange(new[] { paramMailProfile, paramReceivers, paramCopyReceivers, paramBody, paramSubject, paramFileAttachments,paramBodyFormat});
                    else cmd.Parameters.AddRange(new[] { paramMailProfile, paramReceivers, paramCopyReceivers, paramBody, paramSubject, paramFileAttachments });
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("邮件发送成功!-" + DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine("邮件发送失败-"+DateTime.Now);
                Console.WriteLine(ex.Message);
                return;
            }
        }


    }
}
