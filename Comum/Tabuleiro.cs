using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Tabuleiro
    {
        public List<Peca> pecas { get; private set; } = new List<Peca>();

        public List<List<Tuple<byte, byte>>> casas { get; private set; }


        public Tabuleiro(byte largura, byte altura)
        {
            casas = new List<List<Tuple<byte, byte>>>(altura);
            for (byte i = 0; i < altura; i++)
            {
                casas[i] = new List<Tuple<byte, byte>>(largura);
                for (byte j = 0; j < largura; j++)
                {
                    casas[i][j] = new Tuple<byte, byte>(i, j);
                }
            }
        }
    }
}