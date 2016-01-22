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

namespace OakBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public delegate void MyDel();
        public delegate void delegateMessage(TwitchChatMessage message);

        // Chat connections
        public TwitchChatConnection streamerChatConnection;
        public TwitchWhisperConnection streamerWhisperConnection;
        public TwitchChatConnection botChatConnection;
        public TwitchWhisperConnection botWhisperConnection;

        // Collections
        public ObservableCollection<TwitchChatMessage> colChatMessages;
        public ObservableCollection<TwitchUser> colViewers;
        public ObservableCollection<TwitchUser> viewerDatabase;
        public ObservableCollection<WindowViewerChat> colChatWindows;

        // Sync locks for Collections
        private object _lockChat = new object();
        private object _lockViewers = new object();
        private object _lockDatabase = new object();

        // Streamer and Bot account info
        private TwitchCredentials accountStreamer;
        private TwitchCredentials accountBot;

        // Threads
        private Thread streamerChat, botChat, streamerWhisper, botWhisper;


        public MainWindow()
        {
            InitializeComponent();

            // Create Config if not present
            Config.CreateDatabaseIfNotExist();

            // Initialize config
            Config.GetConfigFromDb();

            // Set usernames
            textBoxStreamerName.Text = Config.StreamerUsername;
            textBoxBotName.Text = Config.BotUsername;

            // Initiaze Collections and enable sync between threads
            colChatWindows = new ObservableCollection<WindowViewerChat>();
            colChatMessages = new ObservableCollection<TwitchChatMessage>();
            colChatMessages.CollectionChanged += colChatMessages_Changed; // Add message event hook
            colViewers = new ObservableCollection<TwitchUser>();
            viewerDatabase = new ObservableCollection<TwitchUser>();
            BindingOperations.EnableCollectionSynchronization(colChatMessages, _lockChat);
            BindingOperations.EnableCollectionSynchronization(colViewers, _lockViewers);
            BindingOperations.EnableCollectionSynchronization(viewerDatabase, _lockDatabase);

            // Link listViews with collections
            listViewChat.ItemsSource = colChatMessages;
            listViewViewers.ItemsSource = colViewers;
            lvViewerDatabase.ItemsSource = viewerDatabase;

            

            if (!(Config.BotOAuthKey == "please change" || Config.BotUsername == "please change" || Config.StreamerOAuthKey == "please change" || Config.StreamerUsername == "please change"))
            {
                LoadBot();
            }
            else
            {
                MessageBox.Show("Excuse me!\nSorry for interrupting the start, but to use this bot with all of it's functions, you have to connect a streamer and a bot account to it.");
            }

            // Delete IE Browser Stuff...
            Utils.clearIECache();

        }

        public void LoadBot()
        {
            try
            {
                streamerChat.Abort();
                botChat.Abort();
                //streamerWhisper.Abort();
                //botWhisper.Abort();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {

            }

            accountStreamer = new TwitchCredentials(Config.StreamerUsername, Config.StreamerOAuthKey);
            accountBot = new TwitchCredentials(Config.BotUsername, Config.BotOAuthKey);

            // Start connection for the streamer account, login and join its channel.
            streamerChatConnection = new TwitchChatConnection(accountStreamer, this);
            streamerChatConnection.JoinChannel(accountStreamer.username);
            //streamerWhisperConnection = new TwitchWhisperConnection(credentialStreamer, this);

            // Start connection for the bot account, login and join streamers channel.
            botChatConnection = new TwitchChatConnection(accountBot, this, false);
            botChatConnection.JoinChannel(accountStreamer.username);
            //botWhisperConnection = new TwitchWhisperConnection(credentialBot, this);

            // New thread for the chat connections
            streamerChat = new Thread(new ThreadStart(streamerChatConnection.Run)) { IsBackground = true };
            botChat = new Thread(new ThreadStart(botChatConnection.Run)) { IsBackground = true };
            //streamerWhisper = new Thread(new ThreadStart(streamerWhisperConnection.Run)) { IsBackground = true };
            //botWhisper = new Thread(new ThreadStart(botWhisperConnection.Run)) { IsBackground = true };

            // Start the threads
            streamerChat.Start();
            botChat.Start();
            //streamerWhisper.Start();
            //botWhisper.Start();
        }

        public void ResolveDispatchToUI(TwitchChatMessage chatMessage)
        {
            switch (chatMessage.command)
            {
                //case "353": // Received list of joined names
                //    string[] names = chatMessage.message.Split(' ');
                //    foreach (string name in names)
                //    {
                //        var viewerExist = viewerDatabase.Where(
                //            TwitchUser => TwitchUser.username == chatMessage.author);
                //        if (viewerExist.Any()) // Viewer exists
                //        {
                //            foreach (TwitchUser viewer in viewerExist)
                //            {
                //                colViewers.Add(viewer);
                //            }
                //        }
                //        else // new viewer
                //        {
                //            TwitchUser newViewer = new TwitchUser(chatMessage.author);
                //            viewerDatabase.Add(newViewer);
                //            colViewers.Add(newViewer);
                //        }
                //    }
                //    break;
                //
                case "JOIN": // Person joined channel
                    var viewerJoin = colViewers.Where(TwitchUser =>
                        TwitchUser.username == chatMessage.author);

                    if (!viewerJoin.Any())
                    {
                        var viewerExists = viewerDatabase.Where(TwitchUser =>
                            TwitchUser.username == chatMessage.author);

                        // If viewer exists add a refference to that
                        // in the colViewers.
                        if (viewerExists.Any())
                        {
                            foreach (TwitchUser viewer in viewerExists)
                            {
                                colViewers.Add(viewer);
                            }
                        }
                        // If viewer does not exists create new Viewer
                        // and add to that refference to colViewers.
                        else
                        {
                            TwitchUser newViewer = new TwitchUser(chatMessage.author);
                            viewerDatabase.Add(newViewer);
                            colViewers.Add(newViewer);
                        }
                    }

                    break;
                
                case "PART": // Person left channel
                    var viewerPart = colViewers.Where(TwitchUser =>
                        TwitchUser.username == chatMessage.author);

                    foreach (TwitchUser viewer in viewerPart)
                    {
                        colViewers.Remove(viewer);
                    }

                    break;

                case "PRIVMSG":
                    colChatMessages.Add(chatMessage);
                    break;

                case "WHISPER":
                    colChatMessages.Add(chatMessage);
                    break;

                default:
                    Trace.WriteLine(chatMessage.receivedLine);
                    break;
            }
        }

        private void AddChatMessage(string author, string message)
        {
            // TODO: add FIFO chat limited as well here  
            colChatMessages.Add(new TwitchChatMessage(author, message));
        }

        private void colChatMessages_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (TwitchChatMessage addedMessage in e.NewItems)
                {
                    var result = colChatWindows.Where(WindowUserChat => WindowUserChat.viewer.username == addedMessage.author);
                    if (result.Any())
                    {
                        foreach (WindowViewerChat window in result)
                        {
                            window.AddViewerMessage(addedMessage);
                        }
                    }
                }
            }
        }

        #region Settings EventHandlers

        private void buttonStreamerConnect_Click(object sender, RoutedEventArgs e)
        {
            Config.StreamerUsername = textBoxStreamerName.Text;
            WindowAuthBrowser tab = new WindowAuthBrowser(true);
            tab.Show();
        }

        private void buttonBotConnect_Click(object sender, RoutedEventArgs e)
        {
            Config.BotUsername = textBoxBotName.Text;
            WindowAuthBrowser tab = new WindowAuthBrowser(false);
            tab.Show();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            WindowImportData windowImport = new WindowImportData(this);
            windowImport.Show();
        }

        private void btnBotConnect_Click(object sender, RoutedEventArgs e)
        {
            LoadBot();
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
                    AddChatMessage("SYSTEM", string.Format("Speaking as {0}.", accountStreamer.username));
                }
                else if (SpeakAs.SelectedIndex == 1) // bot
                {
                    AddChatMessage("SYSTEM", string.Format("Speaking as {0}.", accountBot.username));
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
                            streamerWhisperConnection.SendWhisper(ChatSend.Text);
                        }
                        else
                        {
                            // Append this message to colChat in order
                            // to let the streamer see their own messages send.
                            AddChatMessage(accountStreamer.username, ChatSend.Text);

                            // Send message
                            streamerChatConnection.SendChatMessage(ChatSend.Text);
                        }
                    }
                    else if (SpeakAs.SelectedIndex == 1) // Bot
                    {
                        if (ChatSend.Text.StartsWith("/w"))
                        {
                            botWhisperConnection.SendWhisper(ChatSend.Text);
                        }
                        else
                        {
                            // No need to append this message to the colChat,
                            // as the streamers account will receive this message.
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
                // for testing purposes create TwitchUser here
                TwitchChatMessage selectedMessage = (TwitchChatMessage)listViewChat.SelectedItem;
                TwitchUser messageAuthor = new TwitchUser(selectedMessage.author);

                if (messageAuthor.username != "SYSTEM")
                {
                    // Create new userChat window, add to collection and show
                    // If it already exists try to bring it to the foreground
                    var result = colChatWindows.Where(WindowUserChat => WindowUserChat.viewer.username == messageAuthor.username);
                    if (result.Any() == false)
                    {
                        WindowViewerChat userChat = new WindowViewerChat(this, messageAuthor);
                        colChatWindows.Add(userChat);
                        userChat.Show();
                    }
                    else
                    {
                        foreach (WindowViewerChat chat in result)
                        {
                            chat.Activate();
                        }
                    }
                }
            }
        }

        #endregion

        #region Global EventHandlers

        private void OakBot_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Config.SaveConfigToDb();
        }

        #endregion

    }
}