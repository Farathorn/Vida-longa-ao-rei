using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Mercenario : Peca
    {
        public Mercenario(Mercenario copiando) : base(copiando)
        {

        }

        public Mercenario(byte ID, Posicao Posicao) : base(ID, Posicao)
        {


        }
    }
}