# InchirieriMasini

Aplicatie C# / WPF pentru administrarea unei firme de inchirieri auto.

## Structura

- `Inchirieri.Core` - modele de domeniu: `Masina`, `Client`, `Inchiriere`, `Angajat`.
- `Inchirieri.Data` - persistenta in fisiere text si serializatoare.
- `Inchirieri.Wpf` - interfata grafica pentru angajati si clienti.
- `Inchirieri` - varianta simpla de consola.

## Functionalitati

- autentificare pentru angajati si clienti;
- CRUD complet pentru masini, cu salvare in `data/masini.txt`;
- CRUD pentru clienti, cu salvare in `data/clienti.txt`;
- cautare si filtrare masini dupa marca, model, disponibilitate si optiuni;
- rezervare masina cu verificare de suprapuneri;
- calcul automat al pretului in functie de perioada selectata;
- stocare in fisiere text pentru masini, clienti, angajati si rezervari;
- UI WPF distinct, cu tema albastru-indigo si accent amber.

## Rulare

```powershell
dotnet build Inchirieri.sln
dotnet run --project .\Inchirieri.Wpf\Inchirieri.Wpf.csproj
```

Pentru login rapid exista contul initial `admin` / `1234`, creat automat daca nu exista angajati salvati.
