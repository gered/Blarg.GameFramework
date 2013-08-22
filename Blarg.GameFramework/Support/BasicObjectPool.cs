using System;

namespace Blarg.GameFramework.Support
{
	public class BasicObjectPool<T> : ObjectPool<T> where T : class, new()
	{
		public BasicObjectPool()
			: base()
		{
		}

		public BasicObjectPool(int initialCapacity)
			: base(initialCapacity)
		{
		}

		protected override T Allocate()
		{
			return new T();
		}
	}
}

