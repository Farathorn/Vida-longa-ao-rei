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

        public Bot(Tabuleiro jogo, Jogador controlador)
        {
            Jogo = jogo;
            Controlador = controlador;
        }

        public void Etapas()
        {
            GerarArvore();
            Heurística();

        }

        private void GerarArvore(byte profundidade = 5, long? pai = null)
        {
            byte pecasControladas = 0;
            byte pecasInimigas = 0;
            if (Controlador is Defensor)
            {
                foreach (Peca peca in Jogo.pecas)
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
                foreach (Peca peca in Jogo.pecas)
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

            Stack<long?> pilha = new();
            Tabuleiro analisando = new(Jogo);
            long? noPai = ArvoreDePossibilidades.Adicionar(analisando);
            if (noPai is null)
                throw new Exception("Erro desconhecido.");

            long i = 0;
            while (i < profundidade)
            {
                No<Tabuleiro>? NoPai = ArvoreDePossibilidades.BuscarLargamente(noPai);
                if (NoPai is null) throw new Exception("Erro desconhecido");

                foreach (var pecaPossivel in MatrizPossibilidades)
                {
                    foreach (var jogadaPossivel in pecaPossivel)
                    {
                        Tabuleiro possibilidade = new(NoPai.Objeto);
                        jogadaPossivel.peca.Mover(Posicao.Sentido(jogadaPossivel.origem.Coordenada, jogadaPossivel.destino.Coordenada),
                                                                (int)!(jogadaPossivel.origem.Coordenada - jogadaPossivel.destino.Coordenada));


                        long? id = ArvoreDePossibilidades.Adicionar(possibilidade, noPai);
                        pilha.Push(id);
                    }
                }

                NoPai = ArvoreDePossibilidades.BuscarLargamente(pilha.Pop());
                if (NoPai is null) throw new Exception("Erro desconhecido");
                noPai = NoPai.Valor;
                i = NoPai.profundidade;
            }

        }

        private int CalculoPecas(Tabuleiro pai, Tabuleiro filho)
        {
            int pesoEstimado = 0;

            if (Controlador is Atacante)
            {
                if (filho.soldados.Count < pai.soldados.Count) pesoEstimado += 4 * (pai.soldados.Count - filho.soldados.Count);
                if (!filho.rei) pesoEstimado = +10000;
                if (filho.mercenarios.Count < pai.mercenarios.Count) pesoEstimado -= 3 * (pai.mercenarios.Count - filho.mercenarios.Count);
            }
            else
            {
                if (filho.soldados.Count < pai.soldados.Count) pesoEstimado -= 3 * (pai.soldados.Count - filho.soldados.Count);
                if (!filho.rei) pesoEstimado = -10000;
                if (filho.mercenarios.Count < pai.mercenarios.Count) pesoEstimado += 2 * (pai.soldados.Count - filho.soldados.Count);
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

                if (distanciaRefugioFilho00 < 1 || distanciaRefugioFilho01 < 1
                    || distanciaRefugioFilho10 < 1 || distanciaRefugioFilho11 < 1)
                {
                    pesoEstimado = 10000;
                    return pesoEstimado;
                }

                int pesoDistancia00 = (int)(7 / distanciaRefugioPai00);
                int pesoDistancia01 = (int)(7 / distanciaRefugioPai01);
                int pesoDistancia10 = (int)(7 / distanciaRefugioPai10);
                int pesoDistancia11 = (int)(7 / distanciaRefugioPai11);

                if (distanciaRefugioPai00 > distanciaRefugioFilho00) pesoEstimado += 1 * pesoDistancia00 * (int)(distanciaRefugioPai00 - distanciaRefugioFilho00);
                if (distanciaRefugioPai00 < distanciaRefugioFilho00) pesoEstimado -= 1 * pesoDistancia00 * (int)(distanciaRefugioFilho00 - distanciaRefugioPai00);


                if (distanciaRefugioPai01 > distanciaRefugioFilho01) pesoEstimado += 1 * pesoDistancia01 * (int)(distanciaRefugioPai01 - distanciaRefugioFilho01);
                if (distanciaRefugioPai01 < distanciaRefugioFilho01) pesoEstimado -= 1 * pesoDistancia01 * (int)(distanciaRefugioFilho01 - distanciaRefugioPai01);


                if (distanciaRefugioPai10 > distanciaRefugioFilho10) pesoEstimado += 1 * pesoDistancia10 * (int)(distanciaRefugioPai10 - distanciaRefugioFilho10);
                if (distanciaRefugioPai10 < distanciaRefugioFilho10) pesoEstimado -= 1 * pesoDistancia10 * (int)(distanciaRefugioFilho10 - distanciaRefugioPai10);


                if (distanciaRefugioPai11 > distanciaRefugioFilho11) pesoEstimado += 1 * pesoDistancia10 * (int)(distanciaRefugioPai11 - distanciaRefugioFilho11);
                if (distanciaRefugioPai11 < distanciaRefugioFilho11) pesoEstimado -= 1 * pesoDistancia10 * (int)(distanciaRefugioFilho11 - distanciaRefugioPai11);
            }

            return pesoEstimado;
        }

        public void Heurística(byte profundidade = 5)
        {
            Stack<long?> pilha = new();
            Tabuleiro analisando = new(Jogo);
            long? noPai = ArvoreDePossibilidades.Adicionar(analisando);
            if (noPai is null)
                throw new Exception("Erro desconhecido.");

            long i = 1;
            do
            {
                No<Tabuleiro>? NoPai = ArvoreDePossibilidades.BuscarLargamente(noPai);
                if (NoPai is null) throw new Exception("Erro desconhecido");

                No<Tabuleiro> deMelhorPeso = new(NoPai.Objeto, 0, 0);

                foreach (No<Tabuleiro> filho in NoPai.Filhos)
                {
                    filho.Peso = CalculoPecas(NoPai.Objeto, filho.Objeto);
                    filho.Peso += CalculoDistanciaReiRefugio(NoPai.Objeto, filho.Objeto);

                    if (deMelhorPeso.Peso is null) deMelhorPeso = filho;
                    if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = filho;
                }

                NoPai = ArvoreDePossibilidades.BuscarLargamente(pilha.Pop());
                if (NoPai is null) throw new Exception("Erro desconhecido");
                noPai = NoPai.Valor;
                i = NoPai.profundidade;
            }
            while (i < profundidade);
        }
    }
}