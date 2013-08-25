using System;

namespace Blarg.GameFramework.Content
{
	public class ContentManagementException : Exception
	{
		public ContentManagementException()
			: base()
		{
		}

		public ContentManagementException(String message)
			: base(message)
		{
		}

		public ContentManagementException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}

