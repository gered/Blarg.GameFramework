using System;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Events
{
	public abstract class Event : IPoolable
	{
		public abstract void Reset();
	}
}

