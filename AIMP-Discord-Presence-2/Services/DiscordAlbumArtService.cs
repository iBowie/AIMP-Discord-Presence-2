using AIMP.SDK.FileManager.Objects;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AIMP_Discord_Presence_2.Services
{
	public sealed class DiscordAlbumArtService : IAlbumArtService
	{
		private readonly SHA1 _sha1;

		public DiscordAlbumArtService()
		{
			_sha1 = SHA1.Create();
		}

		public void Dispose()
		{
			_sha1.Dispose();
		}

		public string TryGetImageUrl(IAimpFileInfo fileInfo)
		{
			var albumBytes = Encoding.UTF8.GetBytes(fileInfo.Album);

			var hash = _sha1.ComputeHash(albumBytes);
			string hashString = string.Join("", hash.Select(d => d.ToString("x2")));
			return hashString;
		}
	}
}
