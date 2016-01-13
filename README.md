# OakBot
Twitch Bot focused on integration with HTML overlays and Discord written in C#

Goal is a free open solid feature rich Twitch Bot for everyone.

Stream overlay Goals:
  - Outputting of events in textfiles
  - Outputting of events as json
      - Think of currentsong, giveaways, timers ect.
      - This would remove the need for Twitch alerts client.

Twitch Dashboard Goals:
  - Set stream game and title.
  - View session followers, subs and donators
  - View hosts and raids
  - Auto host

Twitch Chat Interface Goals:
  - Right-Mouse-Button context menu:
      - Twitch Profile
      - Compose Twitch Message
      - Purge (/timeout 1)
      - Timeout 5m
      - Timeout 10m
      - Ban
  - Double-click message (chatty like)
      - Opens User in a new window
      - Shows users chat messages only
      - Shows Twitch status
      - Same options as context menu

Command System Goals:
  - Simple text commands
  - Advanced command creation
  - Command chaining
  - Output to chat selectable by streamer or bot account

Song Request Goals:
  - load from YouTube playlist
  - Not capped

Further Goals:
  - Remote OBS integration
  - Point system
  - Importing data from Deepbot and Ankhbot
  - Backup (automated) database to folder/Box/Drive/Dropbox (outside %appdata%)

Other possibilities at a later stage:
  - Alerts
  - Discord integration
  - Let the bot listen to its own channel
  - API to pull out data
