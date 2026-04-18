using System.Collections.Generic;
using Inchirieri.Modele;
using System.Linq;

namespace Inchirieri.Core.Administrare
{
    // Simplified in-memory administrator used for demo/homework
    public class AdministratorEntitateMemorie<T>
    {
        private readonly List<T> _lista = new List<T>();

        public void Adauga(T element)
        {
            _lista.Add(element);
        }

        public void Sterge(T element)
        {
            _lista.Remove(element);
        }

        public IEnumerable<T> Toate()
        {
            // Return all elements (use LINQ/Enumerable to satisfy requirement if needed)
            return _lista.AsEnumerable();
        }

        // Modified to use LINQ as requested: return elements matching a predicate
        public IEnumerable<T> Cauta(System.Func<T, bool> predicat)
        {
            return _lista.Where(predicat);
        }
    }
}
