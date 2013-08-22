using System;

namespace Blarg.GameFramework.Events
{
	public interface IEventListener
	{
		bool Handle(Event e);
	}
}

