﻿using System;
using System.Numerics;
using System.Security.Cryptography;


namespace ScramblerGalua.Model
{
    public class RandomBigInteger : Random
    {
        public RandomBigInteger() : base()
        {
        }
        public RandomBigInteger(int Seed) : base(Seed)
        {

        }
        RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        public BigInteger RandomPolinom(int n)
        {
            byte[] bytes = new byte[n];
            rng.GetBytes(bytes);
            BigInteger res = (1 << 8) ^ bytes[0];
            for (int i = 1; i < bytes.Length; i++)
            {
                res = (res << 8) ^ bytes[i];
            }
            return res |= 1;
        }
        public long LongRandom(long min, long max)
        {
            byte[] buf = new byte[8];
            NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }
        public BigInteger NextBigInteger(int bitLength)
        {
            if (bitLength < 1) return BigInteger.Zero;

            int bytes = bitLength / 8;
            int bits = bitLength % 8;

            // Generates enough random bytes to cover our bits.
            byte[] bs = new byte[bytes + 1];
            NextBytes(bs);

            // Mask out the unnecessary bits.
            byte mask = (byte)(0xFF >> (8 - bits));
            bs[bs.Length - 1] &= mask;

            return new BigInteger(bs);
        }
        public BigInteger NextBigInteger(BigInteger start, BigInteger end)
        {
            if (start == end) return start;

            BigInteger res = end;

            // Swap start and end if given in reverse order.
            if (start > end)
            {
                end = start;
                start = res;
                res = end - start;
            }
            else
                // The distance between start and end to generate a random BigIntger between 0 and (end-start) (non-inclusive).
                res -= start;

            byte[] bs = res.ToByteArray();

            // Count the number of bits necessary for res.
            int bits = 8;
            byte mask = 0x7F;
            while ((bs[bs.Length - 1] & mask) == bs[bs.Length - 1] && mask > 0)
            {
                bits--;
                mask >>= 1;
            }
            bits += 8 * bs.Length;
            // Generate a random BigInteger that is the first power of 2 larger than res, 
            // then scale the range down to the size of res,
            // finally add start back on to shift back to the desired range and return.
            return ((NextBigInteger(bits + 1) * res) / BigInteger.Pow(2, bits + 1)) + start;
        }
    }
}
