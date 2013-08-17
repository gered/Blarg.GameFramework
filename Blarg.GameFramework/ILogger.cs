using System;

namespace Blarg.GameFramework
{
	public interface ILogger
	{
		void Info(string category, string format, params object[] args);
		void Warn(string category, string format, params object[] args);
		void Error(string category, string format, params object[] args);
		void Debug(string category, string format, params object[] args);
	}
}

