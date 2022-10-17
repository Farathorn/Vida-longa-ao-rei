using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VLAR.Comum;
using VLAR.Comum.Agentes;
using VLAR.Estruturas.Arvores;

namespace VLAR.IA
{
    public class Bot
    {
        Tabuleiro Jogo { get; set; }
        Jogador Controlador { get; set; }
        List<List<Movimento>> MatrizPossibilidades { get; set; } = new();
        ArvoreGeral<Tabuleiro> ArvoreDePossibilidades { get; set; } = new();
        byte pecasControladas { get; set; }
        byte pecasInimigas { get; set; }
        Tabuleiro? possibilidadeEscolhida { get; set; } = null;
        List<Tabuleiro?> Trace = new();
        int profundidade = 5;

        public Bot(Tabuleiro jogo, Jogador controlador)
        {
            Jogo = jogo;
            Controlador = controlador;
        }

        public void Etapas()
        {
            GerarArvore();
            Heurística();

            possibilidadeEscolhida = Trace.Last();
            if (possibilidadeEscolhida is null) throw new Exception("Erro na heurística.");

            Movimento movimentoEscolhido = possibilidadeEscolhida.logMovimentos.Last();
            Direcao sentido = Posicao.Sentido(movimentoEscolhido.origem.Coordenada, movimentoEscolhido.destino.Coordenada);
            int quanto = (int)!(movimentoEscolhido.destino.Coordenada - movimentoEscolhido.origem.Coordenada);

            Peca? pecaOriginal = Jogo.pecas.Find(peca =>
            {
                if (peca.Posicao == movimentoEscolhido.origem.Coordenada) return true;
                return false;
            });

            if (pecaOriginal is null) throw new Exception("Peça nula.");
            pecaOriginal.Mover(sentido, quanto);
        }

        private void CalcularMovimentos(Tabuleiro pai)
        {
            byte pecasControladas = 0;
            byte pecasInimigas = 0;

            if (Controlador is Defensor)
            {
                foreach (Peca peca in pai.pecas)
                {
                    if (peca is Soldado || peca is Rei)
                    {
                        pecasControladas++;
                        MatrizPossibilidades.Add(peca.MovimentosPossiveis());
                    }

                    if (peca is Mercenario)
                    {
                        pecasInimigas++;
                    }
                }
            }
            else
            {
                foreach (Peca peca in pai.pecas)
                {
                    if (peca is Mercenario)
                    {
                        pecasControladas++;
                        MatrizPossibilidades.Add(peca.MovimentosPossiveis());
                    }
                    if (peca is Soldado || peca is Rei)
                    {
                        pecasInimigas++;
                    }
                }
            }
            this.pecasControladas = pecasControladas;
            this.pecasInimigas = pecasInimigas;
        }

        private void GerarArvore(long? pai = null)
        {
            ArvoreDePossibilidades.Zerar();

            Stack<long?> pilha = new();
            long? noPai = ArvoreDePossibilidades.Adicionar(Jogo);
            if (noPai is null)
                throw new Exception("Erro desconhecido.");

            No<Tabuleiro>? NoPai = ArvoreDePossibilidades.BuscarLargamente(noPai);
            if (NoPai is null) throw new Exception("Erro desconhecido");

            pilha.Push(noPai);

            long i = 0;
            do
            {
                NoPai = ArvoreDePossibilidades.BuscarLargamente(pilha.Pop());
                if (NoPai is null) throw new Exception("Erro desconhecido");
                noPai = NoPai.Valor;
                i = NoPai.profundidade + 1;
                MatrizPossibilidades = new();
                CalcularMovimentos(NoPai.Objeto);

                if (NoPai.profundidade < profundidade && !pilha.Contains(noPai))
                {
                    foreach (var pecaPossivel in MatrizPossibilidades)
                    {
                        foreach (var jogadaPossivel in pecaPossivel)
                        {

                            Tabuleiro possibilidade = new(NoPai.Objeto);
                            /*foreach (Peca peca in NoPai.Objeto.pecas)
							{
								possibilidade.InserirPeca(peca);
								peca.Tabuleiro = possibilidade;
							}*/

                            Peca? pecaEquivalente = possibilidade.pecas.Find(peca =>
                            {
                                if (peca.Posicao == jogadaPossivel.peca.Posicao) return true;
                                return false;
                            });
                            if (pecaEquivalente is null) throw new Exception("Erro de consistência na árvore.");

                            pecaEquivalente.Mover(Posicao.Sentido(jogadaPossivel.origem.Coordenada, jogadaPossivel.destino.Coordenada),
                                                                    (int)!(jogadaPossivel.origem.Coordenada - jogadaPossivel.destino.Coordenada));


                            long? id = ArvoreDePossibilidades.Adicionar(possibilidade, noPai);
                            pilha.Push(id);
                        }
                    }
                }
            }
            while (i < profundidade && pilha.Count != 0);

        }

