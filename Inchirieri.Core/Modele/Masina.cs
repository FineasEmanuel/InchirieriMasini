using System;

namespace Inchirieri.Modele
{
    [Flags]
    public enum OptiuniMasina
    {
        Niciuna = 0,
        AerConditionat = 1 << 0,
        Navigatie = 1 << 1,
        CutieAutomata = 1 << 2,
        ScauneIncalzite = 1 << 3
    }

    public enum CuloareMasina
    {
        Necunoscut = 0,
        Rosu,
        Alb,
        Negru,
        Albastru
    }

    public class Masina
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Model { get; set; }
        public double PretPeZi { get; set; }
        public bool Disponibila { get; set; }

        // Enum fields requested by the assignment
        public CuloareMasina Culoare { get; set; } = CuloareMasina.Necunoscut;
        public OptiuniMasina Optiuni { get; set; } = OptiuniMasina.Niciuna;

        public Masina(int id, string marca, string model, double pretPeZi, bool disponibila)
        {
            Id = id;
            Marca = marca;
            Model = model;
            PretPeZi = pretPeZi;
            Disponibila = disponibila;
        }
    }
}
