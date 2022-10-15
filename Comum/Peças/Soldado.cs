using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Soldado : Peca
    {
        public Soldado(Tabuleiro Tabuleiro, byte ID, Posicao Posicao) : base(Tabuleiro, ID, Posicao)
        {

        }
    }
}