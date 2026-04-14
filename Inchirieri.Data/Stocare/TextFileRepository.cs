using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inchirieri.Modele;

namespace Inchirieri.Data.Stocare
{
    // Generic simple text-file repository where each line represents an entity serialized as CSV
    public class TextFileRepository<T>
    {
        private readonly string _filePath;
        private readonly Func<string, T> _deserializator;
        private readonly Func<T, string> _serializator;

        public TextFileRepository(string filePath, Func<string, T> deserializator, Func<T, string> serializator)
        {
            _filePath = filePath;
            _deserializator = deserializator;
            _serializator = serializator;

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, string.Empty);
        }

        public IEnumerable<T> GetAll()
        {
            return File.ReadAllLines(_filePath)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .Select(l => _deserializator(l));
        }

        public void Add(T entity)
        {
            var line = _serializator(entity);
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }

        public void Update(Func<T, bool> predicat, Action<T> updateAction)
        {
            var items = GetAll().ToList();
            var any = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (predicat(items[i]))
                {
                    updateAction(items[i]);
                    any = true;
                }
            }

            if (any)
            {
                File.WriteAllLines(_filePath, items.Select(e => _serializator(e)));
            }
        }

        public IEnumerable<T> Find(Func<T, bool> predicat)
        {
            return GetAll().Where(predicat);
        }
    }
}
