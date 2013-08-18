using System;

namespace Blarg.GameFramework.Support
{
	internal static class StringExtensions
	{
		public static string Copy(string source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			var result = new char[source.Length];
			for (int i = 0; i < source.Length; ++i)
				result[i] = source[i];

			return new string(result);
		}
	}
}

