using AIMP.SDK.FileManager.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AIMP_Discord_Presence_2.Services
{
	public sealed class ImgurAlbumArtService : IAlbumArtService
	{
		public const uint SAVEDATA_VERSION = 1;
		public const string MAGIC_WORD = "IMCACHE";

		public sealed class UploadResponse
		{
			[JsonProperty("status")]
			public int status;
			[JsonProperty("success")]
			public bool success;
			[JsonProperty("data")]
			public UploadResponseData data;
		}
		public sealed class UploadResponseData
		{
			[JsonProperty("id")]
			public string id;
			[JsonProperty("deletehash")]
			public string deletehash;
			[JsonProperty("link")]
			public string link;
		}
		public struct CacheEntry
		{
			public string url;
			public string deleteHash;
		}

		private readonly SHA1 _sha1;
		private readonly Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
		private readonly List<string> _cachedHashStrings = new List<string>();
		private readonly HashSet<string> _inWork = new HashSet<string>();
		private readonly HttpClient _http;
		private readonly StreamWriter _sw;
		private readonly object _lock = "";
		private readonly bool _deleteOnShutdown, _deleteOnTrackSwitch;
		private readonly int _maxCacheCount;
		private readonly int _retryCount;
		private readonly int _retryDelay;
		private string _prevSong = "";

		private readonly string _songsPrevLogPath, _songsLogPath;

		public ImgurAlbumArtService(string imgurClientId, int maxCacheCount, bool deleteOnShutdown, bool deleteOnTrackSwitch, int retryCount, int retryDelay)
		{
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			var dir = Path.Combine(appData, "BowieD_AIMPDiscordPresence2", "ImgurProvider");

			_songsLogPath = Path.Combine(dir, "songs.log");
			_songsPrevLogPath = Path.Combine(dir, "songs-prev.log");

			_sha1 = SHA1.Create();
			_http = new HttpClient();
			_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", imgurClientId);

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			if (File.Exists(_songsLogPath))
			{
				if (File.Exists(_songsPrevLogPath))
					File.Delete(_songsPrevLogPath);

				File.Move(_songsLogPath, _songsPrevLogPath);
			}
			_sw = new StreamWriter(_songsLogPath, true, Encoding.UTF8);
			_maxCacheCount = maxCacheCount;
			_deleteOnShutdown = deleteOnShutdown;
			_deleteOnTrackSwitch = deleteOnTrackSwitch;
			_retryCount = retryCount;
			_retryDelay = retryDelay;
		}

		public string TryGetImageUrl(IAimpFileInfo fileInfo)
		{
			if (fileInfo is null)
				return "";

			if (fileInfo.AlbumArt is null)
				return "";

			if (fileInfo.AlbumArt.Width < 1)
				return "";

			if (fileInfo.AlbumArt.Height < 1)
				return "";

			var songBytes = Encoding.UTF8.GetBytes($"{fileInfo.FileName}-{fileInfo.FileSize}");

			var hash = _sha1.ComputeHash(songBytes);
			string hashString = string.Join("", hash.Select(d => d.ToString("x2")));

			lock (_lock)
			{
				if (_prevSong != hashString)
				{
					if (_deleteOnTrackSwitch && _cache.TryGetValue(_prevSong, out var prevCacheEntry))
					{
						_cache.Remove(_prevSong);

						Task.Run(async () =>
						{
							await DeleteOldFileInfo(prevCacheEntry);
						});
					}

					_prevSong = hashString;
				}

				if (_inWork.Contains(hashString)) return "";

				if (_cache.TryGetValue(hashString, out var cacheEntry))
					return cacheEntry.url;

				_inWork.Add(hashString);
			}

			Task.Run(async () =>
			{
				await ProcessFileInfo(fileInfo, hashString);
			});

			return "";
		}

		public async Task DeleteOldFileInfo(CacheEntry cacheEntry)
		{
			if (string.IsNullOrWhiteSpace(cacheEntry.url) ||
				string.IsNullOrWhiteSpace(cacheEntry.deleteHash))
				return;

			for (int i = 0; i < _retryCount; i++)
			{
				await _sw.WriteLineAsync($"[attempt #{i + 1}] trying to delete {cacheEntry.url}");

				try
				{
					await _http.DeleteAsync($"https://api.imgur.com/3/image/{cacheEntry.deleteHash}");

					await _sw.WriteLineAsync($"deleted {cacheEntry.url}");
					await _sw.FlushAsync();

					break;
				}
				catch (Exception ex)
				{
					await _sw.WriteLineAsync($"[attempt #{i + 1}] could not delete {cacheEntry.url}");
					await _sw.WriteLineAsync(ex.ToString());
					await _sw.FlushAsync();

					await Task.Delay(_retryDelay);
				}
			}
		}

		public async Task ProcessFileInfo(IAimpFileInfo fileInfo, string hashString)
		{
			bool hasUploaded = false;

			for (int i = 0; i < _retryCount; i++)
			{
				try
				{
					HttpResponseMessage response;

					using (MemoryStream ms = new MemoryStream())
					{
						fileInfo.AlbumArt.Save(ms, ImageFormat.Jpeg);

						await _sw.WriteLineAsync($"[attempt #{i + 1}] uploading {fileInfo.FileName}");

						ms.Seek(0, SeekOrigin.Begin);

						var multiPart = new MultipartFormDataContent
						{
							{
								new StringContent("raw"),
								"type"
							},
							{
								new StreamContent(ms),
								"image"
							}
						};

						response = _http.PostAsync("https://api.imgur.com/3/image", multiPart).ConfigureAwait(false).GetAwaiter().GetResult();
					}

					var responseText = await response.Content.ReadAsStringAsync();
					var parsedResponse = JsonConvert.DeserializeObject<UploadResponse>(responseText);

					if (!parsedResponse.success)
					{
						await _sw.WriteLineAsync($"[attempt #{i + 1}] could not upload cover for {fileInfo.FileName}");
						await _sw.WriteLineAsync($"[attempt #{i + 1}] return code: {response.StatusCode}");

						if (response.StatusCode == (HttpStatusCode)429)
						{
							await _sw.WriteLineAsync($"[attempt #{i + 1}] you are being rate-limited");

							if (response.Headers.TryGetValues("X-RateLimit-UserLimit", out var rateLimitUserLimit) &&
								response.Headers.TryGetValues("X-RateLimit-UserRemaining", out var rateLimitUserRemaining) &&
								response.Headers.TryGetValues("X-RateLimit-UserReset", out var rateLimitUserReset) &&
								response.Headers.TryGetValues("X-RateLimit-ClientLimit", out var rateLimitClientLimit) &&
								response.Headers.TryGetValues("X-RateLimit-ClientRemaining", out var rateLimitClientRemaining))
							{
								string buildLine(IEnumerable<string> values) => string.Join("; ", values);

								await _sw.WriteLineAsync($"[attempt #{i + 1}] userLimit: {buildLine(rateLimitUserLimit)}");
								await _sw.WriteLineAsync($"[attempt #{i + 1}] userRemaining: {buildLine(rateLimitUserRemaining)}");
								await _sw.WriteLineAsync($"[attempt #{i + 1}] userReset: {buildLine(rateLimitUserReset)}");
								await _sw.WriteLineAsync($"[attempt #{i + 1}] clientLimit: {buildLine(rateLimitClientLimit)}");
								await _sw.WriteLineAsync($"[attempt #{i + 1}] clientRemaining: {buildLine(rateLimitClientRemaining)}");
							}

							break;
						}

						await Task.Delay(_retryDelay);
						continue;
					}

					lock (_lock)
					{
						_cache[hashString] = new CacheEntry()
						{
							url = parsedResponse.data.link,
							deleteHash = parsedResponse.data.deletehash,
						};
						_cachedHashStrings.Add(hashString);

						while (_cachedHashStrings.Count >= _maxCacheCount && _cachedHashStrings.Count > 1)
						{
							var itemToRemove = _cachedHashStrings[0];

							if (_cache.TryGetValue(itemToRemove, out var oldCacheEntry))
							{
								DeleteOldFileInfo(oldCacheEntry).ConfigureAwait(false).GetAwaiter().GetResult();

								_cache.Remove(itemToRemove);
							}

							_cachedHashStrings.RemoveAt(0);
						}

						_inWork.Remove(hashString);
					}

					await _sw.WriteLineAsync($"[attempt #{i + 1}] uploaded {fileInfo.FileName}\t{parsedResponse.data.link}\t{parsedResponse.data.deletehash}");
					await _sw.FlushAsync();

					hasUploaded = true;
					break;
				}
				catch (Exception ex)
				{
					await _sw.WriteLineAsync($"[attempt #{i + 1}] could not upload cover for {fileInfo.FileName}");
					await _sw.WriteLineAsync(ex.ToString());
					await _sw.FlushAsync();

					await Task.Delay(_retryDelay);
				}
			}

			if (!hasUploaded)
			{
				await _sw.WriteLineAsync($"could not upload cover for {fileInfo.FileName} after all attempts");
				await _sw.FlushAsync();

				lock (_lock)
				{
					_cache[hashString] = new CacheEntry()
					{
						url = "",
						deleteHash = "",
					};
					_cachedHashStrings.Add(hashString);

					while (_cachedHashStrings.Count >= _maxCacheCount && _cachedHashStrings.Count > 1)
					{
						var itemToRemove = _cachedHashStrings[0];

						if (_cache.TryGetValue(itemToRemove, out var oldCacheEntry))
						{
							DeleteOldFileInfo(oldCacheEntry).ConfigureAwait(false).GetAwaiter().GetResult();

							_cache.Remove(itemToRemove);
						}

						_cachedHashStrings.RemoveAt(0);
					}

					_inWork.Remove(hashString);
				}
			}
		}

		public void Dispose()
		{
			if (_deleteOnShutdown)
			{
				foreach (var entry in _cache)
				{
					DeleteOldFileInfo(entry.Value).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				_cache.Clear();
				_cachedHashStrings.Clear();
			}

			_sw.Dispose();
			_sha1.Dispose();
			_http.Dispose();
		}
	}
}
