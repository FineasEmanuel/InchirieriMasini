using System;
using System.Globalization;
using Inchirieri.Modele;

namespace Inchirieri.Data.Stocare
{
    public static class InchiriereTextSerializer
    {
        // CSV: masinaId;marca;model;nume;prenume;cnp;start;end;total
        public static Inchiriere Deserialize(string line)
        {
            var p = line.Split(';');
            var id = int.Parse(p[0]);
            var marca = p[1];
            var model = p[2];
            var nume = p[3];
            var prenume = p[4];
            var cnp = p[5];
            var start = DateTime.Parse(p[6]);
            var end = DateTime.Parse(p[7]);
            var total = double.Parse(p[8], CultureInfo.InvariantCulture);

            var masina = new Masina(id, marca, model, 0, false) { Culoare = CuloareMasina.Necunoscut };
            var client = new Client(nume, prenume, cnp);
            return new Inchiriere(masina, client, start, end, total);
        }

        public static string Serialize(Inchiriere i)
        {
            return string.Join(
                ";",
                i.Masina.Id,
                Curata(i.Masina.Marca),
                Curata(i.Masina.Model),
                Curata(i.Client.Nume),
                Curata(i.Client.Prenume),
                Curata(i.Client.CNP),
                i.Start.ToString("o"),
                i.End.ToString("o"),
                i.Total.ToString(CultureInfo.InvariantCulture));
        }

        private static string Curata(string text)
        {
            return text.Replace(';', ',').Trim();
        }
    }
}
