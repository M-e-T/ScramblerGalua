using ScramblerGalua.Model;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ScramblerGalua.Interface
{
    public interface Ikey
    {
        TypeMatrix Matrix { get; }
        BigInteger Polinom { get; }
        BigInteger Omega { get; }
        BigInteger Vector { get; }
    }
}
