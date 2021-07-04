using System;
using System.Numerics;
using System.Threading.Tasks;

namespace ScramblerGalua.Interface
{
    public interface IPolinom
    {
        Task<BigInteger> Generate(int power);
        event Action Progres;
        event Action Сomplete;
    }
}
