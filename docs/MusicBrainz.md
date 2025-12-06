1. Completely close AIMP, make sure it's not open in tray.

2. Go to data folder of the plugin (you can use a shortcut in plugin folder, which can be found in `AIMP/Plugins/aimp_DiscordPresence2`).

3. Open `config.xml` in any text editor.

4. Change `albumArtProvider` to MusicBrainz, making it look like this - `<albumArtProvider>MusicBrainz</albumArtProvider>`. **Capitalization matters!**

5. Change `<musicBrainzUserAgent />` to `<musicBrainzUserAgent>AIMPDiscordRPC/1.0.0 ( YOUR@EMAIL.ADDRESS )</musicBrainzUserAgent>`, replacing `YOUR@EMAIL.ADDRESS` with your email address.

6. Start AIMP, it should be working now.
