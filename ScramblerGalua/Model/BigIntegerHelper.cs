using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ScramblerGalua.Model
{
    public static class BigIntegerHelper
    {
        public static string ToString(BigInteger value, int toBase)
        {
            string res = null;
            switch (toBase)
            {
                case 2:
                    res = ToBinaryString(value).Remove(0, 1);
                    break;
                case 8:
                    res = ToOctalString(value).Remove(0, 1);
                    break;
                case 16:
                    res = value.ToString("X");
                    if (res[0] == '0')
                        res = res.Remove(0, 1);
                    break;
            }
            return res;
        }
        public static BigInteger ToBigInteger(string value, int toBase)
        {
            if (toBase == 2) 
            {
                BigInteger res = 0;
                foreach (char c in value)
                {
                    res <<= 1;
                    res += c == '1' ? 1 : 0;
                }

                return res;
            }
            if (toBase == 8)
            {
                return value.Aggregate(new BigInteger(), (b, c) => b * 8 + c - '0');
            }
            if (toBase == 16)
            {
                return BigInteger.Parse("0" +value, NumberStyles.AllowHexSpecifier);
            }
            else
                throw new Exception();
        }
        private static string ToBinaryString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var index = bytes.Length - 1;
            var base2 = new StringBuilder(bytes.Length * 8);
            var binary = Convert.ToString(bytes[index], 2);
            if (binary[0] != '0' && bigint.Sign == 1) base2.Append('0');
            base2.Append(binary);
            for (index--; index >= 0; index--)
                base2.Append(Convert.ToString(bytes[index], 2).PadLeft(8, '0'));
            return base2.ToString();
        }
        /// <summary>
        ///     Converts from a BigInteger to a hexadecimal string.
        /// </summary>
        private static string ToHexString(this BigInteger bi)
        {
            var bytes = bi.ToByteArray();
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("X2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
        /// <summary>
        ///     Converts from a BigInteger to a octal string.
        /// </summary>
        private static string ToOctalString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var index = bytes.Length - 1;
            var base8 = new StringBuilder((bytes.Length / 3 + 1) * 8);
            var rem = bytes.Length % 3;
            if (rem == 0) rem = 3;
            var base0 = 0;
            while (rem != 0)
            {
                base0 <<= 8;
                base0 += bytes[index--];
                rem--;
            }

            var octal = Convert.ToString(base0, 8);
            if (octal[0] != '0' && bigint.Sign == 1) base8.Append('0');
            base8.Append(octal);
            while (index >= 0)
            {
                base0 = (bytes[index] << 16) + (bytes[index - 1] << 8) + bytes[index - 2];
                base8.Append(Convert.ToString(base0, 8).PadLeft(8, '0'));
                index -= 3;
            }

            return base8.ToString();
        }
    }
}
