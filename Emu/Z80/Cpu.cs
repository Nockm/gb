using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Cpu
    {
        public Reg Reg { get; set; }
        public Mem Mem { get; set; }

        public Instruction[] Default = new Instruction[255];
        public Instruction[] CB = new Instruction[255];

        public Cpu()
        {
            Reg = new Reg();
            Mem = new Mem();

            Default[0x01] = new Instruction("LD BC nn", () => { Reg.BC = ReadU16(); });
            Default[0x11] = new Instruction("LD DE nn", () => { Reg.DE = ReadU16(); });
            Default[0x21] = new Instruction("LD HL nn", () => { Reg.HL = ReadU16(); });
            Default[0x31] = new Instruction("LD SP nn", () => { Reg.SP = ReadU16(); });

            Default[0x02] = new Instruction("LD (BC), A", () => { Mem.WriteU16(val: Reg.A, offset: Reg.BC); Reg.HL--; });
            Default[0x12] = new Instruction("LD (DE), A", () => { Mem.WriteU16(val: Reg.A, offset: Reg.DE); Reg.HL--; });
            Default[0x22] = new Instruction("LD (HL+), A", () => { Mem.WriteU16(val: Reg.A, offset: Reg.HL); Reg.HL++; });
            Default[0x32] = new Instruction("LD (HL-), A", () => { Mem.WriteU16(val: Reg.A, offset: Reg.HL); Reg.HL--; });

            Default[0xAF] = new Instruction("XOR A", () => { XOR(Reg.A); });

            for (short y = 0; y < 8; y++)
                for (short z = 0; z < 8; z++)
                    CB[(64 * 1) + (8 * y) + z] = BIT(y, z);
        }

        private Instruction BIT(short flagIndex, short rIndex)
        {
            string disassembly = String.Format("BIT {0}, {1}", flagIndex, LutRString[rIndex]);
            Action execute = () =>
            {
                Bit(flagIndex, LutRGetValue(rIndex));
            };
            Instruction instruction = new Instruction(disassembly, execute);
            return instruction;
        }

        private Instruction SET(short flagIndex, short rIndex)
        {
            string disassembly = String.Format("SET {0}, {1}", flagIndex, LutRString[rIndex]);
            Action execute = () =>
            {
                byte val = LutRGetValue(rIndex);
                Set(flagIndex, ref val);
                LutRSetValue(rIndex, val);
                Bit(flagIndex, LutRGetValue(rIndex));
            };
            Instruction instruction = new Instruction(disassembly, execute);
            return instruction;
        }

        private void ScratchPad()
        {

        }

        #region Helpers
        /// <summary>
        /// Checks if a bit is one. Result is returned to the Zero flag.
        /// </summary>
        private void Bit(short flagIndex, uint val)
        {
            uint mask = ((uint)1 << flagIndex);
            uint masked = val & mask;
            Reg._Z = masked == 0;
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
            Reg.A ^= s;
            SetFlags(Reg.A == 0, false, false, false);
        }

        public byte ReadU8()
        {
            byte s = Mem.ReadU8(Reg.PC);
            Reg.PC += 1;
            return s;
        }

        public ushort ReadU16()
        {
            ushort s = Mem.ReadU16(Reg.PC);
            Reg.PC += 2;
            return s;
        }

        private void SetFlags(bool z, bool n, bool h, bool c)
        {
            Reg._Z = z;
            Reg._N = n;
            Reg._H = h;
            Reg._C = c;
        }
        #endregion

        #region Table "r"
        private byte LutRGetValue(int index)
        {
            if (index == 0) return Reg.B;
            else if (index == 1) return Reg.C;
            else if (index == 2) return Reg.D;
            else if (index == 3) return Reg.E;
            else if (index == 4) return Reg.H;
            else if (index == 5) return Reg.L;
            else if (index == 6) return Mem.ReadU8(Reg.HL);
            else if (index == 7) return Reg.A;
            else throw new Exception();
        }

        private void LutRSetValue(int index, byte val)
        {
            if (index == 0) Reg.B = val;
            else if (index == 1) Reg.C = val;
            else if (index == 2) Reg.D = val;
            else if (index == 3) Reg.E = val;
            else if (index == 4) Reg.H = val;
            else if (index == 5) Reg.L = val;
            else if (index == 6) Mem.WriteU8(val, Reg.HL);
            else if (index == 7) Reg.A = val;
            else throw new Exception();
        }

        string[] LutRString = new string[] { "B", "C", "D", "E", "H", "L", "HL", "A" };
        #endregion
    }
}
