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
                tabuleiro = value;
            }
        }
        public byte ID { get; protected set; }
        public Posicao Posicao { get; protected set; }

        public Peca(Peca copiando, Tabuleiro? tabuleiro = null)
        {
            this.ID = copiando.ID;
            this.Posicao = copiando.Posicao;
            if (tabuleiro is not null)
            {
                this.Tabuleiro = tabuleiro;
                var novaCasa = Tabuleiro.GetCasa(Posicao);
                if (novaCasa is null) throw new Exception("Casa nula.");
                novaCasa.Ocupante = this;
                novaCasa.condicao = Casa.Condicao.Ocupada;
            }
        }

        public Peca(byte ID, Posicao Posicao)
        {
            this.ID = ID;
            this.Posicao = Posicao;
        }

        public List<Movimento> MovimentosPossiveis()
        {
            if (this.Tabuleiro is null) throw new Exception("Peça não está em um tabuleiro.");

            List<Movimento> movimentosPossiveis = new();

            var casas = Tabuleiro.casas;

            //Verificação de cima
            bool bloqueado = false;
            for (int i = Posicao.x - 1; i > 0; i--)
            {
                Casa analisanda = casas[i][Posicao.y];
                if (analisanda.condicao is Casa.Condicao.Ocupada || (this is not Rei && (analisanda.tipo is Casa.Tipo.Trono || analisanda.tipo is Casa.Tipo.Refugio)))
                    bloqueado = true;
                else if (bloqueado is not true && casas[i][Posicao.y].condicao is Casa.Condicao.Desocupada)
                {
                    Casa? esta = Tabuleiro.GetCasa(this);
                    if (esta is null) throw new Exception("Peça não está em um tabuleiro.");

                    movimentosPossiveis.Add(new Movimento(esta, analisanda, this));
                }
            }

            //Verificação de baixo
            bloqueado = false;
            for (int i = Posicao.x + 1; i < casas.Count; i++)
            {
                Casa analisanda = casas[i][Posicao.y];
                if (analisanda.condicao is Casa.Condicao.Ocupada || (this is not Rei && (analisanda.tipo is Casa.Tipo.Trono || analisanda.tipo is Casa.Tipo.Refugio)))
                    bloqueado = true;
                else if (bloqueado is not true && casas[i][Posicao.y].condicao is Casa.Condicao.Desocupada)
                {
                    Casa? esta = Tabuleiro.GetCasa(this);
                    if (esta is null) throw new Exception("Peça não está em um tabuleiro.");

                    movimentosPossiveis.Add(new Movimento(esta, analisanda, this));
                }
            }

            //Verificação da direita
            bloqueado = false;
            for (int i = Posicao.y + 1; i < casas[0].Count; i++)
            {
                Casa analisanda = casas[Posicao.x][i];
                if (analisanda.condicao is Casa.Condicao.Ocupada || (this is not Rei && (analisanda.tipo is Casa.Tipo.Trono || analisanda.tipo is Casa.Tipo.Refugio)))
                    bloqueado = true;
                else if (bloqueado is not true && casas[Posicao.x][i].condicao is Casa.Condicao.Desocupada)
                {
                    Casa? esta = Tabuleiro.GetCasa(this);
                    if (esta is null) throw new Exception("Peça não está em um tabuleiro.");

                    movimentosPossiveis.Add(new Movimento(esta, analisanda, this));
                }
            }

            //Verificação da esquerda
            bloqueado = false;
            for (int i = Posicao.y - 1; i > 0; i--)
            {
                Casa analisanda = casas[Posicao.x][i];
                if (analisanda.condicao is Casa.Condicao.Ocupada || (this is not Rei && (analisanda.tipo is Casa.Tipo.Trono || analisanda.tipo is Casa.Tipo.Refugio)))
                    bloqueado = true;
                else if (bloqueado is not true && casas[Posicao.x][i].condicao is Casa.Condicao.Desocupada)
                {
                    Casa? esta = Tabuleiro.GetCasa(this);
                    if (esta is null) throw new Exception("Peça não está em um tabuleiro.");

                    movimentosPossiveis.Add(new Movimento(esta, analisanda, this));
                }
            }

            return movimentosPossiveis;
        }

        public virtual bool Mover(Direcao sentido, int quanto)
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

                if (novaCasa.tipo is Casa.Tipo.Refugio || novaCasa.tipo is Casa.Tipo.Trono) return false;

                if (sentido is Direcao.Cima)
                {
                    for (int i = Posicao.x - 1; i > novaCasa.Coordenada.x; i--)
                    {
                        if (casas[i][Posicao.y].condicao is Casa.Condicao.Ocupada || casas[i][Posicao.y].tipo is Casa.Tipo.Trono || casas[i][Posicao.y].tipo is Casa.Tipo.Refugio)
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
                        if (casas[i][Posicao.y].condicao is Casa.Condicao.Ocupada || casas[i][Posicao.y].tipo is Casa.Tipo.Trono || casas[i][Posicao.y].tipo is Casa.Tipo.Refugio)
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

                if (novaCasa.tipo is Casa.Tipo.Refugio || novaCasa.tipo is Casa.Tipo.Trono) return false;

                if (sentido is Direcao.Esquerda)
                {
                    for (int i = Posicao.y - 1; i > novaCasa.Coordenada.y; i--)
                    {
                        if (casas[Posicao.x][i].condicao is Casa.Condicao.Ocupada || casas[Posicao.x][i].tipo is Casa.Tipo.Trono || casas[Posicao.x][i].tipo is Casa.Tipo.Refugio)
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
                        if (casas[Posicao.x][i].condicao is Casa.Condicao.Ocupada || casas[Posicao.x][i].tipo is Casa.Tipo.Trono || casas[Posicao.x][i].tipo is Casa.Tipo.Refugio)
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