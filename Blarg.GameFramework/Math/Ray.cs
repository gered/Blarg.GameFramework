using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Ray : IEquatable<Ray>
	{
		public Vector3 Position;
		public Vector3 Direction;

		public Ray(Vector3 position, Vector3 direction)
		: this(ref position, ref direction)
		{
		}

		public Ray(ref Vector3 position, ref Vector3 direction)
		{
			Position = position;
			Direction = direction;
		}

		public Ray(float positionX, float positionY, float positionZ, float directionX, float directionY, float directionZ)
		{
			Position.X = positionX;
			Position.Y = positionY;
			Position.Z = positionZ;
			Direction.X = directionX;
			Direction.Y = directionY;
			Direction.Z = directionZ;
		}

		public Vector3 GetPositionAt(float distance)
		{
			Vector3 result;
			GetPositionAt(distance, out result);
			return result;
		}

		public void GetPositionAt(float distance, out Vector3 result)
		{
			result.X = Direction.X * distance + Position.X;
			result.Y = Direction.Y * distance + Position.Y;
			result.Z = Direction.Z * distance + Position.Z;
		}

		public static bool operator ==(Ray left, Ray right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Ray left, Ray right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Ray)
				return this.Equals((Ray)obj);
			else
				return false;
		}

		public bool Equals(Ray other)
		{
			return (Position == other.Position && Direction == other.Direction);
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Direction.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{Origin:{0} Direction:{1}}}", Position, Direction);
		}
	}
}
