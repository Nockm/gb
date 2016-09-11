using System;
using System.Collections.Generic;
using System.IO;

namespace Z80
{
    public class State
    {
        #region Registers
        // The GameBoy has instructions & registers similiar to the 8080, 8085, & Z80 microprocessors.
        // The internal 8-bit registers are A, B, C, D, E, F, H & L.
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte F { get; set; }
        public byte L { get; set; }
        public byte H { get; set; }

        // These registers may be used in pairs for 16-bit operations as AF, BC, DE & HL.
        public ushort AF { get { return (ushort)((A << 8) & F); } set { A = (byte)(value >> 8); F = (byte)value; } }
        public ushort BC { get { return (ushort)((B << 8) & C); } set { B = (byte)(value >> 8); C = (byte)value; } }
        public ushort DE { get { return (ushort)((D << 8) & E); } set { D = (byte)(value >> 8); E = (byte)value; } }
        public ushort HL { get { return (ushort)((H << 8) & L); } set { H = (byte)(value >> 8); L = (byte)value; } }

        // Zero Flag (Z): This bit is set when the result of a math operation is zero or two values match when using the CP instruction.
        public bool _Z { get { return (F & 128) != 0; } set { F |= 128; } }

        // Subtract Flag (N): This bit is set if a subtraction was performed in the last math instruction. 
        public bool _N { get { return (F & 64) != 0; } set { F |= 64; } }

        // Half Carry Flag (H): This bit is set if a carry occurred from the lower nibble in the last math operation. 
        public bool _H { get { return (F & 32) != 0; } set { F |= 32; } }

        // Carry Flag (C): This bit is set if a carry occurred from the last math operation or if register A is the smaller value when executing the CP instruction.
        public bool _C { get { return (F & 16) != 0; } set { F |= 16; } }

        // The program counter. Holds the point in memory that the processor is executing code from. No function can change PC except by actually jumping to a different location in memory.
        public ushort PC { get; set; }

        // The stack pointer. Holds the current address of the top of the stack.
        public ushort SP { get; set; }
        #endregion

        #region RAM
        byte[] ramArray;
        MemoryStream ramStream;
        internal void WriteU8(byte val, long offset)
        {
            ramArray[offset] = val;
        }

        internal void WriteU16(ushort val, long offset)
        {
            WriteBytes(BitConverter.GetBytes(val), offset);
        }

        internal void WriteBytes(byte[] data, long offset)
        {
            ramStream.Position = offset;
            ramStream.Write(data, 0, data.Length);
        }

        internal byte ReadU8(int offset)
        {
            return ramArray[offset];
        }

        internal ushort ReadU16(int offset)
        {
            return BitConverter.ToUInt16(ramArray, offset);
        }
        #endregion

        public State()
        {
            // The following is the distribution of the 64KB address space provided by the Game Boy’s memory system:
            //    0x0000-0x3FFF: Permanently-mapped ROM bank.
            //    0x4000-0x7FFF: Area for switchable ROM banks.
            //    0x8000-0x9FFF: Video RAM.
            //    0xA000-0xBFFF: Area for switchable external RAM banks.
            //    0xC000-0xCFFF: Game Boy’s working RAM bank 0 .
            //    0xD000-0xDFFF: Game Boy’s working RAM bank 1.
            //    0xFE00-0xFEFF: Sprite Attribute Table.
            //    0xFF00-0xFF7F: Devices’ Mappings. Used to access I/O devices.
            //    0xFF80-0xFFFE: High RAM Area.
            //    0xFFFF: Interrupt Enable Register.
            ramArray = new byte[64 * 1024];
            ramStream = new MemoryStream(ramArray);
        }

        #region Debugging
        public override string ToString()
        {
            List<string> sb = new List<string>();

            sb.Add(String.Format("A={0:x2}", A));
            sb.Add(String.Format("B={0:x2}", B));
            sb.Add(String.Format("C={0:x2}", C));
            sb.Add(String.Format("D={0:x2}", D));
            sb.Add(String.Format("E={0:x2}", E));
            sb.Add(String.Format("F={0:x2}", F));
            sb.Add(String.Format("L={0:x2}", L));
            sb.Add(String.Format("H={0:x2}", H));

            sb.Add(String.Format("AF={0:x4}", AF));
            sb.Add(String.Format("BC={0:x4}", BC));
            sb.Add(String.Format("DE={0:x4}", DE));
            sb.Add(String.Format("HL={0:x4}", HL));
            sb.Add(String.Format("SP={0:x4}", SP));
            sb.Add(String.Format("PC={0:x4}", PC));

            return String.Join(" ", sb.ToArray()).ToUpper();
        }
        #endregion
    }
}