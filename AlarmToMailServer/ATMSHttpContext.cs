using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer
{
    //DI扩展
    public static class ATMSHttpContext
    {
        public static IServiceProvider ServiceProvider { get; set; }

    }
}
