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
            Casa? novaCasa = null;
            if (sentido is Direcao.Cima || sentido is Direcao.Baixo)
            {
                if (sentido is Direcao.Baixo) quanto -= 2 * quanto;
                if (Posicao.y + quanto < 0 || Posicao.y + quanto > Tabuleiro.casas[0].Count)
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = Tabuleiro.GetCasa(Posicao.Somar(Posicao, new(0, quanto)));
            }
            else
            {
                if (sentido is Direcao.Esquerda) quanto -= 2 * quanto;
                if (Posicao.x + quanto < 0 || Posicao.x + quanto > Tabuleiro.casas[0].Count)
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = Tabuleiro.GetCasa(Posicao.Somar(Posicao, new(quanto, 0))); ;
            }

            if (novaCasa is not null)
            {
                if (novaCasa.condicao is Casa.Condicao.Ocupada)
                    return false;

                Casa? casaVelha = Tabuleiro.GetCasa(Posicao);
                if (casaVelha is null) throw new Exception("Peça estava fora do tabuleiro. Erro indevído.");

                Movimento feito = new(origem: casaVelha,
                                    destino: novaCasa,
                                    peca: this);

                casaVelha.condicao = Casa.Condicao.Desocupada;
                casaVelha.Ocupante = null;
                novaCasa.condicao = Casa.Condicao.Ocupada;
                novaCasa.Ocupante = this;
                Posicao = novaCasa.Coordenada;

                Tabuleiro.logMovimentos.Add(feito);
                Tabuleiro.EfetivarJogada();

                return true;
            }

            return false;
        }
    }
}