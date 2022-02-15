namespace WildernessSurvival
{
    public static class ByteTools
    {
        /// <summary>
        /// 将指定位置一
        /// </summary>
        /// <param name="target">待置一的二进制数</param>
        /// <param name="digit">需置一的位</param>
        public static int SetBit(int target, int digit)
        {
            return target | (1 << digit);
        }

        /// <summary>
        /// 将指定位置零
        /// </summary>
        /// <param name="target">待置零的二进制数</param>
        /// <param name="digit">需置零的位</param>
        public static int ClearBit(int target, int digit)
        {
            return target & (~(1 << digit));
        }

        /// <summary>
        /// 指定的位取反
        /// </summary>
        /// <param name="target">待取反的二进制数</param>
        /// <param name="digit">需取反的位</param>
        public static int ReverseBit(int target, int digit)
        {
            return target & (1 << digit);
        }

        /// <summary>
        /// 获取的某一位的值
        /// </summary>
        /// <param name="target">待获取的二进制数</param>
        /// <param name="digit">需获取的位</param>
        public static int Getbit(int target, int digit)
        {
            return target ^ (1 << digit);
        }
        /// <summary>
        /// 指定位是否开启
        /// </summary>
        /// <param name="target">待查看的二进制数</param>
        /// <param name="digit">需查看的位</param>
        /// <returns></returns>
        public static bool IsOn(int target,int digit)
        {
            return ((target >> digit) & 1) != 0;
        }
    }
}
