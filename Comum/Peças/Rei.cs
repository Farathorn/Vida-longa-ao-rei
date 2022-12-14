using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Rei : Peca
    {
        public Rei(Rei copiando, Tabuleiro? tabuleiro = null) : base(copiando, tabuleiro)
        {

        }

        public Rei(byte ID, Posicao Posicao) : base(ID, Posicao)
        {

        }

        public override bool Mover(Direcao sentido, int quanto)
        {
            if (this.Tabuleiro is null) throw new Exception("Peça não está em um tabuleiro.");

            Casa? casaVelha = Tabuleiro.GetCasa(Posicao);
            if (casaVelha is null) throw new Exception("Peça estava fora do tabuleiro. Erro indevído.");

            foreach (List<Casa> coluna in Tabuleiro.casas)
            {
                foreach (Casa casa in coluna)
                {
                    casa.Movida = false;
                }
            }

            List<List<Casa>> casas = Tabuleiro.casas;
            Casa? novaCasa = null;
            if (sentido is Direcao.Cima || sentido is Direcao.Baixo)
            {
                if (sentido is Direcao.Cima) quanto -= 2 * quanto;
                if ((sentido is Direcao.Cima && Posicao.x + quanto < 0) || (sentido is Direcao.Baixo && Posicao.x + quanto > Tabuleiro.casas[0].Count))
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = Tabuleiro.GetCasa(Posicao.Somar(Posicao, new(quanto, 0)));
                if (novaCasa is null)
                    return false;

                if (sentido is Direcao.Cima)
                {
                    for (int i = Posicao.x - 1; i > novaCasa.Coordenada.x; i--)
                    {
                        if (casas[i][Posicao.y].condicao is Casa.Condicao.Ocupada)
                            return false;
                    }

                    for (int i = Posicao.x; i > novaCasa.Coordenada.x; i--)
                    {
                        casas[i][Posicao.y].Movida = true;
                    }
                }
                if (sentido is Direcao.Baixo)
                {
                    for (int i = Posicao.x + 1; i < novaCasa.Coordenada.x; i++)
                    {
                        if (casas[i][Posicao.y].condicao is Casa.Condicao.Ocupada)
                            return false;
                    }

                    for (int i = Posicao.x; i < novaCasa.Coordenada.x; i++)
                    {
                        casas[i][Posicao.y].Movida = true;
                    }
                }
            }
            else
            {
                if (sentido is Direcao.Esquerda) quanto -= 2 * quanto;
                if ((sentido is Direcao.Esquerda && Posicao.y + quanto < 0) || (sentido is Direcao.Direita && Posicao.y + quanto > Tabuleiro.casas[0].Count - 1))
                    throw new ArgumentException("Nova posição está fora do tabuleiro.");

                novaCasa = Tabuleiro.GetCasa(Posicao.Somar(Posicao, new(0, quanto)));
                if (novaCasa is null)
                    return false;

                if (sentido is Direcao.Esquerda)
                {
                    for (int i = Posicao.y - 1; i > novaCasa.Coordenada.y; i--)
                    {
                        if (casas[Posicao.x][i].condicao is Casa.Condicao.Ocupada)
                            return false;
                    }

                    for (int i = Posicao.y; i > novaCasa.Coordenada.y; i--)
                    {
                        casas[Posicao.x][i].Movida = true;
                    }
                }
                if (sentido is Direcao.Direita)
                {
                    for (int i = Posicao.y + 1; i < novaCasa.Coordenada.y; i++)
                    {
                        if (casas[Posicao.x][i].condicao is Casa.Condicao.Ocupada)
                            return false;
                    }

                    for (int i = Posicao.y; i < novaCasa.Coordenada.y; i++)
                    {
                        casas[Posicao.x][i].Movida = true;
                    }
                }
            }

            casaVelha.Ocupante = this;
            casaVelha.condicao = Casa.Condicao.Ocupada;

            Movimento feito = new(origem: new(casaVelha),
                                destino: new(novaCasa),
                                peca: this);
            Tabuleiro.logMovimentos.Add(feito);

            if (novaCasa.condicao is Casa.Condicao.Ocupada)
                return false;


            casaVelha.condicao = Casa.Condicao.Desocupada;
            casaVelha.Ocupante = null;
            novaCasa.condicao = Casa.Condicao.Ocupada;
            novaCasa.Ocupante = this;
            Posicao = novaCasa.Coordenada;

            Tabuleiro.EfetivarJogada();

            return true;
        }
    }
}