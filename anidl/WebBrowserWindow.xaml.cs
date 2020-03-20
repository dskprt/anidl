using anidl.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace anidl {
    /// <summary>
    /// Interaction logic for WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window {

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(string pchURL, string pchCookieName,
               StringBuilder pchCookieData, ref System.UInt32 pcchCookieData,
               int dwFlags, IntPtr lpReserved);

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(
                int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        public WebBrowserWindow(string url) {
            InitializeComponent();

            string agent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0";

            UrlMkSetSessionOption(0x10000001, agent, agent.Length, 0);

            Browser.Navigating += this.Browser_Navigating;
            Browser.Navigate(url, "_self", null, "");
        }

        private void Browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) {
            if(e.Uri.Host.Contains("kwik.cx") && e.Uri.Query.Contains("__cf_chl_jschl_tk__")) {
                e.Cancel = true;

                StringBuilder cookie = new StringBuilder();
                StringBuilder _cookie = new StringBuilder();
                System.UInt32 size = 256;

                InternetGetCookieEx(e.Uri.ToString(), "cf_clearance", cookie, ref size,
                    0x00002000, IntPtr.Zero);

                InternetGetCookieEx(e.Uri.ToString(), "__cfduid", _cookie, ref size,
                    0x00002000, IntPtr.Zero);

                Settings.cfCookies["kwik"] = cookie.ToString() + ";" + _cookie.ToString();
            }
        }
    }
}
