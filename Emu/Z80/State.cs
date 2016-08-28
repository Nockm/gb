using System.Runtime.InteropServices;

namespace Z80
{
    [StructLayout(LayoutKind.Explicit)]
    public class State
    {
        // The GameBoy has instructions & registers similiar to the 8080, 8085, & Z80 microprocessors.
        // The internal 8-bit registers are A, B, C, D, E, F, H, & L.
        // Theses registers may be used in pairs for 16-bit operations as AF, BC, DE, & HL.

        [FieldOffset(7)]
        public byte H;
        [FieldOffset(6)]
        public byte L;
        [FieldOffset(6)]
        public ushort HL;

        [FieldOffset(5)]
        public byte D;
        [FieldOffset(4)]
        public byte E;
        [FieldOffset(4)]
        public ushort DE;

        [FieldOffset(3)]
        public byte B;
        [FieldOffset(2)]
        public byte C;
        [FieldOffset(2)]
        public ushort BC;

        [FieldOffset(1)]
        public byte A;
        // The F register holds the cpu flags. The operation of these flags is identical to their Z80 relative. The lower four bits of this register always read zero even if written with a one.
        // 7 6 5 4 3 2 1 0
        // Z N H C 0 0 0 0
        [FieldOffset(0)]
        public byte F;
        [FieldOffset(0)]
        public ushort AF;

        // The two remaining 16-bit registers are the program counter (PC) and the stack pointer (SP).
        [FieldOffset(8)]
        public ushort SP;
        [FieldOffset(10)]
        public ushort PC;

        // Zero Flag (Z): This bit is set when the result of a math operation is zero or two values match when using the CP instruction.
        public bool _Z { get { return (F & 7) != 0; } set { F |= 7; } }

        // Subtract Flag (N): This bit is set if a subtraction was performed in the last math instruction. 
        public bool _N { get { return (F & 6) != 0; } set { F |= 6; } }

        // Half Carry Flag (H): This bit is set if a carry occurred from the lower nibble in the last math operation. 
        public bool _H { get { return (F & 5) != 0; } set { F |= 5; } }

        // Carry Flag (C): This bit is set if a carry occurred from the last math operation or if register A is the smaller value when executing the CP instruction.
        public bool _C { get { return (F & 4) != 0; } set { F |= 4; } }
    }
}