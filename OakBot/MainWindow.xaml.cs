using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using OakBotExtentions;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public delegate void MyDel();
        public delegate void DelUI(DispatchUI obj);

        // Chat connections
        public TwitchChatConnection streamerChatConnection;
        public TwitchWhisperConnection streamerWhisperConnection;
        public TwitchChatConnection botChatConnection;
        public TwitchWhisperConnection botWhisperConnection;

        // Ccollections
        private ObservableCollection<WindowUserChat> colChatWindows;
        public ObservableCollection<TwitchChatMessage> colChat;
        public ObservableCollection<TwitchUser> colViewers;
        public ObservableCollection<TwitchUser> userDatabase;

        // Sync locks for Collections
        private object _lockChat = new object();
        private object _lockViewers = new object();
        private object _lockDatabase = new object();

        // Main refferences to Streamer and Bot object
        private TwitchUser userStreamer;
        private TwitchUser userBot;


        public MainWindow()
        {
            InitializeComponent();

            // Initiaze Collections and enable sync between threads
            colChat = new ObservableCollection<TwitchChatMessage>();
            colViewers = new ObservableCollection<TwitchUser>();
            userDatabase = new ObservableCollection<TwitchUser>();
            BindingOperations.EnableCollectionSynchronization(colChat, _lockChat);
            BindingOperations.EnableCollectionSynchronization(colViewers, _lockViewers);
            BindingOperations.EnableCollectionSynchronization(userDatabase, _lockDatabase);

            // Link listViews with collections
            listViewChat.ItemsSource = colChat;

            // Twitch user instances
            userStreamer = new TwitchUser("<streamer user name>");
            userBot = new TwitchUser("<bot user name>");

            // Attach oAuth password to the users creating an TwitchUserCredentials object
            // Twitch IRC oAuth password required. Obtain one from https://twitchapps.com/tmi/
            TwitchCredentials credentialBot = new TwitchCredentials(userBot, "<streamer oauth key>");
            TwitchCredentials credentialStreamer = new TwitchCredentials(userStreamer, "<bot oauth key>");

            // Start connection for the streamer account, login and join its channel.
            streamerChatConnection = new TwitchChatConnection(credentialStreamer, this);
            streamerChatConnection.JoinChannel(userStreamer);
            //streamerWhisperConnection = new TwitchWhisperConnection(credentialStreamer, this);

            // Start connection for the bot account, login and join streamers channel.
            botChatConnection = new TwitchChatConnection(credentialBot, this, false);
            botChatConnection.JoinChannel(userStreamer);
            //botWhisperConnection = new TwitchWhisperConnection(credentialBot, this);

            // New thread for the chat connections
            new Thread(new ThreadStart(streamerChatConnection.Run)) { IsBackground = true }.Start();
            new Thread(new ThreadStart(botChatConnection.Run)) { IsBackground = true }.Start();
            //new Thread(new ThreadStart(streamerWhisperConnection.Run)) { IsBackground = true }.Start();
            //new Thread(new ThreadStart(botWhisperConnection.Run)) { IsBackground = true }.Start();
        }

        public void ResolveDispatchToUI(DispatchUI dispatchedObj)
        {
            switch (dispatchedObj.chatMessage.command)
            {
                //case "353": // Received list of joined names
                //    string[] names = dispatchedObj.chatMessage.message.Split(' ');
                //    foreach (string name in names)
                //    {
                //
                //    }
                //    break;
                //
                //case "JOIN": // Person joined channel
                //
                //    break;
                //
                //case "PART": // Person left channel
                //
                //    break;

                case "PRIVMSG":
                    colChat.Add(dispatchedObj.chatMessage);
                    break;

                case "WHISPER":
                    colChat.Add(dispatchedObj.chatMessage);
                    break;

                default:
                    Trace.WriteLine(dispatchedObj.chatMessage.message);
                    break;

                // Update chat collections in child windows
                // TODO
            }
        }

        private void AddChatMessage(string author, string message)
        {
            // TODO: add FIFO chat limited as well here  
            colChat.Add(new TwitchChatMessage(author, message));
        }

        private void SpeakAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userStreamer != null)
            {
                if (SpeakAs.SelectedIndex == 0) // streamer
                {
                    AddChatMessage("SYSTEM", string.Format("Speaking as {0}.", userStreamer.displayName));
                }
                else if (SpeakAs.SelectedIndex == 1) // bot
                {
                    AddChatMessage("SYSTEM", string.Format("Speaking as {0}.", userBot.displayName));
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
                            AddChatMessage(userStreamer.displayName, ChatSend.Text);

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

        private void buttonStreamerConnect_Click(object sender, RoutedEventArgs e)
        {
            WindowAuthBrowser tab = new WindowAuthBrowser(true);
            tab.Show();
        }

        private void buttonBotConnect_Click(object sender, RoutedEventArgs e)
        {
            WindowAuthBrowser tab = new WindowAuthBrowser(false);
            tab.Show();
        }

        private void listViewChat_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
             if (listViewChat.SelectedIndex != -1)
             {
                // for testing purposes create TwitchUser here
                TwitchChatMessage selectedMessage = (TwitchChatMessage)listViewChat.SelectedItem;
                TwitchUser messageAuthor = new TwitchUser(selectedMessage.author);

                if(messageAuthor.username != "SYSTEM")
                {
                    // Create new userChat window, add to collection and show
                    WindowUserChat userChat = new WindowUserChat(this, messageAuthor);
                    colChatWindows.Add(userChat);
                    userChat.Show();
                }
            }
        }
    }
}