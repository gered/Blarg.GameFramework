using System;

namespace Blarg.GameFramework
{
	public class ConfigFileException : Exception
	{
		public ConfigFileException()
			: base()
		{
		}

		public ConfigFileException(String message)
			: base(message)
		{
		}

		public ConfigFileException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}

