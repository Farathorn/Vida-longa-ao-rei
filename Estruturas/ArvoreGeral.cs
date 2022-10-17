using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Estruturas.Arvores
{
    public class ArvoreGeral<T> : ArvoreI<T>
    {
        private No<T>? raiz = null;
        public No<T>? ultimoResultado = null;
        public List<No<T>> listaLinearNos = new();
        private long limite = 0;
        private bool temLimite = false;
        public T Raiz
        {
            get
            {
                if (raiz is not null)
                    return raiz.Objeto;
                else throw new Exception("Árvore nula.");
            }
        }

        public long tamanho { get; private set; }

        public void ImporLimite(long limite)
        {
            this.limite = limite;
            temLimite = true;
        }

        public void DesfazerLimite()
        {
            temLimite = false;
        }

        public long? Adicionar(T item, long? pai = null)
        {
            if (temLimite && limite == tamanho) return null;

            long valor = listaLinearNos.Count;
            if (raiz is null)
            {
                if (pai is not null) throw new ArgumentException("Árvore vazia. Parâmetro pai deve ser nulo.");
                raiz = new No<T>(item, 0, valor);
                listaLinearNos.Add(raiz);
            }
            else
            {
                No<T>? noPai = BuscaLarga(pai, raiz);
                if (noPai is null) throw new ArgumentException("Nó pai não existe na árvore.");

                No<T> filho = new(item, valor, noPai.profundidade + 1, noPai);
                noPai.Filhos.Add(filho);
                listaLinearNos.Add(filho);
            }

            tamanho++;
            return valor;
        }

        public No<T>? BuscarPreOrdem(long valor)
        {
            ultimoResultado = BuscaPreOrdemRecursiva(valor, raiz);
            return ultimoResultado;
        }

        private No<T>? BuscaPreOrdemRecursiva(long valor, No<T>? no)
        {
            if (no is null) return null;
            if (no.Valor == valor) return no;

            foreach (No<T> filho in no.Filhos)
            {
                No<T>? ultimo = BuscaPreOrdemRecursiva(valor, filho);
                if (ultimo is not null && ultimo.Valor == valor) return ultimo;
            }

            return null;
        }

        public No<T>? BuscarLargamente(long? valor)
        {
            if (valor is null) return null;
            ultimoResultado = BuscaLarga(valor, raiz);
            return ultimoResultado;
        }

        private No<T>? BuscaLarga(long? valor, No<T>? no)
        {
            if (valor is null || no is null) return null;

            Stack<No<T>> visitados = new Stack<No<T>>();
            visitados.Push(no);

            do
            {
                no = visitados.Pop();
                if (no.Valor == valor) return no;

                foreach (No<T> filho in no.Filhos)
                {
                    if (!visitados.Contains(filho))
                    {
                        if (filho.Valor == valor) return filho;
                        visitados.Push(filho);
                    }
                }
            }
            while (visitados.Count != 0);

            return null;
        }

        public T? Remover(long valor)
        {
            No<T>? removendo = BuscaLarga(valor, raiz);
            if (removendo is null) throw new ArgumentException("Valor não existe na árvore.");
            if (removendo.Pai is not null)
            {
                foreach (No<T> filho in removendo.Filhos)
                {
                    removendo.Pai.Filhos.Add(filho);
                }
                removendo.Pai.Filhos.Remove(removendo);
            }
            else Zerar();
            tamanho--;
            return removendo.Objeto;
        }

        public void Zerar()
        {
            raiz = null;
            tamanho = 0;
        }
    }

    public class No<T>
    {
        public T Objeto { get; set; }
        public long? Valor { get; set; }
        public No<T>? Pai { get; set; }
        public List<No<T>> Filhos { get; set; }
        public long profundidade { get; set; }

        public No(T Objeto, long Valor, long profundidade, No<T>? Pai = null, List<No<T>>? Filhos = null)
        {
            this.profundidade = profundidade;
            this.Objeto = Objeto;
            this.Valor = Valor;
            this.Pai = Pai;
            if (Filhos is not null)
                this.Filhos = new List<No<T>>(Filhos);
            else this.Filhos = new List<No<T>>();
        }
    }
}
