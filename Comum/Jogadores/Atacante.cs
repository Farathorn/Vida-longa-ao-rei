using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Jogadores
{
    public class Atacante : Jogador
    {
        //Regras:
        public new bool MovimentaMercenario { get; protected set; } = true;
    }
}