using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arvores
{
    public class ArvoreGeral<T> : ArvoreI<T>
    {
        private No<T> raiz;
        public No<T> ultimoResultado;
        private long limite = 0;
        private bool temLimite = false;
        public T Raiz
        {
            get
            {
                return raiz.Objeto;
            }
        }

        public long tamanho { get; private set; }

        public void ImporLimite (long limite)
        {
            this.limite = limite;
            temLimite = true;
        }

        public void DesfazerLimite ()
        {
            temLimite = false;
        }

        public bool Adicionar (T item, long valor, long? pai = null)
        {
            if (temLimite && limite == tamanho) return false;

            if (tamanho == 0)
            {
                if (pai is not null) throw new ArgumentException("Árvore vazia. Parâmetro pai deve ser nulo.");
                raiz = new No<T>(item, valor);
            }
            else
            {
                No<T> noPai = BuscaLarga(pai, raiz);
                if (noPai is null) throw new ArgumentException("Nó pai não existe na árvore.");

                noPai.Filhos.Add(new No<T>(item, valor, noPai));
            }

            tamanho++;
            return true;
        }

        public No<T> BuscarPreOrdem (long valor)
        {
            ultimoResultado = BuscaPreOrdemRecursiva(valor, raiz);
            return ultimoResultado;
        }

        private No<T> BuscaPreOrdemRecursiva (long valor, No<T> no)
        {
            if (no.Valor == valor) return no;
            if (no is null) return null;

            foreach (No<T> filho in no.Filhos)
            {
                No<T> ultimo = BuscaPreOrdemRecursiva(valor, filho);
                if (ultimo.Valor == valor) return ultimo;
            }

            return null;
        }

        public No<T> BuscarLargamente (long valor)
        {
            ultimoResultado = BuscaLarga(valor, raiz);
            return ultimoResultado;
        }

        private No<T> BuscaLarga (long? valor, No<T> no)
        {
            if (valor == null) return null;

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

        public T Remover (long valor)
        {
            No<T> removendo = BuscaLarga(valor, raiz);
            removendo.Pai.Filhos.Remove(removendo);
            return removendo.Objeto;
        }

        public void Zerar ()
        {
            raiz = null;
            tamanho = 0;
        }
    }

    public class No<T>
    {
        public T Objeto { get; set; }
        public long? Valor { get; set; }
        public No<T> Pai { get; set; }
        public List<No<T>> Filhos { get; set; }

        public No (T Objeto, long Valor, No<T> Pai = null, List<No<T>> Filhos = null)
        {
            this.Objeto = Objeto;
            this.Valor = Valor;
            this.Pai = Pai;
            this.Filhos = Filhos;
        }
    }
}
