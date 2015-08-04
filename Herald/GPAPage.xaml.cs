using Herald.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Herald
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GPAPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public App app = Application.Current as App;
        public TextBlock TextBlockCreate(GPAItem gpaItem)
        {
            TextBlock t = new TextBlock();
            TextBlockChange(t, gpaItem);
            t.FontSize = 20;
            t.Margin = new Thickness(10, 0, 0, 0);
            return t;
        }
        public void TextBlockChange(TextBlock t, GPAItem gpaItem)
        {
            /*"semester": "学期",
            "name": "课程名称",
            "credit": "学分",
            "score": "成绩",
            "type": "类型",
            "extra": "备注"
            */
            t.Text += "学期： " + gpaItem.semester + "\n";
            t.Text += "课程名称： " + gpaItem.name + "\n";
            t.Text += "学分： " + gpaItem.credit + "\n";
            t.Text += "成绩： " + gpaItem.score + "\n";
            t.Text += "类型： " + gpaItem.type + "\n";
            if (!String.IsNullOrEmpty(gpaItem.extra))
                t.Text += "备注： " + gpaItem.extra + "\n";
        }
        public double GetGPA(string str)
        {
            if (str == "及格")
                return 1.5;
            if (str == "中")
                return 2.5;
            if (str == "良")
                return 3.5;
            if (str == "优")
                return 4.5;
            if (str == "通过" || str == "不通过")
                return 0;
            int Score = int.Parse(str);
            if (Score.CompareTo(100)==0)
                return 4.8;
            double GPA = Score / 10;
            GPA = GPA - 5;
            if (Score % 10 >= 6)
                return GPA + 0.8;
            if (Score % 10 >= 3)
                return GPA + 0.5;
            if (Score % 10 >= 0)
                return GPA;
            return 0;
        }
        public double CalGPA(gpa myGPA)
        {
            double Credit = 0;
            double GPA_SUM = 0;
            for (int i = 1; i < myGPA.content.GetLength(0); i++)
            {
                if (myGPA.content[i].extra != "")
                    continue;
                double GPA_temp = GetGPA(myGPA.content[i].score);
                if (GPA_temp>0)
                {
                    Credit += double.Parse(myGPA.content[i].credit);
                    GPA_SUM += double.Parse(myGPA.content[i].credit) * GPA_temp;
                }
            }

            return GPA_SUM / Credit;
        }
        public async void GPAPageChange(){
            Task<string> trt = FileSystem.Read("GPAList");
            string rs = await trt;//re=read result;
            string res;
            int flag=1;//1表示弹窗报错
            Task<string> result;
            if ((!string.IsNullOrEmpty(rs))&&(rs!="error"))
            {
                res = rs;
                flag = 0;
                result = app.User.GPA(flag);
            }
            else
            {
                result = app.User.GPA(flag);
                GPASummary.Text = "因为学校服务器的关系，多长时间能获取成绩单真的看脸—_—";
                res = await result;
            }
            if (res != "error")
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(res));
                try
                {
                    gpa myGPA = JsonParse.Parse<gpa>(ms);
                    GPASummary.Text = "";
                    string[] GPATemp = myGPA.content[0].calculateTime.Split(' ');
                    GPASummary.Text += "平均绩点： " + myGPA.content[0].gpa + "\n";
                    GPASummary.Text += "首修绩点： " + myGPA.content[0].gpaWithoutRevamp + "\n";
                    GPASummary.Text += "计算绩点： " + CalGPA(myGPA).ToString("0.000") + "\n";
                    GPASummary.Text += "绩点计算时间： " + GPATemp[0] + "\n";
                    GPASummary.Text += "绩点详情：\n";
                    for (int i = 1; i < myGPA.content.GetLength(0); i++)
                    {   
                        GPAItems.Items.Add(TextBlockCreate(myGPA.content[i]));
                    }
                    if (myGPA.content.GetLength(0)==1)
                        GPASummary.Text += "木有任何信息~\n";
          
                }
                catch(Exception e)
                {
                }
            }
            else
            {
                GPASummary.Text = "不好意思，出了一点问题。。。";
            }
            res = await result;
            if (res != "error")
            {
                FileSystem.Write("GPAList", res);
            }
            Waiting.IsActive = false;
        }
        public GPAPage()
        {
            this.InitializeComponent();
            Waiting.IsActive = true;
            GPAPageChange();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
