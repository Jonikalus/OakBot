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
using System.Net;
using Microsoft.Win32;
using RestSharp;
using OakBot.Models;
using OakBot.Clients;
using System.Threading.Tasks;

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

        // API Client
        TwitchAuthenticatedClient client;

        // Collections
        public static ObservableCollection<TwitchChatMessage> colChatMessages = new ObservableCollection<TwitchChatMessage>();
        public static ObservableCollection<Viewer> colViewers = new ObservableCollection<Viewer>();
        public static ObservableCollection<Viewer> colDatabase = new ObservableCollection<Viewer>();
        public static ObservableCollection<WindowViewerChat> colChatWindows = new ObservableCollection<WindowViewerChat>();
        public static ObservableCollection<UserCommand> colBotCommands = new ObservableCollection<UserCommand>();
        public static ObservableCollection<Quote> colQuotes = new ObservableCollection<Quote>();
        public static ObservableCollection<Song> colSongs = new ObservableCollection<Song>();

        private object _lockChat = new object();
        private object _lockViewers = new object(); 
        private object _lockDatabase = new object();
        private object _lockSongs = new object();

        private ICollectionView databaseView;

        // Threads
        private Thread streamerChat;
        private Thread botChat;

        // Song Info
        public static bool playState = false;
        public static int indexSong = -1;

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
            DatabaseUtils.LoadAllViewers();
            DatabaseUtils.LoadAllQuotes();

            

            // Enable sync between threads
            BindingOperations.EnableCollectionSynchronization(colChatMessages, _lockChat);
            BindingOperations.EnableCollectionSynchronization(colViewers, _lockViewers);
            BindingOperations.EnableCollectionSynchronization(colDatabase, _lockDatabase);
            BindingOperations.EnableCollectionSynchronization(colSongs, _lockSongs);

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

            lvSongs.ItemsSource = colSongs;

            // Testing Commands 
            colBotCommands.Add(new UserCommand("!test", "Test received!", 30, 0, true));
            colBotCommands.Add(new UserCommand(":yatb", "Yet Another Twitch Bot.", 30, 60, true));
            colBotCommands.Add(new UserCommand("!who", "You are @user@", 0, 0, true));
            colBotCommands.Add(new UserCommand("!block", "@block@ Hello thur!", 0, 0, true));
            colBotCommands.Add(new UserCommand("!followdate", "@user@, you are following since @followdate@.", 0, 0, true));
            colBotCommands.Add(new UserCommand("!followdatetime", "@user@, you are following since @followdatetime@", 0, 0, true));
            colBotCommands.Add(new UserCommand("!vartest", "@var1@ m8", 0, 0, true));
            colBotCommands.Add(new UserCommand("!songrequest", "The song @songrequest@ by @user@ has been requested!", 0, 0, true));
            colBotCommands.Add(new UserCommand("!requestsong", "The song @songrequest@ by @user@ has been requested!", 0, 0, true));
            colBotCommands.Add(new UserCommand("!song", "Currently playing: @song@!", 0, 0, true));
            colBotCommands.Add(new UserCommand("Giveaway", "Just stop...", 0, 0, true, true));
            colBotCommands.Add(new UserCommand("!slap", "@user@ slaps @target@ so hard, he bursts into pieces!", 0, 0, true));
                        
            colSongs.Add(new Song("https://www.youtube.com/watch?v=VEAy700YGuU"));
            colSongs.Add(new Song("https://soundcloud.com/aivisura/steven-universe-strong-in-the-real-way-rebecca-sugar"));

            // BackgroundTask Thread
            BackgroundTasks bg = new BackgroundTasks(60, 120);
            new Thread(new ThreadStart(bg.Run)) { IsBackground = true }.Start();

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

            // Twitch Credentials
            accountStreamer = new TwitchCredentials(Config.StreamerUsername, Config.StreamerOAuthKey);

            // Start Streamer connection and login
            streamerChatConnection = new TwitchChatConnection(accountStreamer, false);
            streamerChatConnection.JoinChannel(Config.ChannelName);

            // Create threads for the chat connections
            streamerChat = new Thread(new ThreadStart(streamerChatConnection.Run)) { IsBackground = true };

            // Start the chat connection threads
            streamerChat.Start();

            // TODO check on success login/connection
            if (true)
            {
                // Disable Settings UI elements
                textBoxStreamerName.IsEnabled = false;
                buttonStreamerConnect.IsEnabled = false;
                cbAutoConnectStreamer.IsEnabled = false;
                btnStreamerConnect.Content = "Disconnect";

                // Enable Twitch Dashboard tab
                tabMainDashboard.IsEnabled = true;

                try
                {
                    client = new TwitchAuthenticatedClient(Config.StreamerOAuthKey, Config.TwitchClientID);
                    txtTitle.Text = Utils.GetClient().GetMyChannel().Status;
                    cbGame.Text = Utils.GetClient().GetMyChannel().Game;
                    tbStreamDelay.Text = Utils.GetClient().GetMyChannel().Delay.ToString();
                    
                    // Get Streamers Avatar
                    using (WebClient wc = new WebClient())
                    {
                        BitmapImage logo = new BitmapImage();
                        logo.BeginInit();
                        logo.StreamSource = wc.OpenRead(client.GetMyChannel().Logo);
                        logo.CacheOption = BitmapCacheOption.OnLoad;
                        logo.EndInit();
                        imgLogo.Source = logo;
                    }

                    // Enable partnered elements when partnered
                    if (client.GetMyUser().Partnered)
                    {
                        // Stream delay
                        tbStreamDelay.IsEnabled = true;
                        
                        // Manual Commercials
                        gbManualCommercials.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

            }
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

        private void lvSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lvSongs.SelectedIndex != -1)
            {
                Song selectedSong = (Song)lvSongs.SelectedItem;
                cefSong.Load(selectedSong.Link);
                playState = true;
                indexSong = lvSongs.SelectedIndex;
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Song first = (Song)lvSongs.Items[0];
                cefSong.Load(first.Link);
                playState = true;
                indexSong = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("No songs in the playlist!");
                playState = false;
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cefSong.Load("javascript:var mv = document.getElementById('movie_player'); mv.setVolume(" + e.NewValue + "); ");
        }

        private void btnPlayerCtl_Click(object sender, RoutedEventArgs e)
        {
            if (playState)
            {
                // Pause
                cefSong.Load("javascript:var mv = document.getElementById('movie_player'); mv.pauseVideo();");
                btnPlayerCtl.Content = "Play";
                
            }
            else
            {
                // Play
                cefSong.Load("javascript:var mv = document.getElementById('movie_player'); mv.playVideo();");
                btnPlayerCtl.Content = "Pause";
            }
            playState = !playState;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(indexSong.ToString());
            if(indexSong == 0)
            {
                indexSong = colSongs.Count - 1;
            }else
            {
                indexSong--;
            }
            Song play = colSongs[indexSong];
            cefSong.Load(play.Link);
            MessageBox.Show(indexSong.ToString());
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(indexSong.ToString());
            if (indexSong == colSongs.Count - 1)
            {
                indexSong = 0;
            }else
            {
                indexSong++;
            }
            Song play = colSongs[indexSong];
            cefSong.Load(play.Link);
            MessageBox.Show(indexSong.ToString());
        }

        #region Manual Commercial

        private void btn30Sec_Click(object sender, RoutedEventArgs e)
        {
            RunManualCommercial(30);
        }

        private void btn60Sec_Click(object sender, RoutedEventArgs e)
        {
            RunManualCommercial(60);
        }

        private void btn90Secs_Click(object sender, RoutedEventArgs e)
        {
            RunManualCommercial(90);
        }

        private void btn120Secs_Click(object sender, RoutedEventArgs e)
        {
            RunManualCommercial(120);
        }

        private void btn150Secs_Click(object sender, RoutedEventArgs e)
        {
            RunManualCommercial(150);
        }

        private void btn180Secs_Click(object sender, RoutedEventArgs e)
        {
            RunManualCommercial(180);
        }

        private async void RunManualCommercial(int length)
        {
            int activationDelay;

            // Get given delay (0 up to and including 60) from textbox.
            // On error, default to 20 and show warning of the actual start.
            try
            {
                activationDelay = Convert.ToInt32(tbManComDelay.Text);
                if (activationDelay < 0 || activationDelay > 60)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                activationDelay = 20;
                MessageBox.Show("Invalid delay given of 0 up to and including 60.\nA commercial will be requested in 20 seconds after pressing OK.",
                    "OakBot Manual Commercial", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Create a new task with a sleep of the given delay
            // Await the task to retrieve exceptions
            try
            {
                Task activateCommercial = new Task(() =>
                {
                    Thread.Sleep(activationDelay * 1000);
                    TwitchResponse r = Utils.GetClient().TriggerCommercial(length);
                    if (r.Status == 422)
                    {
                        MessageBox.Show("Failed to start the commercial:\n" + r.Message,
                            "OakBot Manual Commercial", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(string.Format("The {0} second commercial has started.", length),
                            "OakBot Manual Commercial", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                });

                activateCommercial.Start();
                await activateCommercial;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong starting the commercial:\n" + ex.ToString(),
                    "OakBot Manual Commercial", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (client.GetMyUser().Partnered)
            {
                int streamDelay;

                try
                {
                    streamDelay = Convert.ToInt32(tbStreamDelay.Text);
                    if (streamDelay < 0 || streamDelay > 900)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception)
                {
                    streamDelay = 0;
                    MessageBox.Show("Invalid delay given of 0 up to and including 900.\nDelay is will be reverted to 0 seconds.",
                        "OakBot Channel Update", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //client.Update(txtTitle.Text, cbGame.Text, Convert.ToString(streamDelay));
                client.Update(txtTitle.Text, cbGame.Text, streamDelay);
            }
            else
            {
                client.Update(txtTitle.Text, cbGame.Text);
            }

            MessageBox.Show("Channel information updated!",
                "Oakbot Channel Update", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

    }
}