        private int CalculoPecas(Tabuleiro pai, Tabuleiro filho)
        {
            int pesoEstimado = 0;

            if (Controlador is Atacante)
            {
                if (filho.soldados.Count < pai.soldados.Count) pesoEstimado += 5 * (pai.soldados.Count - filho.soldados.Count);
                if (!filho.rei) pesoEstimado = +10000;
                if (filho.mercenarios.Count < pai.mercenarios.Count) pesoEstimado -= 3 * (pai.mercenarios.Count - filho.mercenarios.Count);
            }
            else
            {
                if (filho.soldados.Count < pai.soldados.Count) pesoEstimado -= 3 * (pai.soldados.Count - filho.soldados.Count);
                if (!filho.rei) pesoEstimado = -10000;
                if (filho.mercenarios.Count < pai.mercenarios.Count) pesoEstimado += 3 * (pai.soldados.Count - filho.soldados.Count);
            }

            return pesoEstimado;
        }

        private int CalculoDistanciaReiRefugio(Tabuleiro pai, Tabuleiro filho)
        {
            int pesoEstimado = 0;

            if (Controlador is Atacante)
            {
                Rei? reiFilho = (Rei?)filho.pecas.Find(peca =>
                {
                    if (peca is Rei) return true;
                    else return false;
                });

                if (reiFilho is null) throw new Exception("Erro desconhecido.");

                Rei? reiPai = (Rei?)pai.pecas.Find(peca =>
                {
                    if (peca is Rei) return true;
                    else return false;
                });

                if (reiPai is null) throw new Exception("Erro desconhecido.");

                double distanciaRefugioPai00 = !(reiPai.Posicao - new Posicao(0, 0));
                double distanciaRefugioFilho00 = !(reiFilho.Posicao - new Posicao(0, 0));

                double distanciaRefugioPai01 = !(reiPai.Posicao - new Posicao(0, filho.casas[0].Count));
                double distanciaRefugioFilho01 = !(reiFilho.Posicao - new Posicao(0, filho.casas[0].Count));

                double distanciaRefugioPai10 = !(reiPai.Posicao - new Posicao(filho.casas.Count, 0));
                double distanciaRefugioFilho10 = !(reiFilho.Posicao - new Posicao(filho.casas.Count, 0));

                double distanciaRefugioPai11 = !(reiPai.Posicao - new Posicao(filho.casas.Count, filho.casas[0].Count));
                double distanciaRefugioFilho11 = !(reiFilho.Posicao - new Posicao(filho.casas.Count, filho.casas[0].Count));

                double distanciaTronoPai = !(reiPai.Posicao - new Posicao(5, 5));
                double distanciaTronoFilho = !(reiFilho.Posicao - new Posicao(5, 5));

                if (distanciaRefugioFilho00 < 1 || distanciaRefugioFilho01 < 1
                    || distanciaRefugioFilho10 < 1 || distanciaRefugioFilho11 < 1)
                {
                    pesoEstimado = 10000;
                    return pesoEstimado;
                }

                int pesoDistancia00 = (int)(7 / distanciaRefugioFilho00 * distanciaTronoFilho);
                int pesoDistancia01 = (int)(7 / distanciaRefugioFilho01 * distanciaTronoFilho);
                int pesoDistancia10 = (int)(7 / distanciaRefugioFilho10 * distanciaTronoFilho);
                int pesoDistancia11 = (int)(7 / distanciaRefugioFilho11 * distanciaTronoFilho);

                if (distanciaRefugioPai00 > distanciaRefugioFilho00) pesoEstimado += 2 * pesoDistancia00 * (int)(distanciaRefugioPai00 - distanciaRefugioFilho00);
                if (distanciaRefugioPai00 < distanciaRefugioFilho00) pesoEstimado -= 2 * pesoDistancia00 * (int)(distanciaRefugioFilho00 - distanciaRefugioPai00);


                if (distanciaRefugioPai01 > distanciaRefugioFilho01) pesoEstimado += 2 * pesoDistancia01 * (int)(distanciaRefugioPai01 - distanciaRefugioFilho01);
                if (distanciaRefugioPai01 < distanciaRefugioFilho01) pesoEstimado -= 2 * pesoDistancia01 * (int)(distanciaRefugioFilho01 - distanciaRefugioPai01);


                if (distanciaRefugioPai10 > distanciaRefugioFilho10) pesoEstimado += 2 * pesoDistancia10 * (int)(distanciaRefugioPai10 - distanciaRefugioFilho10);
                if (distanciaRefugioPai10 < distanciaRefugioFilho10) pesoEstimado -= 2 * pesoDistancia10 * (int)(distanciaRefugioFilho10 - distanciaRefugioPai10);


                if (distanciaRefugioPai11 > distanciaRefugioFilho11) pesoEstimado += 2 * pesoDistancia10 * (int)(distanciaRefugioPai11 - distanciaRefugioFilho11);
                if (distanciaRefugioPai11 < distanciaRefugioFilho11) pesoEstimado -= 2 * pesoDistancia10 * (int)(distanciaRefugioFilho11 - distanciaRefugioPai11);
            }

            return pesoEstimado;
        }

