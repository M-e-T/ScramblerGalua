using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ScramblerGalua.Model
{
    public enum TypeMatrix : short
    {
        Galua,
        Fibonacci,
        GaluaT,
        FibonacciT,
        ReverseGalua,
        ReverseFibonacci,
        ReverseGaluaGaluaT,
        ReverseFibonacciT
    }
    public class MatrixGalua
    {
        public BigInteger Polinom { get; }
        public int MatrixOrder { get; }
        protected BigInteger[] Matrix { get; private set; }
        private List<Action> listActions = new List<Action>() { };
        private BigInteger Omega { get; set; }
        private int MaxLength { get; set; }
        public MatrixGalua(BigInteger polinom, BigInteger omega, TypeMatrix typeMatrix)
        {
            if (polinom <= 0)
                throw new ArgumentOutOfRangeException(nameof(polinom), "The argument must be above zero");
            if (omega <= 0)
                throw new ArgumentOutOfRangeException(nameof(omega), "The argument must be above zero");
            Polinom = polinom;
            Omega = omega;
            MatrixOrder = PowerPolinom(Polinom);
            MaxLength = (int)Math.Pow(2, MatrixOrder) - 1;
            listActions = new List<Action>
            {
                Galua,
                Fibonacci,
                GaluaT,
                FibonacciT,
            };
            Generate(typeMatrix);
        }
        public BigInteger this[int i]
        {
            get
            {
                return Matrix[i];
            }
        }
        private void Generate(TypeMatrix typeMatrix)
        {
            if ((int)typeMatrix > 3)
                Omega = InverseOmega(Omega);
            listActions[(int)typeMatrix % 4].Invoke();
        }
        private void Galua()
        {
            BigInteger _omega = Omega;
            Matrix = new BigInteger[MatrixOrder];
            for (int i = 0; i < MatrixOrder; i++)
            {
                Matrix[i] = _omega;
                _omega <<= 1;
                if (_omega > MaxLength)
                {
                    _omega = _omega ^ Polinom;
                }
            }
            Array.Reverse(Matrix);
        }
        private void GaluaT()
        {
            Galua();
            LeftHandTransposition(Matrix);
        }
        private void Fibonacci()
        {
            Galua();
            RightHandTransposition(Matrix);
        }
        private void FibonacciT()
        {
            Galua();
            RightHandTransposition(Matrix);
            LeftHandTransposition(Matrix);
        }
        private void LeftHandTransposition(BigInteger[] matrix)
        {
            BigInteger[] newMatrix = new BigInteger[MatrixOrder];
            for (int i = 0; i < MatrixOrder; i++)
            {
                int[] c = BigIntegerHelper.ToString(matrix[i], 2).PadLeft(MatrixOrder, '0').ToCharArray().Select(x1 => x1 - '0').ToArray();
                for (int j = 0; j < MatrixOrder; j++)
                {
                    newMatrix[j] <<= 1;
                    if (c[j] == 1)
                        newMatrix[j] += 1;
                }
            }
            Matrix = newMatrix;
        }
        private void RightHandTransposition(BigInteger[] matrix)
        {
            BigInteger[] newMatrix = new BigInteger[MatrixOrder];
            for (int i = MatrixOrder - 1; i >= 0; i--)
            {
                int[] c = BigIntegerHelper.ToString(matrix[i], 2).PadLeft(MatrixOrder, '0').ToCharArray().Select(x1 => x1 - '0').ToArray();
                Array.Reverse(c);
                for (int j = 0; j < MatrixOrder; j++)
                {
                    newMatrix[j] <<= 1;
                    if (c[j] == 1)
                        newMatrix[j] += 1;
                }
            }
            Matrix = newMatrix;
        }
        private BigInteger InverseOmega(BigInteger _omega)
        {
            BigInteger res = 0;
            while (_omega != 1)
            {
                _omega = Div2(_omega, _omega, Polinom, MatrixOrder - 1);
                res = _omega;
                _omega = Div2(_omega, Omega, Polinom, MatrixOrder - 1);
            }
            return res;
        }
        private BigInteger Div2(BigInteger a, BigInteger b, BigInteger c, int d)
        {
            BigInteger p = 0;
            while (b > 0)
            {
                p ^= -(b & 1) & a;
                BigInteger mask = -((a >> d) & 1);
                a = (a << 1) ^ (c & mask);
                b >>= 1;
            }
            return p;
        }
        private int PowerPolinom(BigInteger polinom) => BitOll(polinom) - 1;
        private int BitOll(BigInteger value)
        {
            int res = 0;
            while (value != 0)
            {
                res++;
                value >>= 1;
            }
            return res;
        }
    }
}
