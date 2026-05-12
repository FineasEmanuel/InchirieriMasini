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
        ScauneIncalzite = 1 << 3,
        Bluetooth = 1 << 4,
        CameraMarsarier = 1 << 5,
        PilotAutomat = 1 << 6,
        PachetSport = 1 << 7,
        SenzoriParcare = 1 << 8,
        IluminareAmbientala = 1 << 9,
        CinciLocuri = 1 << 10,
        SapteLocuri = 1 << 11
    }

    public enum CuloareMasina
    {
        Necunoscut = 0,
        Rosu,
        Alb,
        Negru,
        Albastru,
        Verde,
        Gri,
        Galben,
        Orange,
        Violet,
        Argintiu,
        Bej,
        Maro
    }

    public class Masina
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Model { get; set; }
        public double PretPeZi { get; set; }
        public bool Disponibila { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Enum fields requested by the assignment
        public CuloareMasina Culoare { get; set; } = CuloareMasina.Necunoscut;
        public OptiuniMasina Optiuni { get; set; } = OptiuniMasina.Niciuna;
        public string Descriere => $"{Marca} {Model}";
        public string StatusDisponibilitate => Disponibila ? "Disponibila" : "Indisponibila";
        public string PretAfisare => $"{PretPeZi:0.##} lei/zi";

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
