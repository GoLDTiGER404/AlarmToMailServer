using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlarmToMailServer.TimeTask;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlarmToMailServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                var service = services.GetRequiredService <ServerSchedule> () as ServerSchedule;
                service.TaskRun();
                logger.LogInformation("启动定时成功");
            }
            catch (Exception ex)
            {
           
                logger.LogError(ex, "启动定时失败");
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
