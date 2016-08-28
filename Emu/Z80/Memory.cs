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
        /// The following is the distribution of the 64KB address space provided by the Game Boy’s memory system:
        ///    0x0000-0x3FFF: Permanently-mapped ROM bank.
        ///    0x4000-0x7FFF: Area for switchable ROM banks.
        ///    0x8000-0x9FFF: Video RAM.
        ///    0xA000-0xBFFF: Area for switchable external RAM banks.
        ///    0xC000-0xCFFF: Game Boy’s working RAM bank 0 .
        ///    0xD000-0xDFFF: Game Boy’s working RAM bank 1.
        ///    0xFE00-0xFEFF: Sprite Attribute Table.
        ///    0xFF00-0xFF7F: Devices’ Mappings. Used to access I/O devices.
        ///    0xFF80-0xFFFE: High RAM Area.
        ///    0xFFFF: Interrupt Enable Register.

        MemoryStream data = new MemoryStream(64 * 1024);
        byte[] buf = new byte[1024];

        private void ReadBytes(int numBytes)
        {
            data.Read(buf, 0, numBytes);
        }

        public void Write(byte[] buffer, int offset)
        {
            data.Position = offset;
            data.Write(buffer, 0, buffer.Length);
        }

        internal byte ReadU8(int offset)
        {
            data.Seek(offset, SeekOrigin.Begin);
            ReadBytes(1);
            return buf[0];
        }

        internal ushort ReadU16(int offset)
        {
            data.Seek(offset, SeekOrigin.Begin);
            ReadBytes(2);
            return BitConverter.ToUInt16(buf, 0);
        }
    }
}
