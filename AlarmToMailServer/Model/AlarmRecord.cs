using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Model
{
    public class AlarmRecord
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; }

        public DateTime? AlarmDate { get; set; }
    }
}
