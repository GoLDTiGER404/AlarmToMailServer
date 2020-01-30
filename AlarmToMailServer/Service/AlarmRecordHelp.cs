using AlarmToMailServer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    public class AlarmRecordHelp
    {
        string url="";
        public AlarmRecordHelp()
        {
            url = SQLiteUrl.GetValue();
        }
        public List<AlarmRecord> GetByDate(string Date)
        {
            if (!DateTime.TryParse(Date, out DateTime dtt))
            {
                return null;
            }
            List<AlarmRecord> AlarmRecords = new List<AlarmRecord>();
            using (SQLiteConnection conn = new SQLiteConnection())
            {
                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                conStr.DataSource = url;
                conn.ConnectionString = conStr.ToString();
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand($"SELECT * FROM AlarmRecord WHERE julianday(AlarmDate)>julianday('{dtt.ToString("yyyy-MM-dd")}' 00:00:00) AND julianday(AlarmDate)<julianday('{dtt.ToString("yyyy-MM-dd")}' 23:59:59)");
                cmd.Connection = conn;
                DataTable dt = new DataTable();
                SQLiteDataAdapter reader = new SQLiteDataAdapter(cmd);
                reader.Fill(dt);
                reader.Dispose();
                cmd.Dispose();
                if (dt.Rows.Count > 0)
                {                 
                    foreach (DataRow dr in dt.Rows)
                    {
                        AlarmRecord d = new AlarmRecord();
                        d.Id = Int32.Parse(dr["Id"].ToString());
                        var m= dr["DeviceCode"].ToString();
                        if (!string .IsNullOrEmpty(dr["AlarmDate"].ToString())) d.AlarmDate = DateTime.Parse(dr["LastDate"].ToString()); else d.AlarmDate = null;
                        AlarmRecords.Add(d);
                    }
                    return AlarmRecords;
                }
                return null;
            }
        }

        public bool InsertDevice(AlarmRecord AlarmRecord)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                    conStr.DataSource = url;
                    conn.ConnectionString = conStr.ToString();
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO AlarmRecord ('DeviceCode','AlarmDate') VALUES('{AlarmRecord.DeviceCode}','{(DateTime.Parse(AlarmRecord.AlarmDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}')");
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                var m = ex;
                return false;
            }
        }
        public bool UpdateDevice(AlarmRecord AlarmRecord)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {

                    SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                    conStr.DataSource = url;
                    conn.ConnectionString = conStr.ToString();
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"UPDATE AlarmRecord SET AlarmDate='{(DateTime.Parse(AlarmRecord.AlarmDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}' WHERE Id='{AlarmRecord.Id}' AND DeviceCode='{AlarmRecord.DeviceCode}'");
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
