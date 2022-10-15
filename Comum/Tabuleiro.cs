using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Tabuleiro
    {
        public List<Peca> pecas { get; private set; } = new List<Peca>();

        public List<List<Casa>> casas { get; private set; }


        public Tabuleiro(byte largura, byte altura)
        {
            casas = new List<List<Casa>>(altura);
            for (byte i = 0; i < altura; i++)
            {
                casas[i] = new List<Casa>(largura);
                for (byte j = 0; j < largura; j++)
                {
                    casas[i][j] = new Casa(new Tuple<byte, byte>(i, j));
                }
            }
        }
    }

    public class Casa
    {
        public Tuple<byte, byte> Coordenada { get; set; }
        public enum Condicao
        {
            Ocupada,
            Desocupada
        }

        public Condicao condicao { get; set; } = Condicao.Desocupada;

        public Casa(Tuple<byte, byte> Coordenada)
        {
            this.Coordenada = Coordenada;
        }
    }
}