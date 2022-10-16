using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VLAR.Comum.Agentes;

namespace VLAR.Comum
{
    public class Tabuleiro
    {
        public List<Peca> pecas { get; private set; } = new List<Peca>();
        private byte limitePecas { get; set; }
        public List<List<Casa>> casas { get; private set; }
        public List<Jogador> jogadores { get; private set; } = new();
        public List<Espectador> espectadores { get; private set; } = new();
        public List<Movimento> logMovimentos { get; private set; } = new();

        public Tabuleiro(byte largura, byte altura, byte limitePecas)
        {
            this.limitePecas = limitePecas;

            casas = new List<List<Casa>>(altura);
            for (byte i = 0; i < altura; i++)
            {
                casas.Add(new List<Casa>(largura));
                for (byte j = 0; j < largura; j++)
                {
                    casas[i].Add(new Casa(new Posicao(i, j)));

                    if (i == 0 && j == 0)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == 0 && j == largura - 1)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == altura - 1 && j == 0)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == altura - 1 && j == largura - 1)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == altura / 2 && j == largura / 2)
                        casas[i][j].tipo = Casa.Tipo.Trono;

                    else
                        casas[i][j].tipo = Casa.Tipo.Comum;
                }
            }
        }

        public bool InserirPeca(Peca nova)
        {
            if (pecas.Count == limitePecas) return false;

            pecas.Add(nova);
            nova.Tabuleiro = this;

            var casaAlterada = casas[nova.Posicao.x][nova.Posicao.y];
            casaAlterada.condicao = Casa.Condicao.Ocupada;
            casaAlterada.Ocupante = nova;

            return true;
        }

        public bool RemoverPeca(Peca removenda)
        {
            if (pecas.Count == 0) return false;

            pecas.Remove(removenda);
            var casaAlterada = casas[removenda.Posicao.x][removenda.Posicao.y];
            casaAlterada.condicao = Casa.Condicao.Desocupada;
            casaAlterada.Ocupante = null;

            return true;
        }

        public void InserirJogador(Jogador player)
        {
            jogadores.Add(player);
            player.Tabuleiro = this;
        }

        public Casa GetCasa(Posicao posicao)
        {
            return casas[posicao.x][posicao.y];
        }

        public Casa GetCasa(Peca peca)
        {
            return casas[peca.Posicao.x][peca.Posicao.y];
        }
    }

    public class Casa
    {
        public Posicao Coordenada { get; private set; }
        public enum Condicao
        {
            Ocupada,
            Desocupada
        }

        public enum Tipo
        {
            Comum,
            Trono,
            Refugio
        }

        public Tipo tipo { get; set; }
        public Condicao condicao { get; set; } = Condicao.Desocupada;
        public Peca? Ocupante { get; set; } = null;

        public Casa(Posicao Coordenada, Tipo tipo = Tipo.Comum)
        {
            this.tipo = tipo;
            this.Coordenada = Coordenada;
        }
    }

    public class Movimento
    {
        public Casa origem;
        public Casa destino;
        public Peca peca;

        public Movimento(Movimento copiando)
        {
            this.origem = copiando.origem;
            this.destino = copiando.destino;
            this.peca = copiando.peca;
        }

        public Movimento(Casa origem, Casa destino, Peca peca)
        {
            this.origem = origem;
            this.destino = destino;
            this.peca = peca;
        }
    }
}