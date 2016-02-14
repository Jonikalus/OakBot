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

                if (splitMessage.Count() > 2 && splitMessage[1] == "add")
                {
                    try
                    {
                        string[] splitEntry = splitMessage[2].Split('-');
                        Quote newQuote = new Quote(splitEntry[0], splitEntry[1], "testing");

                        // Add this quote to collection and save to database
                        App.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            MainWindow.colQuotes.Add(newQuote);
                        }));

                        // Save to database
                        DatabaseUtils.AddQuote(newQuote);

                        // Display result
                        MainWindow.instance.botChatConnection.SendChatMessage(string.Format("Quote has been added with ID of: {0}", newQuote.Id));

                    }
                    catch (Exception ex)
                    {
                        MainWindow.instance.botChatConnection.SendChatMessage(ex.ToString());
                    }
                }
                else if (splitMessage.Count() > 2 && splitMessage[1] == "remove")
                {


                }
                else
                {
                    int quoteId = 0;

                    try
                    {
                        quoteId = int.Parse(splitMessage[1]);
                    }
                    catch
                    {
                        Random rnd = new Random((int)DateTime.Now.Ticks);
                        quoteId = rnd.Next(MainWindow.colQuotes.Count);
                    }

                    if (quoteId < MainWindow.colQuotes.Count())
                    {
                        Quote q = MainWindow.colQuotes[quoteId];

                        string response = string.Format("Quote #{0}: \"{1}\" - {2} {3} {4}",
                            quoteId,
                            q.QuoteString, q.Quoter,
                            q.DisplayDate ? "[" + q.DateString + "]" : "",
                            q.DisplayGame ? "while playing " + q.Game : "");

                        // Send response
                        MainWindow.instance.botChatConnection.SendChatMessage(response);

                        // Add to colChatMessages so user can see response of the bot
                        MainWindow.colChatMessages.Add(new TwitchChatMessage(
                            MainWindow.instance.accountBot.UserName, response));

                    }
                    else
                    {
                        // Send response
                        MainWindow.instance.botChatConnection.SendChatMessage("There is no quote with that id.");

                        // Add to colChatMessages so user can see the response of the bot
                        MainWindow.colChatMessages.Add(new TwitchChatMessage(MainWindow.instance.accountBot.UserName, "There is no quote with that id."));
                    }

                }
            }
        }
    }
}
