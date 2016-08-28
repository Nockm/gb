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
        }

        public void InitInstructions()
        {
            Default[0x01] = new Instruction("LD BC nn", () => { Reg.BC = ReadU16(); });
            Default[0x11] = new Instruction("LD DE nn", () => { Reg.DE = ReadU16(); });
            Default[0x21] = new Instruction("LD HL nn", () => { Reg.HL = ReadU16(); });
            Default[0x31] = new Instruction("LD SP nn", () => { Reg.SP = ReadU16(); });
            Default[0xAF] = new Instruction("XOR A", () => { XOR(Reg.A); });
        }

        public void XOR(byte s)
        {
            Reg.A ^= s;
            Reg._Z = Reg.A == 0;
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
    }
}