        public int CalculoExpansão(Tabuleiro pai, Tabuleiro filho)
        {
            int pesoEstimado = 0;

            if (Controlador is Atacante)
            {
                for (int i = 0; i < filho.mercenarios.Count; i++)
                {
                    if (!(filho.mercenarios[i].Posicao - new Posicao(5, 5)) - !(pai.mercenarios[i].Posicao - new Posicao(5, 5)) < 4)
                    {
                        pesoEstimado += 1;
                    }
                }
            }
            else
            {

            }

            return pesoEstimado;
        }

        public void Heurística()
        {
            Stack<long?> pilha = new();

            No<Tabuleiro>? NoPai = ArvoreDePossibilidades.BuscarLargamente(0);
            if (NoPai is null) throw new Exception("Erro desconhecido");
            long? noPai = NoPai.Valor;

            pilha.Push(NoPai.Valor);
            long i;
            do
            {
                NoPai = ArvoreDePossibilidades.BuscarLargamente(pilha.Pop());
                if (NoPai is null) throw new Exception("Erro desconhecido");
                noPai = NoPai.Valor;
                i = NoPai.profundidade + 1;

                No<Tabuleiro> deMelhorPeso = new(NoPai.Objeto, 0, 0);

                foreach (No<Tabuleiro> filho in NoPai.Filhos)
                {
                    filho.Peso = CalculoPecas(NoPai.Objeto, filho.Objeto);
                    filho.Peso += CalculoDistanciaReiRefugio(NoPai.Objeto, filho.Objeto);

                    if (deMelhorPeso.Peso is null) deMelhorPeso = filho;
                    if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = filho;

                    pilha.Push(filho.Valor);
                }

                if (deMelhorPeso is null) throw new Exception("Erro lógico na árvore.");

                possibilidadeEscolhida = deMelhorPeso.Objeto;
            }
            while (i <= profundidade && pilha.Count != 0);

            var k = NoPai.Pai;
            while (k.Pai != null)
            {
                Trace.Add(k.Objeto);
                k = k.Pai;
            }
        }
    }
}