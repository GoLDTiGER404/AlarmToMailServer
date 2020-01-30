using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZGAutoCenter.TimeTask;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using AlarmToMailServer.Model;
using AlarmToMailServer.TimeTask;

namespace AlarmToMailServer.Service
{
    /// <summary>
    /// 拆炉记录处理主业务逻辑
    /// </summary>
    public class RFServerCenter
    {
        //TODO 对比数据触发报警

        public static IConfiguration Configuration { get; set; }

        public async Task Handle(string TaskName, string TaskUrl)
        {
            try
            {
                Console.WriteLine($"{TaskName}当次轮询启动----" + DateTime.Now);
                RemoveRecordHelp removeRecordHelp = new RemoveRecordHelp();
                SqlHelpAsync sqlHelp = new SqlHelpAsync(TaskUrl);
                List<HisDevice> hisDevices = new List<HisDevice>();
                List<RemoveRecord> removeRecords = new List<RemoveRecord>();
                removeRecords = removeRecordHelp.GetAll();
                List<DeviceManage> deviceManages = new List<DeviceManage>();
                List<HisDeviceAvg> hisDeviceAvgs = new List<HisDeviceAvg>();
                deviceManages = await sqlHelp.GetList<DeviceManage>();

                foreach (var dm in deviceManages)
                {
                    hisDevices = await sqlHelp.GetListByFilter<HisDevice>($"SELECT TOP 1 * FROM  His_{dm.DeviceName} order by HisTime desc");

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
                //判断写入拆炉
                foreach (var h in hisDeviceAvgs)
                {
                    if (h.nMode != 13) continue;//未达报警条件
                    if (removeRecords != null)//初始化加载
                    {
                        bool isexsit = false;
                        foreach (var d in removeRecords)
                        {
                            if (d.DeviceCode == h.DeviceCode)
                            {
                                isexsit = true;
                                break;
                            }
                        }
                        if (isexsit) //判断Device存在
                        {
                            bool PLIDexsit = true;
                            foreach (var d in removeRecords)
                            {
                                if (d.DeviceCode == h.DeviceCode)
                                {
                                    if (d.PLID == h.ID)
                                    {
                                        PLIDexsit = false;
                                        break;
                                    }
                                }

                            }
                            if (PLIDexsit)//新周期
                            {
                                RemoveRecord removeRecord = new RemoveRecord();
                                try
                                {
                                    //TODO入库
                                    removeRecord.PLID = h.ID;
                                    removeRecord.DeviceCode = h.DeviceCode;
                                    removeRecord.StopDate = h.HisTime;
                                    var u = removeRecord.RemoveDate;
                                    removeRecordHelp.InsertDevice(removeRecord);

                                    Console.WriteLine($">>>>>>>>{TaskName}检测到{removeRecord.ZoneCode}片区炉台{h.DeviceCode}拆清情况!入库成功!<<<<<<<<<<<<");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($">>>>>>>>{TaskName}-{removeRecord.ZoneCode}片区炉台{h.DeviceCode}拆清入库失败!<<<<<<<<<<<<");
                                    Console.WriteLine(ex.Message.ToString());
                                }
                                continue;
                            }

                        }
                        else//第一次入库
                        {
                            RemoveRecord removeRecord = new RemoveRecord();
                            try
                            {
                                //TODO入库
                                removeRecord.PLID = h.ID;
                                removeRecord.DeviceCode = h.DeviceCode;
                                removeRecord.StopDate = h.HisTime;
                                var u = removeRecord.RemoveDate;
                                removeRecordHelp.InsertDevice(removeRecord);

                                Console.WriteLine($">>>>>>>>{TaskName}检测到{removeRecord.ZoneCode}片区炉台{h.DeviceCode}拆清情况!入库成功!<<<<<<<<<<<<");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($">>>>>>>>{TaskName}-{removeRecord.ZoneCode}片区炉台{h.DeviceCode}拆清入库失败!<<<<<<<<<<<<");
                                Console.WriteLine(ex.Message.ToString());
                            }
                            continue;
                        }
                    }
                    else//初始化入库
                    {
                        RemoveRecord removeRecord = new RemoveRecord();
                        try
                        {
                            //TODO入库
                            removeRecord.PLID = h.ID;
                            removeRecord.DeviceCode = h.DeviceCode;
                            removeRecord.StopDate = h.HisTime;
                            removeRecordHelp.InsertDevice(removeRecord);
                            Console.WriteLine($">>>>>>>>{TaskName}检测到{removeRecord.ZoneCode}片区炉台{h.DeviceCode}拆清情况!入库成功!<<<<<<<<<<<<");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($">>>>>>>>{TaskName}-{removeRecord.ZoneCode}片区炉台{h.DeviceCode}拆清入库失败!<<<<<<<<<<<<");
                            Console.WriteLine(ex.Message.ToString());
                        }
                        continue;
                    }
                }
                Console.WriteLine($"{TaskName}当次轮询处理结束----" + DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{TaskName}当次轮询失败结束----" + DateTime.Now + $"失败:{ex.Message}");
            }
        }

        public HisDeviceAvg Maping(HisDeviceAvg hisDeviceAvg, HisDevice hisDevice)
        {

            var hp = hisDevice.GetType().GetProperties();
            foreach (var h in hp)
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
