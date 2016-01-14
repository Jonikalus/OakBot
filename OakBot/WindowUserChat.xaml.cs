using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for ChatUserDetails.xaml
    /// </summary>
    public partial class WindowUserChat : Window
    {
        private MainWindow mW;
        private TwitchUser user;
        private ObservableCollection<TwitchChatMessage> colChat;

        public WindowUserChat(MainWindow window, TwitchUser viewer)
        {
            mW = window;
            user = viewer;
            InitializeComponent();

            // Set other information
            this.Title = "Chat: " + user.displayName;
            this.lblDisplayName.Content = user.displayName;

            // copy list
            colChat = new ObservableCollection<TwitchChatMessage>(mW.colChat);

            // Set item source for the listView
            listViewChat.ItemsSource = colChat;

            // Filter out only the messages of the user
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewChat.ItemsSource);
            view.Filter = item =>
            {
                TwitchUser filter = item as TwitchUser;
                if (filter == null)
                {
                    return false;
                }
                else
                {
                    return filter.username.Contains(user.username);
                }
            };

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
