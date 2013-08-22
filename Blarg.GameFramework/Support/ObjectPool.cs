using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.Support
{
	public abstract class ObjectPool<T> where T : class
	{
		List<T> _objects;
		int _objectCount;

		public ObjectPool()
			: this(16)
		{
		}

		public ObjectPool(int initialCapacity)
		{
			_objects = new List<T>(initialCapacity);
			_objectCount = 0;
		}

		protected abstract T Allocate();

		public T Take()
		{
			if (_objectCount == 0)
				return Allocate();
			else
			{
				// remove the last object in the list
				int index = _objectCount - 1;
				T obj = _objects[index];

				// shrink the object list
				_objects[index] = null;
				--_objectCount;

				return obj;
			}
		}

		public void Free(T obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			int index = _objectCount;
			if (index >= _objects.Count)
				// need to grow the object list by one
				_objects.Add(obj);
			else
				_objects[index] = obj;

			++_objectCount;

			if (obj is IPoolable)
				((IPoolable)obj).Reset();
		}
	}
}

