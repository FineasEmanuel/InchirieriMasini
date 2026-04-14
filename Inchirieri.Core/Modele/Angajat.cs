namespace Inchirieri.Modele
{
    public class Angajat
    {
        public string Username { get; }
        public string Parola { get; }

        public Angajat(string username, string parola)
        {
            Username = username;
            Parola = parola;
        }
    }
}
