using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Soldado : Peca
    {
        public Soldado(Soldado copiando, Tabuleiro? tabuleiro) : base(copiando, tabuleiro)
        {

        }

        public Soldado(byte ID, Posicao Posicao) : base(ID, Posicao)
        {

        }
    }
}