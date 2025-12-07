1. Download Album Cover Gatherer.

2. Run it.

3. When prompted with `Directory >>`, input your music directory.

4. When prompted with `Output Directory >>`, input some empty directory (or directory with previous output).

5. Wait it to finish working.

6. Upload those images to your own website into a single folder.

7. Completely close AIMP, make sure it's not open in tray.

8. Go to data folder of the plugin (you can use a shortcut in plugin folder, which can be found in `AIMP/Plugins/aimp_DiscordPresence2`).

9. Open `config.xml` in any text editor.

10. Change `albumArtProvider` to StaticWebsite, making it look like this - `<albumArtProvider>StaticWebsite</albumArtProvider>`. **Capitalization matters!**

19. Change `<staticWebsiteUrlFormat></staticWebsiteUrlFormat>` to `<staticWebsiteUrlFormat>YOUR WEBSITE URL</staticWebsiteUrlFormat>`, replacing `YOUR WEBSITE URL` with your link to the website in a proper format, for instance: `<staticWebsiteUrlFormat>http://example.com/albumCovers/{0}</staticWebsiteUrlFormat>`, where `{0}` is where plugin will input proper name for current album.

20. Start AIMP, it should be working now.
