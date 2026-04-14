using Inchirieri.Modele;

namespace Inchirieri.Data.Stocare
{
    public static class ClientTextSerializer
    {
        // CSV: nume;prenume;cnp
        public static Client Deserialize(string line)
        {
            var p = line.Split(';');
            return new Client(p[0], p[1], p[2]);
        }

        public static string Serialize(Client c)
        {
            return string.Join(";", c.Nume, c.Prenume, c.CNP);
        }
    }
}
