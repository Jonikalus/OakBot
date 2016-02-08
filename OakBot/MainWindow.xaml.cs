using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Globalization;

// http://stackoverflow.com/questions/2505492/wpf-update-binding-in-a-background-thread

namespace OakBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        // Instance of itself
        public static MainWindow instance;

        // Streamer and Bot account info
        public TwitchCredentials accountStreamer;
        public TwitchCredentials accountBot;

        // Chat connections
        public TwitchChatConnection botChatConnection;
        public TwitchChatConnection streamerChatConnection;

        // Collections
        public static ObservableCollection<TwitchChatMessage> colChatMessages = new ObservableCollection<TwitchChatMessage>();
        public static ObservableCollection<Viewer> colViewers = new ObservableCollection<Viewer>();
        public static ObservableCollection<Viewer> colDatabase = new ObservableCollection<Viewer>();
        public static ObservableCollection<WindowViewerChat> colChatWindows = new ObservableCollection<WindowViewerChat>();
        public static ObservableCollection<BotCommand> colBotCommands = new ObservableCollection<BotCommand>();
        public static ObservableCollection<Quote> colQuotes = new ObservableCollection<Quote>();

        private object _lockChat = new object();
        private object _lockViewers = new object(); 
        private object _lockDatabase = new object();

        private ICollectionView databaseView;

        // Threads
        private Thread streamerChat;
        private Thread botChat;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // Initialize instance
            instance = this;

            this.DataContext = this;

            if (!Directory.Exists(Config.AppDataPath)) Directory.CreateDirectory(Config.AppDataPath);
            if (!Directory.Exists(Config.AppDataPath + "\\Webserver")) Directory.CreateDirectory(Config.AppDataPath + "\\Webserver");

            // Initialize config
            Config.GetConfigFromDb();
            LoadConfigToUI();
            ViewerDB.LoadAllViewers();

            // Enable sync between threads
            BindingOperations.EnableCollectionSynchronization(colChatMessages, _lockChat);
            BindingOperations.EnableCollectionSynchronization(colViewers, _lockViewers);
            BindingOperations.EnableCollectionSynchronization(colDatabase, _lockDatabase);

            // Create Event for collection changed
            colChatMessages.CollectionChanged += colChatMessages_Changed;

            // Link listViews with collections
            listViewChat.ItemsSource = colChatMessages;
            listViewViewers.ItemsSource = colViewers;

            // Database listView with filter
            lvViewerDatabase.ItemsSource = colDatabase;
            databaseView = CollectionViewSource.GetDefaultView(lvViewerDatabase.ItemsSource);
            databaseView.Filter = DatabaseFilter;
            lblFilterCnt.Content = databaseView.Cast<Viewer>().Count();

            lvCommands.ItemsSource = colBotCommands;
            lvQuotes.ItemsSource = colQuotes;

            // Testing Commands 
            colBotCommands.Add(new BotCommand("!test", "Test received!", 30, 0, true));
            colBotCommands.Add(new BotCommand(":yatb", "Yet Another Twitch Bot.", 30, 60, true));
            colBotCommands.Add(new BotCommand("!who", "You are @user@", 0, 0, true));
            colBotCommands.Add(new BotCommand("!block", "@block@ Hello thur!", 0, 0, true));
            colBotCommands.Add(new BotCommand("!followdate", "@user@, you are following since @followdate@.", 0, 0, true));
            colBotCommands.Add(new BotCommand("!followdatetime", "@user@, you are following since @followdatetime@", 0, 0, true));
            colBotCommands.Add(new BotCommand("!vartest", "@var1@ m8", 0, 0, true));

            // Testing Quotes
            colQuotes.Add(new Quote(1, "Hello world!", "Ocgineer", "Trove", false, "Ocgineer"));
            colQuotes.Add(new Quote(2, "Hi! I'm tEM!", "TGR", "Undertale", true, "Legendary_Studios"));
            colQuotes.Add(new Quote(3, "You can walk over water if you run fast enough.", "Ocgineer", "Trove", true, "Lucanus"));

            // Auto connect
            if (Config.AutoConnectBot)
            {
                ConnectBot();

                // Can only connect if Bot is connected
                if (Config.AutoConnectStreamer)
                {
                    ConnectStreamer();
                }
            }

            
        }

        #endregion

        #region Methods

        private void ConnectBot()
        {
            try
            {
                botChat.Abort();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {

            }

            // Disable UI elements
            textBoxBotName.IsEnabled = false;
            buttonBotConnect.IsEnabled = false;
            tbChannelName.IsEnabled = false;
            cbServerIP.IsEnabled = false;
            cbServerPort.IsEnabled = false;
            cbAutoConnectBot.IsEnabled = false;
            btnBotConnect.Content = "Disconnect";

            // Twitch Credentials
            accountBot = new TwitchCredentials(Config.BotUsername, Config.BotOAuthKey);

            // Start Bot connection and login
            botChatConnection = new TwitchChatConnection(accountBot);
            botChatConnection.JoinChannel(Config.ChannelName);

            // Create threads for the chat connections
            botChat = new Thread(new ThreadStart(botChatConnection.Run)) { IsBackground = true };

            // Start the chat connection threads
            botChat.Start();
        }

        public void DisconnectBot()
        {
            try
            {
                botChat.Abort();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {

            }

            // Enable UI elements
            textBoxBotName.IsEnabled = true;
            buttonBotConnect.IsEnabled = true;
            tbChannelName.IsEnabled = true;
            cbServerIP.IsEnabled = true;
            cbServerPort.IsEnabled = true;
            cbAutoConnectBot.IsEnabled = true;
            btnBotConnect.Content = "Connect";
        }

        private void ConnectStreamer()
        {
            try
            {
                streamerChat.Abort();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {

            }

            // Disable UI elements
            textBoxStreamerName.IsEnabled = false;
            buttonStreamerConnect.IsEnabled = false;
            cbAutoConnectStreamer.IsEnabled = false;
            btnStreamerConnect.Content = "Disconnect";

            // Twitch Credentials
            accountStreamer = new TwitchCredentials(Config.StreamerUsername, Config.StreamerOAuthKey);

            // Start Streamer connection and login
            streamerChatConnection = new TwitchChatConnection(accountStreamer, false);
            streamerChatConnection.JoinChannel(Config.ChannelName);

            // Create threads for the chat connections
            streamerChat = new Thread(new ThreadStart(streamerChatConnection.Run)) { IsBackground = true };

            // Start the chat connection threads
            streamerChat.Start();
        }

        public void DisconnectStreamer()
        {
            try
            {
                streamerChat.Abort();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {

            }

            // Enable UI elements
            textBoxStreamerName.IsEnabled = true;
            buttonStreamerConnect.IsEnabled = true;
            cbAutoConnectStreamer.IsEnabled = true;
            btnStreamerConnect.Content = "Connect";
        }

        public void LoadConfigToUI()
        {
            textBoxBotName.Text = Config.BotUsername;
            tbChannelName.Text = Config.ChannelName;
            cbAutoConnectBot.IsChecked = Config.AutoConnectBot;
            textBoxStreamerName.Text = Config.StreamerUsername;
            cbAutoConnectBot.IsChecked = Config.AutoConnectStreamer;
            cbAutoConnectStreamer.IsChecked = Config.AutoConnectStreamer;
        }

        #endregion

        #region Settings EventHandlers

        #region BotConnect

        private void textBoxBotName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxBotName.Text))
            {
                MessageBox.Show("Please enter the bot Twitch username.",
                    "Bot Twitch Username", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Config.BotUsername = textBoxBotName.Text.Trim().ToLower();
            }
        }

        private void buttonBotConnect_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxBotName.Text == "notSet" || string.IsNullOrWhiteSpace(textBoxBotName.Text))
            {
                MessageBox.Show("Please enter the bot Twitch username first before trying to connect with Twitch.",
                    "Bot Twitch Connect", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                WindowAuthBrowser tab = new WindowAuthBrowser(false);
                tab.Show();
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void comboBox_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void cbAutoConnectBot_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAutoConnectBot.IsChecked == true)
            {
                Config.AutoConnectBot = true;
            }
            else
            {
                Config.AutoConnectBot = false;
            }
        }

        private void cbAutoConnectBot_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbAutoConnectBot.IsChecked == false)
            {
                Config.AutoConnectBot = false;
            }
            else
            {
                Config.AutoConnectBot = true;
            }
        }

        private void btnBotConnect_Click(object sender, RoutedEventArgs e)
        {
            if(btnBotConnect.Content.ToString() == "Connect")
            {
                ConnectBot();
            }
            else
            {
                DisconnectBot();
            }
        }

        #endregion

        #region Streamer Connect

        private void textBoxStreamerName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxStreamerName.Text))
            {
                MessageBox.Show("Please enter the streamer Twitch username if you want to use the features that require a connected streamer account.",
                    "Streamer Twitch Username", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Config.StreamerUsername = textBoxStreamerName.Text.Trim().ToLower();
            }
        }

        private void buttonStreamerConnect_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxStreamerName.Text == "notSet" || string.IsNullOrWhiteSpace(textBoxStreamerName.Text))
            {
                MessageBox.Show("Please enter the streamer Twitch username first before trying to connect with Twitch if you want to use features that require a connected streamer account.",
                    "Streamer Twitch Connect", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                WindowAuthBrowser tab = new WindowAuthBrowser(true);
                tab.Show();
            }
        }

        private void cbAutoConnectStreamer_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAutoConnectStreamer.IsChecked == true)
            {
                Config.AutoConnectStreamer = true;
            }
            else
            {
                Config.AutoConnectStreamer = false;
            }
        }

        private void cbAutoConnectStreamer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbAutoConnectStreamer.IsChecked == false)
            {
                Config.AutoConnectStreamer = false;
            }
            else
            {
                Config.AutoConnectStreamer = true;
            }
        }

        private void btnStreamerConnect_Click(object sender, RoutedEventArgs e)
        {
            if (btnBotConnect.Content.ToString() != "Connect")
            {
                if (btnStreamerConnect.Content.ToString() == "Connect")
                {
                    ConnectStreamer();
                }
                else
                {
                    DisconnectStreamer();
                }
            }
            else
            {
                MessageBox.Show("You need to connect the bot account first before you can connect the streamer account.\n" +
                    "If you only want to use the bot functionality and not the Twitch dashboard login with your account as bot.",
                    "Streamer Chat Connect", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            WindowImportData windowImport = new WindowImportData();
            windowImport.ShowDialog();
        }

        #endregion

        #region Dashboard EventHandlers


        #endregion

        #region Twitch Chat

        private void SpeakAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountStreamer != null)
            {
                if (SpeakAs.SelectedIndex == 0) // streamer
                {
                    colChatMessages.Add(new TwitchChatMessage("OakBot", string.Format("Speaking as {0}.", accountStreamer.UserName)));
                }
                else if (SpeakAs.SelectedIndex == 1) // bot
                {
                    colChatMessages.Add(new TwitchChatMessage("OakBot", string.Format("Speaking as {0}.", accountBot.UserName)));
                }
            }
        }

        private void ChatSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Get the line and 
                if (!string.IsNullOrWhiteSpace(ChatSend.Text))
                {
                    // Speak as Streamer or Bot
                    if (SpeakAs.SelectedIndex == 0) // streamer
                    {
                        if (ChatSend.Text.StartsWith("/w"))
                        {
                            //streamerWhisperConnection.SendWhisper(ChatSend.Text);
                        }
                        else
                        {
                            // No need to append this message to the colChat,
                            // as the bot (primary) account will receive this message.
                            streamerChatConnection.SendChatMessage(ChatSend.Text);
                        }
                    }
                    else if (SpeakAs.SelectedIndex == 1) // Bot
                    {
                        if (ChatSend.Text.StartsWith("/w"))
                        {
                            //botWhisperConnection.SendWhisper(ChatSend.Text);
                        }
                        else
                        {
                            // Append this message to colChat in order
                            // to let the streamer see their own messages send.
                            colChatMessages.Add(new TwitchChatMessage(accountBot.UserName, ChatSend.Text));
                            botChatConnection.SendChatMessage(ChatSend.Text);
                        }
                    }
                }

                // Clear the chat input
                ChatSend.Clear();
            }
        }

        private void listViewChat_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewChat.SelectedIndex != -1)
            {
                // Get the selected TwitchChatMessage object
                TwitchChatMessage selectedMessage = (TwitchChatMessage)listViewChat.SelectedItem;

                // Get viewer's Viewer object to attach to the new chat window
                // This also prevents chat opening of "OakBot" system messages
                // Creating new Viewer objects is handled by TwitchChatConnection on time
                var isInDatabase = colDatabase.FirstOrDefault(x => x.UserName == selectedMessage.Author);
                if (isInDatabase != null)
                {
                    // Check if the child chat window is open already
                    var isChatOpen = colChatWindows.FirstOrDefault(x => x.Viewer.UserName == isInDatabase.UserName);
                    if (isChatOpen != null)
                    {
                        isChatOpen.Activate();
                    }
                    else
                    {
                        WindowViewerChat userChat = new WindowViewerChat(this, isInDatabase);
                        colChatWindows.Add(userChat);
                        userChat.Show();
                    }
                }
            }
        }

        private void btnViewerAddPoints_Click(object sender, RoutedEventArgs e)
        {
            foreach (Viewer viewer in colViewers)
            {
                viewer.Points += 10;
            }
        }

        #endregion

        #region Database

        private void tbFilterOnName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(databaseView != null)
            {
                databaseView.Refresh();
                lblFilterCnt.Content = databaseView.Cast<Viewer>().Count();
            }
        }

        private bool DatabaseFilter(object item)
        {
            Viewer viewer = item as Viewer;
            return viewer.UserName.Contains(tbFilterOnName.Text);
        }

        #endregion

        #region Global EventHandlers

        private void colChatMessages_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (TwitchChatMessage addedMessage in e.NewItems)
                {
                    // Method 1
                    var chatWindow = colChatWindows.FirstOrDefault(x => x.Viewer.UserName == addedMessage.Author);
                    if (chatWindow != null)
                    {
                        chatWindow.AddViewerMessage(addedMessage);
                    }

                    // Method 2
                    //var openWindow = colChatWindows.Where(x => x.viewer.username == addedMessage.author);
                    //foreach (WindowViewerChat window in openWindow)
                    //{
                    //    window.AddViewerMessage(addedMessage);
                    //}

                    // Method 3
                    //colChatWindows.Where(x => x.viewer.username == addedMessage.author).ToList().ForEach(
                    //    y => y.AddViewerMessage(addedMessage));
                }
            }
        }

        private void OakBot_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Utils.StartWebserver();
        }

        private void lvViewerDatabase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvViewerDatabase.SelectedIndex != -1)
            {
                // Get the selected Viewer object
                Viewer selectedViewer = (Viewer)lvViewerDatabase.SelectedItem;

                // Check if the child chat window is open already
                // No need for validation as the Viewer is opened directly from the database collection
                var isChatOpen = colChatWindows.FirstOrDefault(x => x.Viewer.UserName == selectedViewer.UserName);
                if (isChatOpen != null)
                {
                    isChatOpen.Activate();
                }
                else
                {
                    WindowViewerChat userChat = new WindowViewerChat(this, selectedViewer);
                    colChatWindows.Add(userChat);
                    userChat.Show();
                }
            }
        }

        private void btnDatabaseCleanup_Click(object sender, RoutedEventArgs e)
        {
            WindowDatabaseCleanup windowCleanup = new WindowDatabaseCleanup();
            windowCleanup.ShowDialog();
        }

        // This doesn't work as it doesnt contain an INotify
        // These should be properties later on in a MVVM.
        // Now the filter count is updated after refresh of the view.
        //public int TotalDatabaseCount
        //{
        //    get
        //    {
        //        return colDatabase.Count();
        //    }
        //}
        //
        //public int FilterDatabaseCount
        //{
        //    get
        //    {
        //        return databaseView.Cast<Viewer>().Count();
        //    }
        //}

    }
}