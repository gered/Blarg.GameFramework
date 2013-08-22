using System;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Entities
{
	public abstract class Component : IPoolable
	{
		public abstract void Reset();
	}
}

