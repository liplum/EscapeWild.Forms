namespace WildernessSurvival
{
    public static class ByteTools
    {
        public static int SetBitOn(this int target, int digit)
        {
            return target | (1 << digit);
        }

        public static int SetBitOff(this int target, int digit)
        {
            return target & ~(1 << digit);
        }

        public static int ReverseBit(this int target, int digit)
        {
            return target & (1 << digit);
        }

        public static int GetBitAt(this int target, int digit)
        {
            return target ^ (1 << digit);
        }

        public static bool IsBitOn(this int target, int digit)
        {
            return ((target >> digit) & 1) != 0;
        }
    }
}