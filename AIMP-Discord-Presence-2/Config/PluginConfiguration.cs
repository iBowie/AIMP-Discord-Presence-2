using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AIMP_Discord_Presence_2.Config
{
	public sealed class PluginConfiguration
	{
		// Common
		public string discordApplicationId;
		[JsonConverter(typeof(StringEnumConverter))]
		public EAlbumArtProvider albumArtProvider;
		public double updateFrequency;
		public bool addPresenceButtons;
		public bool displaySmallLogo;
		public int retryCount;
		public int retryDelayMs;

		// Imgur
		public string imgurClientId;
		public bool automaticallyDeleteOnSongSwitch;
		public bool automaticallyDeleteOnPluginShutdown;
		public int maxCacheCount;

		// MusicBrainz
		public string musicBrainzUserAgent;

		public void LoadDefaults()
		{
			discordApplicationId = "429559336982020107";
			albumArtProvider = EAlbumArtProvider.MusicBrainz;
			updateFrequency = 5.0;
			addPresenceButtons = true;
			displaySmallLogo = true;
			retryCount = 5;
			retryDelayMs = 500;

			imgurClientId = "";
			automaticallyDeleteOnPluginShutdown = true;
			automaticallyDeleteOnSongSwitch = false;
			maxCacheCount = 4;

			musicBrainzUserAgent = "";
		}

		public void SanityCheck()
		{
			maxCacheCount = System.Math.Max(maxCacheCount, 2);
			updateFrequency = System.Math.Max(updateFrequency, 1);
			retryCount = System.Math.Max(retryCount, 1);
			retryDelayMs = System.Math.Max(retryDelayMs, 100);
		}
	}

	public enum EAlbumArtProvider
	{
		None = 0,
		Imgur = 1,
		Discord = 2,
		MusicBrainz = 3,
	}
}
