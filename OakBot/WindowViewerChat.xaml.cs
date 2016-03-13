using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for ChatUserDetails.xaml
    /// </summary>
    public partial class WindowViewerChat : Window
    {
        #region Private Fields

        private object colLock = new object();
        private ObservableCollection<IrcMessage> colViewerMessages;
        private Viewer viewer;
        private MainWindow window;

        #endregion Private Fields

        #region Public Constructors

        public WindowViewerChat(MainWindow window, Viewer viewer)
        {
            // Set fields
            this.window = window;
            this.viewer = viewer;

            // Init Window and set datacontext to this
            // for databinding to the attached Viewer
            InitializeComponent();
            DataContext = this;

            // Init IrcMessage collection and enable sync between threads
            colViewerMessages = new ObservableCollection<IrcMessage>();
            BindingOperations.EnableCollectionSynchronization(colViewerMessages, colLock);

            // Rather than copying all messages just collect the selected viewers
            // messages to save system resources in case of huge global chat history.
            var viewerMessages = MainWindow.colChatMessages.Where(
                TwitchChatMessage => TwitchChatMessage.Author == viewer.UserName);
            foreach (IrcMessage message in viewerMessages)
            {
                colViewerMessages.Add(message);
            }

            // Set item source for the listView and apply filter
            listViewChat.ItemsSource = colViewerMessages;

            // TODO quick and dirty isFollowing / isSub test
            cbFollowing.IsChecked = viewer.isFollowing();
            cbSubscribed.IsChecked = viewer.isSubscribed();
            using (WebClient wc = new WebClient())
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.StreamSource = wc.OpenRead(Utils.GetClient().GetMyChannel().Logo);
                logo.CacheOption = BitmapCacheOption.OnLoad;
                logo.EndInit();
                image.Source = logo;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public Viewer Viewer
        {
            get
            {
                return viewer;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void AddViewerMessage(IrcMessage message)
        {
            colViewerMessages.Add(message);
        }

        #endregion Public Methods

        #region Private Methods

        private void btnBan_Click(object sender, RoutedEventArgs e)
        {
            window.streamerChatConnection.SendChatMessage(string.Format("/ban {0}", viewer.UserName));
        }

        private void btnPurge_Click(object sender, RoutedEventArgs e)
        {
            window.streamerChatConnection.SendChatMessage(string.Format("/timeout {0} 1", viewer.UserName));
        }

        private void btnTimeout10_Click(object sender, RoutedEventArgs e)
        {
            window.streamerChatConnection.SendChatMessage(string.Format("/timeout {0}", viewer.UserName));
        }

        private void btnTimeout5_Click(object sender, RoutedEventArgs e)
        {
            window.streamerChatConnection.SendChatMessage(string.Format("/timeout {0} 300", viewer.UserName));
        }

        private void btnTwitchCompose_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://www.twitch.tv/message/compose?to={0}", viewer.UserName));
        }

        private void btnTwitchProfile_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://www.twitch.tv/{0}/profile", viewer.UserName));
        }

        private void btnUnban_Click(object sender, RoutedEventArgs e)
        {
            window.streamerChatConnection.SendChatMessage(string.Format("/unban {0}", viewer.UserName));
        }

        private void windowViewerChat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.colChatWindows.Remove(this);
        }

        #endregion Private Methods
    }
}