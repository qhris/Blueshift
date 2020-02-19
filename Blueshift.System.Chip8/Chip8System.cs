using OpenTK.Input;

namespace Blueshift.System.Chip8
{
    public class Chip8System
    {
        private readonly KeyMapper _keyMapper;

        public Chip8System()
        {
            _keyMapper = new KeyMapper();
            Memory = new RandomAccessMemory();
            PPU = new PPU(Memory);
            CPU = new Processor(Memory, PPU);
        }

        public Processor CPU { get; }
        public PPU PPU { get; }
        public IBus Memory { get; }

        public void Power()
        {
            CPU.Power();
            PPU.Power();
        }

        public void KeyDown(Key key)
        {
            byte? systemKey = _keyMapper.MapKey(key);
            if (systemKey.HasValue)
            {
                CPU.KeyDown(systemKey.Value);
            }
        }

        public void KeyUp(Key key)
        {
            byte? systemKey = _keyMapper.MapKey(key);
            if (systemKey.HasValue)
            {
                CPU.KeyUp(systemKey.Value);
            }
        }
    }
}
