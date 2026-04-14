using System;
using System.Collections.Generic;
using Inchirieri.Modele;

class Program
{
    static void Main()
    {
        List<Masina> masini = InitializareMasini();
        List<Client> clienti = new List<Client>();
        List<Inchiriere> inchirieri = new List<Inchiriere>();
        List<Angajat> angajati = InitializareAngajati();

        Console.WriteLine("=== LOGIN ===");

        Console.Write("Username: ");
        string user = Console.ReadLine();

        Console.Write("Parola: ");
        string pass = Console.ReadLine();

        Angajat angajatLogat = angajati
            .Find(a => a.Username == user && a.Parola == pass);

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
            int opt = int.Parse(Console.ReadLine());

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
            }
        }
    }

    // 🔹 AFISARE
    static void AfisareMasini(List<Masina> masini)
    {
        foreach (var m in masini)
        {
            Console.WriteLine($"{m.Id} - {m.Marca} {m.Model} - {m.PretPeZi} lei - {(m.Disponibila ? "Disponibila" : "Indisponibila")}");
        }
    }

    static void AfisareMasiniDisponibile(List<Masina> masini)
    {
        foreach (var m in masini)
        {
            if (m.Disponibila)
            {
                Console.WriteLine($"{m.Id} - {m.Marca} {m.Model}");
            }
        }
    }

    // 🔹 INCHIRIERE
    static void InchiriazaMasina(List<Masina> masini, List<Client> clienti, List<Inchiriere> inchirieri)
    {
        AfisareMasiniDisponibile(masini);

        Console.Write("Alege ID masina: ");
        int id = int.Parse(Console.ReadLine());

        Masina masina = masini.Find(m => m.Id == id && m.Disponibila);

        if (masina == null)
        {
            Console.WriteLine("Masina indisponibila!");
            return;
        }

        // Client
        Console.Write("Nume: ");
        string nume = Console.ReadLine();

        Console.Write("Prenume: ");
        string prenume = Console.ReadLine();

        Console.Write("CNP: ");
        string cnp = Console.ReadLine();

        if (cnp.Length != 13)
        {
            Console.WriteLine("CNP invalid!");
            return;
        }

        Client client = clienti.Find(c => c.CNP == cnp);

        if (client == null)
        {
            client = new Client(nume, prenume, cnp);
            clienti.Add(client);
        }

        // Perioada
        Console.Write("Data inceput (yyyy-mm-dd): ");
        DateTime start = DateTime.Parse(Console.ReadLine());

        Console.Write("Data sfarsit: ");
        DateTime end = DateTime.Parse(Console.ReadLine());

        int zile = (end - start).Days;

        if (zile <= 0)
        {
            Console.WriteLine("Perioada invalida!");
            return;
        }

        double total = zile * masina.PretPeZi;

        Console.WriteLine($"Total: {total} lei");

        Console.Write("Confirmi? (y/n): ");
        string conf = Console.ReadLine();

        if (conf.ToLower() != "y")
            return;

        Inchiriere inchiriere = new Inchiriere(masina, client, start, end, total);

        inchirieri.Add(inchiriere);

        masina.Disponibila = false;

        Console.WriteLine("Inchiriere realizata!");

        // "Contract"
        Console.WriteLine("\n--- CONTRACT ---");
        Console.WriteLine($"Client: {client.Nume} {client.Prenume}");
        Console.WriteLine($"Masina: {masina.Marca} {masina.Model}");
        Console.WriteLine($"Perioada: {start.ToShortDateString()} - {end.ToShortDateString()}");
        Console.WriteLine($"Total: {total} lei");
    }

    // 🔹 DATE INITIALE
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
}chat