using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Herald
{
    public static class FileSystem
    {
        // 写文件的 Demo
        public static async void Write(String SaveName,String Content)
        {
            // 获取应用程序数据存储文件夹
            StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;

            // 在指定的应用程序数据存储文件夹内创建指定的文件
            StorageFile storageFile = await applicationFolder.CreateFileAsync(SaveName, CreationCollisionOption.ReplaceExisting);

            // 将指定的文本内容写入到指定的文件
            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            {
                byte[] content = Encoding.UTF8.GetBytes(Content);
                await stream.WriteAsync(content, 0, content.Length);
            }
        }

        // 读文件的 Demo
        public static async Task<string> Read(String ReadName)
        {
            // 获取应用程序数据存储文件夹
            StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = null;
            try
            {
                // 在指定的应用程序数据存储文件夹中查找指定的文件
                storageFile = await applicationFolder.GetFileAsync(ReadName);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                // 没找到指定的文件
                return null;
            }

            // 获取指定的文件的文本内容
            if (storageFile != null)
            {
                IRandomAccessStreamWithContentType accessStream = await storageFile.OpenReadAsync();

                using (Stream stream = accessStream.AsStreamForRead((int)accessStream.Size))
                {
                    byte[] content = new byte[stream.Length];
                    await stream.ReadAsync(content, 0, (int)stream.Length);
                    return Encoding.UTF8.GetString(content, 0, content.Length);
                }
            }
            return "error";
        }
        // 读文件的 Demo
        public static async void Delete(String ReadName)
        {
            // 获取应用程序数据存储文件夹
            StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = null;
            try
            {
                // 在指定的应用程序数据存储文件夹中查找指定的文件
                storageFile = await applicationFolder.GetFileAsync(ReadName);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                // 没找到指定的文件
                //throw ex;
            }
            // 获取指定的文件的文本内容
            if (storageFile != null)
            {
                StorageDeleteOption var = StorageDeleteOption.Default;
                storageFile.DeleteAsync(var);
            }
        }
    }

    //解析json
    internal static class JsonParse
    {
        public static T Parse<T>(Stream stream)
        {
            return JsonParse._parse<T>(stream);
        }
        private static T _parse<T>(Stream stream)
        {
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
        }

        public static string constructJsonString(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                byte[] byteArr = ms.ToArray();
                return Encoding.UTF8.GetString(byteArr, 0, byteArr.Length);
            }
        }
    }
    //api接口系列
    internal class URLDic
    {
        public readonly string auth = "http://herald.seu.edu.cn/uc/auth";
        public readonly string check = "http://herald.seu.edu.cn/uc/check";
        public readonly string update = "http://herald.seu.edu.cn/uc/update";
        public readonly string deauth = "http://herald.seu.edu.cn/uc/deauth";
        public readonly string srtp = "http://herald.seu.edu.cn/api/srtp";//DataContract完成
        public readonly string term = "http://herald.seu.edu.cn/api/term";//DataContract完成
        public readonly string sidebar = "http://herald.seu.edu.cn/api/sidebar";//DataContract完成
        public readonly string curriculum = "http://herald.seu.edu.cn/api/curriculum";//DataContract完成
        public readonly string gpa = "http://herald.seu.edu.cn/api/gpa";//DataContract完成
        public readonly string pe = "http://herald.seu.edu.cn/api/pe";//DataContract完成
        public readonly string simsimi = "http://herald.seu.edu.cn/api/simsimi";
        public readonly string lecture = "http://herald.seu.edu.cn/api/lecture";//DataContract完成
        public readonly string card = "http://herald.seu.edu.cn/api/card";//DataContract完成
        public readonly string nic = "http://herald.seu.edu.cn/api/nic";//DataContract完成
        public readonly string library = "http://herald.seu.edu.cn/api/library";//DataContract完成
        public readonly string renew = "http://herald.seu.edu.cn/api/renew";//DataContract完成
        public readonly string search = "http://herald.seu.edu.cn/api/search";//DataContract完成
        public readonly string queryEmptyClassrooms = "http://herald.seu.edu.cn/queryEmptyClassrooms/query/";
    }
    //用户类
    public class HeraldUser
    {
        public Client postClient;
        private URLDic Dic;
        private string APPID = "5b45c345a764a6d35c905e1a70a590ba";
        public string UUID { get; set; }
        public string userID { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public bool Status { get; set; }
        public string ImagePath { get; set; }
        public HeraldUser()
        {
            this.postClient = new Client();
            this.Dic = new URLDic();
            //postClient.server = "http://127.0.0.1/test.php";//test部分
            this.Name = null;
            this.Status = false;
            this.Token = null;
            this.userID = null;
        }
        //用户登录授权获取uuid
        public async Task<int> Auth(string name, string password, Action call = null)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("user",name),
                        new KeyValuePair<string, string>("password",password),
                        new KeyValuePair<string, string>("appid",APPID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.auth);
                string res = await ret;
                Status = true;
                UUID = res;
                if (call != null)
                    call();
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
            
        }
        //检查uuid合法性
        public async Task<int> Check()
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid",UUID),
                        new KeyValuePair<string, string>("appid",APPID),
                    }
            );
            try {
                Task<string> ret = postClient.Post(postData, null, null, Dic.check);
                string res = await ret;     
            }
            catch (Exception e)
            {
                return 0;   
            }
            return 1;
                  
        }
        //更新绑定信息
        /*
       cardnum	一卡通
       password	统一认证密码
       *number	可选:学号
       *pe_password	可选:跑操查询密码
       *lib_username	可选:图书馆用户名
       *lib_password	可选:图书馆密码
       *card_query_password	可选:一卡通查询密码
       */
        public async Task<int> Update(string cardnum, string password, string number = null, string pe_password = null, string lib_username = null, string lib_password = null, string card_query_password = null)
        {
            List<KeyValuePair<string, string>> postdata =
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("cardnum",cardnum),
                        new KeyValuePair<string, string>("password",password),
                        
                    };
            if (number != null)
                postdata.Add(new KeyValuePair<string, string>("number", number));
            if (pe_password != null)
                postdata.Add(new KeyValuePair<string, string>("pe_password", pe_password));
            if (lib_username != null)
                postdata.Add(new KeyValuePair<string, string>("lib_username", lib_username));
            if (lib_password != null)
                postdata.Add(new KeyValuePair<string, string>("lib_password", lib_password));
            if (card_query_password != null)
                postdata.Add(new KeyValuePair<string, string>("card_query_password", card_query_password));
            var postData = new HttpFormUrlEncodedContent(postdata);
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.update);
                string res = await ret;
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }

        //注销UUID
        public async Task<int> Deauth()
        {
            List<KeyValuePair<string, string>> postdata =
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid",UUID),       
                    };
            var postData = new HttpFormUrlEncodedContent(postdata);
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.deauth);
                string res = await ret;
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }
        //获取json格式的SRTP信息
        public async Task<string> SRTP(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.srtp,flag);
                string res = await ret;
                if (res.Contains("{\n  \"content\": \"time out\", \n  \"code\": 408\n}"))
                    throw new Exception();//灵异问题待解决。。。抛出了异常返回的不是error。。。
                return res;
            }
            catch (Exception e)
            {
                return "error";
            }

        }
        //获取json格式的学期信息
        public async Task<string> Term()
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            Task<string> ret = postClient.Post(postData, null, null, Dic.term);
            string res = await ret;
            return res;
        }
        //获取json格式课程信息
        public async Task<string> SideBar()
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            Task<string> ret = postClient.Post(postData, null, null, Dic.sidebar);
            string res = await ret;
            return res;
        }

        //获取json格式的课程表
        public async Task<string> Curriculum(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.curriculum, flag);
                string res = await ret;
                if (res.Contains("{\n  \"content\": \"time out\", \n  \"code\": 408\n}"))
                    throw new Exception();//灵异问题待解决。。。抛出了异常返回的不是error。。。
                return res;
            }
            catch (Exception e)
            {
                return "error";
            }
        }


        //获取json格式GPA信息
        public async Task<string> GPA(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.gpa,flag);
                string res = await ret;
                if (res.Contains("{\n  \"content\": \"time out\", \n  \"code\": 408\n}"))
                    throw new Exception();//灵异问题待解决。。。抛出了异常返回的不是error。。。
                return res;
            }
            catch(Exception e)
            {
                //throw e;
                return "error";
            }
        }

        //获取json格式跑操信息
        public async Task<string> PE(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.pe,flag);
                string res = await ret;
                return res;
            }
            catch(Exception e)
            {
                return "error";
            }
        }

        //直接返回调戏结果
        public async Task<string> Simsimi(string Message)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                        new KeyValuePair<string, string>("msg", Message),            
                    }
            );
            Task<string> ret = postClient.Post(postData, null, null, Dic.simsimi);
            string res = await ret;
            return res;
        }

        //获取json格式人文讲座信息
        public async Task<string> Lecture(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
             try
            {
            Task<string> ret = postClient.Post(postData, null, null, Dic.lecture,flag);
            string res = await ret;
            return res;
            }
             catch (Exception e)
             {
                 return "error";
             }
        }
        //获取json格式一卡通余额，明细查询
        public async Task<string> Card(int flag=1,string TimeDelta = null)
        {
            List<KeyValuePair<string, string>> postdata =
                   new List<KeyValuePair<string, string>>
                    {
                      new KeyValuePair<string, string>("uuid", UUID),   
                    };
            if (TimeDelta != null)
                postdata.Add(new KeyValuePair<string, string>("timedelta", TimeDelta));//确认
            var postData = new HttpFormUrlEncodedContent(postdata);
             
            try{

                Task<string> ret = postClient.Post(postData, null, null, Dic.card,flag);
                string res = await ret;
                return res;
            }
            catch(Exception e)
            {
                return "error";
            }
            
        }
        //获取json格式校园网用户状态
        public async Task<string> NIC(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.nic, flag);
                string res = await ret;
                return res;
            }
            catch(Exception e)
            {
                return "error";
            }

        }
        //获取json格式图书管已借阅的图书
        public async Task<string> Library(int flag=1)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                    }
            );
            try
            {
                Task<string> ret = postClient.Post(postData, null, null, Dic.library, flag);
                string res = await ret;
                return res;
            }
            catch (Exception e)
            {
                return "error";
            }

        }
        //图书管已借阅的图书续借
        public async Task<string> Renew(string BarCode)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                        new KeyValuePair<string, string>("barcode", BarCode),
                    }
            );
            try{
                Task<string> ret = postClient.Post(postData, null, null, Dic.renew);
                string res = await ret;
                return res;
            }
            catch(Exception e)
            {
                return "error";
            }
        }
        //图书管图书查询
        public async Task<string> Search(string Book)
        {
            var postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("uuid", UUID),
                         new KeyValuePair<string, string>("book", Book),
                    }
            );
            try
            {
            Task<string> ret = postClient.Post(postData, null, null, Dic.search);
            string res = await ret;
            return res;
            }
            catch(Exception e)
            {
                return "error";
            }
        }
        //空教室查询
        public async Task<string> QueryEmptyClassrooms(string Location, string Time, int Start, int End)
        {
            string getQueryEmptyClassrooms = Dic.queryEmptyClassrooms + Location + "/" + Time + "/" + Start.ToString() + "/" + End.ToString();
            Task<string> ret = postClient.Get(null, null, getQueryEmptyClassrooms);
            string res = await ret;
            return res;
        }

        public async Task<string> QueryEmptyClassrooms(string Location, int Week, int Day, int Start, int End)
        {
            string getQueryEmptyClassrooms = Dic.queryEmptyClassrooms + Location + "/" + Week.ToString() + "/" + Day.ToString() + "/" + Start.ToString() + "/" + End.ToString();
            Task<string> ret = postClient.Get(null, null, Dic.queryEmptyClassrooms);
            string res = await ret;
            return res;
        }
    }
}
