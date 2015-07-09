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
    public sealed partial class LibPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public App app = Application.Current as App;
        public TextBlock TextBlockCreate(BookItem Bookitem)
        {
            TextBlock t = new TextBlock();
            TextBlockChange(t, Bookitem);
            t.FontSize = 20;
            t.Margin = new Thickness(10, 0, 0, 0);
            return t;
        }
        public void TextBlockChange(TextBlock t, BookItem Bookitem)
        {
            /*"due_date": "2014-11-17",
            "author": "徐森林，薛春华编著",
            "barcode": "2296308",
            "render_date": "2014-10-18",
            "place": "中文图书阅览室（3）",
            "title": "实变函数论",
            "renew_time": "0"
            */
            t.Text += "书名： " + Bookitem.title + "\n";
            t.Text += "归还时间： " + Bookitem.due_date + "\n";
            t.Text += "作者： " + Bookitem.author + "\n";
            t.Text += "条码号： " + Bookitem.barcode + "\n";
            t.Text += "借阅时间： " + Bookitem.render_date + "\n";
            t.Text += "馆藏地： " + Bookitem.place + "\n";
            t.Text += "续借次数： " + Bookitem.renew_time + "\n";
            //t.Tapped += ReNewTap;
        }
        public async void ReNewTap(object sender, TappedRoutedEventArgs e)
        {
            TextBlock temp = e.OriginalSource as TextBlock;
            app.User.Renew(temp.Text);


        }
        public async void LibPageChange()
        {
            Task<string> trt = FileSystem.Read("LibList");
            string rs = await trt;//re=read result;
            string res;
            int flag = 1;//1表示弹窗报错
            Task<string> result;
            if ((!string.IsNullOrEmpty(rs)) && (rs != "error"))
            {
                res = rs;
                flag = 0;
                result = app.User.Library(flag);
            }
            else
            {
                result = app.User.Library(flag);
                res = await result;
            }
            if (res != "error")
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(res));
                try
                {
                    Library myLibrary = JsonParse.Parse<Library>(ms);
                    LibSummary.Text = "";
                    for (int i = 0; i < myLibrary.content.GetLength(0); i++)
                    {
                        LibItems.Items.Add(TextBlockCreate(myLibrary.content[i]));
                    }
                    if (myLibrary.content.GetLength(0)==0)
                        LibSummary.Text = "你没有借阅图书哦~";
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                LibSummary.Text = "不好意思，出了一点问题。。。";
            }
            res = await result;
            if (res != "error")
            {
                FileSystem.Write("LibList", res);
            }
        }
        public LibPage()
        {
            this.InitializeComponent();
            LibPageChange();
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

        public TextBlock TextBlockCreate(SearchBookResult searchBookResult)
        {
            TextBlock t = new TextBlock();
            TextBlockChange(t, searchBookResult);
            t.Margin = new Thickness(20, 0, 0, 0);
            t.FontSize = 20;
            t.TextWrapping = TextWrapping.WrapWholeWords;
            return t;
        }
        public void TextBlockChange(TextBlock t, SearchBookResult searchBookResult)
        {
            /*
            "index": "H33-44/12",
            "all": "2",
            "name": "德福考试高分突破真题集",
            "author": "(德) 德福考试院编",
            "publish": "外语教学与研究出版社2011",
            "type": "中文图书",
            "left": "1"
            */
            t.Text += "书名： " + searchBookResult.name + "\n";
            t.Text += "索引号： " + searchBookResult.index + "\n";
            t.Text += "作者： " + searchBookResult.author + "\n";
            t.Text += "类型： " + searchBookResult.type + "\n";
            t.Text += "出版社： " + searchBookResult.publish + "\n";
            t.Text += "可借/总共： " + searchBookResult.left + "/" + searchBookResult.all + "\n";
        }
        private async void BookSearch_Click(object sender, RoutedEventArgs e)
        {
            Task<string> searchres = app.User.Search(SearchInfo.Text);
            string res = await searchres;
            if (res!="error")
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(res));
                try
                {
                    SearchItems.Items.Clear();
                    Search mySearch = JsonParse.Parse<Search>(ms);
                    for (int i = 1; i < mySearch.content.GetLength(0); i++)
                    {
                     
                        SearchItems.Items.Add(TextBlockCreate(mySearch.content[i]));
                    }
                }
                catch (Exception ee)
                {
                }
            }
            else
            {
                //SearchResult.Text = "不好意思，出了一点问题。。。";
            }
        }

    }
}
