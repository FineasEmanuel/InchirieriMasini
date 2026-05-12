using Inchirieri.Modele;

class Program
{
    static void Main()
    {
        List<Masina> masini = InitializareMasini();
        List<Client> clienti = new();
        List<Inchiriere> inchirieri = new();
        List<Angajat> angajati = InitializareAngajati();

        Console.WriteLine("=== LOGIN ===");
        Console.Write("Username: ");
        string user = Console.ReadLine() ?? string.Empty;

        Console.Write("Parola: ");
        string pass = Console.ReadLine() ?? string.Empty;

        Angajat? angajatLogat = angajati.Find(a => a.Username == user && a.Parola == pass);
        if (angajatLogat == null)
        {
            Console.WriteLine("Autentificare esuata!");
            return;
        }

        Console.WriteLine("Autentificare reusita!");

        bool ruleaza = true;
        while (ruleaza)
        {
            Console.WriteLine("\n1. Afisare masini");
            Console.WriteLine("2. Masini disponibile");
            Console.WriteLine("3. Inchiriere masina");
            Console.WriteLine("0. Iesire");
            Console.Write("Optiune: ");

            if (!int.TryParse(Console.ReadLine(), out int opt))
            {
                Console.WriteLine("Optiune invalida.");
                continue;
            }

            switch (opt)
            {
                case 1:
                    AfisareMasini(masini);
                    break;
                case 2:
                    AfisareMasiniDisponibile(masini);
                    break;
                case 3:
                    InchiriazaMasina(masini, clienti, inchirieri);
                    break;
                case 0:
                    ruleaza = false;
                    break;
                default:
                    Console.WriteLine("Optiune necunoscuta.");
                    break;
            }
        }
    }

    static void AfisareMasini(List<Masina> masini)
    {
        foreach (Masina masina in masini)
        {
            Console.WriteLine($"{masina.Id} - {masina.Marca} {masina.Model} - {masina.PretPeZi} lei - {masina.StatusDisponibilitate}");
        }
    }

    static void AfisareMasiniDisponibile(List<Masina> masini)
    {
        foreach (Masina masina in masini.Where(m => m.Disponibila))
        {
            Console.WriteLine($"{masina.Id} - {masina.Marca} {masina.Model}");
        }
    }

    static void InchiriazaMasina(List<Masina> masini, List<Client> clienti, List<Inchiriere> inchirieri)
    {
        AfisareMasiniDisponibile(masini);

        Console.Write("Alege ID masina: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID invalid.");
            return;
        }

        Masina? masina = masini.Find(m => m.Id == id && m.Disponibila);
        if (masina == null)
        {
            Console.WriteLine("Masina indisponibila!");
            return;
        }

        Console.Write("Nume: ");
        string nume = Console.ReadLine() ?? string.Empty;

        Console.Write("Prenume: ");
        string prenume = Console.ReadLine() ?? string.Empty;

        Console.Write("CNP: ");
        string cnp = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) || cnp.Length != 13)
        {
            Console.WriteLine("Date client invalide!");
            return;
        }

        Client? client = clienti.Find(c => c.CNP == cnp);
        if (client == null)
        {
            client = new Client(nume, prenume, cnp);
            clienti.Add(client);
        }

        Console.Write("Data inceput (yyyy-mm-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
        {
            Console.WriteLine("Data de inceput invalida.");
            return;
        }

        Console.Write("Data sfarsit (yyyy-mm-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime end))
        {
            Console.WriteLine("Data de sfarsit invalida.");
            return;
        }

        int zile = (end - start).Days;
        if (zile <= 0)
        {
            Console.WriteLine("Perioada invalida!");
            return;
        }

        double total = zile * masina.PretPeZi;
        Console.WriteLine($"Total: {total:0.##} lei");
        Console.Write("Confirmi? (y/n): ");
        string confirmare = Console.ReadLine() ?? string.Empty;
        if (!confirmare.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Inchiriere inchiriere = new Inchiriere(masina, client, start, end, total);
        inchirieri.Add(inchiriere);
        masina.Disponibila = false;

        Console.WriteLine("Inchiriere realizata!");
        Console.WriteLine("\n--- CONTRACT ---");
        Console.WriteLine($"Client: {client.Nume} {client.Prenume}");
        Console.WriteLine($"Masina: {masina.Marca} {masina.Model}");
        Console.WriteLine($"Perioada: {start.ToShortDateString()} - {end.ToShortDateString()}");
        Console.WriteLine($"Total: {total:0.##} lei");
    }

    static List<Masina> InitializareMasini()
    {
        return new List<Masina>
        {
            new Masina(1, "Dacia", "Logan", 100, true),
            new Masina(2, "BMW", "X5", 300, true),
            new Masina(3, "Audi", "A4", 250, true)
        };
    }

    static List<Angajat> InitializareAngajati()
    {
        return new List<Angajat>
        {
            new Angajat("admin", "1234")
        };
    }
}
