using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Model
{
    public class SendMailEntity
    {
        public string DeviceCode { get; set; }

        public DateTime? AlarmDate { get; set; }

        public string Remark { get; set; }
    }
}
