using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Posicao
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Posicao(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Posicao(Posicao a, Posicao b)
        {
            x = a.x + b.x;
            y = a.y + b.y;
        }

        public Posicao Somar(Posicao somante)
        {
            x += somante.x;
            y += somante.y;

            return this;
        }

        static public Posicao Somar(Posicao a, Posicao b)
        {
            return new Posicao(a.x + b.x, a.y + b.y);
        }
    }

    public enum Direcao
    {
        Cima,
        Esquerda,
        Direita,
        Baixo
    }
}