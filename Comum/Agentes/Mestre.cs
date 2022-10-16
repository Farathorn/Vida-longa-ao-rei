using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Agentes
{
    public class Mestre : Espectador
    {
        public override bool PodeJogar { get; protected set; } = true;
        public override bool MovimentaMercenario { get; protected set; } = true;
        public override bool MovimentaRei { get; protected set; } = true;
        public override bool MovimentaSoldado { get; protected set; } = true;
        public override bool PoderDeMestre { get; protected set; } = true;

        public Mestre(string Nome) : base(Nome)
        {

        }
    }
}