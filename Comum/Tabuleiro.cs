using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VLAR.Comum.Agentes;

namespace VLAR.Comum
{
    public class Tabuleiro
    {
        public long ID { get; set; } = 0;
        public List<Peca> pecas { get; private set; } = new();
        public List<Soldado> soldados { get; private set; } = new();
        public List<Mercenario> mercenarios { get; private set; } = new();
        public bool rei { get; private set; } = false;
        private byte limitePecas { get; set; }
        public List<List<Casa>> casas { get; private set; } = new();
        public List<Jogador> jogadores { get; private set; } = new();
        public List<Espectador> espectadores { get; private set; } = new();
        public List<Movimento> logMovimentos { get; private set; } = new();
        public bool JogoTerminado { get; private set; } = false;

        public Tabuleiro(Tabuleiro copiando)
        {
            ID = copiando.ID + 1;
            limitePecas = copiando.limitePecas;
            rei = copiando.rei;
            jogadores = new(copiando.jogadores);
            espectadores = new(copiando.espectadores);

            for (int i = 0; i < copiando.casas.Count; i++)
            {
                casas.Add(new());
                for (int j = 0; j < copiando.casas[0].Count; j++)
                {
                    casas[i].Add(new(copiando.casas[i][j]));
                }
            }

            foreach (Movimento movimento in copiando.logMovimentos)
            {
                logMovimentos.Add(new Movimento(movimento));
            }

            foreach (Mercenario mercenario in copiando.mercenarios)
            {
                var novo = new Mercenario(mercenario);
                novo.Tabuleiro = this;
                var novaCasa = novo.Tabuleiro.GetCasa(novo.Posicao);
                if (novaCasa is null) throw new Exception("Casa nula.");
                novaCasa.Ocupante = novo;
                novaCasa.condicao = Casa.Condicao.Ocupada;

                mercenarios.Add(novo);
                pecas.Add(novo);

                var casaCopiona = copiando.GetCasa(mercenario);
                if (casaCopiona is null) throw new Exception("Casa inválida.");
                var casaOcupada = new Casa(casaCopiona);
                if (casaOcupada is null) throw new Exception("Casa inválida.");
                casaOcupada.Ocupante = novo;
            }
            foreach (Soldado soldado in copiando.soldados)
            {
                var novo = new Soldado(soldado);
                novo.Tabuleiro = this;
                var novaCasa = novo.Tabuleiro.GetCasa(novo.Posicao);
                if (novaCasa is null) throw new Exception("Casa nula.");
                novaCasa.Ocupante = novo;
                novaCasa.condicao = Casa.Condicao.Ocupada;

                soldados.Add(novo);
                pecas.Add(novo);

                var casaCopiona = copiando.GetCasa(soldado);
                if (casaCopiona is null) throw new Exception("Casa inválida.");
                var casaOcupada = new Casa(casaCopiona);
                if (casaOcupada is null) throw new Exception("Casa inválida.");
                casaOcupada.Ocupante = novo;
            }

            Rei? reiCopiao = (Rei?)copiando.pecas.Find(peca =>
            {
                if (peca is Rei) return true;
                return false;
            });
            if (reiCopiao is null) throw new Exception("Rei nulo");

            Rei? Rei = new(reiCopiao);
            Rei.Tabuleiro = this;
            var NovaCasa = Rei.Tabuleiro.GetCasa(Rei.Posicao);
            if (NovaCasa is null) throw new Exception("Casa nula.");
            NovaCasa.Ocupante = Rei;
            NovaCasa.condicao = Casa.Condicao.Ocupada;

            pecas.Add(Rei);

            var CasaCopiona = copiando.GetCasa(reiCopiao);
            if (CasaCopiona is null) throw new Exception("Casa inválida.");
            var CasaOcupada = new Casa(CasaCopiona);
            if (CasaOcupada is null) throw new Exception("Casa inválida.");
            CasaOcupada.Ocupante = Rei;



            casas = new List<List<Casa>>(copiando.casas.Count);
            for (byte i = 0; i < copiando.casas.Count; i++)
            {
                casas.Add(new List<Casa>(copiando.casas[0].Count));
                for (byte j = 0; j < copiando.casas[0].Count; j++)
                {
                    casas[i].Add(new Casa(new Posicao(i, j)));

                    if (i == 0 && j == 0)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == 0 && j == copiando.casas[0].Count - 1)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == copiando.casas.Count - 1 && j == 0)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == copiando.casas.Count - 1 && j == copiando.casas[0].Count - 1)
                        casas[i][j].tipo = Casa.Tipo.Refugio;

