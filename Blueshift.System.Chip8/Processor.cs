using System;
using Blueshift.Core.Extensions;

namespace Blueshift.System.Chip8
{
    public partial class Processor
    {
        enum KeyState
        {
            Released,
            Pressed,
        }

        private readonly IBus _bus;
        private readonly PPU _ppu;

        private Random _random;
        private ushort[] _stack;
        private KeyState[] _keys;
        private bool _isWaitingForKeypress;
        private int _keyPressRegister;

        private byte _delayTimer;
        private byte _soundTimer;

        public Processor(IBus bus, PPU ppu)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _ppu = ppu ?? throw new ArgumentNullException(nameof(ppu));

            _random = new Random();
            _stack = new ushort[0x20];
            _keys = new KeyState[0x10];
            V = new byte[16];
            PC = 0;
        }

        delegate byte Algorithm(byte a, byte b);

        public byte[] V { get; private set; }

        public ushort I { get; private set; }

        public ushort PC { get; private set; }

        public int SP { get; private set; }

        public void Power()
        {
            I = 0x0000;
            SP = 0x00;
            PC = 0x0200;
        }

        public void KeyDown(byte key)
        {
            if (key >= 0x10)
            {
                throw new ArgumentException("Key must be in the range [0x00:0x1F] inclusive.", nameof(key));
            }

            _keys[key] = KeyState.Pressed;
            if (_isWaitingForKeypress)
            {
                V[_keyPressRegister] = key;
                _isWaitingForKeypress = false;
            }
        }

        public void KeyUp(byte key)
        {
            if (key >= 0x10)
            {
                throw new ArgumentException("Key must be in the range [0x00:0x1F] inclusive.", nameof(key));
            }

            _keys[key] = KeyState.Released;
        }

        public void Execute()
        {
            if (_delayTimer > 0)
            {
                _delayTimer--;
            }

            if (_soundTimer > 0)
            {
                _soundTimer--;
            }

            if (_isWaitingForKeypress)
            {
                return;
            }

            ushort instruction = ReadInstruction();
            ushort address = instruction.And(0x0FFF);
            byte value = (byte)instruction.And(0xFF);

            var nibbles = (
                (byte)((instruction & 0xF000) >> 12),
                (byte)((instruction & 0x0F00) >>  8),
                (byte)((instruction & 0x00F0) >>  4),
                (byte)((instruction & 0x000F) >>  0));

            Action instructionAction = nibbles switch
            {
                (0x0,   0x0,   0xE,   0x0) => () => InstructionClearScreen(),
                (0x0,   0x0,   0xE,   0xE) => () => InstructionReturnSubroutine(),
                (0x1,     _,     _,     _) => () => InstructionJump(address),
                (0x2,     _,     _,     _) => () => InstructionCall(address),
                (0x3, var x,     _,     _) => () => InstructionSkip(V[x] == value),
                (0x4, var x,     _,     _) => () => InstructionSkip(V[x] != value),
                (0x5, var x, var y,   0x0) => () => InstructionSkip(V[x] == V[y]),
                (0x6, var x,     _,     _) => () => V[x] = value,
                (0x7, var x,     _,     _) => () => InstructionAdd(x, value),
                (0x8, var x, var y,   0x0) => () => V[x] = V[y],
                (0x8, var x, var y,   0x1) => () => InstructionImplied(AlgorithmOR, ref V[x], V[y]),
                (0x8, var x, var y,   0x2) => () => InstructionImplied(AlgorithmAND, ref V[x], V[y]),
                (0x8, var x, var y,   0x3) => () => InstructionImplied(AlgorithmXOR, ref V[x], V[y]),
                (0x8, var x, var y,   0x4) => () => InstructionImplied(AlgorithmADD, ref V[x], V[y]),
                (0x8, var x, var y,   0x5) => () => InstructionImplied(AlgorithmSUB, ref V[x], V[y]),
                (0x8, var x, var y,   0x6) => () => InstructionImplied(AlgorithmSHR, ref V[x], V[y]),
                (0x8, var x, var y,   0x7) => () => InstructionImplied(AlgorithmSUBN, ref V[x], V[y]),
                (0x8, var x, var y,   0xE) => () => InstructionImplied(AlgorithmSHL, ref V[x], V[y]),
                (0x9, var x, var y,   0x0) => () => InstructionSkip(V[x] != V[y]),
                (0xA,     _,     _,     _) => () => I = address,
                (0xB,     _,     _,     _) => () => InstructionJump(address.Add(V[0x0])),
                (0xC, var x,     _,     _) => () => V[x] = (byte)(_random.Next(0, 255) & value),
                (0xD, var x, var y, var n) => () => InstructionDrawSprite(V[x], V[y], n),
                (0xE, var x,   0x9,   0xE) => () => InstructionSkip(_keys[V[x].And(0x0F)] == KeyState.Pressed),
                (0xE, var x,   0xA,   0x1) => () => InstructionSkip(_keys[V[x].And(0x0F)] == KeyState.Released),
                (0xF, var x,   0x0,   0x7) => () => V[x] = _delayTimer,
                (0xF, var x,   0x0,   0xA) => () => InstructionWaitKeypress(x),
                (0xF, var x,   0x1,   0x5) => () => _delayTimer = V[x],
                (0xF, var x,   0x1,   0x8) => () => _soundTimer = V[x],
                (0xF, var x,   0x1,   0xE) => () => I += V[x],
                (0xF, var x,   0x2,   0x9) => () => InstructionLoadSpriteAddress(V[x]),
                (0xF, var x,   0x3,   0x3) => () => InstructionBCD(V[x]),
                (0xF, var x,   0x5,   0x5) => () => InstructionStoreRegisters(x),
                (0xF, var x,   0x6,   0x5) => () => InstructionLoadRegisters(x),
                _ => () => { },
            };

            instructionAction?.Invoke();
        }

        private ushort ReadInstruction()
        {
            ushort value = ReadWord(PC);
            PC += 2;
            return value;
        }

        private void PushStack(ushort value)
        {
            _stack[SP++] = PC;

            if (SP >= _stack.Length)
            {
                SP = 0;
            }
        }

        private ushort PopStack()
        {
            --SP;

            if (SP < 0)
            {
                SP = (byte)(_stack.Length - 1);
            }

            return _stack[SP];
        }

        private ushort ReadWord(ushort address)
        {
            byte hi = _bus.Read(address++);
            byte lo = _bus.Read(address);

            return (ushort)((hi << 8) | lo);
        }
    }
}
