using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot
{
    public class BotCommands
    {

        public static void RunBotCommand(string command, TwitchChatMessage message)
        {
            // !quote > display random quote
            // !quote # > display quote of given id
            // !quote add quote - quoter > adds quote
            // !quote remove # > removes quote by given id
            // If given Id is int and not found, display not exists
            // If given Id is not int, dont display
            if (command == "!quote")
            {
                string[] splitMessage = message.Message.Split(new char[] { ' ' }, 3);

                if (splitMessage.Count() >= 2 && splitMessage[1].ToLower() == "add")
                {
                    try
                    {
                        // Split quote and quoter on -
                        string[] splitEntry = splitMessage[2].Split('-');

                        // Create new quote with game that the streamer on channel is/was playing
                        Quote newQuote = new Quote(splitEntry[0].Trim(), splitEntry[1].Trim(),
                            Utils.GetClient().GetMyChannel().Game);

                        // Add new quote to collection
                        App.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            MainWindow.colQuotes.Add(newQuote);
                        }));

                        // Save to database
                        DatabaseUtils.AddQuote(newQuote);

                        // Send response
                        SendAndShowMessage(string.Format("Quote has been added with ID of: {0}", newQuote.Id));

                    }
                    catch (Exception)
                    {
                        SendAndShowMessage("To add a quote use: !quote add <quote> - <quoter> No need to use \" as it will be added on display.");
                    }
                }
                else if (splitMessage.Count() >= 2 && splitMessage[1].ToLower() == "remove")
                {
                    try
                    {
                        int idToRemove = int.Parse(splitMessage[2]);

                        if (idToRemove < MainWindow.colQuotes.Count())
                        {
                            // Remove quote from collection
                            App.Current.Dispatcher.BeginInvoke(new Action(delegate
                            {
                                MainWindow.colQuotes.RemoveAt(idToRemove);
                            }));

                            // Update whole database file (dynamic id)
                            DatabaseUtils.SaveAllQuotes();

                            // Send response
                            SendAndShowMessage("Quote removed with id: " + splitMessage[2]);
                        }
                        else
                        {
                            // Send response
                            SendAndShowMessage("The quote with the given id does not exist.");
                        }
                    }
                    catch
                    {
                        SendAndShowMessage("Given id is not a valid id number");
                    }
                }
                else
                {

                    Quote q;

                    try
                    {
                        // Try to get quote from given id
                        q = MainWindow.colQuotes[int.Parse(splitMessage[1])];
                    }
                    catch
                    {
                        // Get a random quote if arg is not parsable or out of range
                        Random rnd = new Random((int)DateTime.Now.Ticks);
                        q = MainWindow.colQuotes[rnd.Next(0, MainWindow.colQuotes.Count)];
                    }

                    // Send response
                    SendAndShowMessage(string.Format("Quote #{0}: \"{1}\" - {2} {3} {4}",
                        q.Id,
                        q.QuoteString, q.Quoter,
                        q.DisplayDate ? "[" + q.DateString + "]" : "",
                        q.DisplayGame ? "while playing " + q.Game : "")
                    );
                }
            }
            else if (command == "!songrequest")
            {
                string[] splitMessage = message.Message.Split(new char[] { ' ' }, 2);

                if (splitMessage.Count() > 1)
                {
                    Song requestedSong = new Song(splitMessage[1]);
                    if (requestedSong.Type != SongType.INVALID)
                    {
                        // Add to colSongs
                        App.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            MainWindow.colSongs.Add(requestedSong);
                        }));

                        // Display response
                        SendAndShowMessage("The following song has been added: " + Utils.getTitleFromYouTube(splitMessage[1]));
                    }
                    else
                    {
                        // Display response
                        SendAndShowMessage("Invalid song link or id.");
                    }
                }
            }
            else if (command == "!currentsong")
            {
                if (MainWindow.playState)
                {
                    SendAndShowMessage("Current song playing: " + MainWindow.colSongs[MainWindow.indexSong].SongName);
                } 
            }
        }

        private static void SendAndShowMessage(string message)
        {
            // Send the message to IRC
            MainWindow.instance.botChatConnection.SendChatMessage(message);

            // Add message to collection
            App.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                MainWindow.colChatMessages.Add(new TwitchChatMessage(
                    MainWindow.instance.accountBot.UserName, message));
            }));
        }
    }
}
