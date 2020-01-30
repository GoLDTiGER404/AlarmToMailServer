using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZGAutoCenter.TimeTask;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using AlarmToMailServer.Model;

namespace AlarmToMailServer.Service
{
    /// <summary>
    /// 处理主业务逻辑
    /// </summary>
    public class ServerCenter
    {
        //TODO 对比数据触发报警

        public static IConfiguration Configuration { get; set; }

        public void Handle()
        {
            Console.WriteLine("当次轮询启动----"+DateTime.Now);
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
            DeviceRecordHelp deviceRecordHelp = new DeviceRecordHelp();
            SqlHelp sqlHelp = new SqlHelp(Configuration.GetSection("Connection").Value);
            List<HisDevice> hisDevices = new List<HisDevice>();
            List<DeviceRecord> deviceRecords = new List<DeviceRecord>();
            deviceRecords = deviceRecordHelp.GetAll();
            List<DeviceManage> deviceManages = new List<DeviceManage>();
            List<HisDeviceAvg> hisDeviceAvgs = new List<HisDeviceAvg>();
            deviceManages =sqlHelp.GetList<DeviceManage>();
  
            foreach (var dm in deviceManages)
            {
              hisDevices = sqlHelp.GetListByFilter<HisDevice>($"SELECT TOP 1 * FROM  His_{dm.DeviceName} order by HisTime desc");

                if (hisDevices != null)
                {
                    foreach (var h in hisDevices)   //将所有历史表的第一行全部存起
                    {
                        HisDeviceAvg hisDeviceAvg = new HisDeviceAvg();
                        hisDeviceAvg = Maping(hisDeviceAvg, h);
                        hisDeviceAvg.DeviceCode = dm.DeviceName;
                        hisDeviceAvgs.Add(hisDeviceAvg);

                    }
                }
            }
         //   Console.WriteLine("中段读取结束----" + DateTime.Now);
            //报警处理
            foreach (var h in hisDeviceAvgs)
            {
                if (deviceRecords != null)//初始化加载
                {
                    bool isexsit = false;
                    foreach (var d in deviceRecords)
                    {
                        if (d.DeviceCode == h.DeviceCode)
                        {
                            isexsit = true;
                            break;
                        }
                    }
                    if (isexsit) //判断表变化
                    {
                        bool timeexsit = false;
                        foreach (var d in deviceRecords)
                        {
                            if (d.DeviceCode == h.DeviceCode)
                            {
                                if (d.LastDate != h.HisTime)
                                {
                                    timeexsit = true;
                                    break;
                                }
                            }

                        }
                        if (timeexsit)//表有变化
                        {
                            var r = deviceRecords.Where(d => d.DeviceCode == h.DeviceCode).FirstOrDefault();
                            if (h.nMode == 4 && h.dbZhuJiaReGongLv == 0)//触发报警条件
                            {
                                if (!r.AlarmSign.HasValue || !r.AlarmSign.Value)//如果报警不存在继续发邮件
                                {
                                    r.AlarmSign = true;
                                    r.LastDate = h.HisTime;
                                    //TODO入库
                                    deviceRecordHelp.UpdateDevice(r);//更新设备记录表
                                    AlarmRecordHelp alarmRecordHelp = new AlarmRecordHelp();
                                    AlarmRecord alarmRecord = new AlarmRecord();
                                    alarmRecord.DeviceCode = h.DeviceCode;
                                    alarmRecord.AlarmDate = h.HisTime;
                                    alarmRecordHelp.InsertDevice(alarmRecord);//写入报警记录表
                                    //TODO 报警
                                    SendMailEntity sendMailEntity = new SendMailEntity();
                                    sendMailEntity.AlarmDate = h.HisTime;
                                    sendMailEntity.DeviceCode = h.DeviceCode;
                                    SendMail sendMail = new SendMail(sendMailEntity);
                                    try
                                    {
                                        Console.WriteLine($">>>>>>>>检测到炉台{h.DeviceCode}报警!<<<<<<<<<<<<");
                                        sendMail.SendMailMain();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("邮件发送失败！");
                                        Console.WriteLine(ex.Message.ToString());
                                    }
                                    continue;
                                }
                            }
                            if (h.nMode == 13)//报警结束更新
                            {
                                r.AlarmSign = false;
                                r.LastDate = h.HisTime;
                                //TODO入库
                                deviceRecordHelp.UpdateDevice(r);
                                continue;
                            }
                            //没有触发报警条件的只更新时间
                            r.LastDate = h.HisTime;
                            //TODO入库
                            deviceRecordHelp.UpdateDevice(r);
                            continue;

                        }
                    }
                    else//第一次入库
                    {
                        DeviceRecord di = new DeviceRecord();
                        di.DeviceCode = h.DeviceCode;
                        di.LastDate = h.HisTime;
                        di.AlarmSign = false;
                        deviceRecordHelp.InsertDevice(di);       //入库     
                    }
                }
                else//初始化入库
                {
                    DeviceRecord di = new DeviceRecord();
                    di.DeviceCode = h.DeviceCode;
                    di.LastDate = h.HisTime;
                    di.AlarmSign = false;
                    deviceRecordHelp.InsertDevice(di);       //入库     
                }
            }
            Console.WriteLine("当次轮询报警处理结束----" + DateTime.Now);
        }

        public HisDeviceAvg Maping(HisDeviceAvg hisDeviceAvg, HisDevice hisDevice)
        {
        
            var hp = hisDevice.GetType().GetProperties();
            foreach(var h in hp )
            {
               
                var plist = hisDeviceAvg.GetType().GetProperties();
                foreach (var p in plist)
                {
                    String _name = p.Name;
                    if (h.Name == p.Name)
                    {
                        if (p.PropertyType.Equals(typeof(String)))//判断属性的类型是不是String
                        {
                            p.SetValue(hisDeviceAvg, h.GetValue(hisDevice), null);//给泛型的属性赋值
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(h.GetValue(hisDevice).ToString()))//只写不为空的
                            {
                                if (p.PropertyType.Name != "Nullable`1")//对非Nullable进行判断
                                {
                                    if (p.PropertyType.IsEnum)
                                    {
                                        p.SetValue(hisDeviceAvg, (h.GetValue(hisDevice)), null);//强制做转换
                                    }
                                    else
                                    {
                                        p.SetValue(hisDeviceAvg, Convert.ChangeType(h.GetValue(hisDevice), p.PropertyType), null);//强制做转换
                                    }
                                }
                                else
                                {
                                    foreach (var tp in p.PropertyType.GenericTypeArguments)//过滤nullable的属性
                                    {
                                        if (tp.Name != null)
                                        {
                                            p.SetValue(hisDeviceAvg, Convert.ChangeType(h.GetValue(hisDevice), tp), null);//强制做转换
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return hisDeviceAvg;
        }



    }
}
