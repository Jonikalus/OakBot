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
        public static int Cooldown { get; set; }
        public static int StartDelay { get; set; }
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

        public static List<Viewer> enteredViewers = new List<Viewer>();

        public static Timer minigameTimer = new Timer();

        public enum MGRank
        {
            EVERYONE,
            REGULAR,
            SUBSCRIBER
        }

    }
}
