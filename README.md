# OakBot
Twitch Bot focused on integration with HTML overlays and Discord written in C#

Goal is a free solid Twitch Bot mimicing features from both Deepbot and Ankhbot,
but without monthly payment (no cloud) and richer in features than Ankhbot.

Main goal would be outputting of text and json files of on-going bot activities,
such as giveaways, timers, queue that an HTML overlay can read and render on the stream.

Secondary goal is to provide an integration with Discord to synchronize roles, commands and giveaways.

Currently the bot will connect only to the streamers chat (so it does not join its own channel).
Database useage would be SQLlite and import from Deepbot and Anhkbot should be available.
