using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Model
{
    public  static class DBClusterDictionary
    {
        private static Dictionary<string, string> DBCD;

        public static Dictionary<string, string> GetValue()
        {
            return DBCD;
        }

        public static void SetValue(Dictionary<string, string> _DBCD)
        {
            DBCD = _DBCD;
        }
    }
}
