using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Model
{
    public class RemoveRecord
    {
        public int Id { get; set; }
        public string PLID { get; set; }

        public string ZoneCode { get; set; }

        private string dcode;
        public string DeviceCode { get { return dcode; } set { if (value != null) ZoneCode = value.Trim().ToString().Substring(0, 2);dcode = value;  } }

        public DateTime? StopDate { get { return sDate; } set { if (value != null) RemoveDate = value.Value.AddHours(6); sDate = value; } }

        private DateTime? sDate;
        public DateTime? RemoveDate { get; set; }

        public string Remark { get; set; }
    }
}
