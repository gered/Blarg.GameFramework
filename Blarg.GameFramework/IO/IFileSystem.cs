using System;
using System.IO;

namespace Blarg.GameFramework.IO
{
	public interface IFileSystem
	{
		Stream Open(string filename, FileOpenMode mode = FileOpenMode.Open);
		string TranslateFilePath(string path);

		string AssetsPath { get; }
		string StoragePath { get; }
	}
}
