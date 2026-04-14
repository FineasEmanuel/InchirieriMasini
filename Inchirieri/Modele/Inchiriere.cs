using System;
using System.Collections.Generic;
using System.Text;

namespace Inchirieri.Modele
{
    internal class Inchiriere
    {
        public Masina Masina { get; set; }
        public Client Client { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Total { get; set; }
    }
}
