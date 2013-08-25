using System;

namespace Blarg.GameFramework
{
	public class ServiceLocatorException : Exception
	{
		public ServiceLocatorException()
			: base()
		{
		}

		public ServiceLocatorException(String message)
			: base(message)
		{
		}

		public ServiceLocatorException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}

