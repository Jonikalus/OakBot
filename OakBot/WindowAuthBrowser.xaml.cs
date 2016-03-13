using mshtml;
using System;
using System.Windows;

namespace OakBot
{
    /// <summary>
    /// Interaktionslogik für WindowAuthBrowser.xaml
    /// </summary>
    public partial class WindowAuthBrowser : Window
    {
        private bool _isStreamer;

        //Twitch Auth Link Streamer scope
        private static string twitchAuthLinkStreamer = string.Format("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri=http://localhost&scope=user_read+user_blocks_edit+user_blocks_read+user_follows_edit+channel_read+channel_editor+channel_commercial+channel_stream+channel_subscriptions+user_subscriptions+channel_check_subscription+chat_login", Config.TwitchClientID);

        //Twitch Auth Link Bot scope
        private static string twitchAuthLinkBot = string.Format("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={0}&redirect_uri=http://localhost&scope=chat_login", Config.TwitchClientID);

        public WindowAuthBrowser(bool isStreamer)
        {
            _isStreamer = isStreamer;
            InitializeComponent();

            Utils.clearIECache();
            Utils.InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
            Utils.SuppressWininetBehavior();

            if (_isStreamer)
            {
                wbTwitchAuth.Navigate(twitchAuthLinkStreamer);
            }
            else
            {
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
                    Config.StreamerOAuthKey = Utils.GetTwitchAuthToken(e.Uri.AbsoluteUri);
                }
                else
                {
                    Config.BotOAuthKey = Utils.GetTwitchAuthToken(e.Uri.AbsoluteUri);
                }
                this.Close();
            }
        }

        private void wbTwitchAuth_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            Utils.clearIECache();
            MessageBox.Show(e.Url.AbsoluteUri);
            if (e.Url.Host.Trim() == "localhost")
            {
                if (_isStreamer)
                {
                    Config.StreamerOAuthKey = "oauth:" + Utils.GetTwitchAuthToken(e.Url.AbsoluteUri);
                }
                else
                {
                    Config.BotOAuthKey = "oauth:" + Utils.GetTwitchAuthToken(e.Url.AbsoluteUri);
                }
                this.Close();
            }
        }

        private void wbTwitchAuth_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Utils.HideScriptErrors(wbTwitchAuth, true);
        }

        private void wbTwitchAuth_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            HTMLDocument doc = (HTMLDocument)wbTwitchAuth.Document;
            object tb = doc.getElementById("username");
            if (tb != null)
            {
                if (tb is IHTMLInputElement)
                {
                    if (_isStreamer)
                    {
                        ((IHTMLInputElement)tb).value = Config.StreamerUsername;
                    }
                    else
                    {
                        ((IHTMLInputElement)tb).value = Config.BotUsername;
                    }
                }
                else
                {
                    MessageBox.Show("Not a box!");
                }
            }
            else
            {
                MessageBox.Show("No field!");
            }
        }
    }
}