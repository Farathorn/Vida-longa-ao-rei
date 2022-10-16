using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Agentes
{
    public abstract class Jogador : Agente
    {
        //Regras:
        public override bool PodeJogar { get; protected set; } = true;

        public Jogador(string Nome) : base(Nome)
        {

        }

        public bool Movimentar(Peca peca, Direcao sentido, int quanto)
        {
            if (PodeJogar)
            {
                if (peca is Rei && this.MovimentaRei)
                {
                    return peca.Mover(sentido, quanto);
                }
                else if (peca is Soldado && this.MovimentaSoldado)
                {
                    return peca.Mover(sentido, quanto);
                }
                else if (peca is Mercenario && this.MovimentaMercenario)
                {
                    return peca.Mover(sentido, quanto);
                }
            }
            throw new Exception("Agente não possui privilégios no jogo.");
        }
    }
}