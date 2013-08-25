using System;

namespace Blarg.GameFramework
{
	public static class MathConstants
	{
		public const float Pi = 3.14159274f;                // 180 degrees
		public const float HalfPi = Pi / 2.0f;              // 90 degrees
		public const float QuarterPi = Pi / 4.0f;           // 45 degrees
		public const float TwoPi = Pi * 2.0f;               // 360 degrees

		public const float PiOver180 = Pi / 180.0f;
		public const float DegToRad = PiOver180;            // for converting degrees to radians
		public const float RadToDeg = 1.0f / PiOver180;     // for converting radians to degrees;

		public const float Radians0 = 0.0f;
		public const float Radians45 = Pi / 4.0f;
		public const float Radians90 = Pi / 2.0f;
		public const float Radians135 = (3.0f * Pi) / 4.0f;
		public const float Radians180 = Pi;
		public const float Radians225 = (5.0f * Pi) / 4.0f;
		public const float Radians270 = (3.0f * Pi) / 2.0f;
		public const float Radians315 = (7.0f * Pi) / 4.0f;
		public const float Radians360 = Pi * 2.0f;

		public const float Radians60 = 60.0f * DegToRad;

		public const float Tolerance = 0.000001f;
	}
}
