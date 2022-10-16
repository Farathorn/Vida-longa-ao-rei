using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VLAR.Comum;

namespace VLAR.Comum.Agentes
{
    public class Atacante : Jogador
    {
        //Regras:
        public new bool MovimentaMercenario { get; protected set; } = true;

        public Atacante(string Nome) : base(Nome)
        {

        }
    }
}