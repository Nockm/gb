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

        public Instruction[] Default = new Instruction[255];
        public Instruction[] CB = new Instruction[255];

        public Machine()
        {
            Default[0x01] = new Instruction("LD BC nn", () => { State.BC = ReadU16(); });
            Default[0x11] = new Instruction("LD DE nn", () => { State.DE = ReadU16(); });
            Default[0x21] = new Instruction("LD HL nn", () => { State.HL = ReadU16(); });
            Default[0x31] = new Instruction("LD SP nn", () => { State.SP = ReadU16(); });

            Default[0x02] = new Instruction("LD (BC), A", () => { State.WriteU16(val: State.A, offset: State.BC); State.HL--; });
            Default[0x12] = new Instruction("LD (DE), A", () => { State.WriteU16(val: State.A, offset: State.DE); State.HL--; });
            Default[0x22] = new Instruction("LD (HL+), A", () => { State.WriteU16(val: State.A, offset: State.HL); State.HL++; });
            Default[0x32] = new Instruction("LD (HL-), A", () => { State.WriteU16(val: State.A, offset: State.HL); State.HL--; });

            Default[0xAF] = new Instruction("XOR A", () => { XOR(State.A); });
        }

        public void Load(byte[] data, long offset)
        {
            State.WriteBytes(data, offset);
        }

        #region Helpers
        /// <summary>
        /// Checks if a bit is one. Result is returned to the Zero flag.
        /// </summary>
        private void Bit(short flagIndex, uint val)
        {
            uint mask = ((uint)1 << flagIndex);
            uint masked = val & mask;
            State._Z = masked == 0;
        }

        /// <summary>
        /// Sets the bit (bit=1)
        /// </summary>
        private void Set(short flagIndex, ref byte reg)
        {
            reg |= (byte)(1 << flagIndex);
        }

        /// <summary>
        /// Resets the bit (bit=0)
        /// </summary>
        private void Res(short flagIndex, ref byte reg)
        {
            reg &= (byte)~(1 << flagIndex);
        }

        public void XOR(byte s)
        {
            State.A ^= s;
            SetFlags(State.A == 0, false, false, false);
        }

        public byte ReadU8()
        {
            byte s = State.ReadU8(State.PC);
            State.PC += 1;
            return s;
        }

        public ushort ReadU16()
        {
            ushort s = State.ReadU16(State.PC);
            State.PC += 2;
            return s;
        }

        private void SetFlags(bool z, bool n, bool h, bool c)
        {
            State._Z = z;
            State._N = n;
            State._H = h;
            State._C = c;
        }
        #endregion
    }
}
