using System.Collections.Generic;
using OpenTK.Input;

namespace Blueshift.System.Chip8
{
    public class KeyMapper
    {
        private readonly Dictionary<Key, byte> _mappedKeys;

        public KeyMapper()
        {
            _mappedKeys = new Dictionary<Key, byte>
            {
                [Key.Number1] = 0x01,
                [Key.Number2] = 0x02,
                [Key.Number3] = 0x03,
                [Key.Number4] = 0x0C,
                [Key.Q] = 0x04,
                [Key.W] = 0x05,
                [Key.E] = 0x06,
                [Key.R] = 0x0D,
                [Key.A] = 0x07,
                [Key.S] = 0x08,
                [Key.D] = 0x09,
                [Key.F] = 0x0E,
                [Key.Z] = 0x0A,
                [Key.X] = 0x00,
                [Key.C] = 0x0B,
                [Key.V] = 0x0F,
            };
        }

        public byte? MapKey(Key key)
        {
            if (_mappedKeys.TryGetValue(key, out byte value))
            {
                return value;
            }

            return null;
        }
    }
}
