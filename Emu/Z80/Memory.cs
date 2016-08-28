using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80
{
    public class Mem
    {
        byte[] array;
        MemoryStream stream;

        public Mem()
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
            array = new byte[64 * 1024];
            stream = new MemoryStream(array);
        }

        #region Write
        internal void WriteU8(byte val, long offset)
        {
            array[offset] = val;
        }

        internal void WriteU16(ushort val, long offset)
        {
            WriteBytes(BitConverter.GetBytes(val), offset);
        }

        internal void WriteBytes(byte[] data, long offset)
        {
            stream.Position = offset;
            stream.Write(data, 0, data.Length);
        }
        #endregion

        #region Read
        internal byte ReadU8(int offset)
        {
            return array[offset];
        }

        internal ushort ReadU16(int offset)
        {
            return BitConverter.ToUInt16(array, offset);
        }
        #endregion
    }
}
