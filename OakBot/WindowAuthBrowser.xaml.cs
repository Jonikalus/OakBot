using System.Windows;
using System.Web;
using System;

namespace OakBot
{
    /// <summary>
    /// Interaktionslogik für WindowAuthBrowser.xaml
    /// </summary>
    public partial class WindowAuthBrowser : Window
    {
        public bool isStreamer;

        // Twitch Application
        private static string twitchClientID = "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
        private static string twitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";

        //Twitch Auth Link Streamer scope
        private static string twitchAuthLinkStreamer = string.Format("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri=http://localhost&scope=user_read+user_blocks_edit+user_blocks_read+user_follows_edit+channel_read+channel_editor+channel_commercial+channel_stream+channel_subscriptions+user_subscriptions+channel_check_subscription+chat_login", twitchClientID);

        //Twitch Auth Link Bot scope
        private static string twitchAuthLinkBot = string.Format("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri=http://localhost&scope=chat_login", twitchClientID);

        

        public WindowAuthBrowser(bool isStreamer)
        {
            InitializeComponent();
            
            this.isStreamer = isStreamer;
            if (isStreamer)
            {

                Utils.InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
                Utils.SuppressWininetBehavior();
                wbTwitchAuth.Navigate(twitchAuthLinkStreamer);
            }
            else
            {
                Utils.InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
                Utils.SuppressWininetBehavior();
                wbTwitchAuth.Navigate(twitchAuthLinkBot);
            }
        }

        private void wbTwitchAuth_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            Utils.clearIECache();
            if (e.Uri.Host.Trim() == "localhost")
            {
                if (isStreamer)
                {
                    Config.StreamerOAuthKey = Utils.getAuthTokenFromUrl(e.Uri.AbsoluteUri);
                }else
                {
                    Config.BotOAuthKey = Utils.getAuthTokenFromUrl(e.Uri.AbsoluteUri);
                }
                Config.SaveConfigToDb();
                this.Close();
            }
        }
    }
}
