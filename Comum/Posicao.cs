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

        public static Posicao operator +(Posicao a, Posicao b)
        {
            return new Posicao(a.x + b.x, a.y + b.y);
        }

        public static Posicao operator -(Posicao a, Posicao b)
        {
            return new Posicao(a.x - b.x, a.y - b.y);
        }

        public static double operator !(Posicao vetor)
        {
            return Math.Sqrt(vetor.x * vetor.x + vetor.y * vetor.y);
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

        static public bool operator ==(Posicao a, Posicao b)
        {
            if (a.x == b.x && a.y == b.y) return true;

            return false;
        }

        static public bool operator !=(Posicao a, Posicao b)
        {
            if (a.x != b.x || a.y != b.y) return true;

            return false;
        }

        static public Direcao Sentido(Posicao a, Posicao b)
        {
            var subtraidos = b - a;
            if (!(subtraidos.x == 0 ^ subtraidos.y == 0))
            {
                throw new Exception("Sentido nulo");
            }

            if (subtraidos.x < 0) return Direcao.Cima;
            if (subtraidos.x > 0) return Direcao.Baixo;
            if (subtraidos.y > 0) return Direcao.Direita;
            if (subtraidos.y < 0) return Direcao.Esquerda;

            throw new Exception("Erro desconhecido no sentido calculado");
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