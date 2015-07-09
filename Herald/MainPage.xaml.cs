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
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public App app = Application.Current as App;
        public async void PersonalInfoChange()
        {
            string wifires,peres=null,cardres;
            Task<string> PEres ;
            Task<string> WIFIres;
            Task<string> Cardres;
            double delta = (DateTime.Now - app.current).TotalSeconds;          
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //card
            if (localSettings.Values.ContainsKey("CARD"))
            {
                cardres = localSettings.Values["CARD"].ToString();
                CardMoney.Visibility = Visibility.Visible;
            }
            else
            {
                CardMoney.Visibility = Visibility.Collapsed;
                Cardres = app.User.Card(0);
                cardres = await Cardres;
                if (cardres != "error")
                {
                    CardMoney.Visibility = Visibility.Visible;
                }
            }
            //wifi
            if (localSettings.Values.ContainsKey("WIFI"))
            {
                wifires = localSettings.Values["WIFI"].ToString();
                WIFI.Visibility = Visibility.Visible;
            }
            else
            {          
                WIFI.Visibility = Visibility.Collapsed;
                WIFIres = app.User.NIC(0);
                wifires = await WIFIres;
                if (wifires != "error")
                {
                    WIFI.Visibility = Visibility.Visible;
                }
            }
            //pe
            if (localSettings.Values.ContainsKey("PE"))
            {
                peres = localSettings.Values["PE"].ToString();
                Run.Visibility = Visibility.Visible;           
            }
            else
            {
                Run.Visibility = Visibility.Collapsed;
                PEres = app.User.PE(0);
                peres = await PEres;
                if (peres != "error")
                {
                    Run.Visibility = Visibility.Visible;
                }          
            }
            //card
            if (cardres != "error")
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(cardres));
                try
                {
                    Card CardInfo = JsonParse.Parse<Card>(ms);
                    CardMoney.Text = "一卡通还剩" + CardInfo.content.left + "元";
                }
                catch (Exception e) { }
            }
            else
            {
                CardMoney.Text = "出问题惹~_~";
            }
            if (delta > 600)
            {
                Cardres = app.User.Card(0);
                cardres = await Cardres;
                app.current = DateTime.Now;
            }
            if (cardres != "error")
                localSettings.Values["CARD"] = cardres;            
            //wifi
            if (wifires != "error")
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(wifires));
                try
                {
                    NIC NICInfo = JsonParse.Parse<NIC>(ms);
                    if (NICInfo.content.web.state == "未开通")
                        SEUNet.Visibility = Visibility.Collapsed;
                    else
                        WIFI.Text = "SEU-WLAN已使用" + NICInfo.content.web.used+"\n电子帐户还剩"+NICInfo.content.left;

                }
                catch (Exception e) { }
            }
            else
            {
                WIFI.Text = "出问题惹~_~";
            } 
            if (delta>600)
            {
                WIFIres = app.User.NIC(0);
                wifires = await WIFIres;
                app.current = DateTime.Now;
            }
            if (wifires != "error")
                localSettings.Values["WIFI"] = wifires;            
            //pe
            if (peres != "error")
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(peres));
                try
                {
                    pe PEInfo = JsonParse.Parse<pe>(ms);
                    int curTimes = Convert.ToInt32(PEInfo.content);
                    if (curTimes < 10) { Run.Text = "你已经跑了" + PEInfo.content + "次啦"; }
                    else if (curTimes < 22) { Run.Text = "你已经跑了" + PEInfo.content + "次啦，就快跑完一半了"; }
                    else if (curTimes < 33) { Run.Text = "你已经跑了" + PEInfo.content + "次啦，跑完一半了~"; }
                    else if (curTimes < 40) { Run.Text = "你已经跑了" + PEInfo.content + "次啦，加油加油^_^"; }
                    else if (curTimes < 45) { Run.Text = "你已经跑了" + PEInfo.content + "次啦，还差一点点不要放弃治疗~"; }
                    else { Run.Text = "你已经完成45次跑操啦，现在已经跑了" + PEInfo.content + "次"; }
                }
                catch (Exception e) { }
            }
            else
            {
                Run.Text = "出问题惹~_~";
            }
            if (delta>600)
            {
                PEres = app.User.PE(0);
                peres = await PEres;
            }
            if (peres != "error")
                localSettings.Values["PE"] = peres;
            Waiting.IsActive = false;
        }      
        DispatcherTimer dt = null;
        int timesToTick = 2;
        bool IsExit = false;
        public MainPage()
        {
            this.InitializeComponent();
            Waiting.IsActive = true;
            PersonalInfoChange();
            
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;


        }
        async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
          {
              e.Handled = true; // We've handled this button press
              if (!IsExit)
              {
                  //ExitSB.Begin(); 
                  IsExit = true;
                   if(dt == null)
                  {
                    dt = new DispatcherTimer();
                  }                
                  dt.Interval = TimeSpan.FromSeconds(2);
                  dt.Tick += timer_Tick;
                  dt.Start();
              }
              else
              {
                  App.Current.Exit(); 
              } 
          }
        void timer_Tick(object sender, object e)
        {
            timesToTick--;
            if (timesToTick == 0)
            {
                timesToTick = 2;
                dt.Tick -= timer_Tick;
                dt.Stop();
                IsExit = false;
            }
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
            while (Frame.BackStackDepth > 0)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UpdateInfo));
        }
        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            int res=await app.User.Deauth();
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
                FileSystem.Delete("GPAList");
                FileSystem.Delete("CurriculumList");
                FileSystem.Delete("SRTPList");
                FileSystem.Delete("LibList");
                FileSystem.Delete("LectureList");
            Frame.Navigate(typeof(UserLogin));
        }

        private void Curricculum_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(StuCurriculum));
        }

        private void GPA_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(GPAPage));
        }

        private void SRTP_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SRTPPage));
        }

        private void Lib_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(LibPage));
        }

        private void Lecture_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(LecturePage));
        }

        private void Words_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Words));
        }

    }
}
