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
        public State State = new State();

        public Dictionary<byte, Instruction[]> InstructionSet = new Dictionary<byte, Instruction[]>
        {
            // Single-byte opcode bank.
            { 0x00, new Instruction[255] }, 

            // Double-byte opcode banks.
            { 0xCB, new Instruction[255] },
            { 0xED, new Instruction[255] },
            { 0xDD, new Instruction[255] },
            { 0xFD, new Instruction[255] },
        };

        public Machine()
        {
            // http://marc.rawer.de/Gameboy/Docs/GBCPUman.pdf
            // http://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html
            InstructionSet[0x00][0x00] = new Instruction(4, "NOP", () => { }); // No operation.
            InstructionSet[0x00][0x01] = new Instruction(4, "STOP 0", () => { }); // Halt CPU & LCD display until button pressed.
            InstructionSet[0x00][0x01] = new Instruction(3, "LD BC nn", () => { State.BC = ReadU16(); });
            InstructionSet[0x00][0x11] = new Instruction(3, "LD DE nn", () => { State.DE = ReadU16(); });
            InstructionSet[0x00][0x21] = new Instruction(3, "LD HL nn", () => { State.HL = ReadU16(); });
            InstructionSet[0x00][0x31] = new Instruction(3, "LD SP nn", () => { State.SP = ReadU16(); });
            InstructionSet[0x00][0x02] = new Instruction(8, "LD (BC), A", () => { State.WriteU16(val: State.A, offset: State.BC); State.HL--; });
            InstructionSet[0x00][0x12] = new Instruction(8, "LD (DE), A", () => { State.WriteU16(val: State.A, offset: State.DE); State.HL--; });
            InstructionSet[0x00][0x22] = new Instruction(8, "LD (HL+), A", () => { State.WriteU16(val: State.A, offset: State.HL); State.HL++; });
            InstructionSet[0x00][0x32] = new Instruction(8, "LD (HL-), A", () => { State.WriteU16(val: State.A, offset: State.HL); State.HL--; });
            InstructionSet[0x00][0xAF] = new Instruction(4, "XOR A", () => { XOR(State.A); });

            InstructionSet[0x00][0xC3] = new Instruction(4, "JP a16", () => { State.PC = ReadU16(updatePC: false); });

            // If following condition is true then add n to current address and jump to it:
            // Use with:
            // = one byte signed immediate value
            // = NZ, Jump if Z flag is reset.
            // = Z,  Jump if Z flag is set.
            // = NC, Jump if C flag is reset.
            // = C,  Jump if C flag is set.
            // Opcodes:
            // Instruction  Parameters      Opcode  Cycles
            //           NZ,*            20         8
            //           Z,*             28         8
            //           NC,*            30         8
            //           C,*             38         8
            InstructionSet[0x00][0x20] = new Instruction(12 / 8, "JR NZ,r8", () =>
            {
                if (State._Z)
                {
                    byte d = ReadU8();
                    State.PC = d;
                }
            });

            InstructionSet[0xCB][0x7C] = new Instruction(8, "BIT 7,H", () => { BIT(7, State.H); });
        }

        private void ScratchPad()
        {
            SET(3, State.B);
        }

        public void Load(byte[] data, long offset)
        {
            State.WriteBytes(data, offset);
        }

        #region Helpers
        /// <summary>
        /// Checks if a bit is one. Result is returned to the Zero flag.
        /// </summary>
        private void BIT(short flagIndex, uint val)
        {
            // Z - Set if bit b of register r is 0.
            // N - Reset.
            // H - Set.
            // C - Not affected
            uint mask = ((uint)1 << flagIndex);
            uint masked = val & mask;
            SetZNHC(masked == 0, false, true, null);
        }

        /// <summary>
        /// Sets the bit (bit=1)
        /// </summary>
        private byte SET(short flagIndex, byte reg)
        {
            return (byte)(reg | (byte)(1 << flagIndex));
        }

        /// <summary>
        /// Resets the bit (bit=0)
        /// </summary>
        private void RES(short flagIndex, ref byte reg)
        {
            reg &= (byte)~(1 << flagIndex);
        }

        public void XOR(byte s)
        {
            State.A ^= s;
            SetZNHC(State.A == 0, false, false, false);
        }

        public byte ReadU8()
        {
            byte s = State.ReadU8(State.PC);
            State.PC += 1;
            return s;
        }

        public ushort ReadU16(bool updatePC = true)
        {
            ushort s = State.ReadU16(State.PC);

            if (updatePC)
            {
                State.PC += 2;
            }

            return s;
        }

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
