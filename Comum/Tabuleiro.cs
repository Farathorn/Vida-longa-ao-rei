using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Comum
{
    public class Tabuleiro
    {
        public List<Peca> pecas { get; private set; } = new List<Peca>();
        private byte limitePecas { get; set; }
        public List<List<Casa>> casas { get; private set; }


        public Tabuleiro(byte largura, byte altura, byte limitePecas)
        {
            this.limitePecas = limitePecas;

            casas = new List<List<Casa>>(altura);
            for (byte i = 0; i < altura; i++)
            {
                casas[i] = new List<Casa>(largura);
                for (byte j = 0; j < largura; j++)
                {
                    casas[i][j] = new Casa(new Posicao(i, j));
                }
            }
        }

        public bool InserirPeca(Peca nova)
        {
            if (pecas.Count == limitePecas) return false;

            pecas.Add(nova);
            casas[nova.Posicao.x][nova.Posicao.y].condicao = Casa.Condicao.Ocupada;
            casas[nova.Posicao.x][nova.Posicao.y].Ocupante = nova;

            return true;
        }
    }

    public class Casa
    {
        public Posicao Coordenada { get; private set; }
        public enum Condicao
        {
            Ocupada,
            Desocupada
        }

        public Condicao condicao { get; set; } = Condicao.Desocupada;
        public Peca? Ocupante { get; set; } = null;

        public Casa(Posicao Coordenada)
        {
            this.Coordenada = Coordenada;
        }
    }
}