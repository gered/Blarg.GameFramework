using System;

namespace Blarg.GameFramework.Content
{
	public class ContentContainer<T> where T : class
	{
		public T Content;
		public bool IsPinned;

		internal ContentContainer(T content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			Content = content;
			IsPinned = false;
		}
	}
}
