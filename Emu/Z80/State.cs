using System;
using System.Collections.Generic;
using System.IO;

namespace Z80
{
    /// <summary>
    /// Represents the state of the Machine (register and memory contents).
    /// This state should be able to be saved and loaded to resume playback.
    /// </summary>
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
        public ushort AF { get { return (ushort)((A << 8) | F); } set { A = (byte)(value >> 8); F = (byte)value; } }
        public ushort BC { get { return (ushort)((B << 8) | C); } set { B = (byte)(value >> 8); C = (byte)value; } }
        public ushort DE { get { return (ushort)((D << 8) | E); } set { D = (byte)(value >> 8); E = (byte)value; } }
        public ushort HL { get { return (ushort)((H << 8) | L); } set { H = (byte)(value >> 8); L = (byte)value; } }

        // Zero Flag       (Z) This bit is set when the result of a math operation is zero or two values match when using the CP instruction.
        // Subtract Flag   (N) This bit is set if a subtraction was performed in the last math instruction. 
        // Half Carry Flag (H) This bit is set if a carry occurred from the lower nibble in the last math operation. 
        // Carry Flag      (C) This bit is set if a carry occurred from the last math operation or if register A is the smaller value when executing the CP instruction.
        public bool _Z { get { return Flag.Get(F, 7); } set { if (value) F = Flag.Set(F, 7); else F = Flag.Reset(F, 7); } }
        public bool _N { get { return Flag.Get(F, 6); } set { if (value) F = Flag.Set(F, 6); else F = Flag.Reset(F, 6); } }
        public bool _H { get { return Flag.Get(F, 5); } set { if (value) F = Flag.Set(F, 5); else F = Flag.Reset(F, 5); } }
        public bool _C { get { return Flag.Get(F, 4); } set { if (value) F = Flag.Set(F, 4); else F = Flag.Reset(F, 4); } }

        // The program counter. Holds the point in memory that the processor is executing code from. No function can change PC except by actually jumping to a different location in memory.
        public ushort PC { get; set; }

        // The stack pointer. Holds the current address of the top of the stack.
        public ushort SP { get; set; }
        #endregion

        #region RAM
        /// <summary>
        /// 64k RAM allocation.
        /// </summary>
        byte[] ramArray;

        /// <summary>
        /// Wrapper around ramArray for easier operations in bulk, e.g. loading a ROM.
        /// </summary>
        MemoryStream ramStream;

        /// <summary>
        /// Write 1 byte at the specified offset.
        /// </summary>
        internal void WriteU8(byte val, long offset)
        {
            ramArray[offset] = val;
        }

        /// <summary>
        /// Write 2 bytes at the specified offset.
        /// </summary>
        internal void WriteU16(ushort val, long offset)
        {
            WriteBytes(BitConverter.GetBytes(val), offset);
        }

        /// <summary>
        /// Write bytes at the specified offset.
        /// </summary>
        internal void WriteBytes(byte[] data, long offset)
        {
            ramStream.Position = offset;
            ramStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Read 1 byte at the specified offset.
        /// </summary>
        internal byte ReadU8(int offset)
        {
            return ramArray[offset];
        }

        /// <summary>
        /// Read 2 bytes at the specified offset.
        /// </summary>
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
        /// <summary>
        /// Override ToString() so the debugger shows the state for easier debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SerializeRegisters();
        }

        /// <summary>
        /// A nicely-formatted and comparable string representation of the Registers.
        /// </summary>
        private string SerializeRegisters()
        {
            // Produce output which looks like:
            // AF=01B0 BC=0013 DE=00D8 HL=014D SP=FFFE PC=0150
            List<string> sb = new List<string>();

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