using System;
using System.Numerics;

using ScramblerGalua.Interface;

namespace ScramblerGalua.Model
{
    public class CryptoKey : BitOperation, Ikey
    {
        public TypeMatrix Matrix { get; }
        public BigInteger Polinom { get; }
        public BigInteger Omega { get; }
        public BigInteger Vector { get; }

        public CryptoKey(TypeMatrix tm, BigInteger polinom, BigInteger omega, BigInteger vector)
        {
            if (polinom < 7 || omega < 2 || vector < 1)
                throw new ArgumentException("The value was too small");
            if (PowerPolinom(polinom) - PowerPolinom(vector) < 1)
                throw new ArgumentException(nameof(vector), "Vector сan't be greater than or equal to polinom");
            if (PowerPolinom(polinom) - PowerPolinom(Omega) < 1)
                throw new ArgumentException(nameof(omega), "Omega сan't be greater than or equal to polinom");
            Matrix = tm;
            Polinom = polinom;
            Omega = omega;
            Vector = vector;
        }
    }
}
