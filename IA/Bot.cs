using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VLAR.Comum;

namespace VLAR.IA
{
    public class Bot
    {
        Tabuleiro Jogo { get; set; }

        public Bot(Tabuleiro jogo)
        {
            Jogo = jogo;
        }

        public void Etapas()
        {
            GerarArvore();
            Heurística();

        }

        private void GerarArvore()
        {

        }

        public void Heurística()
        {

        }
    }
}