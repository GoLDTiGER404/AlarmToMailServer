using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
//using Sql.Data;
//using Sql.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using System.Linq;
//using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace ZGAutoCenter.TimeTask
{
    public class SqlHelp
    {
        string url;
        public SqlHelp(string Url)
        {
            url = Url;
        }

        public List<T> GetList<T>()
        {
            using (SqlConnection connection = new SqlConnection(url))
            {
                System.Type t = typeof(T);
                Assembly ass = Assembly.GetAssembly(t);//获取泛型程序集
                List<T> tlist = new List<T>();
                connection.Open();
                string tablename = "t_" + typeof(T).Name;
                string sql = $"SELECT * FROM {tablename}";
                var dt = UrlSqlExcute(sql, connection);
                if (dt.Rows.Count < 1) return null;
                var plist = typeof(T).GetProperties();
                if (dt.Columns.Count == plist.Length)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Object _obj = ass.CreateInstance(t.FullName);//泛型实例化
                        foreach (var p in plist)
                        {
                            String _name = p.Name;
                            if (p.PropertyType.Equals(typeof(String)))//判断属性的类型是不是String
                            {
                                p.SetValue((T)_obj, dr[_name].ToString(), null);//给泛型的属性赋值
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dr[_name].ToString()))//只写不为空的
                                {
                                    if (p.PropertyType.Name != "Nullable`1")//对非Nullable进行判断
                                    {
                                        if (p.PropertyType.IsEnum)
                                        {
                                            //p.SetValue((T)_obj, (DState)Enum.Parse(typeof(DState), dr[_name].ToString()), null);//强制做转换
                                        }
                                        else
                                        {
                                            p.SetValue((T)_obj, Convert.ChangeType(dr[_name].ToString(), p.PropertyType), null);//强制做转换
                                        }
                                    }
                                    else
                                    {
                                        foreach (var tp in p.PropertyType.GenericTypeArguments)//过滤nullable的属性
                                        {
                                            if (tp.Name != null)
                                            {
                                                p.SetValue((T)_obj, Convert.ChangeType(dr[_name].ToString(), tp), null);//强制做转换
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        tlist.Add((T)_obj);
                    }
                    return tlist;
                }
                else return null;
            }
        }
        public void InsertOne<T>(T e)
        {
            using (SqlConnection connection = new SqlConnection(url))
            {
                connection.Open();
                string tablename = "t_" + e.GetType().Name;
                string colname = "";
                string value = "";
                var plist = e.GetType().GetProperties();
                foreach (var p in plist)
                {
                   // var PrimaryKey = p.GetCustomAttribute(typeof(KeyAttribute));
                    //if (PrimaryKey == null)//主键外
                    //{
                    //    if (p.GetValue(e) != null)
                    //    {
                    //        if (p.PropertyType.IsEnum)
                    //        {
                    //            colname = colname + p.Name + ",";
                    //          //  value = value + "'" + GetEnum(p.GetValue(e).ToString()) + "'" + ",";
                    //        }
                    //        else
                    //        {
                    //            colname = colname + p.Name + ",";
                    //            value = value + "'" + p.GetValue(e) + "'" + ",";
                    //        }
                    //    }
                    //}
                }
                colname = colname.Substring(0, colname.Length - 1);
                value = value.Substring(0, value.Length - 1);
                string sql = $"INSERT INTO {tablename} ({colname}) VALUES({value})";
                UrlSqlExcuteq(sql, connection);
            }
        }


        public void Update<T>(T e)
        {
            using (SqlConnection connection = new SqlConnection(url))
            {
                connection.Open();
                string tablename = "t_" + e.GetType().Name;
                string fliter = "";
                string set = "";
                var plist = e.GetType().GetProperties();
                foreach (var p in plist)
                {
                   //// var PrimaryKey = p.GetCustomAttribute(typeof(KeyAttribute));
                   // if (p.GetValue(e) != null)
                   // {
                   //     if (PrimaryKey == null)
                   //     {
                   //         if (p.PropertyType.IsEnum)
                   //         {
                   //             //set = set + p.Name + " = " + "'" + GetEnum(p.GetValue(e).ToString()) + "'" + ",";
                   //         }
                   //         else
                   //         {
                   //             set = set + p.Name + " = " + "'" + p.GetValue(e) + "'" + ",";
                   //         }
                   //     } 
                   //     else
                   //     {
                   //         fliter = p.Name + " = " + "'" + p.GetValue(e) + "'";
                   //     }
                   // }
                }
                set = set.Substring(0, set.Length - 1);
                string sql = $"UPDATE {tablename} SET {set} WHERE {fliter} ";
                UrlSqlExcuteq(sql, connection);
            }
        }



        public DataTable LocalSqlExcute(string sql, SqlConnection connection)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataAdapter reader = new SqlDataAdapter(command);
                reader.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return dt;
            }
        }

        public DataTable UrlSqlExcute(string sql, SqlConnection connectionurl)
        {
            DataTable dt = new DataTable();
            SqlCommand command = new SqlCommand(sql, connectionurl);
            SqlDataAdapter reader = new SqlDataAdapter(command);
            reader.Fill(dt);
            return dt;
        }

        private object LocalSqlExcuteq(string sql, SqlConnection connection)
        {

            SqlCommand command = new SqlCommand(sql, connection);
            var M = command.ExecuteScalar();
            return M;
        }

        private object UrlSqlExcuteq(string sql, SqlConnection connectionurl)
        {
            SqlCommand command = new SqlCommand(sql, connectionurl);
            var M = command.ExecuteScalar();
            return M;
        }

        //private int GetEnum(string str)
        //{
        //    //foreach (var item in Enum.GetValues(typeof(DState)))
        //    //{
        //    //    if (item.ToString() == str)
        //    //        return (int)item;//0[1]
        //    //}
        //    //return 0;
        //}

    }
}
