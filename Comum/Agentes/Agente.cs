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

        //Ambiente
        protected Tabuleiro? tabuleiro = null;
        public Tabuleiro? Tabuleiro
        {
            get
            {
                return tabuleiro;
            }
            set
            {
                if (tabuleiro is null) tabuleiro = value;
            }
        }

        //Informações:
        public string Nome { get; set; }

        public Agente(string Nome)
        {
            this.Nome = Nome;
        }
    }
}