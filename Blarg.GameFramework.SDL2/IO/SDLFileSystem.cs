using System;
using System.IO;
using System.Reflection;
using SDL2;

namespace Blarg.GameFramework.IO
{
	public class SDLFileSystem : IFileSystem
	{
		ILogger _logger;

		public string AssetsPath { get; private set; }
		public string StoragePath { get; private set; }

		public SDLFileSystem(ILogger logger)
		{
			_logger = logger;

			AssetsPath = GetAssetsPath();
			StoragePath = GetStoragePath();

			if (!Directory.Exists(AssetsPath))
				logger.Warn("SDL_FILESYSTEM", "Attempting to use assets directory {0} which doesn't exist.", AssetsPath);
		}

		public Stream Open(string filename, FileOpenMode mode)
		{
			string realPath = TranslateFilePath(filename);
			return new FileStream(realPath, (FileMode)mode);
		}

		public string TranslateFilePath(string path)
		{
			if (path.StartsWith("assets://"))
				return Path.Combine(AssetsPath, path.Substring(9));
			else if (path.StartsWith("storage://"))
				return Path.Combine(StoragePath, path.Substring(10));
			else
				return path;
		}

		private string GetAssetsPath()
		{
			string envAssetsPath = Environment.GetEnvironmentVariable("ASSETS_DIR");
			string workingDirPath = Path.Combine(Environment.CurrentDirectory, "assets");

			if (!String.IsNullOrEmpty(envAssetsPath))
			{
				_logger.Info("SDL_FILESYSTEM", "Environment variable ASSETS_DIR value found.");
				return envAssetsPath;
			}
			else if (Directory.Exists(workingDirPath))
			{
				_logger.Info("SDL_FILESYSTEM", "'Assets' found under the current working directory.");
				return workingDirPath;
			}
			else
			{
				// fallback to the default otherwise
				_logger.Info("SDL_FILESYSTEM", "Assuming 'Assets' directory located next to application executable.");
				return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "assets");
			}
		}

		private string GetStoragePath()
		{
			// TODO: set a proper path
			return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		}
	}
}

