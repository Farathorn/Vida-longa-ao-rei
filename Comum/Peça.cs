using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public abstract class Peca
    {
        private byte ID { get; set; }
        private Tuple<byte, byte> Coordenada { get; set; }

        public enum Direcao
        {
            Cima,
            Esquerda,
            Direita,
            Baixo
        }

        public bool Mover (Direcao sentido, long casas)
        {

            return true;
        }
    }
}