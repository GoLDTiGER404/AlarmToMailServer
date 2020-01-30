using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Model
{
    public static class SQLiteUrl
    {
        private static string url {get;set;}

        public static string GetValue()
        {
            return url;
        }

        public static void SetValue(string _url)
        {
            url = _url;
        }
    }

}
