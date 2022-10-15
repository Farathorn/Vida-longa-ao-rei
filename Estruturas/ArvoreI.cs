using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLAR.Estruturas.Arvores
{
    public interface ArvoreI<T>
    {
        bool Adicionar (T item, long valor, long? pai);

        T Remover (long valor);

        void Zerar ();
    }
}