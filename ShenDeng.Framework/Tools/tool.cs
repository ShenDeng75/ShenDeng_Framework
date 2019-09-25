using ExcelDataReader;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using ShenDeng.Framework.Application;

namespace ShenDeng.Framework.Tools
{
    public static class tool
    {
        // 读取Excel文件到DateSet
        public static DataTable Excel2DataTable(string excelPath)
        {
            using (FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))//定义文件流
            {
                int index = excelPath.LastIndexOf('.');//获取文件扩展名前‘.’的位置
                string extensionName = excelPath.Substring(index + 1);
                IExcelDataReader excelReader;
                if (extensionName == "xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (extensionName == "xlsx")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    throw new Exception("文件格式错误!");
                }
                DataTable result = excelReader.AsDataSet().Tables[0];
                return result;
            }
        }
        // 解析ajax返回的json数据
        public static T Deserialize<T>(Stream _stream)
        {
            var stream = new StreamReader(_stream);
            var str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var datas = js.Deserialize<T>(str);
            return datas;
        }
        // 写入数据到Excel
        public static string Output2Excel(string path)
        {
            
            HSSFWorkbook wk = new HSSFWorkbook();   //创建工作薄
            ISheet tb = wk.CreateSheet("样件");   //创建一个名称为Sheet的表
            TableHead(tb);    // 设置表头
            int i = 1;
            foreach (var exe in new[] {"1", "2"})   // 添加数据
            {
                IRow r = tb.CreateRow(i);    // 第几行
                r.CreateCell(1).SetCellValue(exe);  // 第几列赋值
                i++;
            }
            using (FileStream fs = File.OpenWrite(path)) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
            {
                wk.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
            }
            string[] filenames = path.Split(new[] { '/', '\\' });
            string filename = "/File/" + filenames[filenames.Length - 1];
            return filename;
        }
        // 设置表头
        public static void TableHead(ISheet tb)
        {
            List<string> heads = new List<string> { "封样日期", "物料编号"};
            IRow r0 = tb.CreateRow(0);   // 第一行
            int i = 0;
            foreach (var h in heads)
            {
                ICell cell = r0.CreateCell(i); // 列
                cell.SetCellValue(h);
                i++;
            }
        }
        // 序列化
        public static string getJsonByObject<T>(T exe)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(exe.GetType());
            MemoryStream stream = new MemoryStream();   //实例化一个内存流，用于存放序列化后的数据
            serializer.WriteObject(stream, exe);    //使用WriteObject序列化对象
            byte[] dataBytes = new byte[stream.Length];  //写入内存流中
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            string result = Encoding.UTF8.GetString(dataBytes);   //通过UTF8格式转换为字符串
            JObject jobj = (JObject)JsonConvert.DeserializeObject(result);  // 将Json字符串转为Json数据
            return result;
        }
        // 发送 NG 邮件
        public static void SendMail(string exe, string emailaddre, string password, MailServer mailServer)
        {
            SmtpClient client = new SmtpClient("maila.skyworth.com", 25);  // ***SMTP协议

            client.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            client.UseDefaultCredentials = true;
            string username = emailaddre.Split(new[] { '@' })[0];
            client.Credentials = new System.Net.NetworkCredential(username, password);//用户名、密码

            MailMessage msg = new MailMessage();   // ***邮件
            msg.From = new MailAddress(emailaddre, "样件管理系统");   //发件人地址
            msg.Subject = "样件审核结果 NG 通知";    //邮件标题
            msg.Body = SendContent(exe);    //邮件内容
            msg.BodyEncoding = Encoding.UTF8;    //邮件内容编码
            msg.IsBodyHtml = true;   //是否是HTML邮件
            msg.Priority = MailPriority.Normal;    //邮件优先级
            Tos(mailServer, exe, msg);   // 接收者

            client.Send(msg);     // 发送
        }
        // 添加接收邮箱
        public static void Tos(MailServer mailServer, string mateclass, MailMessage msg)
        {
            var duty = "";
            if (mateclass[0] == '数')
                duty = "数字";
            else if (mateclass[0] == '无')
                duty = "无线";
            else
                duty = "新世界";
            var yf = mailServer.Find2Duty(duty);
            var iqc = mailServer.Find2Duty("IQC");
            foreach (var i in yf)
                msg.To.Add(i.MailAddre);
            foreach (var i in iqc)
                msg.To.Add(i.MailAddre);
        }
        // 发送内容
        public static string SendContent(string exe)
        {
            string now = DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd");
            string result = "";
            result += "<div><h3>您好！以下是 " + now + " 来自IQC的检验结果</h3>";
            result += "签收日期：<strong>" + exe + "</strong><br /></div>";
            return result;
        }
    }
}
