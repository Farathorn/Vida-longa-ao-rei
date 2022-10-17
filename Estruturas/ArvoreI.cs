using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Estruturas.Arvores
{
    public interface ArvoreI<T>
    {
        long? Adicionar(T item, long? pai);

        T? Remover(long valor);

        void Zerar();
    }
}