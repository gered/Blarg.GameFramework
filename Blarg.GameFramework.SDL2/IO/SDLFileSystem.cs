using System;
using System.IO;
using System.Reflection;
using SDL2;

namespace Blarg.GameFramework.IO
{
	public class SDLFileSystem : IFileSystem
	{
		string _assetsPath;

		public SDLFileSystem(ILogger logger)
		{
			string envAssetsPath = Environment.GetEnvironmentVariable("ASSETS_DIR");
			string workingDirPath = Path.Combine(Environment.CurrentDirectory, "assets");

			if (!String.IsNullOrEmpty(envAssetsPath))
			{
				logger.Info("SDL_FILESYSTEM", "Environment variable ASSETS_DIR value found.");
				_assetsPath = envAssetsPath;
			}
			else if (Directory.Exists(workingDirPath))
			{
				logger.Info("SDL_FILESYSTEM", "'Assets' found under the current working directory.");
				_assetsPath = workingDirPath;
			}
			else
			{
				// fallback to the default otherwise
				logger.Info("SDL_FILESYSTEM", "Assuming 'Assets' directory located next to application executable.");
				_assetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "assets");
			}

			if (!Directory.Exists(_assetsPath))
				logger.Warn("SDL_FILESYSTEM", "Attempting to use assets directory {0} which doesn't exist.", _assetsPath);
		}

		public Stream Open(string filename)
		{
			string realPath = TranslateFilePath(filename);
			return new FileStream(realPath, FileMode.Open);
		}

		public string TranslateFilePath(string path)
		{
			if (path.StartsWith("assets://"))
				return Path.Combine(_assetsPath, path.Substring(9));
			else
				return path;
		}

		public string AssetsPath
		{
			get { return _assetsPath; }
		}
	}
}

