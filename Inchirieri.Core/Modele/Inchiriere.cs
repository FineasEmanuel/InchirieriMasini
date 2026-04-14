using System;

namespace Inchirieri.Modele
{
    public class Inchiriere
    {
        public Masina Masina { get; }
        public Client Client { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
        public double Total { get; }

        public Inchiriere(Masina masina, Client client, DateTime start, DateTime end, double total)
        {
            Masina = masina;
            Client = client;
            Start = start;
            End = end;
            Total = total;
        }
    }
}
