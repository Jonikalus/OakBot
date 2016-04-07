using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OakBot
{
    public static class GroupMinigame
    {
        public static string Command { get; set; }
        public static double Cooldown { get; set; }
        public static double StartDelay { get; set; }
        public static int MaxAmount { get; set; }
        public static int MinimumUsers { get; set; }
        public static MGRank MinimumPermission { get; set; }
        public static int ViewerChance { get; set; }
        public static int RegularChance { get; set; }
        public static int SubscriberChance { get; set; }
        public static int ModeratorChance { get; set; }
        public static int ViewerPayout { get; set; }
        public static int RegularPayout { get; set; }
        public static int SubscriberPayout { get; set; }
        public static int ModeratorPayout { get; set; }
        public static string FirstEntryMessage { get; set; }
        public static string OnCooldownMessage { get; set; }
        public static string OffCooldownMessage { get; set; }
        public static string OnStartMessage { get; set; }
        public static string OnFailedMessage { get; set; }
        public static string ResultMessage { get; set; }
        public static string OnSoloWin { get; set; }
        public static string OnSoloFail { get; set; }
        public static string On100Win { get; set; }
        public static string On75To99Win { get; set; }
        public static string On25To74Win { get; set; }
        public static string On1To24Win { get; set; }
        public static string OnFail { get; set; }

        private static bool gameInProgress = false;

        public static List<Viewer> enteredViewers = new List<Viewer>();

        public static Timer minigameTimer = new Timer();
        public static Timer cooldownTimer = new Timer();

        public static TwitchChatConnection botConn = MainWindow.instance.botChatConnection;

        public static void Initialize()
        {
            Command = "!raid";
            Cooldown = 10;
            StartDelay = 10;
            MinimumUsers = 1;
            FirstEntryMessage = "$user$ has started a raid!";
            OffCooldownMessage = "Raid is no longer on CD";
            OnCooldownMessage = "Raid is on CD";
            OnFailedMessage = "Not enough ppl";
            OnStartMessage = "Raid starts, it's on CD now";
            minigameTimer.AutoReset = false;
            minigameTimer.Enabled = true;
            minigameTimer.Interval = StartDelay * 1000;
            minigameTimer.Elapsed += MinigameTimer_Elapsed;
            cooldownTimer.Elapsed += CooldownTimer_Elapsed;
            cooldownTimer.Interval = Cooldown * 1000;
            cooldownTimer.AutoReset = false;
            cooldownTimer.Enabled = true;
            botConn.ChatMessageReceived += BotChatConnection_ChatMessageReceived;
        }

        private static void CooldownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            gameInProgress = false;
            botConn.SendChatMessage(OffCooldownMessage);
        }

        private static void BotChatConnection_ChatMessageReceived(object o, Args.ChatMessageReceivedEventArgs e)
        {
            if(e.Message.Message == Command)
            {
                StartRaidGame();
                enteredViewers.Add(new Viewer(e.Message.Author));
            }
        }

        public static void StartRaidGame()
        {
            if (!gameInProgress)
            {
                minigameTimer.Start();
                gameInProgress = true;
                botConn.SendChatMessage(FirstEntryMessage);
            }else
            {
                botConn.SendChatMessage(OnCooldownMessage);
            }
        }

        private static void MinigameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(enteredViewers.Count < MinimumUsers)
            {
                botConn.SendChatMessage(OnFailedMessage);
            }else
            {
                botConn.SendChatMessage(OnStartMessage);
                cooldownTimer.Start();
            }
        }

        public enum MGRank
        {
            EVERYONE,
            REGULAR,
            SUBSCRIBER
        }

    }
}
