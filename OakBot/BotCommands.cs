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
                string[] splitMessage = Regex.Split(message.Message, @"\s+");

                if (splitMessage.Count() == 1)
                {
                    // Only !quote is given
                    Random rnd = new Random((int)DateTime.Now.Ticks);
                    int i = rnd.Next(MainWindow.colQuotes.Count);
                    Quote q = MainWindow.colQuotes[i];

                    string response = string.Format("Quote #{0}: \"{1}\" - {2} {3} {4}", i, 
                        q.QuoteString, q.Quoter,
                        q.DisplayDate ? "[" + q.DateString + "]" : "",
                        q.DisplayGame ? "while playing " + q.Game : "");

                    // Send Response
                    MainWindow.instance.botChatConnection.SendChatMessage(response);

                    // Add to colChatMessages so user can see response of the bot
                    MainWindow.colChatMessages.Add(new TwitchChatMessage(
                        MainWindow.instance.accountBot.UserName, response));
  
                }

            }
             
            
            
            
            //case "quote":
            //                        if (split.Count() == 1)
            //{
            //
            //}
            //else if (split.Count() == 2)
            //{
            //    try
            //    {
            //        int i = int.Parse(split[1]);
            //        Quote q = MainWindow.colQuotes[i];
            //        return string.Format("Quote #{0}: \"{1}\" - {2}{3}{4}", i, q.QuoteString, q.Quoter, q.DisplayGame ? " [" + q.Game + "] " : " ", q.DateString);
            //    }
            //    catch (Exception)
            //    {
            //
            //        return "There is no quote with that number!";
            //    }
            //}
            //else
            //{
            //    return "";
            //}

        }


    }
}
