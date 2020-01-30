using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AlarmToMailServer.Model;
using AlarmToMailServer.Service;
using AlarmToMailServer.TimeTask;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlarmToMailServer
{
    public class Program
    {
        private static IConfiguration Configuration { get; set; }
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            Configuration = new ConfigurationBuilder()
         .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
         .Build();
            ConfigDBCluster();
            string TaskTime= "0 0 20 * * ?";
            string Date = "";
            string Hour=Configuration.GetSection("TaskTimeConfig").GetSection("Hour").Value.ToString();
            string Minute=Configuration.GetSection("TaskTimeConfig").GetSection("Minute").Value.ToString();
            string Second=Configuration.GetSection("TaskTimeConfig").GetSection("Second").Value.ToString();
            Date = Hour + ":" + Minute + ":" + Second;
            if (!DateTime.TryParse(Date, out DateTime dt)) logger.LogError("定时时间错误！已使用系统默认时间<20:00:00>进行邮件发送");
            else { TaskTime = $"{Second} {Minute} {Hour} * * ?"; logger.LogInformation($"已设定发送时间{Date}"); }
            string Sqlite = AppDomain.CurrentDomain.BaseDirectory + "\\AlarmToMail.db";
            SQLiteUrl.SetValue(Sqlite);
           var service = services.GetRequiredService<ServerSchedule>() as ServerSchedule;
            var dayservice = services.GetRequiredService<DayServerSchedule>() as DayServerSchedule;
            try
            {        
                await service.TaskRun();
                await dayservice.TaskRun(TaskTime);
                logger.LogInformation("启动定时成功");
            }
            catch (Exception ex)
            {        
                logger.LogError(ex, "启动定时失败");
            }
            await host.RunAsync();
            
        }

        private static void ConfigDBCluster()
        {
            var DBALL = Configuration.GetSection("DBCluster").GetChildren().ToList();
            Dictionary<string, string> db = new Dictionary<string, string>();
            foreach (var d in DBALL)
            {
                db.Add(d.Key, d.Value);
            }
            DBClusterDictionary.SetValue(db);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
