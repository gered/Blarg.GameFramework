using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	public static class MathHelpers
	{
		/// <summary>
		/// Converts coordinates from spherical to cartesian.
		/// </summary>
		/// <param name="radius">distance from the origin "O" to the point "P" (the "r" value)</param>
		/// <param name="angleTheta">the angle (in radians) between the zenith direction and the line segment OP</param>
		/// <param name="angleRho">the signed angle (in radians) measured from the azimuth reference direction to the orthogonal projection of the line segment OP on the reference plane</param>
		/// <param name="cartesianCoords">the output cartesian coordinates</param>
		public static void GetCartesianCoordsFromSpherical(float radius, float angleTheta, float angleRho, out Vector3 cartesianCoords)
		{
			cartesianCoords.X = radius * (float)Math.Sin(angleTheta) * (float)Math.Sin(angleRho);
			cartesianCoords.Y = radius * (float)Math.Cos(angleTheta);
			cartesianCoords.Z = radius * (float)Math.Sin(angleTheta) * (float)Math.Cos(angleRho);
		}

		/// <summary>
		/// Converts an angle around the Y axis to a direction vector that
		/// lies flat on the XZ plane. Note that the angle is offset so that 0 degrees
		/// points directly down the +Y axis on a 2D cartesian grid, instead of the +X 
		/// axis as one might expect.
		/// </summary>
		/// <returns>direction vector on the XZ plane</returns>
		/// <param name="angle">the Y axis angle (in radians)</param>
		public static Vector3 GetDirectionFromYAxisOrientation(float angle)
		{
			Vector3 facing;
			facing.Y = 0.0f;

			// TODO: perhaps the whole "90 degree offset" thing we're doing below 
			//       is too scenario-specific. maybe have an overload of this function
			//       which accepts an offset angle parameter?
			// 
			// GetPointOnCircle() returns an angle based on a 2D cartesian grid where
			// 0 degrees points in the +X direction. We want it to point in the
			// +Y direction (2D cartesian Y = our 3D Z), so we offset our angle by 90
			// degrees before calling it to get the intended result
			float adjustedAngle = RolloverClamp(angle - MathConstants.Radians90, MathConstants.Radians0, MathConstants.Radians360);
			GetPointOnCircle(1.0f, adjustedAngle, out facing.X, out facing.Z);

			return facing;
		}

		/// <summary>
		/// Converts euler angles to an equivalent direction vector. Note that this just
		/// uses one of many other ways to do this (the others using any other rotation
		/// order).
		/// </summary>
		/// <returns>direction vector</returns>
		/// <param name="yaw">the yaw rotation angle (in radians)</param>
		/// <param name="pitch">the pitch rotation angle (in radians)</param>
		public static Vector3 GetDirectionFromAngles(float yaw, float pitch)
		{
			Vector3 result;
			result.X = (float)Math.Cos(yaw) * (float)Math.Cos(pitch);
			result.Y = (float)Math.Sin(yaw) * (float)Math.Cos(pitch);
			result.Z = (float)Math.Sin(pitch);

			return result;
		}

		/// <summary>
		/// Returns the angle between two 2D points. Note that the returned angle is
		/// offset so that 0 degrees points directly down the +Y axis on a 2D cartesian
		/// grid, instead of the +X axis as one might expect.
		/// </summary>
		/// <returns>the angle (in radians) between the two points</returns>
		/// <param name="x1">X coordinate of the first point</param>
		/// <param name="y1">Y coordinate of the first point</param>
		/// <param name="x2">X coordinate of the second point</param>
		/// <param name="y2">Y coordinate of the second point</param>
		public static float GetAngleBetweenPoints(float x1, float y1, float x2, float y2)
		{
			float angle = (float)Math.Atan2(y2 - y1, x2 - x1);

			// we offset the angle by 90 degrees to ensure 0 degrees points in the
			// +Y direction on a 2D cartesian grid instead of +X. This corresponds with
			// the rest of our direction coordinate system.
			return angle - MathConstants.Radians90;
		}

		/// <summary>
		/// Solves a quadratic equation and returns the lowest root.
		/// </summary>
		/// <returns>true if the quadratic could be solved, false if not</returns>
		/// <param name="a">the value of the a variable in the quadratic equation</param>
		/// <param name="b">the value of the b variable in the quadratic equation</param>
		/// <param name="c">the value of the c variable in the quadratic equation</param>
		/// <param name="maxR">the maximum root value we will accept as an answer. anything that is higher will not be accepted as a solution</param>
		/// <param name="root">the variable to hold the lowest root value if one is found</param>
		public static bool GetLowestQuadraticRoot(float a, float b, float c, float maxR, out float root)
		{
			root = 0.0f;
			float determinant = (b * b) - (4.0f * a * c);

			// If the determinant is negative, there is no solution (can't square root a negative)
			if (determinant < 0.0f)
				return false;

			float sqrtDeterminant = (float)Math.Sqrt(determinant);
			float root1 = (-b - sqrtDeterminant) / (2 * a);
			float root2 = (-b + sqrtDeterminant) / (2 * a);
			// Sort so root1 <= root2
			if (root1 > root2)
			{
				float temp = root2;
				root2 = root1;
				root1 = temp;
			}

			// Get the lowest root
			if (root1 > 0 && root1 < maxR)
			{
				root = root1;
				return true;
			}

			if (root2 > 0 && root2 < maxR)
			{
				root = root2;
				return true;
			}

			// No valid solutions found
			return false;
		}

		/// <summary>
		/// Returns the 2D point on a circle's circumference given a radius and angle.
		/// </summary>
		/// <param name="radius">the radius of the circle</param>
		/// <param name="angle">the angle around the circle (in radians)</param>
		/// <param name="x">the variable to hold the X point on the circle's circumference</param>
		/// <param name="y">the variable to hold the Y point on the circle's circumference</param>
		public static void GetPointOnCircle(float radius, float angle, out float x, out float y)
		{
			x = radius * (float)Math.Cos(angle);
			y = radius * (float)Math.Sin(angle);
		}

		/// <summary>
		/// Returns the 2D point on a circle's circumference given a radius and angle.
		/// </summary>
		/// <param name="radius">the radius of the circle</param>
		/// <param name="angle">the angle around the circle (in radians)</param>
		/// <param name="point">the vector to hold the point on the circle's circumference</param>
		public static void GetPointOnCircle(float radius, float angle, out Vector2 point)
		{
			point.X = radius * (float)Math.Cos(angle);
			point.Y = radius * (float)Math.Sin(angle);
		}

		/// <summary>
		/// Clamps a value to a given range.
		/// </summary>
		/// <returns>the clamped value</returns>
		/// <param name="value">the value to be clamped</param>
		/// <param name="low">the low end of the range to clamp to</param>
		/// <param name="high">the high end of the range to clamp to</param>
		public static float Clamp(float value, float low, float high)
		{
			if (value < low)
				return low;
			if (value > high)
				return high;

			return value;
		}

		/// <summary>
		/// Clamps a value to a given range.
		/// </summary>
		/// <returns>the clamped value</returns>
		/// <param name="value">the value to be clamped</param>
		/// <param name="low">the low end of the range to clamp to</param>
		/// <param name="high">the high end of the range to clamp to</param>
		public static int Clamp(int value, int low, int high)
		{
			if (value < low)
				return low;
			if (value > high)
				return high;

			return value;
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <returns>equivalent value in radians</returns>
		/// <param name="degrees">degree value to convert</param>
		public static float DegreesToRadians(float degrees)
		{
			return degrees * MathConstants.DegToRad;
		}

		/// <summary>
		/// A fast method for calculating the inverse square root of a nunmber.
		/// </summary>
		/// <returns>The inverse square root of the given number.</returns>
		/// <param name="x">The value to get the inverse square root of.</param>
		/// <remarks>Copy of the famous method from the Quake 3 source.</remarks>
		public static float FastInverseSqrt(float x)
		{
			FloatIntUnion convert;
			convert.i = 0;
			convert.x = x;
			float xhalf = 0.5f * x;
			convert.i = 0x5f3759df - (convert.i >> 1);
			x = convert.x;
			x = x * (1.5f - xhalf * x * x);
			return x;
		}

		/// <summary>
		/// Checks two floats for equality using a defined "tolerance" to account
		/// for floating point rounding errors, etc.
		/// </summary>
		/// <returns>true if equal, false if not</returns>
		/// <param name="a">first value to check</param>
		/// <param name="b">second value to check</param>
		/// <param name="tolerance">tolerance value to use</param>
		public static bool IsCloseEnough(float a, float b, float tolerance = MathConstants.Tolerance)
		{
			return Math.Abs((a - b) / ((b == 0.0f) ? 1.0f : b)) < tolerance;
		}

		/// <summary>
		/// Determines if a given number is a power of two.
		/// </summary>
		/// <returns>true if a power of two, false if not</returns>
		/// <param name="n">number to check</param>
		public static bool IsPowerOf2(int n)
		{
			return (n != 0) && ((n & (n - 1)) == 0);
		}

		/// <summary>
		/// Linearly interpolates between two values.
		/// </summary>
		/// <param name="a">first value (low end of range)</param>
		/// <param name="b">second value (high end of range)</param>
		/// <param name="t">the amount to interpolate between the two values</param>
		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		/// <summary>
		/// Given a linearly interpolated value and the original range (high and 
		/// low) of the linear interpolation, this will determine what the original
		/// 0.0 to 1.0 value (weight) was used to perform the interpolation.
		/// </summary>
		/// <returns>the interpolation value (weight, in the range 0.0 to 1.0) used in the original interpolation</returns>
		/// <param name="a">first value (low end of range)</param>
		/// <param name="b">second value (high end of range)</param>
		/// <param name="lerpValue">the result of the original interpolation</param>
		public static float InverseLerp(float a, float b, float lerpValue)
		{
			return (lerpValue - a) / (b - a);
		}

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <returns>equivalent value in degrees</returns>
		/// <param name="radians">radian value to convert</param>
		public static float RadiansToDegrees(float radians)
		{
			return radians * MathConstants.RadToDeg;
		}

		/// <summary>
		/// Clamps a value to a given range, but if the value is outside the range
		/// instead of returning the low/high end of the range, this will continue
		/// counting after moving to the opposite end of the range to arrive at a
		/// final value.
		/// </summary>
		/// <returns>the clamped value</returns>
		/// <param name="value">the value to be clamped</param>
		/// <param name="low">the low end of the range to clamp to</param>
		/// <param name="high">the high end of the range to clamp to</param>
		public static float RolloverClamp(float value, float low, float high)
		{
			float temp = value;
			// TODO: this is really shitty... make it better
			do
			{
				float range = Math.Abs(high - low);
				if (temp < low)
					temp = temp + range;
				if (value > high)
					temp = temp - range;
			}
			while (temp < low || temp > high); // loop through as many times as necessary to put the value within the low/high range

			return temp;
		}

		/// <summary>
		/// Re-scales a given value from an old min/max range to a new and
		/// different min/max range such that the value is approximately
		/// at the same distance between both min and max values.
		/// </summary>
		/// <returns>re-scaled value which will fall between newMin and newMax</returns>
		/// <param name="value">the value to be rescaled which is currently between originalMin and originalMax</param>
		/// <param name="originalMin">original min value (low end of range)</param>
		/// <param name="originalMax">original max value (high end of range)</param>
		/// <param name="newMin">new min value (low end of range)</param>
		/// <param name="newMax">new max value (high end of range)</param>
		public static float ScaleRange(float value, float originalMin, float originalMax, float newMin, float newMax)
		{
			return (value / ((originalMax - originalMin) / (newMax - newMin))) + newMin;
		}

		/// <summary>
		/// Interpolates between two values using a cubic equation.
		/// </summary>
		/// <returns>the interpolated value</returns>
		/// <param name="low">low end of range to interpolate between</param>
		/// <param name="high">high end of range to interpolate between</param>
		/// <param name="t">amount to interpolate by (the weight)</param>
		public static float SmoothStep(float low, float high, float t)
		{
			float n = Clamp(t, 0.0f, 1.0f);
			return Lerp(low, high, (n * n) * (3.0f - (2.0f * n)));
		}

		// this is to allow access to raw float bits
		[StructLayout(LayoutKind.Explicit)]
		public struct FloatIntUnion
		{
			[FieldOffset(0)]
			public float x;

			[FieldOffset(0)]
			public int i;
		}
	}
}
