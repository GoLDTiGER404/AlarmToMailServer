using AlarmToMailServer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmToMailServer.Service
{
    public class RemoveRecordHelp
    {
        string url = "";
        public RemoveRecordHelp()
        {
            url = SQLiteUrl.GetValue();
        }

        public List<RemoveRecord> GetAll()
        {
            List<RemoveRecord> removeRecords = new List<RemoveRecord>();
            using (SQLiteConnection conn = new SQLiteConnection())
            {
                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                conStr.DataSource = url;
                conn.ConnectionString = conStr.ToString();
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM  RemoveRecord");
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
                        RemoveRecord d = new RemoveRecord();
                        d.Id = Int32.Parse(dr["Id"].ToString());
                        d.DeviceCode = dr["DeviceCode"].ToString();
                        d.PLID = dr["PLID"].ToString();
                        d.ZoneCode = dr["ZoneCode"].ToString();
                        d.Remark = dr["Remark"].ToString();
                        if (!string .IsNullOrEmpty(dr["RemoveDate"].ToString())) d.RemoveDate = DateTime.Parse(dr["RemoveDate"].ToString()); else d.RemoveDate = null;
                        if (!string.IsNullOrEmpty(dr["StopDate"].ToString())) d.StopDate = DateTime.Parse(dr["StopDate"].ToString()); else d.StopDate = null;
                        removeRecords.Add(d);
                    }
                    return removeRecords;
                }
                return null;
            }
        }

        public bool InsertDevice(RemoveRecord removeRecord)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                    conStr.DataSource =url;
                    conn.ConnectionString = conStr.ToString();
                    conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO RemoveRecord ('PLID','ZoneCode','DeviceCode','StopDate','RemoveDate','Remark') VALUES('{removeRecord.PLID}','{removeRecord.ZoneCode}','{removeRecord.DeviceCode}','{(DateTime.Parse(removeRecord.StopDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}','{(DateTime.Parse(removeRecord.RemoveDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}','{removeRecord.Remark}')");
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool UpdateDevice(RemoveRecord removeRecord)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {

                    SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
                    conStr.DataSource = url;
                    conn.ConnectionString = conStr.ToString();
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"UPDATE RemoveRecord SET StopDate='{(DateTime.Parse(removeRecord.StopDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}',RemoveDate='{(DateTime.Parse(removeRecord.RemoveDate.ToString())).ToString("yyyy-MM-dd HH:mm:ss")}',Remark='{removeRecord.Remark}' WHERE Id='{removeRecord.Id}' AND DeviceCode='{removeRecord.DeviceCode}' AND PLID='{removeRecord.PLID}'");
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
