using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.Support
{
	public static class DictionaryExtensions
	{
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : class
		{
			TValue result;
			dict.TryGetValue(key, out result);
			return result;
		}
	}
}

