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
        List<No<Tabuleiro>> Trace = new();
        int profundidade = 3;

        public Bot(Tabuleiro jogo, Jogador controlador)
        {
            Jogo = jogo;
            Controlador = controlador;
        }

        public void Etapas()
        {
            GerarArvore();

            possibilidadeEscolhida = Trace.Last().Objeto;
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

        private void CalcularMovimentos(Tabuleiro pai, Jogador perspectiva)
        {
            byte pecasControladas = 0;
            byte pecasInimigas = 0;

            if (perspectiva is Defensor)
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

            long? NoID = ArvoreDePossibilidades.Adicionar(Jogo);
            if (NoID is null)
                throw new Exception("Erro desconhecido.");

            No<Tabuleiro>? No = ArvoreDePossibilidades.listaOrdenadaLargura[0];
            if (No is null) throw new Exception("Erro desconhecido");

            LinkedList<long?> fila = new();
            fila.AddLast(NoID);

            No<Tabuleiro>? deMelhorPeso = null;

            long quantidadeAnalisada = 0;

            long? idQueRecalculou = null;

            long i = 0;
            long iAnterior = 0;
            No<Tabuleiro>? anterior = null;
            do
            {
                var primeiroDaFila = fila.First();
                fila.RemoveFirst();
                if (primeiroDaFila is not null)
                {
                    No = ArvoreDePossibilidades.listaOrdenadaLargura[(int)primeiroDaFila];
                    if (No is null) throw new Exception("Erro desconhecido");

                    No<Tabuleiro>? noRuim = No.Filhos.Find((no) =>
                    {
                        if (no is not null && no.eNoRuim) return true;
                        return false;
                    });

                    if (noRuim is not null) No = noRuim;

                    int indiceFilhos = 0;

                    NoID = No.Valor;
                    i = No.profundidade + 1;

                    if (i != iAnterior && i <= profundidade)
                    {
                        deMelhorPeso = null;
                        iAnterior = i;
                        anterior = null;
                    }

                    MatrizPossibilidades = new();
                    if (No.profundidade % 2 == 0) CalcularMovimentos(No.Objeto, Controlador);
                    else CalcularMovimentos(No.Objeto, Jogador.GerarOpostoDe(Controlador));



                    bool noReconsiderado = false;

                    if (No.profundidade < profundidade && !fila.Contains(NoID))
                    {
                        foreach (var pecaPossivel in MatrizPossibilidades)
                        {
                            foreach (var jogadaPossivel in pecaPossivel)
                            {
                                quantidadeAnalisada++;

                                No<Tabuleiro>? filho;
                                long? pesoNoAntes;
                                if (No != noRuim)
                                {
                                    filho = new(No);
                                    if (NoID is null)
                                        throw new Exception("Erro desconhecido.");
                                    filho.Objeto = new(No.Objeto, ArvoreDePossibilidades.listaOrdenadaLargura.Count);
                                    pesoNoAntes = No.Peso;

                                    Peca? pecaEquivalente = filho.Objeto.pecas.Find(peca =>
                                    {
                                        if (peca.Posicao == jogadaPossivel.peca.Posicao) return true;
                                        return false;
                                    });
                                    if (pecaEquivalente is null) throw new Exception("Erro de consistência na árvore.");

                                    pecaEquivalente.Mover(Posicao.Sentido(jogadaPossivel.origem.Coordenada, jogadaPossivel.destino.Coordenada),
                                                                            (int)!(jogadaPossivel.origem.Coordenada - jogadaPossivel.destino.Coordenada));

                                    //Heurística
                                    if (i % 2 == 0) filho.Peso -= Heurística(filho, No, Jogador.GerarOpostoDe(Controlador));
                                    else filho.Peso += Heurística(filho, No, Controlador);

                                    long? id = null;
                                    if (i > 2)
                                    {

                                        if (anterior is null || anterior.Peso < filho.Peso)
                                        {
                                            id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                            if (id is null) throw new Exception("Item não adicionado.");

                                            anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            if (anterior is not null)
                                            {
                                                anterior.Peso = filho.Peso;
                                                fila.AddLast(id);
                                            }

                                            if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                        }
                                        else if (anterior.Peso == filho.Peso && anterior.Valor is not null)
                                        {
                                            Random rand = new(DateTime.Now.CompareTo(DateTime.MinValue));

                                            if (rand.NextDouble() >= 0.5)
                                            {
                                                ArvoreDePossibilidades.listaOrdenadaLargura[(int)anterior.Valor] = null;

                                                id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                                if (id is null) throw new Exception("Item não adicionado.");

                                                anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                if (anterior is not null)
                                                {
                                                    anterior.Peso = filho.Peso;
                                                    fila.AddLast(id);
                                                }

                                                if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                        if (id is null) throw new Exception("Item não adicionado.");

                                        anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                        if (anterior is not null)
                                        {
                                            anterior.Peso = filho.Peso;
                                            fila.AddLast(id);
                                        }

                                        if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                        else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                    }

                                    if (No.Pai is not null && No.Peso != pesoNoAntes && anterior is not null && anterior.Valor != idQueRecalculou)
                                    {
                                        noReconsiderado = true;
                                        No.eNoRuim = true;
                                        anterior.eNoQueRecalculou = true;
                                        fila.AddFirst(No.Pai.Valor);
                                        foreach (No<Tabuleiro>? filhoIgnorando in No.Filhos)
                                        {
                                            if (filhoIgnorando is not null)
                                                fila.RemoveLast();
                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    if (indiceFilhos < No.Filhos.Count - 1)
                                    {
                                        filho = No.Filhos[indiceFilhos];
                                        if (filho is not null)
                                        {
                                            filho.Peso = No.Peso;

                                            if (i % 2 == 0) filho.Peso -= Heurística(filho, No, Jogador.GerarOpostoDe(Controlador));
                                            else filho.Peso += Heurística(filho, No, Controlador);

                                            long? id = null;
                                            if (i > 2)
                                            {

                                                if (anterior is null || anterior.Peso < filho.Peso)
                                                {
                                                    id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                                    if (id is null) throw new Exception("Item não adicionado.");

                                                    anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                    if (anterior is not null)
                                                    {
                                                        anterior.Peso = filho.Peso;
                                                        fila.AddLast(id);
                                                    }

                                                    if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                    else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                }
                                                else if (anterior.Peso == filho.Peso && anterior.Valor is not null)
                                                {
                                                    Random rand = new();

                                                    if (rand.NextDouble() >= 0.5)
                                                    {
                                                        ArvoreDePossibilidades.listaOrdenadaLargura[(int)anterior.Valor] = null;

                                                        id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                                        if (id is null) throw new Exception("Item não adicionado.");

                                                        anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                        if (anterior is not null)
                                                        {
                                                            anterior.Peso = filho.Peso;
                                                            fila.AddLast(id);
                                                        }

                                                        if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                        else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                                if (id is null) throw new Exception("Item não adicionado.");

                                                anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                if (anterior is not null)
                                                {
                                                    anterior.Peso = filho.Peso;
                                                    fila.AddLast(id);
                                                }

                                                if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        filho = new(No);
                                        if (NoID is null)
                                            throw new Exception("Erro desconhecido.");

                                        filho.Objeto = new(No.Objeto, ArvoreDePossibilidades.listaOrdenadaLargura.Count);

                                        pesoNoAntes = No.Peso;

                                        Peca? pecaEquivalente = filho.Objeto.pecas.Find(peca =>
                                        {
                                            if (peca.Posicao == jogadaPossivel.peca.Posicao) return true;
                                            return false;
                                        });
                                        if (pecaEquivalente is null) throw new Exception("Erro de consistência na árvore.");

                                        pecaEquivalente.Mover(Posicao.Sentido(jogadaPossivel.origem.Coordenada, jogadaPossivel.destino.Coordenada),
                                                                                (int)!(jogadaPossivel.origem.Coordenada - jogadaPossivel.destino.Coordenada));

                                        //Heurística
                                        if (i % 2 == 0) filho.Peso -= Heurística(filho, No, Jogador.GerarOpostoDe(Controlador));
                                        else filho.Peso += Heurística(filho, No, Controlador);

                                        long? id = null;
                                        if (i > 2)
                                        {

                                            if (anterior is null || anterior.Peso < filho.Peso)
                                            {
                                                id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                                if (id is null) throw new Exception("Item não adicionado.");

                                                anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                if (anterior is not null)
                                                {
                                                    anterior.Peso = filho.Peso;
                                                    fila.AddLast(id);
                                                }

                                                if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            }
                                            else if (anterior.Peso == filho.Peso && anterior.Valor is not null)
                                            {
                                                Random rand = new(DateTime.Now.CompareTo(DateTime.MinValue));

                                                if (rand.NextDouble() >= 0.5)
                                                {
                                                    ArvoreDePossibilidades.listaOrdenadaLargura[(int)anterior.Valor] = null;

                                                    id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                                    if (id is null) throw new Exception("Item não adicionado.");

                                                    anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                    if (anterior is not null)
                                                    {
                                                        anterior.Peso = filho.Peso;
                                                        fila.AddLast(id);
                                                    }

                                                    if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                    else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                                }
                                            }
                                        }
                                        else
                                        {
                                            id = ArvoreDePossibilidades.Adicionar(filho.Objeto, NoID);
                                            if (id is null) throw new Exception("Item não adicionado.");

                                            anterior = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            if (anterior is not null)
                                            {
                                                anterior.Peso = filho.Peso;
                                                fila.AddLast(id);
                                            }

                                            if (deMelhorPeso is null) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                            else if (deMelhorPeso.Peso < filho.Peso) deMelhorPeso = ArvoreDePossibilidades.listaOrdenadaLargura[(int)id];
                                        }

                                        if (No.Pai is not null && No.Peso != pesoNoAntes && anterior is not null && anterior.Valor != idQueRecalculou)
                                        {
                                            noReconsiderado = true;
                                            No.eNoRuim = true;
                                            anterior.eNoQueRecalculou = true;
                                            fila.AddFirst(No.Pai.Valor);
                                            foreach (No<Tabuleiro>? filhoIgnorando in No.Filhos)
                                            {
                                                if (filhoIgnorando is not null)
                                                    fila.Remove(filhoIgnorando.Valor);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            if (noReconsiderado) break;
                        }
                    }
                }
            }
            while (No.profundidade < profundidade && fila.Count != 0);

            Trace = new();

            if (deMelhorPeso is null) throw new Exception("Heurística problemática ou zugswang.");
            No<Tabuleiro> k = deMelhorPeso;
            while (k.Pai is not null)
            {
                Trace.Add(k);
                k = k.Pai;
            }
        }

        private int CalculoPecas(No<Tabuleiro> pai, No<Tabuleiro> filho, Jogador perspectiva)
        {
            int pesoEstimado = 0;

            if (perspectiva is Atacante)
            {
                if (!filho.Objeto.rei) pesoEstimado += 300;
                if (filho.Objeto.soldados.Count < pai.Objeto.soldados.Count)
                {
                    pesoEstimado += 60 * (pai.Objeto.soldados.Count - filho.Objeto.soldados.Count);


                    if (Controlador != perspectiva)
                    {

                        pai.Peso -= 150 * (pai.Objeto.soldados.Count - filho.Objeto.soldados.Count);
                    }
                }

                if (Controlador != perspectiva && !filho.Objeto.rei) pai.Peso -= 10000;
            }
            else
            {
                if (filho.Objeto.mercenarios.Count < pai.Objeto.mercenarios.Count)
                {
                    pesoEstimado += 40 * (pai.Objeto.mercenarios.Count - filho.Objeto.mercenarios.Count);

                    if (Controlador != perspectiva)
                    {
                        pai.Peso -= 30 * (pai.Objeto.mercenarios.Count - filho.Objeto.mercenarios.Count);
                    }
                }
            }

            return pesoEstimado;
        }

        private int CalculoColadoNoRei(No<Tabuleiro> pai, No<Tabuleiro> filho, Jogador perspectiva)
        {
            int pesoEstimado = 0;

            if (perspectiva is Atacante)
            {
                Casa? lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(1, 0));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(-1, 0));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(0, -1));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(0, 1));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                pesoEstimado *= 5;
            }
            else
            {
                Casa? lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(1, 0));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(-1, 0));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(0, -1));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                lateral = filho.Objeto.GetCasa(filho.Objeto.pecas.Last().Posicao + new Posicao(0, 1));
                if (lateral is not null && lateral.Ocupante is Mercenario) pesoEstimado++;

                pesoEstimado *= 5;
            }

            return pesoEstimado;
        }

        private int CalculoDistanciaReiRefugio(No<Tabuleiro> pai, No<Tabuleiro> filho, Jogador perspectiva)
        {
            int pesoEstimado = 0;

            if (perspectiva is Defensor)
            {
                if (!filho.Objeto.rei && perspectiva is Atacante) return 10000;

                Rei? reiFilho = (Rei?)filho.Objeto.pecas.Find(peca =>
                {
                    if (peca is Rei) return true;
                    else return false;
                });

                if (reiFilho is null) throw new Exception("Erro desconhecido.");

                Rei? reiPai = (Rei?)pai.Objeto.pecas.Find(peca =>
                {
                    if (peca is Rei) return true;
                    else return false;
                });

                if (reiPai is null) throw new Exception("Erro desconhecido.");

                double distanciaRefugioPai00 = !(reiPai.Posicao - new Posicao(0, 0));
                double distanciaRefugioFilho00 = !(reiFilho.Posicao - new Posicao(0, 0));

                double distanciaRefugioPai01 = !(reiPai.Posicao - new Posicao(0, filho.Objeto.casas[0].Count));
                double distanciaRefugioFilho01 = !(reiFilho.Posicao - new Posicao(0, filho.Objeto.casas[0].Count));

                double distanciaRefugioPai10 = !(reiPai.Posicao - new Posicao(filho.Objeto.casas.Count, 0));
                double distanciaRefugioFilho10 = !(reiFilho.Posicao - new Posicao(filho.Objeto.casas.Count, 0));

                double distanciaRefugioPai11 = !(reiPai.Posicao - new Posicao(filho.Objeto.casas.Count, filho.Objeto.casas[0].Count));
                double distanciaRefugioFilho11 = !(reiFilho.Posicao - new Posicao(filho.Objeto.casas.Count, filho.Objeto.casas[0].Count));

                double distanciaTronoPai = !(reiPai.Posicao - new Posicao(5, 5));
                double distanciaTronoFilho = !(reiFilho.Posicao - new Posicao(5, 5));

                if (distanciaRefugioFilho00 == 0 || distanciaRefugioFilho01 == 0
                    || distanciaRefugioFilho10 == 0 || distanciaRefugioFilho11 == 0)
                {
                    if (Controlador == perspectiva)
                    {
                        pesoEstimado = 300;
                        return pesoEstimado;
                    }
                    else
                    {
                        pai.Peso -= 10000;
                        return -10000;
                    }
                }

                int pesoDistancia00 = (int)((7 / distanciaRefugioFilho00) * distanciaTronoFilho);
                int pesoDistancia01 = (int)((7 / distanciaRefugioFilho01) * distanciaTronoFilho);
                int pesoDistancia10 = (int)((7 / distanciaRefugioFilho10) * distanciaTronoFilho);
                int pesoDistancia11 = (int)((7 / distanciaRefugioFilho11) * distanciaTronoFilho);

                if (distanciaRefugioPai00 > distanciaRefugioFilho00) pesoEstimado += 3 * pesoDistancia00 * (int)(distanciaRefugioPai00 - distanciaRefugioFilho00);
                if (distanciaRefugioPai00 < distanciaRefugioFilho00) pesoEstimado -= 3 * pesoDistancia00 * (int)(distanciaRefugioFilho00 - distanciaRefugioPai00);


                if (distanciaRefugioPai01 > distanciaRefugioFilho01) pesoEstimado += 3 * pesoDistancia01 * (int)(distanciaRefugioPai01 - distanciaRefugioFilho01);
                if (distanciaRefugioPai01 < distanciaRefugioFilho01) pesoEstimado -= 3 * pesoDistancia01 * (int)(distanciaRefugioFilho01 - distanciaRefugioPai01);


                if (distanciaRefugioPai10 > distanciaRefugioFilho10) pesoEstimado += 3 * pesoDistancia10 * (int)(distanciaRefugioPai10 - distanciaRefugioFilho10);
                if (distanciaRefugioPai10 < distanciaRefugioFilho10) pesoEstimado -= 3 * pesoDistancia10 * (int)(distanciaRefugioFilho10 - distanciaRefugioPai10);


                if (distanciaRefugioPai11 > distanciaRefugioFilho11) pesoEstimado += 3 * pesoDistancia10 * (int)(distanciaRefugioPai11 - distanciaRefugioFilho11);
                if (distanciaRefugioPai11 < distanciaRefugioFilho11) pesoEstimado -= 3 * pesoDistancia10 * (int)(distanciaRefugioFilho11 - distanciaRefugioPai11);

                if (Controlador != perspectiva)
                {
                    pai.Peso -= (int?)((int)pesoEstimado * 1.5);
                }
            }

            return pesoEstimado;
        }

        public int CalculoExpansão(No<Tabuleiro> pai, No<Tabuleiro> filho)
        {
            int pesoEstimado = 0;

            if (Controlador is Atacante)
            {
                for (int i = 0; i < filho.Objeto.mercenarios.Count; i++)
                {
                    if (!(filho.Objeto.mercenarios[i].Posicao - new Posicao(5, 5)) - !(pai.Objeto.mercenarios[i].Posicao - new Posicao(5, 5)) < 4)
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

        public int CalculoQuadrantes(Tabuleiro pai, Tabuleiro filho, Jogador perspectiva)
        {
            int pesoEstimado = 0;

            double qtdeQuadranteCentralMercenarios = 0;
            double qtdeQuadranteCentralSoldados = 0;
            bool reiQuadranteCentral = false;
            for (int i = 0, j = 3; j < 8; i++, j++)
            {
                for (int n = 3; n < 8; n++)
                {
                    if (filho.casas[j][n].Ocupante is Mercenario) qtdeQuadranteCentralMercenarios++;
                    if (filho.casas[j][n].Ocupante is Soldado) qtdeQuadranteCentralSoldados++;
                    if (filho.casas[j][n].Ocupante is Rei) reiQuadranteCentral = true;
                }
            }

            double qtdeQuadrante00Mercenarios = 0;
            double qtdeQuadrante00Soldados = 0;
            bool reiQuadrante00 = false;
            for (int i = 0, j = 0; j < 5; i++, j++)
            {
                for (int n = 0; n < 5; n++)
                {
                    if (filho.casas[j][n].Ocupante is Mercenario) qtdeQuadrante00Mercenarios++;
                    if (filho.casas[j][n].Ocupante is Soldado) qtdeQuadrante00Soldados++;
                    if (filho.casas[j][n].Ocupante is Rei) reiQuadrante00 = true;
                }
            }

            double qtdeQuadrante01Mercenarios = 0;
            double qtdeQuadrante01Soldados = 0;
            bool reiQuadrante01 = false;
            for (int i = 0, j = 0; j < 5; i++, j++)
            {
                for (int n = 6; n < 11; n++)
                {
                    if (filho.casas[j][n].Ocupante is Mercenario) qtdeQuadrante01Mercenarios++;
                    if (filho.casas[j][n].Ocupante is Soldado) qtdeQuadrante01Soldados++;
                    if (filho.casas[j][n].Ocupante is Rei) reiQuadrante01 = true;
                }
            }

            double qtdeQuadrante10Mercenarios = 0;
            double qtdeQuadrante10Soldados = 0;
            bool reiQuadrante10 = false;
            for (int i = 0, j = 6; j < 11; i++, j++)
            {
                for (int n = 0; n < 5; n++)
                {
                    if (filho.casas[j][n].Ocupante is Mercenario) qtdeQuadrante10Mercenarios++;
                    if (filho.casas[j][n].Ocupante is Soldado) qtdeQuadrante10Soldados++;
                    if (filho.casas[j][n].Ocupante is Rei) reiQuadrante10 = true;
                }
            }

            double qtdeQuadrante11Mercenarios = 0;
            double qtdeQuadrante11Soldados = 0;
            bool reiQuadrante11 = false;
            for (int i = 0, j = 6; j < 11; i++, j++)
            {
                for (int n = 6; n < 11; n++)
                {
                    if (filho.casas[j][n].Ocupante is Mercenario) qtdeQuadrante11Mercenarios++;
                    if (filho.casas[j][n].Ocupante is Soldado) qtdeQuadrante11Soldados++;
                    if (filho.casas[j][n].Ocupante is Rei) reiQuadrante11 = true;
                }
            }

            double pesoQuadranteCentral = 0;
            double pesoQuadrante00 = 0;
            double pesoQuadrante01 = 0;
            double pesoQuadrante10 = 0;
            double pesoQuadrante11 = 0;
            if (!reiQuadranteCentral)
            {
                if (perspectiva is Atacante)
                {
                    if (reiQuadrante00)
                    {
                        pesoQuadrante00 = qtdeQuadrante00Mercenarios / 24;
                    }
                    else if (reiQuadrante01)
                    {
                        pesoQuadrante01 = qtdeQuadrante00Mercenarios / 24;
                    }
                    else if (reiQuadrante10)
                    {
                        pesoQuadrante10 = qtdeQuadrante00Mercenarios / 24;
                    }
                    else if (reiQuadrante11)
                    {
                        pesoQuadrante11 = qtdeQuadrante00Mercenarios / 24;
                    }
                }
                else
                {
                    if (reiQuadrante00)
                    {
                        pesoQuadrante00 = qtdeQuadrante00Soldados / 12;
                    }
                    else if (reiQuadrante01)
                    {
                        pesoQuadrante01 = qtdeQuadrante00Soldados / 12;
                    }
                    else if (reiQuadrante10)
                    {
                        pesoQuadrante10 = qtdeQuadrante00Soldados / 12;
                    }
                    else if (reiQuadrante11)
                    {
                        pesoQuadrante11 = qtdeQuadrante00Soldados / 12;
                    }
                }
            }
            else
            {
                if (perspectiva is Atacante)
                {
                    pesoQuadranteCentral = (qtdeQuadranteCentralMercenarios / 24);
                }
                else
                {
                    pesoQuadranteCentral = Math.Abs(1 - qtdeQuadranteCentralSoldados / 12);
                }
            }

            pesoEstimado += (int)((pesoQuadrante00 + pesoQuadrante01 + pesoQuadrante10 + pesoQuadrante11 + pesoQuadranteCentral) * 100);


            return pesoEstimado;
        }

        public int Heurística(No<Tabuleiro> noAnalisado, No<Tabuleiro> noPai, Jogador perspectiva)
        {
            int pesoCalculado = 0;
            pesoCalculado += CalculoPecas(noPai, noAnalisado, perspectiva);
            pesoCalculado += CalculoDistanciaReiRefugio(noPai, noAnalisado, perspectiva);
            pesoCalculado += CalculoQuadrantes(noPai.Objeto, noAnalisado.Objeto, perspectiva);
            return pesoCalculado;
        }
    }
}