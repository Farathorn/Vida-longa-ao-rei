using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public abstract class Peca
    {
        private Tabuleiro Tabuleiro { get; set; }
        private byte ID { get; set; }
        public Tuple<byte, byte> Coordenada { get; private set; }

        public enum Direcao
        {
            Cima,
            Esquerda,
            Direita,
            Baixo
        }

        public Peca(Tabuleiro Tabuleiro, byte ID, Tuple<byte, byte> Coordenada)
        {
            this.Tabuleiro = Tabuleiro;

            this.ID = ID;
            this.Coordenada = Coordenada;
        }

        public bool Mover(Direcao sentido, byte quanto)
        {
            List<List<Casa>> casas = Tabuleiro.casas;
            if (sentido is Direcao.Cima || sentido is Direcao.Baixo)
            {
                var novaCasa = casas[Coordenada.Item1][Coordenada.Item2 + quanto];

                if (novaCasa.condicao is Casa.Condicao.Desocupada)
                {
                    novaCasa.condicao = Casa.Condicao.Ocupada;
                    novaCasa.Ocupante = this;
                }
                else return false;
            }
            else
            {
                var novaCasa = casas[Coordenada.Item1 + quanto][Coordenada.Item2];

                if (novaCasa.condicao is Casa.Condicao.Desocupada)
                {
                    novaCasa.condicao = Casa.Condicao.Ocupada;
                    novaCasa.Ocupante = this;
                }
                else return false;
            }

            return true;
        }
    }
}