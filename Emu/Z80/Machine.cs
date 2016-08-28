using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Machine
    {
        State State = new State();
        Memory Memory = new Memory();
        Instruction[] noPrefix = new Instruction[255];
        Instruction[] cbPrefix = new Instruction[255];

        public Machine()
        {
            InitInstructions();
        }

        private void InitInstructions()
        {
            noPrefix[0x01] = new Instruction("LD BC nn", () => { State.BC = ReadU16(); });
            noPrefix[0x11] = new Instruction("LD DE nn", () => { State.DE = ReadU16(); });
            noPrefix[0x21] = new Instruction("LD HL nn", () => { State.HL = ReadU16(); });
            noPrefix[0x31] = new Instruction("LD SP nn", () => { State.SP = ReadU16(); });
            noPrefix[0xAF] = new Instruction("XOR A", () => { XOR(State.A); });
        }

        private void XOR(byte s)
        {
            State.A ^= s;
            State._Z = State.A == 0;
        }

        private byte ReadU8()
        {
            byte s = Memory.ReadU8(State.PC);
            State.PC += 1;
            return s;
        }

        private ushort ReadU16()
        {
            ushort s = Memory.ReadU16(State.PC);
            State.PC += 2;
            return s;
        }

        public void Run()
        {
            while (true)
            {
                Cycle();
            }
        }

        private void Cycle()
        {
            Instruction instruction = FetchAndDecode();
            instruction.Action();
        }

        public Instruction FetchAndDecode()
        {
            byte opcode = ReadU8();

            DebugOpcode debugOpcode = new DebugOpcode(opcode);

            switch (opcode)
            {
                case 0xCB: return cbPrefix[ReadU8()];
                default: return noPrefix[opcode];
            }
        }

        public void Startup()
        {
            /// The boot ROM is a bootstrap program which is a 256 bytes big piece of
            /// code which checks the cartridge header is correct, scrolls the
            /// Nintendo bootup graphics and plays the "po-ling" sound.
            byte[] bootstrap = Properties.Resources.Bootstrap;

            /// When the Gameboy is turned on, the bootstrap ROM is situated in a
            /// memory page at positions $0-$FF (0-255). The CPU enters at $0 at
            /// startup, and the last two instructions of the code writes to a special
            /// register which disables the internal ROM page, thus making the lower
            /// 256 bytes of the cartridge ROM readable. The last instruction is
            /// situated at position $FE and is two bytes big, which means that right
            /// after that instruction has finished, the CPU executes the instruction
            /// at $100, which is the entry point code on a cartridge.
            Memory.Write(bootstrap, 0);

            // http://gbdev.gg8.se/wiki/articles/Gameboy_Bootstrap_ROM
        }
    }
}
