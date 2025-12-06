rmdir publish\AIMP-Discord-Presence-2 /q /s
del publish\aimp_DiscordPresence2.zip

dotnet build AIMP-Discord-Presence-2 -r win-x86 -f net48 -o publish\AIMP-Discord-Presence-2

powershell Compress-Archive publish\AIMP-Discord-Presence-2\* publish\aimp_DiscordPresence2.zip

rmdir publish\AIMP-Discord-Presence-2 /q /s
