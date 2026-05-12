using System;
using System.Globalization;
using Inchirieri.Modele;

namespace Inchirieri.Data.Stocare
{
    public static class MasinaTextSerializer
    {
        // Simple text format: id;marca;model;pret;disponibila;culoare;optiuni;imageUrl
        public static Masina Deserialize(string line)
        {
            var parts = line.Split(';');
            var id = int.Parse(parts[0]);
            var marca = parts[1];
            var model = parts[2];
            var pret = double.Parse(parts[3], CultureInfo.InvariantCulture);
            var disponibila = bool.Parse(parts[4]);
            var culoare = Enum.TryParse(parts.Length > 5 ? parts[5] : "Necunoscut", out CuloareMasina c)
                ? c
                : CuloareMasina.Necunoscut;
            var optiuni = Enum.TryParse(parts.Length > 6 ? parts[6] : "Niciuna", out OptiuniMasina o)
                ? o
                : OptiuniMasina.Niciuna;

            var m = new Masina(id, marca, model, pret, disponibila)
            {
                Culoare = culoare,
                Optiuni = optiuni,
                ImageUrl = parts.Length > 7 ? parts[7] : string.Empty
            };

            return m;
        }

        public static string Serialize(Masina m)
        {
            return string.Join(
                ";",
                m.Id,
                Curata(m.Marca),
                Curata(m.Model),
                m.PretPeZi.ToString(CultureInfo.InvariantCulture),
                m.Disponibila,
                m.Culoare,
                m.Optiuni,
                Curata(m.ImageUrl));
        }

        private static string Curata(string text)
        {
            return text.Replace(';', ',').Trim();
        }
    }
}
