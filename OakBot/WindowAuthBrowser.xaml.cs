using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace OakBot
{
    /// <summary>
    /// Interaktionslogik für TwitchAuthBrowser.xaml
    /// </summary>
    public partial class WindowAuthBrowser : Window
    {
        public WindowAuthBrowser(string link)
        {
            InitializeComponent();
            wbTwitchAuth.Navigate(link);
        }

        private void wbTwitchAuth_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri.Host.Trim() == "localhost")
            {
                Utils.getAuthTokenFromUrl(e.Uri.AbsoluteUri);
            }
        }
    }
}
