using System;
using System.Text;

namespace Z80
{
    public class Main
    {
        Machine Machine = new Machine();
        Log Log = new Log();

        public void Run()
        {
            while (true)
            {
                Log.Write(Machine.State.ToString());
                Step();
            }
        }

        private void Step()
        {
            Instruction instruction = FetchAndDecode();
            instruction.Execute();
        }

        public Instruction FetchAndDecode()
        {
            byte bank = 0x00;
            byte opcode = Machine.ReadU8();

            // An optional prefix byte may appear before the opcode, changing its meaning and causing the Z80 
            // to look up the opcode in a different bank of instructions.
            //
            // The prefix byte, if present, may have the values CB, DD, ED, or FD (these are hexadecimal values).
            //
            // Although there are opcodes which have these values too, there is no ambiguity: the first byte in 
            // the instruction, if it has one of these values, is always a prefix byte.
            switch (opcode)
            {
                case 0xCB:
                case 0xED:
                case 0xDD:
                case 0xFD:
                    {
                        bank = opcode;
                        opcode = Machine.ReadU8();
                        break;
                    }
            }

            Instruction instruction = Machine.InstructionSet[bank][opcode];
            DebugOpcode temp = new DebugOpcode(opcode);

            if (instruction == null)
            {
                throw new NotImplementedException(String.Format("Opcode {0:x2}{1:x2} not implemented!", bank, opcode));
            }

            return instruction;
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
            Machine.Load(bootstrap, 0);

            // http://gbdev.gg8.se/wiki/articles/Gameboy_Bootstrap_ROM

            byte[] tetris = Properties.Resources.Tetris;
            Machine.Load(tetris, 0x000);

            Machine.State.AF = 0x01B0;
            Machine.State.BC = 0x0013;
            Machine.State.DE = 0x00D8;
            Machine.State.HL = 0x014D;
            Machine.State.SP = 0xFFFE;
            Machine.State.PC = 0x0100;
        }
    }
}