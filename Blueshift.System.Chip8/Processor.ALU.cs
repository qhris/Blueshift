using Blueshift.Core.Extensions;

namespace Blueshift.System.Chip8
{
    public partial class Processor
    {
        private byte AlgorithmAND(byte x, byte y) => (byte)(x & y);

        private byte AlgorithmOR(byte x, byte y) => (byte)(x | y);

        private byte AlgorithmXOR(byte x, byte y) => (byte)(x ^ y);

        private byte AlgorithmADD(byte x, byte y)
        {
            int value = x + y;
            V[0xF] = (byte)(value > 255 ? 1 : 0);
            return (byte)(value);
        }

        private byte AlgorithmSUB(byte x, byte y)
        {
            V[0xF] = (byte)(x > y ? 1 : 0);
            return (byte)(x - y);
        }

        private byte AlgorithmSUBN(byte x, byte y)
        {
            V[0xF] = (byte)(y > x ? 1 : 0);
            return (byte)(y - x);
        }

        private byte AlgorithmSHR(byte x, byte _)
        {
            V[0xF] = (byte)(x.IsBitSet(0) ? 1 : 0);
            return (byte)(x >> 1);
        }

        private byte AlgorithmSHL(byte x, byte _)
        {
            V[0xF] = (byte)(x.IsBitSet(7) ? 1 : 0);
            return (byte)(x << 1);
        }
    }
}
