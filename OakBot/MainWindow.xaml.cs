using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
        // Twitch Application
        public string twitchClientID = "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
        public string twitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";

        //public delegate void MyDel();
        public delegate void DelUI(DispatchUI obj);

        private TwitchChatConnection botChatConnection;
        private TwitchChatConnection streamerChatConnection;

        public MainWindow()
        {
            InitializeComponent();

            // Twitch user instances
            TwitchUser userStreamer = new TwitchUser("<streamer user name>");
            TwitchUser userBot = new TwitchUser("<bot user name>");
            
            // Attach oAuth password to the users creating an TwitchUserCredentials object
            // Twitch IRC oAuth password required. Obtain one from https://twitchapps.com/tmi/
            TwitchCredentials credentialBot = new TwitchCredentials(userBot, "<streamer oauth key>");
            TwitchCredentials credentialStreamer = new TwitchCredentials(userStreamer, "<bot oauth key>");

            // Start connection for the streamer account, login and join its channel.
            streamerChatConnection = new TwitchChatConnection(credentialStreamer, this);
            streamerChatConnection.JoinChannel(userStreamer);

            // Start connection for the bot account, login and join streamers channel.
            botChatConnection = new TwitchChatConnection(credentialBot, this, true);
            botChatConnection.JoinChannel(userStreamer);

            // New thread for the chat connections
            new Thread(new ThreadStart(streamerChatConnection.Run)) { IsBackground = true }.Start();
            new Thread(new ThreadStart(botChatConnection.Run)) { IsBackground = true }.Start();
        }

        public void ResolveDispatchToUI(DispatchUI dispatchedObj)
        {
            string time = "[" + DateTime.Now.ToShortTimeString() + "] ";

            switch (dispatchedObj.botEvent.command)
            {
                case "353": // Received list of joined names
                    string[] names = dispatchedObj.botEvent.message.Split(' ');
                    foreach (string name in names)
                    {
                        ChatViewers.AppendText(name, Brushes.WhiteSmoke);
                        ChatViewers.Document.ContentEnd.InsertLineBreak();
                    }
                    break;

                case "JOIN": // Person joined channel
                    ChatViewers.AppendText(dispatchedObj.botEvent.author, Brushes.Yellow);
                    ChatViewers.Document.ContentEnd.InsertLineBreak();
                    break;

                case "PART": // Person left channel
                    ChatViewers.AppendText(dispatchedObj.botEvent.author, Brushes.Red);
                    ChatViewers.Document.ContentEnd.InsertLineBreak();
                    break;

                case "PRIVMSG":
                    ChatReceived.AppendText(time + dispatchedObj.botEvent.author + ": " +
                        dispatchedObj.botEvent.message, Brushes.WhiteSmoke);
                    ChatReceived.Document.ContentEnd.InsertLineBreak();
                    break;

                default:
                    ChatReceived.AppendText(dispatchedObj.botEvent.line, Brushes.Red);
                    ChatReceived.Document.ContentEnd.InsertLineBreak();
                    break;
            }
        }

        private void SpeakAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatReceived != null)
            {
                // Get the Selected ComboBoxItem
                var typeItem = SpeakAs.SelectedItem as ComboBoxItem;

                // Add speaking as text to the ChatReceived colored.
                ChatReceived.AppendText("Now speaking as: " + typeItem.Content.ToString(),
                    Brushes.Aquamarine);
                ChatReceived.Document.ContentEnd.InsertLineBreak();
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
                        Trace.WriteLine(ChatSend.Text);
                        streamerChatConnection.SendChatMessage(ChatSend.Text);
                    }
                    else if (SpeakAs.SelectedIndex == 1) // Bot
                    {
                        Trace.WriteLine(ChatSend.Text);
                        botChatConnection.SendChatMessage(ChatSend.Text);
                    }
                }

                // Clear the chat input
                ChatSend.Clear();
            }
        }

        private void ChatReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChatReceived.ScrollToEnd();
        }
    }
}