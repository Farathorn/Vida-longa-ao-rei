using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Rei : Peca
    {
        public Rei(Tabuleiro Tabuleiro, byte ID, Posicao Posicao) : base(Tabuleiro, ID, Posicao)
        {
            
        }

        public override bool Mover(Direcao sentido, int quanto)
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