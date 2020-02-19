using Blueshift.Core.Extensions;

namespace Blueshift.System.Chip8
{
    public partial class Processor
    {
        private void InstructionImplied(Algorithm alu, ref byte output, byte input)
            => output = alu(output, input);

        private void InstructionAdd(int register, byte value)
        {
            V[register] += value;
        }

        private void InstructionSkip(bool condition)
        {
            if (condition)
            {
                PC += 2;
            }
        }

        private void InstructionJump(ushort address)
        {
            PC = address;
        }

        private void InstructionReturnSubroutine()
        {
            PC = PopStack();
        }

        private void InstructionClearScreen()
        {
            _ppu.ClearScreen();
        }

        private void InstructionCall(ushort address)
        {
            PushStack(PC);
            PC = address;
        }

        private void InstructionBCD(byte value)
        {
            _bus.Write(I.Add(0), (byte)(value / 100));
            _bus.Write(I.Add(1), (byte)(value % 100 / 10));
            _bus.Write(I.Add(2), (byte)(value % 10));
        }

        private void InstructionLoadRegisters(int last)
        {
            for (int x = 0; x <= last; ++x)
            {
                V[x] = _bus.Read((ushort)(I + x));
            }
        }

        private void InstructionStoreRegisters(int last)
        {
            for (int x = 0; x <= last; ++x)
            {
                _bus.Write((ushort)(I + x), V[x]);
            }
        }

        private void InstructionLoadSpriteAddress(byte value)
        {
            if (value <= 0x0F)
            {
                I = (ushort)(value * 5);
            }
        }

        private void InstructionWaitKeypress(int register)
        {
            _isWaitingForKeypress = true;
            _keyPressRegister = register;
        }

        private void InstructionDrawSprite(byte x, byte y, byte height)
        {
            bool collision = _ppu.DrawSprite(I, x, y, height);
            V[0xF] = (byte)(collision ? 1 : 0);
        }
    }
}
