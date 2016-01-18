using System.Windows;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for ChatUserDetails.xaml
    /// </summary>
    public partial class WindowViewerChat : Window
    {
        #region Fields

        private MainWindow _mW;
        private TwitchUser _viewer;
        private ObservableCollection<TwitchChatMessage> colViewerMessages;

        #endregion

        #region Constructors

        public WindowViewerChat(MainWindow mW, TwitchUser viewer)
        {
            _mW = mW;
            _viewer = viewer;
            InitializeComponent();

            // Set information
            this.Title = "Chat: " + _viewer.displayName;
            this.lblDisplayName.Content = _viewer.displayName;

            // Rather than copying all messages just collect the selected viewers
            // messages to save system resources in case of huge global chat history.
            colViewerMessages = new ObservableCollection<TwitchChatMessage>();
            var viewerMessages = mW.colChatMessages.Where(
                TwitchChatMessage => TwitchChatMessage.author == viewer.username);
            foreach(TwitchChatMessage message in viewerMessages)
            {
                colViewerMessages.Add(message);
            }

            // Set item source for the listView and apply filter
            listViewChat.ItemsSource = colViewerMessages;
        }

        #endregion

        #region Methods

        public void AddViewerMessage(TwitchChatMessage message)
        {
            colViewerMessages.Add(message);
        }

        #endregion

        #region EventHandlers

        private void btnPurge_Click(object sender, RoutedEventArgs e)
        {
            _mW.streamerChatConnection.SendChatMessage(string.Format("/timeout {0} 1", _viewer.username));
        }

        private void btnTimeout5_Click(object sender, RoutedEventArgs e)
        {
            _mW.streamerChatConnection.SendChatMessage(string.Format("/timeout {0} 300", _viewer.username));
        }

        private void btnTimeout10_Click(object sender, RoutedEventArgs e)
        {
            _mW.streamerChatConnection.SendChatMessage(string.Format("/timeout {0}", _viewer.username));
        }

        private void btnBan_Click(object sender, RoutedEventArgs e)
        {
            _mW.streamerChatConnection.SendChatMessage(string.Format("/ban {0}", _viewer.username));
        }

        private void btnUnban_Click(object sender, RoutedEventArgs e)
        {
            _mW.streamerChatConnection.SendChatMessage(string.Format("/unban {0}", _viewer.username)); 
        }

        private void btnTwitchProfile_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://www.twitch.tv/{0}/profile", _viewer.username));
        }

        private void btnTwitchCompose_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://www.twitch.tv/message/compose?to={0}", _viewer.username));
        }

        #endregion

        #region Properties

        public TwitchUser viewer
        {
            get
            {
                return _viewer;
            }
        }

        #endregion
    }
}
