
namespace Z80
{
    /// <summary>
    /// The opcode (operation code) is a single byte whose bit pattern indicates the 
    /// operation we need the Z80 to perform (register loading, arithmetic, I/O, etc.). 
    /// The opcode may also contain information regarding the operation's parameters 
    /// (operands), e.g. the registers which will be used/affected by the operation.
    /// 
    /// -------------------------------------------------
    /// |  7  |  6  |  5  |  4  |  3  |  2  |  1  |  0  |
    /// -------------------------------------------------
    /// |     x     |        y        |        z        |
    /// -------------------------------------------------
    /// |           |     p     |  q  |                 |
    /// -------------------------------------------------
    /// </summary>
    public struct DebugOpcode
    {
        public int x, y, z;
        public int p, q;

        public DebugOpcode(byte b)
        {
            x = (b >> 6) & 3;
            y = (b >> 3) & 7;
            z = (b >> 0) & 7;
            p = (b >> 4) & 3;
            q = (b >> 3) & 1;
        }
    }
}
