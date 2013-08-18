using System;
using System.IO;
using System.Reflection;

namespace Blarg.GameFramework.Resources
{
	public static class ResourceUtils
	{
		public static Stream GetResource(string filename)
		{
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename");

			return Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
		}

		public static string GetTextResource(string filename)
		{
			using (var stream = GetResource(filename))
			{
				if (stream == null)
					return null;

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}
