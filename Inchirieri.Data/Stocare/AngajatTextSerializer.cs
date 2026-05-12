using Inchirieri.Modele;

namespace Inchirieri.Data.Stocare
{
    public static class AngajatTextSerializer
    {
        // CSV: username;parola
        public static Angajat Deserialize(string line)
        {
            var p = line.Split(';');
            return new Angajat(p[0], p[1]);
        }

        public static string Serialize(Angajat a)
        {
            return string.Join(";", Curata(a.Username), Curata(a.Parola));
        }

        private static string Curata(string text)
        {
            return text.Replace(';', ',').Trim();
        }
    }
}
