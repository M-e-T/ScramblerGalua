using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using ScramblerGalua.Interface;
namespace ScramblerGalua.Model
{
    public class Omega : IOmega
    {
        private int Power;

        private CancellationTokenSource cts;
        private CancellationToken token;

        private List<BigInteger> Divisors = new List<BigInteger>()
        {
            3,5,15,17,51,85,255,257,771,1285,3855,4369,13107,21845,65535,65537,196611,327685,983055,1114129,3342387,5570645,16711935,
            16843009,50529027,84215045,252645135,286331153,858993459,1431655765,4294967295,4294967297,12884901891,21474836485,64424509455,
            73014444049,219043332147,365072220245,1095216660735,1103806595329,3311419785987,5519032976645,16557098929935,18764712120593,28470681808895,
            56294136361779,93823560602965,281479271743489,844437815230467,1407396358717445,4222189076152335,4785147619639313,14355442858917939,23925738098196565,
            71777214294589695,217020518514230019,361700864190383365,723401728380766673,1085102592571150095,1229782938247303441,3689348814741910323,6148914691236517205
        };
        public Omega(int power)
        {
            Power = power;
        }
        public Task<BigInteger> Generate(BigInteger polinom)
        {
            cts = new CancellationTokenSource();
            token = cts.Token;

            var taskList = Enumerable.Range(0, Environment.ProcessorCount).Select(x => Task.Run(() => GeneratePrimitiveOE(polinom)));
            var result = Task.Factory.ContinueWhenAny(taskList.ToArray(), x => x.Result);
            return result;
        }
        private BigInteger GeneratePrimitiveOE(BigInteger polinom)
        {
            BigInteger Omega;
            BigInteger polynomial = polinom;
            while (token.IsCancellationRequested == false)
            {
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    byte[] numBytes = new byte[Power / 8];
                    rng.GetBytes(numBytes);
                    Omega = new BigInteger(numBytes);
                }
                if (CheckOmega(Omega, polynomial))
                {
                    cts.Cancel();
                    return Omega;
                }
            }
            return BigInteger.Zero;
        }
        private bool CheckOmega(BigInteger OE, BigInteger polynomial)
        {
            BigInteger maximum = BigInteger.Pow(2, Power - 1);
            foreach (var divider in Divisors)
            {
                if (ModPower(OE, divider, polynomial, maximum) == 1) return false;
            }
            return true;
        }
        private BigInteger ModPower(BigInteger w, BigInteger k, BigInteger f, BigInteger max)
        {
            BigInteger result = 0, helper = 1, buffer = w;
            BigInteger n = 1;
            while (k > 0)
            {
                n <<= 1;
                if (n > k)
                {
                    n >>= 1;
                    k -= n;
                    n = 1;
                    helper = MultPow(w, helper, f, max);
                    w = buffer;
                    n <<= 1;
                }
                if (k == 1)
                {
                    result = MultPow(w, helper, f, max);
                    w = result;
                    k--;
                    break;
                }
                result = MultPow(w, w, f, max);
                w = result;
            }
            return result;
        }
        private BigInteger MultPow(BigInteger first, BigInteger second, BigInteger modulo, BigInteger max)
        {
            BigInteger result;
            if (first < second)
            {
                result = first;
                first = second;
                second = result;
            }
            result = 0;
            while (first > 0 && second > 0)
            {
                if ((second & 1) != 0)
                    result ^= first;
                if (first >= max)
                    first = (first << 1) ^ modulo;
                else
                    first <<= 1;
                second >>= 1;
            }
            return result;
        }
    }
}
