using System.Numerics;
using System.Threading.Tasks;

namespace ScramblerGalua.Interface
{
    public interface IOmega
    {
        Task<BigInteger> Generate(BigInteger polinom);
    }
}
