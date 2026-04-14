using System;
using Inchirieri.Modele;

namespace Inchirieri.Data.Stocare
{
    public static class MasinaTextSerializer
    {
        // Simple CSV: id;marca;model;pret;disponibila;culoare;optiuni
        public static Masina Deserialize(string line)
        {
            var parts = line.Split(';');
            var id = int.Parse(parts[0]);
            var marca = parts[1];
            var model = parts[2];
            var pret = double.Parse(parts[3]);
            var disponibila = bool.Parse(parts[4]);
            var culoare = Enum.TryParse(typeof(CuloareMasina), parts.Length > 5 ? parts[5] : "Necunoscut", out var c) ? (CuloareMasina)c : CuloareMasina.Necunoscut;
            var optiuni = Enum.TryParse(typeof(OptiuniMasina), parts.Length > 6 ? parts[6] : "Niciuna", out var o) ? (OptiuniMasina)o : OptiuniMasina.Niciuna;

            var m = new Masina(id, marca, model, pret, disponibila)
            {
                Culoare = culoare,
                Optiuni = optiuni
            };

            return m;
        }

        public static string Serialize(Masina m)
        {
            return string.Join(";", m.Id, m.Marca, m.Model, m.PretPeZi, m.Disponibila, m.Culoare, m.Optiuni);
        }
    }
}
