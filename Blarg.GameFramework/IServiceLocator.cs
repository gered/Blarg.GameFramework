using System;

namespace Blarg.GameFramework
{
	public interface IServiceLocator
	{
		T Get<T>() where T : class;
		object Get(Type type);
	}
}

