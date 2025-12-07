using AIMP.SDK.FileManager.Objects;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AIMP_Discord_Presence_2.Services
{
	public sealed class StaticWebsiteAlbumArtService : IAlbumArtService
	{
		private readonly SHA1 _sha1;
		private readonly string _websiteUrlFormat;

		public StaticWebsiteAlbumArtService(string websiteUrlFormat)
		{
			_sha1 = SHA1.Create();
			_websiteUrlFormat = websiteUrlFormat;
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

			return string.Format(_websiteUrlFormat, hashString + ".jpg");
		}
	}
}
