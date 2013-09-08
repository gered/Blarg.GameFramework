using System;

namespace Blarg.GameFramework.Support
{
	// this class exists because i am stupid and can never remember the
	// operators used for setting/clearing/toggling bits
	//
	// ... and of course, you can't use generics with bitwise operations :(

	public static class BitExtensions
	{
		public static int GetBitmaskFor(uint startBit, uint numBits)
		{
			int temp = MathHelpers.Pow(2, startBit);
			return (temp * (MathHelpers.Pow(2, numBits))) - temp;
		}

		#region Is Set

		public static bool IsBitSet(this long bitfield, long bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this ulong bitfield, ulong bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this int bitfield, int bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this uint bitfield, uint bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this short bitfield, short bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this ushort bitfield, ushort bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this byte bitfield, byte bit)
		{
			return (bitfield & bit) != 0;
		}

		#endregion

		#region Set

		public static long SetBit(this long bitfield, long bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static ulong SetBit(this ulong bitfield, ulong bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static int SetBit(this int bitfield, int bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static uint SetBit(this uint bitfield, uint bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static short SetBit(this short bitfield, short bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static ushort SetBit(this ushort bitfield, ushort bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static byte SetBit(this byte bitfield, byte bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		#endregion

		#region Toggle

		public static long ToggleBit(this long bitfield, long bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static ulong ToggleBit(this ulong bitfield, ulong bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static int ToggleBit(this int bitfield, int bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static uint ToggleBit(this uint bitfield, uint bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static short ToggleBit(this short bitfield, short bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static ushort ToggleBit(this ushort bitfield, ushort bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static byte ToggleBit(this byte bitfield, byte bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		#endregion

		#region Clear

		public static long ClearBit(this long bitfield, long bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static ulong ClearBit(this ulong bitfield, ulong bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static int ClearBit(this int bitfield, int bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static uint ClearBit(this uint bitfield, uint bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static short ClearBit(this short bitfield, short bit)
		{
			bitfield &= (short)~bit;
			return bitfield;
		}

		public static ushort ClearBit(this ushort bitfield, ushort bit)
		{
			bitfield &= (ushort)~bit;
			return bitfield;
		}

		public static byte ClearBit(this byte bitfield, byte bit)
		{
			bitfield &= (byte)~bit;
			return bitfield;
		}

		#endregion

		#region Bit values

		#region Extraction

		public static long ExtractValue(this long bitfield, long mask, int shift)
		{
			return (bitfield & mask) >> shift;
		}

		public static ulong ExtractValue(this ulong bitfield, ulong mask, int shift)
		{
			return (bitfield & mask) >> shift;
		}

		public static int ExtractValue(this int bitfield, int mask, int shift)
		{
			return (bitfield & mask) >> shift;
		}

		public static uint ExtractValue(this uint bitfield, uint mask, int shift)
		{
			return (bitfield & mask) >> shift;
		}

		public static short ExtractValue(this short bitfield, short mask, int shift)
		{
			return (short)(((int)bitfield & mask) >> shift);
		}

		public static ushort ExtractValue(this ushort bitfield, ushort mask, int shift)
		{
			return (ushort)(((int)bitfield & mask) >> shift);
		}

		public static byte ExtractValue(this byte bitfield, byte mask, int shift)
		{
			return (byte)(((int)bitfield & mask) >> shift);
		}

		#endregion

		#region Setting

		public static long SetValue(this long bitfield, long value, long bitmask, int shift, uint valueMaxBitLength)
		{
			long maxValue = MathHelpers.Pow(2, valueMaxBitLength) - 1;
			long actualValue = (value > maxValue ? maxValue : value) << shift;
			return (bitfield | actualValue);
		}

		public static ulong SetValue(this ulong bitfield, ulong value, ulong bitmask, int shift, uint valueMaxBitLength)
		{
			ulong maxValue = (ulong)MathHelpers.Pow((long)2, valueMaxBitLength) - 1;
			ulong actualValue = (value > maxValue ? maxValue : value) << shift;
			return (bitfield | actualValue);
		}

		public static int SetValue(this int bitfield, int value, int bitmask, int shift, uint valueMaxBitLength)
		{
			int maxValue = MathHelpers.Pow(2, valueMaxBitLength) - 1;
			int actualValue = (value > maxValue ? maxValue : value) << shift;
			return (bitfield | actualValue);
		}

		public static uint SetValue(this uint bitfield, uint value, uint bitmask, int shift, uint valueMaxBitLength)
		{
			uint maxValue = (uint)MathHelpers.Pow(2, valueMaxBitLength) - 1;
			uint actualValue = (value > maxValue ? maxValue : value) << shift;
			return (bitfield | actualValue);
		}

		public static short SetValue(this short bitfield, short value, short bitmask, int shift, uint valueMaxBitLength)
		{
			short maxValue = (short)(MathHelpers.Pow(2, valueMaxBitLength) - 1);
			int actualValue = (value > maxValue ? maxValue : value) << shift;
			return (short)((int)bitfield | actualValue);
		}

		public static ushort SetValue(this ushort bitfield, ushort value, ushort bitmask, int shift, uint valueMaxBitLength)
		{
			ushort maxValue = (ushort)(MathHelpers.Pow(2, valueMaxBitLength) - 1);
			int actualValue = (value > maxValue ? maxValue : value) << shift;
			return (ushort)((int)bitfield | actualValue);
		}

		public static byte SetValue(this byte bitfield, byte value, byte bitmask, int shift, uint valueMaxBitLength)
		{
			byte maxValue = (byte)(MathHelpers.Pow(2, valueMaxBitLength) - 1);
			int actualValue = (value > maxValue ? maxValue : value) << shift;
			return (byte)((int)bitfield | actualValue);
		}

		#endregion

		#endregion
	}
}

