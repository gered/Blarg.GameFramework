using System;
using System.IO;

namespace Blarg.GameFramework.IO
{
	public interface IFileSystem
	{
		Stream Open(string filename);
		string TranslateFilePath(string path);

		string AssetsPath { get; }
	}
}
