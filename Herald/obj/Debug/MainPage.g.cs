﻿

#pragma checksum "C:\Users\Qian\Documents\Visual Studio 2015\Projects\Herald\Herald\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1FEA73491C027B2F68F1BF3063607CCA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Herald
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 20 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Update_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 21 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Logout_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 22 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Words_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 40 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Curricculum_Button_Tapped;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 41 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.GPA_Button_Tapped;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 42 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.SRTP_Button_Tapped;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 43 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Lib_Button_Tapped;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 44 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Lecture_Button_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


