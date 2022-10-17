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

        private void GerarArvore(byte profundidade = 1, long? pai = null)
        {
            byte pecasControladas = 0;
            if (Controlador is Defensor)
            {
                foreach (Peca peca in Jogo.pecas)
                {
                    if (peca is Soldado || peca is Rei)
                    {
                        pecasControladas++;
                        MatrizPossibilidades.Add(peca.MovimentosPossiveis());
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

                }
            }

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
                        Tabuleiro possibilidade = new(analisando);
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

        public void Heurística()
        {

        }
    }
}