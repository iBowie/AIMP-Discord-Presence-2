using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

Console.Write("Directory >> ");
string? dir = Console.ReadLine();

if (!Directory.Exists(dir))
{
	Console.WriteLine("Specify a valid directory.");
	Console.ReadKey(true);
	return;
}

Console.Write("Output Directory >> ");
string? outDir = Console.ReadLine();

if (!Directory.Exists(outDir))
{
	Console.WriteLine("Specify a valid directory.");
	Console.ReadKey(true);
	return;
}

var allFiles = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

using SHA1 sha1 = SHA1.Create();

foreach (var file in allFiles)
{
	using FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.Read);

	fs.Seek(0, SeekOrigin.Begin);

	var ext = Path.GetExtension(file).ToLowerInvariant();

	switch (ext)
	{
		case ".mp3":
		case ".flac":
		case ".m4a":
			{
				try
				{
					using var tagsFile = TagLib.File.Create(file);

					var picData = tagsFile.Tag.Pictures[0].Data.Data;

					Image pic = Image.Load(picData);

					int min = Math.Min(pic.Width, pic.Height);

					if (min < 512)
					{
						double scale = 512.0 / min;

						int scaledWidth = (int)Math.Ceiling(pic.Width * scale);
						int scaledHeight = (int)Math.Ceiling(pic.Height * scale);

						Console.WriteLine($"{file}'s cover is too small ({pic.Width}x{pic.Height}). Scaling up to ({scaledWidth}x{scaledHeight})...");

						pic.Mutate(x => x.Resize(scaledWidth, scaledHeight));
					}

					var albumBytes = Encoding.UTF8.GetBytes(tagsFile.Tag.Album);

					var hash = sha1.ComputeHash(albumBytes);
					string hashString = string.Join("", hash.Select(d => d.ToString("x2")));
					string outFileName = Path.Combine(outDir, $"{hashString}.jpg");

					if (File.Exists(outFileName)) continue;

					pic.SaveAsJpeg(outFileName);
				}
				catch
				{
					Console.WriteLine("Could not get album cover from " + file);
				}
			}
			break;
		default:
			continue;
	}
}