namespace Z80
{
    /// <summary>
    /// Utilities for flag operations.
    /// </summary>
    public static class Flag
    {
        /// <summary>
        /// Get the result of setting the bit at <paramref name="position"/> for <paramref name="val"/> to 1.
        /// </summary>
        public static byte Set(byte val, short position)
        {
            return (byte)(val | (byte)(1 << position));
        }

        /// <summary>
        /// Get the result of setting the bit at <paramref name="position"/> for <paramref name="val"/> to 0.
        /// </summary>
        public static byte Reset(byte val, short position)
        {
            return val &= (byte)~(1 << position);
        }

        /// <summary>
        /// Get the bit at <paramref name="position"/> of <paramref name="val"/>.
        /// </summary>
        public static bool Get(byte val, short position)
        {
            return (val & ((uint)1 << position)) != 0;
        }
    }
}
