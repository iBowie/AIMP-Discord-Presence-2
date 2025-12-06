rmdir publish\AIMP-Discord-Presence-2 /q /s
del publish\aimp_DiscordPresence2.zip

dotnet build AIMP-Discord-Presence-2 -r win-x86 -f net48 -o publish\aimp_DiscordPresence2

powershell Compress-Archive publish\aimp_DiscordPresence2 publish\aimp_DiscordPresence2.zip

rmdir publish\aimp_DiscordPresence2 /q /s
