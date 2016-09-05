namespace Z80
{
    public class Machine
    {
        Cpu Cpu = new Cpu();

        private Host host;

        public Machine(Host host)
        {
            this.host = host;
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
            instruction.Execute();
        }

        public Instruction FetchAndDecode()
        {
            byte opcode = Cpu.ReadU8();

            DebugOpcode debugOpcode = new DebugOpcode(opcode);

            switch (opcode)
            {
                case 0xCB:
                    {
                        opcode = Cpu.ReadU8();
                        debugOpcode = new DebugOpcode(opcode);
                        return Cpu.CB[opcode];
                    }
                default: return Cpu.Default[opcode];
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
            Cpu.Mem.WriteBytes(bootstrap, 0);

            // http://gbdev.gg8.se/wiki/articles/Gameboy_Bootstrap_ROM
        }
    }
}
