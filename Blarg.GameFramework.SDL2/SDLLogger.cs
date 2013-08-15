using System;
using System.Text;

namespace Blarg.GameFramework
{
	public class SDLLogger : IPlatformLogger
	{
		StringBuilder _sb;

		public SDLLogger()
		{
			_sb = new StringBuilder(8192);
		}

		public void Info(string category, string format, params object[] args)
		{
			WriteLine("INFO", category, format, args);
		}

		public void Warn(string category, string format, params object[] args)
		{
			WriteLine("WARN", category, format, args);
		}

		public void Error(string category, string format, params object[] args)
		{
			WriteLine("ERROR", category, format, args);
		}

		public void Debug(string category, string format, params object[] args)
		{
			WriteLine("DEBUG", category, format, args);
		}

		private void WriteLine(string tag, string category, string format, params object[] args)
		{
			_sb.Clear();

			var date = DateTime.Now;

			_sb.AppendFormat("[{0:00}:{1:00}:{2:00},{3:000}] ", date.Hour, date.Minute, date.Second, date.Millisecond);
			_sb.AppendFormat("[{0}] [{1}] ", tag, category);
			_sb.AppendFormat(format, args);
			_sb.Append(Environment.NewLine);

			Console.Write(_sb.ToString());
		}
	}
}

