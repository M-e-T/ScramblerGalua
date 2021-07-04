using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using ScramblerGalua.Interface;

namespace ScramblerGalua.Model
{
    public class Polinom : IPolinom
    {
        private CancellationTokenSource cts;
        private CancellationToken token;

        public event Action Progres;
        public event Action Сomplete;

        private int Power;
        private int PowerDiv;
        private int Bits;

        public bool isCansel { get; private set; } = false;

        public void Cansel()
        {
            cts.Cancel();
        }
        public Task<BigInteger> Generate(int power)
        {
            Power = power;
            PowerDiv = power + 1;
            cts = new CancellationTokenSource();
            token = cts.Token;
            BigInteger min = BigInteger.Pow(2, (int)power) + 1;
            BigInteger max = min * 2;
            Bits = BitOll((BigInteger.Pow(2, BitOll(min) - 1)));

            var taskList = Enumerable.Range(0, Environment.ProcessorCount).Select(x => Task.Run(() => GenerateBigInteger(Power/8)));
            var result = Task.Factory.ContinueWhenAny(taskList.ToArray(), x => x.Result);
            return result;
        }

        private BigInteger GenerateBigInteger(int countBytes)
        {
            BigInteger Vector = 2;
            var randomBigInteger = new RandomBigInteger();          
            while (token.IsCancellationRequested == false)
            {
                BigInteger testPolinom = randomBigInteger.RandomPolinom(countBytes);
                if ((BitInt(testPolinom) & 1) == 1)
                {
                    int Res = 2;
                    BigInteger _Res = Div2(Vector, Vector << 1, testPolinom);
                    while (_Res > 1 && Res < PowerDiv)
                    {
                        Res += 1;
                        _Res = Div2(_Res, _Res << 1, testPolinom);
                    }
                    if (_Res == 1 && Res == Power)
                    {
                        cts.Cancel();
                        return testPolinom;
                    }
                }
            }
            return BigInteger.Zero;
        }
        private BigInteger Div2(BigInteger a, BigInteger b, BigInteger i)
        {
            BigInteger p = 0;
            while (b > 0)
            {
                p ^= -(b & 1) & a;
                a = (a << 1) ^ (i & -((a >> Bits) & 1));
                b >>= 1;
            }
            return p;
        }
        public int BitInt(BigInteger i)
        {
            int count = 0;
            while (i != 0)
            {
                if ((i & 1) == 1)
                    count++;
                i >>= 1;
            }
            return count;
        }
        public int BitOll(BigInteger i)
        {
            int count = 0;
            while (i != 0)
            {
                count++;
                i >>= 1;
            }
            return count - 1;
        }
    }
}
