using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Agentes
{
    public class Defensor : Jogador
    {
        public new bool MovimentaRei { get; protected set; } = true;
        public new bool MovimentaSoldado { get; protected set; } = true;

        public Defensor(string Nome) : base(Nome)
        {

        }
    }
}