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

        public Peca(byte ID, Tuple<byte, byte> Coordenada)
        {
            this.ID = ID;
            this.Coordenada = Coordenada;
        }

        public bool Mover(Direcao sentido, long casas)
        {

            return true;
        }
    }
}