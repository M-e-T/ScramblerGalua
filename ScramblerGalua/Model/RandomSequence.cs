using System.Numerics;

using ScramblerGalua.Interface;

namespace ScramblerGalua.Model
{
    public class RandomSequence : MatrixGalua
    {
        public BigInteger Vector { get; private set; }
        public RandomSequence(Ikey key):base(key.Polinom,key.Omega, key.Matrix)
        {
            Vector = key.Vector;
        }
        public BigInteger NextByte()
        {
            BigInteger result = 0;
            for (int j = 0; j < MatrixOrder; j++)
            {
                if ((Vector & 1) == 1)
                    result ^= Matrix[j];
                Vector >>= 1;
            }
            Vector = result;
            return Vector;
        }
    }
}
