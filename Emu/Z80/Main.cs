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
            byte opcode = Machine.ReadU8();
            Instruction[] instructionSet = Machine.Default;

            //DebugOpcode debugOpcode = new DebugOpcode(opcode);

            if (opcode == 0xCB)
            {
                instructionSet = Machine.CB;
                opcode = Machine.ReadU8();
            }

            return instructionSet[opcode];
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
        }
    }
}