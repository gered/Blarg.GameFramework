using System;

namespace Blarg.GameFramework.Content
{
	public interface IContentLoaderBase : IDisposable
	{
		Type ContentType { get; }

		void RemoveAll(bool removePinnedContent = false);

		void OnLoad();
		void OnUnload();
		void OnNewContext();
		void OnLostContext();
	}
}
