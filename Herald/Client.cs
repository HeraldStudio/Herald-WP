using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Herald
{

    public class Client
    {
        public string server { set; get; }
        private HttpClient httpClient;
        private CancellationTokenSource cts;
        public Client()
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
        }
        public Client(String str)
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
            server = str;
        }
        private async Task<string> HttpRequestAsync(Func<Task<string>> httpRequestFuncAsync, Action<String> Change, Action Wait,int flag)
        {
            string responseBody;
            try
            {
                responseBody = await httpRequestFuncAsync();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                responseBody = "请求被取消";
                throw new Exception();
            }
            catch (Exception ex)
            {
                responseBody = "错误信息：" + ex.Message;
                if (responseBody.Contains("HRESULT"))
                    responseBody = "没有网络的样子\r\n\n" + responseBody;
                if (responseBody.Contains("401"))
                    responseBody = "一卡通或者密码不太对哦\r\n\n" + responseBody;
                if (responseBody.Contains("408"))
                    responseBody = "超时了，你的脸不行呀\r\n\n" + responseBody;
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "看起来出错了呢~_~", //标题
                    Content = responseBody,//内容
                    FullSizeDesired = false,  //是否全屏展示
                    PrimaryButtonText = "OK",
                };
                if (flag==1)
                    dialog.ShowAsync();
                throw new Exception();
            }
            finally
            {
                if (Wait != null)
                    Wait();
            }
            if (Change != null)
                Change(responseBody);
            return responseBody;
        }
        public async Task<string> Post(HttpFormUrlEncodedContent postData, Action<String> TextChange, Action Wait, string url = null,int flag=1)
        {

            Task<string> temp = HttpRequestAsync(async () =>
            {
                string resourceAddress;
                if (url == null)
                    resourceAddress = server;
                else
                    resourceAddress = url;
                string responseBody;
                HttpResponseMessage response = await httpClient.PostAsync(
                    new Uri(resourceAddress),
                    postData).
                    AsTask(cts.Token);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);             
                return responseBody;
            }, TextChange, Wait,flag);
            return await temp;
        }
        public async Task<string> Get(Action<String> Change, Action Wait, string url = null,int flag=1)
        {

            Task<string> temp = HttpRequestAsync(async () =>
            {
                string resourceAddress;
                if (url == null)
                    resourceAddress = server;
                else
                    resourceAddress = url;
                string responseBody;
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(resourceAddress)).AsTask(cts.Token);
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                return responseBody;
            }, Change, Wait,flag);
            return await temp;
        }
    }
}
