namespace Blueshift.System.Chip8
{
    public interface IBus
    {
        /// <summary>
        /// Reads a single byte at the specified address. Address is wrapped around 0x1000.
        /// </summary>
        byte Read(ushort address);

        /// <summary>
        /// Writes a single byte at the specified address. Address is wrapped around 0x1000.
        /// </summary>
        void Write(ushort address, byte value);

        /// <summary>
        /// Writes a block of data into the RAM.
        /// </summary>
        void WriteBlock(byte[] data, int address = 0x0200);
    }
}
