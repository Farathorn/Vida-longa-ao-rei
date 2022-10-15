using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Agentes
{
    public class Mestre : Espectador
    {
        public new bool PodeJogar { get; protected set; } = true;
        public new bool MovimentaMercenario { get; protected set; } = true;
        public new bool MovimentaRei { get; protected set; } = true;
        public new bool MovimentaSoldado { get; protected set; } = true;
        public new bool PoderDeMestre { get; protected set; } = true;
    }
}