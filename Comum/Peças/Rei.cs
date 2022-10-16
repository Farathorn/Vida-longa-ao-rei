using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Rei : Peca
    {
        public Rei(byte ID, Posicao Posicao) : base(ID, Posicao)
        {

        }

        public override bool Mover(Direcao sentido, int quanto)
        {
            if (this.Tabuleiro is null) throw new Exception("Peça não está em um tabuleiro.");

            List<List<Casa>> casas = Tabuleiro.casas;
            Casa novaCasa;
            if (sentido is Direcao.Cima || sentido is Direcao.Baixo)
            {
                if (sentido is Direcao.Baixo) quanto -= 2 * quanto;
                if (Posicao.y + quanto < 0 || Posicao.y + quanto > Tabuleiro.casas[0].Count)
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = casas[Posicao.x][Posicao.y + quanto];
            }
            else
            {
                if (sentido is Direcao.Esquerda) quanto -= 2 * quanto;
                if (Posicao.x + quanto < 0 || Posicao.x + quanto > Tabuleiro.casas[0].Count)
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = casas[Posicao.x + quanto][Posicao.y];
            }

            if (novaCasa.condicao is Casa.Condicao.Desocupada)
            {
                novaCasa.condicao = Casa.Condicao.Ocupada;
                novaCasa.Ocupante = this;
                Posicao = novaCasa.Coordenada;
            }
            else return false;

            return true;
        }
    }
}