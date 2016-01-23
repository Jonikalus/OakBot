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
        // Streamer and Bot account info
        private TwitchCredentials accountStreamer;
        private TwitchCredentials accountBot;

        // Chat connections
        public TwitchChatConnection botChatConnection;
        public TwitchChatConnection streamerChatConnection;

        // Collections and 
        public static ObservableCollection<TwitchChatMessage> colChatMessages;
        private object _lockChat = new object();
        public static ObservableCollection<TwitchUser> colViewers;
        private object _lockViewers = new object();
        public static ObservableCollection<TwitchUser> viewerDatabase;
        private object _lockDatabase = new object();
        public static ObservableCollection<WindowViewerChat> colChatWindows;

        // Threads
        private Thread streamerChat;
        private Thread botChat;

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
            cbAutoConnectBot.IsChecked = Config.AutoConnect;

            // Set Channel Name
            tbChannelName.Text = Config.ChannelName;

            // Initiaze Collections and enable sync between threads
            colChatWindows = new ObservableCollection<WindowViewerChat>();
            
            colChatMessages = new ObservableCollection<TwitchChatMessage>();
            BindingOperations.EnableCollectionSynchronization(colChatMessages, _lockChat);
            colChatMessages.CollectionChanged += colChatMessages_Changed;

            colViewers = new ObservableCollection<TwitchUser>();
            BindingOperations.EnableCollectionSynchronization(colViewers, _lockViewers);

            viewerDatabase = new ObservableCollection<TwitchUser>();
            BindingOperations.EnableCollectionSynchronization(viewerDatabase, _lockDatabase);

            // Link listViews with collections
            listViewChat.ItemsSource = colChatMessages;
            listViewViewers.ItemsSource = colViewers;
            lvViewerDatabase.ItemsSource = viewerDatabase;



            //if (!(Config.BotOAuthKey == "please change" ||
            //    Config.BotUsername == "please change" ||
            //    Config.StreamerOAuthKey == "please change" ||
            //    Config.StreamerUsername == "please change"))
            //{
            //    LoadBot();
            //}
            //else
            //{
            //    MessageBox.Show("Excuse me!\nSorry for interrupting the start, but to use this bot with all of it's functions, you have to connect a streamer and a bot account to it.");
            //}
        }

        public void LoadBot()
        {
            try
            {
                //streamerChat.Abort();
                botChat.Abort();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {

            }

            // Twitch Credentials
            accountBot = new TwitchCredentials(Config.BotUsername, Config.BotOAuthKey);
            accountStreamer = new TwitchCredentials(Config.StreamerUsername, Config.StreamerOAuthKey);
            
            // TODO: Validate credentials before trying to login the chat

            // Start Bot connection and login
            botChatConnection = new TwitchChatConnection(accountBot);
            botChatConnection.JoinChannel(Config.ChannelName);

            // Start Streamer connection and login
            streamerChatConnection = new TwitchChatConnection(accountStreamer, false);
            streamerChatConnection.JoinChannel(Config.ChannelName);

            // Create threads for the chat connections
            botChat = new Thread(new ThreadStart(botChatConnection.Run)) { IsBackground = true };
            streamerChat = new Thread(new ThreadStart(streamerChatConnection.Run)) { IsBackground = true };

            // Start the chat connection threads
            botChat.Start();
            streamerChat.Start();

        }

        #region Settings EventHandlers

        private void buttonStreamerConnect_Click(object sender, RoutedEventArgs e)
        {
            Utils.clearIECache();
            Config.StreamerUsername = textBoxStreamerName.Text;
            WindowAuthBrowser tab = new WindowAuthBrowser(true);
            tab.Show();
        }

        private void buttonBotConnect_Click(object sender, RoutedEventArgs e)
        {
            Utils.clearIECache();
            Config.BotUsername = textBoxBotName.Text;
            WindowAuthBrowser tab = new WindowAuthBrowser(false);
            tab.Show();
        }

        private void tbChannelName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbChannelName.Text))
            {
                Config.ChannelName = tbChannelName.Text;
                Config.SaveConfigToDb();
            }
        }

        private void cbAutoConnectBot_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAutoConnectBot.IsChecked == true)
            {
                Config.AutoConnect = true;
                Config.SaveConfigToDb();
            }
        }

        private void cbAutoConnectBot_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbAutoConnectBot.IsChecked == false)
            {
                Config.AutoConnect = false;
                Config.SaveConfigToDb();
            }
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
                    colChatMessages.Add(new TwitchChatMessage("OakBot", string.Format("Speaking as {0}.", accountStreamer.username)));
                }
                else if (SpeakAs.SelectedIndex == 1) // bot
                {
                    colChatMessages.Add(new TwitchChatMessage("OakBot", string.Format("Speaking as {0}.", accountBot.username)));
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
                            colChatMessages.Add(new TwitchChatMessage(accountBot.username, ChatSend.Text));
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

                // Get viewer's TwitchUser object to attach to the new chat window
                // This also prevents chat opening of "OakBot" system messages
                // Creating new TwitchUser objects is handled by TwitchChatConnection on time
                var isInDatabase = viewerDatabase.FirstOrDefault(x => x.username == selectedMessage.author);
                if (isInDatabase != null)
                {
                    // Check if the child chat window is open already
                    var isChatOpen = colChatWindows.FirstOrDefault(x => x.viewer.username == isInDatabase.username);
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
            foreach (TwitchUser viewer in colViewers)
            {
                viewer.points += 10;
            }

            // TODO: INotfiy event to update UI on changed values!!!
        }

        #endregion

        #region Global EventHandlers

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

        private void OakBot_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Config.ChannelName = tbChannelName.Text;
            Config.SaveConfigToDb();
        }


        #endregion

    }
}