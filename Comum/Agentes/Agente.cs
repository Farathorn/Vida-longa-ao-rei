using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Agentes
{
    public abstract class Agente
    {
        //Regras:
        public bool PodeJogar { get; protected set; } = false;
        public bool MovimentaRei { get; protected set; } = false;
        public bool MovimentaSoldado { get; protected set; } = false;
        public bool MovimentaMercenario { get; protected set; } = false;
        public bool PoderDeMestre { get; protected set; } = false;
        public bool IA { get; protected set; } = false;
    }
}