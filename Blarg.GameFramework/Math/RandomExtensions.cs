using System;

namespace Blarg.GameFramework
{
	public static class RandomExtensions
	{
		public static float NextFloat(this Random random)
		{
			return (float)random.NextDouble();
		}

		public static float NextFloat(this Random random, float maxValue)
		{
			return ((float)random.NextDouble()) * maxValue;
		}

		public static float NextFloat(this Random random, float minValue, float maxValue)
		{
			return ((float)random.NextDouble()) * (maxValue - minValue) + minValue;
		}

		public static double NextDouble(this Random random, double maxValue)
		{
			return random.NextDouble() * maxValue;
		}

		public static double NextDouble(this Random random, double minValue, double maxValue)
		{
			return random.NextDouble() * (maxValue - minValue) + minValue;
		}
	}
}

