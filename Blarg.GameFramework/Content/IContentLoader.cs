using System;

namespace Blarg.GameFramework.Content
{
	public interface IContentLoader<T> : IContentLoaderBase where T : class
	{
		T Get(string name, object contentParameters);
		T Pin(string name, object contentParameters);

		string GetNameOf(T content);
	}
}
