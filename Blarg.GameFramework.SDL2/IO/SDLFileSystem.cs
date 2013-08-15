using System;
using System.IO;
using System.Reflection;
using SDL2;

namespace Blarg.GameFramework.IO
{
	public class SDLFileSystem : IFileSystem
	{
		string _assetsPath;

		public SDLFileSystem()
		{
			string executablePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			_assetsPath = Path.Combine(executablePath, "assets");
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

