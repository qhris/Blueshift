namespace Blueshift.Core.Extensions
{
    public static class NumericExtensions
    {
        public static byte Add(this byte value, int rhs)
            => (byte)(value + rhs);

        public static ushort Add(this ushort value, int rhs)
            => (ushort)(value + rhs);

        public static bool IsBitSet(this byte value, int bit)
            => (value & (1 << bit)) != 0;

        public static bool IsBitSet(this sbyte value, int bit)
            => (value & (1 << bit)) != 0;

        public static bool IsBitSet(this short value, int bit)
            => (value & (1 << bit)) != 0;

        public static bool IsBitSet(this ushort value, int bit)
            => (value & (1 << bit)) != 0;

        public static bool IsBitSet(this int value, int bit)
            => (value & (1 << bit)) != 0;

        public static bool IsBitSet(this uint value, int bit)
            => (value & (1 << bit)) != 0;

        public static byte And(this byte value, byte rhs)
            => (byte)(value & rhs);

        public static ushort And(this ushort value, ushort rhs)
            => (ushort)(value & rhs);

        public static uint And(this uint value, uint rhs)
            => value & rhs;

        public static ushort RShift(this ushort value, int bits)
            => (ushort)(value >> bits);
    }
}
