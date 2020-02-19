using System;
using Blueshift.Core.Extensions;

namespace Blueshift.System.Chip8
{
    public class PPU
    {
        private static readonly byte[] s_defaultSprites = new byte[]
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        private readonly IBus _bus;
        private bool[,] _video;

        public PPU(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            _bus = bus;
            _video = new bool[0x40, 0x20];
        }

        public int ResolutionX => 0x40;

        public int ResolutionY => 0x20;

        public void Power()
        {
            ClearScreen();
            _bus.WriteBlock(s_defaultSprites, address: 0x0000);
        }

        public bool DrawSprite(ushort address, byte x, byte y, byte height)
        {
            if (height == 0)
            {
                return false;
            }

            x = (byte)(x % 0x40);
            y = (byte)(y % 0x20);
            height = (byte)(height % 0x10);

            bool collision = false;
            for (int scanline = 0; scanline < height; ++scanline)
            {
                int drawY = (scanline + y) % 0x20;

                byte data = _bus.Read(address++);
                for (int bit = 0; bit < 8; ++bit)
                {
                    int drawX = (bit + x) % 0x40;

                    bool pixelValue = data.IsBitSet(7 - bit);
                    bool previousValue = _video[drawX, drawY];
                    bool newValue = pixelValue ^ previousValue;
                    _video[drawX, drawY] = newValue;

                    if (previousValue && !newValue)
                    {
                        collision = true;
                    }
                }
            }

            return collision;
        }

        public void ClearScreen()
        {
            for (int x = 0; x < ResolutionX; ++x)
            {
                for (int y = 0; y < ResolutionY; ++y)
                {
                    _video[x, y] = false;
                }
            }
        }

        public void FillFramebuffer(byte[] pixels)
        {
            if (pixels is null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }

            if (pixels.Length != ResolutionX * ResolutionY * 3)
            {
                throw new ArgumentException("invalid pixel buffer size", nameof(pixels));
            }

            int offset = 0;
            for (int y = 0; y < ResolutionY; ++y)
            {
                for (int x = 0; x < ResolutionX; ++x)
                {
                    byte value = (byte)(_video[x, y] ? 255 : 0);
                    pixels[offset + 0] = value;
                    pixels[offset + 1] = value;
                    pixels[offset + 2] = value;

                    offset += 3;
                }
            }
        }
    }
}
