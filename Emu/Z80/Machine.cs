using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Machine
    {
        /// <summary>
        /// The internal state of the machine: Registers and RAM.
        /// </summary>
        public State State = new State();

        /// <summary>
        /// Implementation of instructions, indexed by opcode.
        /// </summary>
        public Dictionary<byte, Instruction[]> InstructionSet = new Dictionary<byte, Instruction[]>
        {
            // Allocate storage for single-byte opcode bank.
            { 0x00, new Instruction[255] }, 

            // Allocate storage for double-byte opcode banks.
            { 0xCB, new Instruction[255] },
            { 0xED, new Instruction[255] },
            { 0xDD, new Instruction[255] },
            { 0xFD, new Instruction[255] },
        };

        public Machine()
        {
            InitialiseInstructionSet();
        }

        #region Implementation
        /// <summary>
        /// The hard work implementing all opcodes.
        /// http://marc.rawer.de/Gameboy/Docs/GBCPUman.pdf
        /// http://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html
        /// </summary>
        private void InitialiseInstructionSet()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////
            // NOP: No operation.
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0x00][0x00] = new Instruction(4, "NOP", () => { });

            ///////////////////////////////////////////////////////////////////////////////////////////
            // STOP: Halt CPU & LCD display until button pressed.
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0x00][0x01] = new Instruction(4, "STOP 0", () => { });

            ///////////////////////////////////////////////////////////////////////////////////////////
            // LD n,nn: Put value nn into n.
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0x00][0x01] = new Instruction(3, "LD BC nn", () => { State.BC = ReadU16(); });
            InstructionSet[0x00][0x11] = new Instruction(3, "LD DE nn", () => { State.DE = ReadU16(); });
            InstructionSet[0x00][0x21] = new Instruction(3, "LD HL nn", () => { State.HL = ReadU16(); });
            InstructionSet[0x00][0x31] = new Instruction(3, "LD SP nn", () => { State.SP = ReadU16(); });

            ///////////////////////////////////////////////////////////////////////////////////////////
            // LD (nn) A
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0x00][0x02] = new Instruction(8, "LD (BC), A", () => { State.WriteU16(val: State.A, offset: State.BC); });
            InstructionSet[0x00][0x12] = new Instruction(8, "LD (DE), A", () => { State.WriteU16(val: State.A, offset: State.DE); });
            InstructionSet[0x00][0x22] = new Instruction(8, "LD (HL+), A", () => { State.WriteU16(val: State.A, offset: State.HL); State.HL++; });
            InstructionSet[0x00][0x32] = new Instruction(8, "LD (HL-), A", () => { State.WriteU16(val: State.A, offset: State.HL); State.HL--; });

            ///////////////////////////////////////////////////////////////////////////////////////////
            // XOR: Logical exclusive OR n with register A, result in A.
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Flags affected:
            // Z - Set if result is zero.
            // N - Reset.
            // H - Reset.
            // C - Reset.
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0x00][0xAF] = new Instruction(4, "XOR A", () => { State.A ^= State.A; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xA8] = new Instruction(4, "XOR B", () => { State.A ^= State.B; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xA9] = new Instruction(4, "XOR C", () => { State.A ^= State.C; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xAA] = new Instruction(4, "XOR D", () => { State.A ^= State.D; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xAB] = new Instruction(4, "XOR E", () => { State.A ^= State.E; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xAC] = new Instruction(4, "XOR H", () => { State.A ^= State.H; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xAD] = new Instruction(4, "XOR L", () => { State.A ^= State.L; SetZNHC(State.A == 0, false, false, false); });
            InstructionSet[0x00][0xAE] = new Instruction(4, "XOR (HL)", () => { State.A ^= State.ReadU8(State.HL); SetZNHC(State.A == 0, false, false, false); });
            //InstructionSet[0x00][0xEE] = new Instruction(4, "XOR *", () => { State.A ^= State.A; SetZNHC(State.A == 0, false, false, false); });

            InstructionSet[0x00][0xC3] = new Instruction(4, "JP a16", () => { State.PC = ReadU16(incrementPC: false); });

            ///////////////////////////////////////////////////////////////////////////////////////////
            // JR: If condition is true then add n to current address and jump to it.
            ///////////////////////////////////////////////////////////////////////////////////////////
            // ?? - one byte signed immediate value
            // NZ - Jump if Z flag is reset.
            // Z  - Jump if Z flag is set.
            // NC - Jump if C flag is reset.
            // C  - Jump if C flag is set.
            // Opcodes:
            // Instruction  Parameters      Opcode  Cycles
            // NZ,*            20         8
            // Z,*             28         8
            // NC,*            30         8
            // C,*             38         8
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0x00][0x20] = new Instruction(12 / 8, "JR NZ,r8", () => { if (State._Z) { State.PC += PC_ReadU8(); } });

            ///////////////////////////////////////////////////////////////////////////////////////////
            // BIT: Checks if a bit is one. Result is returned to the Zero flag.
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Z - Set if bit b of register r is 0.
            // N - Reset.
            // H - Set.
            // C - Not affected
            ///////////////////////////////////////////////////////////////////////////////////////////
            InstructionSet[0xCB][0x7C] = new Instruction(8, "BIT 7,H", () => { SetZNHC(!Flag.Get(State.H, 7), false, true, null); }); // 
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Read the next byte addressed by the Program Counter.
        /// </summary>
        public byte PC_ReadU8(bool incrementPC = true)
        {
            byte s = State.ReadU8(State.PC);
            if (incrementPC) State.PC += 1;
            return s;
        }

        /// <summary>
        /// Read the next 2 bytes addressed by the Program Counter.
        /// </summary>
        public ushort ReadU16(bool incrementPC = true)
        {
            ushort s = State.ReadU16(State.PC);
            if (incrementPC) State.PC += 2;
            return s;
        }

        /// <summary>
        /// Easy way to update all flags to set/reset/unchanged by specifying true/false/null respectively.
        /// </summary>
        private void SetZNHC(bool? z, bool? n, bool? h, bool? c)
        {
            if (z.HasValue) State._Z = z.Value;
            if (n.HasValue) State._N = n.Value;
            if (h.HasValue) State._H = h.Value;
            if (c.HasValue) State._C = c.Value;
        }
        #endregion
    }
}
