1. Go to [Imgur](https://imgur.com/).

2. Open profile settings (you will need an Imgur account).

3. Go to `Applications` tab.

4. Create a new application, name it however you want. If asked, set `Authorization callback URL` to `https://imgur.com/`.

5. Grab the newly created client ID, keep it in mind.

6. Completely close AIMP, make sure it's not open in tray.

7. Go to data folder of the plugin (you can use a shortcut in plugin folder, which can be found in `AIMP/Plugins/aimp_DiscordPresence2`).

8. Open `config.xml` in any text editor.

9. Change `albumArtProvider` to Imgur, making it look like this - `<albumArtProvider>Imgur</albumArtProvider>`. **Capitalization matters!**

10. Change `<imgurClientId />` to `<imgurClientId>YOUR CLIENT ID</imgurClientId>`, replacing `YOUR CLIENT ID` with our newly obtained ID.

11. Start AIMP, it should be working now.
