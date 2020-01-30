using AlarmToMailServer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    public class DeviceRecordHelp
    {
        string url = "";
        public DeviceRecordHelp()
        {
            url = SQLiteUrl.GetValue();
        }

        public List<DeviceRecord> GetAll()
        {
            List<DeviceRecord> deviceRecords = new List<DeviceRecord>();
            using (SQLiteConnection conn = new SQLiteConnection())
            {
                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                conStr.DataSource = url;
                conn.ConnectionString = conStr.ToString();
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM DeviceRecord");
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
                        DeviceRecord d = new DeviceRecord();
                        d.Id = Int32.Parse(dr["Id"].ToString());
                        var m= dr["DeviceCode"].ToString();
                        d.DeviceCode = dr["DeviceCode"].ToString();
                        if (!string .IsNullOrEmpty(dr["LastDate"].ToString())) d.LastDate = DateTime.Parse(dr["LastDate"].ToString()); else d.LastDate = null;
                        if (!string.IsNullOrEmpty(dr["LastCount"].ToString())) d.LastCount = Int32.Parse(dr["LastCount"].ToString()); else d.LastCount = null;
                        if (!string.IsNullOrEmpty(dr["AlarmSign"].ToString())) d.AlarmSign = bool.Parse(dr["AlarmSign"].ToString()); else d.AlarmSign = null;
                        deviceRecords.Add(d);
                    }
                    return deviceRecords;
                }
                return null;
            }
        }

        public bool InsertDevice(DeviceRecord deviceRecord)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                    conStr.DataSource =url;
                    conn.ConnectionString = conStr.ToString();
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO DeviceRecord ('DeviceCode','LastDate','AlarmSign') VALUES('{deviceRecord.DeviceCode}','{(DateTime.Parse(deviceRecord.LastDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}','{deviceRecord.AlarmSign}')");
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
        public bool UpdateDevice(DeviceRecord deviceRecord)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {

                    SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                    conStr.DataSource = url;
                    conn.ConnectionString = conStr.ToString();
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"UPDATE DeviceRecord SET LastDate='{(DateTime.Parse(deviceRecord.LastDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}',LastCount='{deviceRecord.LastCount}',AlarmSign='{deviceRecord.AlarmSign}' WHERE Id='{deviceRecord.Id}' AND DeviceCode='{deviceRecord.DeviceCode}'");
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
