using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public abstract class Peca
    {
        protected Tabuleiro? tabuleiro = null;
        public Tabuleiro? Tabuleiro
        {
            get
            {
                return tabuleiro;
            }
            set
            {
                if (tabuleiro is null) tabuleiro = value;
            }
        }
        protected byte ID { get; set; }
        public Posicao Posicao { get; protected set; }

        public Peca(byte ID, Posicao Posicao)
        {
            this.ID = ID;
            this.Posicao = Posicao;
        }

        public virtual bool Mover(Direcao sentido, int quanto)
        {
            if (this.Tabuleiro is null) throw new Exception("Peça não está em um tabuleiro.");

            List<List<Casa>> casas = Tabuleiro.casas;
            Casa? novaCasa = null;
            if (sentido is Direcao.Cima || sentido is Direcao.Baixo)
            {
                if (sentido is Direcao.Cima) quanto -= 2 * quanto;
                if (Posicao.x + quanto < 0 || Posicao.x + quanto > Tabuleiro.casas[0].Count)
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");


                novaCasa = Tabuleiro.GetCasa(Posicao.Somar(Posicao, new(quanto, 0)));
                if (novaCasa is null)
                    return false;

                if (sentido is Direcao.Cima)
                {
                    for (int i = Posicao.x - 1; i > novaCasa.Coordenada.x; i--)
                    {
                        if (casas[i][Posicao.y].condicao is Casa.Condicao.Ocupada || casas[i][Posicao.y].tipo is Casa.Tipo.Trono)
                            return false;
                    }
                }
                if (sentido is Direcao.Baixo)
                {
                    for (int i = Posicao.x + 1; i < novaCasa.Coordenada.x; i++)
                    {
                        if (casas[i][Posicao.y].condicao is Casa.Condicao.Ocupada || casas[i][Posicao.y].tipo is Casa.Tipo.Trono)
                            return false;
                    }
                }
            }
            else
            {
                if (sentido is Direcao.Esquerda) quanto -= 2 * quanto;
                if (Posicao.y + quanto < 0 || Posicao.y + quanto > Tabuleiro.casas[0].Count)
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = Tabuleiro.GetCasa(Posicao.Somar(Posicao, new(0, quanto)));
                if (novaCasa is null)
                    return false;

                if (sentido is Direcao.Esquerda)
                {
                    for (int i = Posicao.y - 1; i > novaCasa.Coordenada.y; i--)
                    {
                        if (casas[Posicao.x][i].condicao is Casa.Condicao.Ocupada || casas[Posicao.x][i].tipo is Casa.Tipo.Trono)
                            return false;
                    }
                }
                if (sentido is Direcao.Direita)
                {
                    for (int i = Posicao.y + 1; i < novaCasa.Coordenada.y; i++)
                    {
                        if (casas[Posicao.x][i].condicao is Casa.Condicao.Ocupada || casas[Posicao.x][i].tipo is Casa.Tipo.Trono)
                            return false;
                    }
                }
            }


            if (novaCasa.tipo is Casa.Tipo.Refugio || novaCasa.tipo is Casa.Tipo.Trono) return false;


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
    }
}