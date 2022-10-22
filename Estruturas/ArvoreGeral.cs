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
        public List<No<T>?> listaOrdenadaLargura = new();
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

            long valor = listaOrdenadaLargura.Count;
            if (raiz is null)
            {
                if (pai is not null) throw new ArgumentException("Árvore vazia. Parâmetro pai deve ser nulo.");
                raiz = new No<T>(item, valor, 0);
                raiz.Peso = 0;
                listaOrdenadaLargura.Add(raiz);
            }
            else
            {
                if (pai is null) throw new ArgumentException("Nenhum pai especificado.");
                No<T>? noPai = listaOrdenadaLargura[(int)pai];
                if (noPai is null) throw new ArgumentException("Nó pai não existe na árvore.");

                No<T> filho = new(item, valor, noPai.profundidade + 1, noPai);
                noPai.Filhos.Add(filho);
                listaOrdenadaLargura.Add(filho);
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

            foreach (No<T>? filho in no.Filhos)
            {
                No<T>? ultimo = BuscaPreOrdemRecursiva(valor, filho);
                if (ultimo is not null && ultimo.Valor == valor) return ultimo;
            }

            return null;
        }

        public No<T>? BuscarLargamente(long? valor)
        {
            if (valor is null) return null;
            ultimoResultado = BuscaLarga(valor);
            return ultimoResultado;
        }

        private No<T>? BuscaLarga(long? valor)
        {
            if (valor is null || raiz is null) return null;

            Queue<No<T>> visitados = new Queue<No<T>>();
            visitados.Enqueue(raiz);

            do
            {
                var no = visitados.Dequeue();
                if (no.Valor == valor) return no;

                foreach (No<T>? filho in no.Filhos)
                {
                    if (filho is not null && !visitados.Contains(filho))
                    {
                        if (filho.Valor == valor) return filho;
                        visitados.Enqueue(filho);
                    }
                }
            }
            while (visitados.Count != 0);

            return null;
        }

        public T? RemoverNo(long? valor)
        {
            if (valor is null) throw new Exception("Valor inválido.");
            No<T>? removendo = listaOrdenadaLargura[(int)valor];
            if (removendo is null) throw new ArgumentException("Valor não existe na árvore.");
            if (removendo.Pai is not null)
            {
                foreach (No<T>? filho in removendo.Filhos)
                {
                    if (filho is not null) RemoverNo(filho.Valor);
                }
                removendo.Pai.Filhos[removendo.Pai.Filhos.IndexOf(removendo)] = null;
                listaOrdenadaLargura.RemoveAt((int)valor);
            }
            else Zerar();
            tamanho--;
            return removendo.Objeto;
        }

        public void Zerar()
        {
            raiz = null;
            listaOrdenadaLargura = new();
            tamanho = 0;
        }
    }

    public class No<T>
    {
        public T Objeto { get; set; }
        public long? Valor { get; set; }
        public No<T>? Pai { get; set; }
        public List<No<T>?> Filhos { get; set; }
        public List<List<No<T>>> caminhosPerpassantes = new();
        public long profundidade { get; set; }
        public int? Peso { get; set; }
        public bool Visitado { get; set; } = false;
        public bool eNoRuim { get; set; } = false;
        public bool eNoQueRecalculou { get; set; } = false;

        public No(No<T> copiando)
        {
            this.Objeto = copiando.Objeto;
            this.Valor = copiando.Valor;
            this.Pai = copiando.Pai;
            this.Filhos = copiando.Filhos = new(copiando.Filhos);
            this.caminhosPerpassantes = new(copiando.caminhosPerpassantes);
            this.profundidade = copiando.profundidade;
            this.Peso = copiando.Peso;
            this.Visitado = false;
        }

        public No(T objeto, long valor, long profundidade, No<T>? pai = null, int? peso = null, List<No<T>?>? filhos = null)
        {
            this.profundidade = profundidade;
            this.Objeto = objeto;
            this.Valor = valor;
            this.Peso = peso;
            this.Pai = pai;
            if (filhos is not null)
                this.Filhos = new(filhos);
            else this.Filhos = new();
            this.Visitado = false;
        }
    }
}
