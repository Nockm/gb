using System;
using System.Text;

namespace Z80
{
    public class Main
    {
        Machine Machine;
        Log Log;

        public void Run()
        {
            Machine = new Machine();
            Log = new Log();
            InitTetris();

            while (true)
            {
                // Optional but useful logging of the machine state.
                // Also acts as a unit test as it asserts the registers at each stage for the first N invoked instructions.
                Log.Update(Machine.State.ToString());

                // Perform one cycle of the Fetch-Decode-Execute loop.
                Step();
            }
        }

        /// <summary>
        /// Perform one cycle of the Fetch-Decode-Execute loop.
        /// </summary>
        private void Step()
        {
            Instruction instruction = FetchAndDecode();
            instruction.Execute();
        }

        /// <summary>
        /// Get the next instruction to execute.
        /// </summary>
        public Instruction FetchAndDecode()
        {
            byte bank = 0x00; // 0 is the default bank of instructions.
            byte opcode = Machine.PC_ReadU8();

            // An optional prefix byte may appear before the opcode, changing its meaning and causing the Z80 to 
            // look up the opcode in a different bank of instructions.
            switch (opcode)
            {
                // The prefix byte, if present, may have the values CB, DD, ED, or FD.
                // Although there are opcodes which have these values too, there is no ambiguity:
                // The first byte in the instruction, if it has one of these values, is always a prefix byte.
                case 0xCB:
                case 0xED:
                case 0xDD:
                case 0xFD:
                    {
                        bank = opcode;
                        opcode = Machine.PC_ReadU8();
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

        public void InitTetris()
        {
            // Load the ROM.
            Machine.State.WriteBytes(Properties.Resources.Tetris, 0);

            // Load the Expected states which acts as a unit test.
            Log.ExpectedStates = ExpectedStates.Tetris;

            // Initialise registers right before the first instruction is to be executed.
            // TODO: These registers are probably the same for every game, not just Tetris. If so, move to its own function.
            Machine.State.AF = 0x01B0;
            Machine.State.BC = 0x0013;
            Machine.State.DE = 0x00D8;
            Machine.State.HL = 0x014D;
            Machine.State.SP = 0xFFFE;
            Machine.State.PC = 0x0100;
        }

        private void LoadBootstrap()
        {
            // http://gbdev.gg8.se/wiki/articles/Gameboy_Bootstrap_ROM

            // The boot ROM is a bootstrap program which is a 256 bytes big piece of
            // code which checks the cartridge header is correct, scrolls the
            // Nintendo bootup graphics and plays the "po-ling" sound.
            byte[] bootstrap = Properties.Resources.Bootstrap;

            // When the Gameboy is turned on, the bootstrap ROM is situated in a
            // memory page at positions $0-$FF (0-255). The CPU enters at $0 at
            // startup, and the last two instructions of the code writes to a special
            // register which disables the internal ROM page, thus making the lower
            // 256 bytes of the cartridge ROM readable. The last instruction is
            // situated at position $FE and is two bytes big, which means that right
            // after that instruction has finished, the CPU executes the instruction
            // at $100, which is the entry point code on a cartridge.
            Machine.State.WriteBytes(bootstrap, 0);
        }
    }
}