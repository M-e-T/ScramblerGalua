using System;

namespace ScramblerGalua.Interface
{
    public interface IEncrypting
    {
        event Action<int> Progres;
        void Encrypt(string fileInput, string fileOutput);
    }
}