                    else if (i == copiando.casas.Count / 2 && j == copiando.casas[0].Count / 2)
                        casas[i][j].tipo = Casa.Tipo.Trono;

                    else
                        casas[i][j].tipo = Casa.Tipo.Comum;
                }
            }
        }

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
            if (nova is Soldado) soldados.Add((Soldado)nova);
            if (nova is Rei) rei = true;
            if (nova is Mercenario) mercenarios.Add((Mercenario)nova);

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

        public Casa? GetCasa(Posicao posicao)
        {
            if (!verificarPosicaoValida(posicao)) return null;
            return casas[posicao.x][posicao.y];
        }

        public Casa? GetCasa(Peca peca)
        {
            if (!verificarPosicaoValida(peca.Posicao)) return null;
            return casas[peca.Posicao.x][peca.Posicao.y];
        }

        public bool verificarPosicaoValida(Posicao posicao)
        {
            if (posicao.x < 0
                || posicao.x > casas.Count - 1)
                return false;

            if (posicao.y < 0
                || posicao.y > casas.Count - 1)
                return false;

            return true;
        }

        public void AtacanteGanha()
        {
            JogoTerminado = true;
            Console.WriteLine("Atacante vence!");
        }

        public void DefensorGanha()
        {
            JogoTerminado = true;
            Console.WriteLine("Defensor vence!");
        }

        public void EfetivarJogada()
        {
            Movimento ultimo = logMovimentos.Last();
            var novaPosicao = ultimo.destino.Coordenada;

            //Verificação à esquerda
            var cerco = novaPosicao + new Posicao(-2, 0);
            if (verificarPosicaoValida(cerco))
            {
                var cercante = GetCasa(cerco);
                if ((ultimo.peca is Soldado || ultimo.peca is Rei) && cercante is not null
                    && (cercante.Ocupante is Soldado || cercante.Ocupante is Rei))
                {
                    var conferida = GetCasa(cerco + new Posicao(1, 0));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Mercenario)
                        RemoverPeca(cercada);

                }
                else if (ultimo.peca is Mercenario && cercante is not null
                        && cercante.Ocupante is Mercenario)
                {
                    var conferida = GetCasa(cerco + new Posicao(1, 0));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Soldado)
                        RemoverPeca(cercada);
                    else if (cercada is not null && cercada is Rei)
                    {
                        var deCima = GetCasa(cerco + new Posicao(0, 1));
                        var deBaixo = GetCasa(cerco + new Posicao(0, -1));

                        if (deCima is not null && deCima.Ocupante is Mercenario
                            && deBaixo is not null && deBaixo.Ocupante is Mercenario)
                        {
                            RemoverPeca(cercada);
                            AtacanteGanha();
                        }
                    }
                }
            }
            //Verificação á direita
            cerco = novaPosicao + new Posicao(2, 0);
            if (verificarPosicaoValida(cerco))
            {
                var cercante = GetCasa(cerco);
                if ((ultimo.peca is Soldado || ultimo.peca is Rei) && cercante is not null
                    && (cercante.Ocupante is Soldado || cercante.Ocupante is Rei))
                {
                    var conferida = GetCasa(cerco + new Posicao(-1, 0));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Mercenario)
                        RemoverPeca(cercada);

                }
                else if (ultimo.peca is Mercenario && cercante is not null
                        && cercante.Ocupante is Mercenario)
                {
                    var conferida = GetCasa(cerco + new Posicao(-1, 0));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Soldado)
                        RemoverPeca(cercada);
                    else if (cercada is not null && cercada is Rei)
                    {
                        var deCima = GetCasa(cerco + new Posicao(0, 1));
                        var deBaixo = GetCasa(cerco + new Posicao(0, -1));

                        if (deCima is not null && deCima.Ocupante is Mercenario
                            && deBaixo is not null && deBaixo.Ocupante is Mercenario)
                        {
                            RemoverPeca(cercada);
                            AtacanteGanha();
                        }
                    }
                }
            }
            //Verificar em baixo
            cerco = novaPosicao + new Posicao(0, -2);
            if (verificarPosicaoValida(cerco))
            {
                var cercante = GetCasa(cerco);
                if ((ultimo.peca is Soldado || ultimo.peca is Rei) && cercante is not null
                    && (cercante.Ocupante is Soldado || cercante.Ocupante is Rei))
                {
                    var conferida = GetCasa(cerco + new Posicao(0, 1));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Mercenario)
                        RemoverPeca(cercada);

                }
                else if (ultimo.peca is Mercenario && cercante is not null
                        && cercante.Ocupante is Mercenario)
                {
                    var conferida = GetCasa(cerco + new Posicao(0, 1));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Soldado)
                        RemoverPeca(cercada);
                    else if (cercada is not null && cercada is Rei)
                    {
                        var daDireita = GetCasa(cerco + new Posicao(1, 0));
                        var daEsquerda = GetCasa(cerco + new Posicao(-1, 0));

                        if (daDireita is not null && daDireita.Ocupante is Mercenario
                            && daEsquerda is not null && daEsquerda.Ocupante is Mercenario)
                        {
                            RemoverPeca(cercada);
                            AtacanteGanha();
                        }
                    }
                }
            }
            //Verificar em cima
            cerco = novaPosicao + new Posicao(0, 2);
            if (verificarPosicaoValida(cerco))
            {
                var cercante = GetCasa(cerco);
                if ((ultimo.peca is Soldado || ultimo.peca is Rei) && cercante is not null
                    && (cercante.Ocupante is Soldado || cercante.Ocupante is Rei))
                {
                    var conferida = GetCasa(cerco + new Posicao(0, -1));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Mercenario)
                        RemoverPeca(cercada);

                }
                else if (ultimo.peca is Mercenario && cercante is not null
                        && cercante.Ocupante is Mercenario)
                {
                    var conferida = GetCasa(cerco + new Posicao(0, -1));
                    Peca? cercada = null;
                    if (conferida is not null)
                        cercada = conferida.Ocupante;

                    if (cercada is not null && cercada is Soldado)
                        RemoverPeca(cercada);
                    else if (cercada is not null && cercada is Rei)
                    {
                        var daDireita = GetCasa(cerco + new Posicao(1, 0));
                        var daEsquerda = GetCasa(cerco + new Posicao(-1, 0));

                        if (daDireita is not null && daDireita.Ocupante is Mercenario
                            && daEsquerda is not null && daEsquerda.Ocupante is Mercenario)
                        {
                            RemoverPeca(cercada);
                            AtacanteGanha();
                        }
                    }
                }
            }

            //Verificar rei no refúgio
            if (ultimo.peca is Rei)
            {
                Rei rei = (Rei)ultimo.peca;

                if (rei.Posicao == new Posicao(0, 0)) DefensorGanha();
                if (rei.Posicao == new Posicao(0, casas[0].Count)) DefensorGanha();
                if (rei.Posicao == new Posicao(casas.Count, 0)) DefensorGanha();
                if (rei.Posicao == new Posicao(casas.Count, casas[0].Count)) DefensorGanha();

            }
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

        public Casa(Casa copiando)
        {
            Coordenada = copiando.Coordenada;
            tipo = copiando.tipo;
            condicao = copiando.condicao;
            Ocupante = copiando.Ocupante;
        }

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