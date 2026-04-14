namespace Inchirieri.Modele
{
    public class Client
    {
        public string Nume { get; }
        public string Prenume { get; }
        public string CNP { get; }

        public Client(string nume, string prenume, string cnp)
        {
            Nume = nume;
            Prenume = prenume;
            CNP = cnp;
        }
    }
}
