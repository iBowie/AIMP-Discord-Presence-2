using AIMP.SDK.FileManager.Objects;
using System;

namespace AIMP_Discord_Presence_2.Services
{
	public interface IAlbumArtService : IDisposable
	{
		string TryGetImageUrl(IAimpFileInfo fileInfo);
	}
}
