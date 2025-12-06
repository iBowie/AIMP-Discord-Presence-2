1. Download Album Cover Gatherer.

2. Run it.

3. When prompted with `Directory >>`, input your music directory.

4. When prompted with `Output Directory >>`, input some empty directory (or directory with previous output).

5. Wait it to finish working.

6. Go on [Discord Developer Portal](https://discord.com/developers/applications).

7. Log in your Discord account.

8. Press `New Application`, input `AIMP` or something else to your liking. This will dictate what other people on Discord will see next to "Listening to".

9. Upload some icon that you will use for Rich Presence.

10. Go to Rich Presence > Art Assets.

11. Press "Add Image(s)", and select all images you have gathered by using the app.

12. Upload them without changing their name (it's how plugin will find them).

13. Wait for process to complete. After uploading it may take a while before they show up in Discord.

14. Copy `Application Id`, displayed in `General Information` tab.

15. Completely close AIMP, make sure it's not open in tray.

16. Go to data folder of the plugin (you can use a shortcut in plugin folder, which can be found in `AIMP/Plugins/aimp_DiscordPresence2`).

17. Open `config.xml` in any text editor.

18. Change `albumArtProvider` to Discord, making it look like this - `<albumArtProvider>Discord</albumArtProvider>`. **Capitalization matters!**

19. Change `<discordApplicationId>429559336982020107</discordApplicationId>` to `<discordApplicationId>YOUR APPLICATION ID</discordApplicationId>`, replacing `YOUR APPLICATION ID` with your newly obtained discord application id.

20. Start AIMP, it should be working now.
