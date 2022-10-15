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
        public Posicao Posicao { get; private set; }

        public Peca(Tabuleiro Tabuleiro, byte ID, Posicao Posicao)
        {
            this.Tabuleiro = Tabuleiro;

            this.ID = ID;
            this.Posicao = Posicao;
        }

        public bool Mover(Direcao sentido, int quanto)
        {
            List<List<Casa>> casas = Tabuleiro.casas;
            if (sentido is Direcao.Cima || sentido is Direcao.Baixo)
            {
                if (sentido is Direcao.Baixo) quanto -= 2 * quanto;
                var novaCasa = casas[Posicao.x][Posicao.y + quanto];

                if (novaCasa.condicao is Casa.Condicao.Desocupada)
                {
                    novaCasa.condicao = Casa.Condicao.Ocupada;
                    novaCasa.Ocupante = this;
                    Posicao = novaCasa.Coordenada;
                }
                else return false;
            }
            else
            {
                if (sentido is Direcao.Esquerda) quanto -= 2 * quanto;
                var novaCasa = casas[Posicao.x + quanto][Posicao.y];

                if (novaCasa.condicao is Casa.Condicao.Desocupada)
                {
                    novaCasa.condicao = Casa.Condicao.Ocupada;
                    novaCasa.Ocupante = this;
                    Posicao = novaCasa.Coordenada;
                }
                else return false;
            }

            return true;
        }
    }
}