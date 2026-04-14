using System;
using System.Collections.Generic;
using System.Text;

namespace Inchirieri.Modele
{
    internal class Masina
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Model { get; set; }
        public double PretPeZi { get; set; }
        public bool Disponibila { get; set; }
    }
}
