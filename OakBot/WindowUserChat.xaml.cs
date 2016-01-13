using System.Windows;
using System.Diagnostics;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for ChatUserDetails.xaml
    /// </summary>
    public partial class WindowUserChat : Window
    {
        private MainWindow mW;
        private TwitchUser user;

        public WindowUserChat(MainWindow window, TwitchUser viewer)
        {
            mW = window;
            user = viewer;
            InitializeComponent();
        }

        private void btnPurge_Click(object sender, RoutedEventArgs e)
        {
            mW.streamerChatConnection.SendChatMessage(string.Format("/timeout {0} 1", user.username));
        }

        private void btnTimeout5_Click(object sender, RoutedEventArgs e)
        {
            mW.streamerChatConnection.SendChatMessage(string.Format("/timeout {0} 300", user.username));
        }

        private void btnTimeout10_Click(object sender, RoutedEventArgs e)
        {
            mW.streamerChatConnection.SendChatMessage(string.Format("/timeout {0}", user.username));
        }

        private void btnBan_Click(object sender, RoutedEventArgs e)
        {
            mW.streamerChatConnection.SendChatMessage(string.Format("/ban {0}", user.username));
        }

        private void btnUnban_Click(object sender, RoutedEventArgs e)
        {
            mW.streamerChatConnection.SendChatMessage(string.Format("/unban {0}", user.username)); 
        }

        private void btnTwitchProfile_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://wwww.twitch.tv/{0}/profile", user.username));
        }

        private void btnTwitchCompose_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://www.twitch.tv/message/compose?to={0}", user.username));
        }
    }
}
