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
        private bool _isStreamer;

        //Twitch Auth Link Streamer scope
        private static string twitchAuthLinkStreamer = string.Format("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri=http://localhost&scope=user_read+user_blocks_edit+user_blocks_read+user_follows_edit+channel_read+channel_editor+channel_commercial+channel_stream+channel_subscriptions+user_subscriptions+channel_check_subscription+chat_login", Config.twitchClientID);

        //Twitch Auth Link Bot scope
        private static string twitchAuthLinkBot = string.Format("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri=http://localhost&scope=chat_login", Config.twitchClientID);

        

        public WindowAuthBrowser(bool isStreamer)
        {
            InitializeComponent();          
            _isStreamer = isStreamer;

            if (_isStreamer)
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
                if (_isStreamer)
                {
                    Config.StreamerOAuthKey = "oauth:" + Utils.getAuthTokenFromUrl(e.Uri.AbsoluteUri);
                }
                else
                {
                    Config.BotOAuthKey = "oauth:" + Utils.getAuthTokenFromUrl(e.Uri.AbsoluteUri);
                }
                Config.SaveConfigToDb();
                this.Close();
            }
        }
    }
}
