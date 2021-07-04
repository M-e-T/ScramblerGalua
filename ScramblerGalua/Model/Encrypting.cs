using System;
using System.IO;

using ScramblerGalua.Interface;

namespace ScramblerGalua.Model
{
    class Encrypting: IEncrypting
    {
        private RandomSequence Sequence;
        public event Action<int> Progres;

        public Encrypting(Ikey key)
        {
            Sequence = new RandomSequence(key);
        }
        public void Encrypt(string fileInput, string fileOutput)
        {
            byte[] inputByte = OpenFile(fileInput);
            byte[] outputByte = new byte[inputByte.Length];
            int persent = outputByte.Length / 100 == 0 ? 1 : inputByte.Length / 100;
            persent++;
            for (int i = 0; i < outputByte.Length; i++)
            {
                byte[] byteKey = Sequence.NextByte().ToByteArray();
                byte gamma = 0;
                for (int j = 0; j < byteKey.Length; j++)
                {
                    gamma ^= byteKey[j];
                }
                outputByte[i] = (byte)(inputByte[i] ^ gamma);
                if (i % persent == 0)
                {
                    int progress = (int)((double)i / inputByte.Length * 100);
                    Progres.Invoke(progress);
                }    
            }
            SaveFile(fileOutput, outputByte);
        }
        private byte[] OpenFile(string fileInput)
        {
            return File.ReadAllBytes(fileInput);
        }
        private void SaveFile(string fileOutput, byte[] bytes)
        {
            File.WriteAllBytes(fileOutput, bytes);
        }
    }
}
