
using System.Numerics;


namespace ScramblerGalua.Model
{
    public class BitOperation
    {
        public BigInteger TrimLeft(BigInteger value)
        {
            return SetBit(value, PowerPolinom(value));
        }
        public BigInteger Trim(BigInteger polinom)
        {
            polinom >>= 1;
            polinom = SetBit(polinom, PowerPolinom(polinom));
            return polinom;
        }
        public BigInteger SetBit(BigInteger value, int bit)
        {
            return value ^ (1L << bit);
        }
        public BigInteger GetBit(BigInteger value, int bit)
        {
            return value >> bit & 1;
        }
        public BigInteger ZeroingBit(BigInteger value, int bit)
        {
            return value & ~(1L << bit);
        }
        public int PowerPolinom(BigInteger polinom) => BitOll(polinom) - 1;

        public int BitOll(BigInteger value)
        {
            int res = 0;
            while (value != 0)
            {
                res++;
                value >>= 1;
            }
            return res;
        }
        public int BitInt(BigInteger value)
        {
            int res = 0;
            while (value != 0)
            {
                if ((value & 1) == 1)
                    res++;
                value >>= 1;
            }
            return res;
        }
    }
}
