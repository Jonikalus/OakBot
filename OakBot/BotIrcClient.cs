using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace OakBot
{
    public class BotIrcClient
    {
        #region Fields

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        private int _throttle;
        private DateTime _lastSend;

        private TwitchCredentials _loggedinAs;

        #endregion Fields

        #region Constructors

        public BotIrcClient(string ip, int port, TwitchCredentials user)
        {
            // Set vars
            _throttle = 1550;
            _lastSend = DateTime.MinValue;
            _loggedinAs = user;

            tcpClient = new TcpClient(ip, port);

            // TODO: connection check > ASYNC TIMEOUT RETURN ERROR

            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            // TODO: make "login" method in TwitchChatConnections to split error handling

            WriteLine("PASS oauth:" + user.OAuth);
            WriteLine("NICK " + user.UserName);

            // TODO: successfull login check > in TwitchChatConnection

            // Twitch doesnt require to set username/domain/realname
            //WriteLine("USER " + user.username + " 8 * :" + user.username);
        }

        #endregion Constructors

        #region Methods

        public void WriteLineThrottle(string message)
        {
            double timeSpan = (DateTime.Now - _lastSend).TotalMilliseconds;
            if (timeSpan < _throttle)
            {
                Thread.Sleep(Convert.ToInt32(_throttle - timeSpan));
            }
            _lastSend = DateTime.Now;

            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void WriteLine(string message)
        {
            _lastSend = DateTime.Now;

            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public string ReadLine()
        {
            try
            {
                return inputStream.ReadLine();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Returns the user that is logged in on
        /// the current IRC connection.
        /// </summary>
        public TwitchCredentials loggedinAs
        {
            get
            {
                return _loggedinAs;
            }
        }

        /// <summary>
        /// Set throttle for Write to IRC chat in ms.
        /// 1550 for non-moderators (20 messages / 30 sec)
        /// 350 for moderators (100 messages / 30 sec)
        /// Includes 50ms margin on both.
        /// </summary>
        public int throttle
        {
            get
            {
                return _throttle;
            }

            set
            {
                _throttle = value;
            }
        }

        #endregion Properties
    }
}