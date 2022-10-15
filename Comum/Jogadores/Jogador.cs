using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum.Jogadores
{
    public abstract class Jogador : Agente
    {
        //Regras:
        public new bool PodeJogar = true;
    }
}