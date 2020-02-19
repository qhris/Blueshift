namespace Blueshift.System.Chip8
{
    internal class RandomAccessMemory : IBus
    {
        private byte[] _memory;

        public RandomAccessMemory()
        {
            _memory = new byte[0x1000];
        }

        public byte Read(ushort address)
            => _memory[address & 0x0FFF];

        public void Write(ushort address, byte value)
            => _memory[address & 0x0FFF] = value;

        public void WriteBlock(byte[] data, int address = 0x0200)
        {
            foreach (byte value in data)
            {
                Write((ushort)address, value);
                ++address;
            }
        }
    }
}
