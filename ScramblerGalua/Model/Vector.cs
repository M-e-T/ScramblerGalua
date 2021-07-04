using System;
using System.Numerics;
using System.Security.Cryptography;

namespace ScramblerGalua.Model
{
    public class Vector
    {
        public BigInteger Generate(int power)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] numBytes = new byte[power];
                rng.GetBytes(numBytes);
                numBytes[numBytes.Length - 1] = 0;
                BigInteger Vector = new BigInteger(numBytes) % new BigInteger(Math.Pow(2, power));
                return Vector;
            }
        }
    }
}
