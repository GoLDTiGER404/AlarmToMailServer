using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Model
{
    public class DeviceRecord
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; }
        public DateTime? LastDate { get; set; }
        public int? LastCount { get; set; }
        public bool? AlarmSign { get; set; }
    }
}
