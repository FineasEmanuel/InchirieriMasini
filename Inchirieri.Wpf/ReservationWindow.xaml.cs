using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Inchirieri.Data.Stocare;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class ReservationWindow : Window
    {
        private readonly Masina _masina;
        private readonly TextFileRepository<Inchiriere> _repoRezervari;
        private readonly TextFileRepository<Client> _repoClienti;

        public ReservationWindow(Masina masina)
        {
            InitializeComponent();

            _masina = masina;
            _repoRezervari = new TextFileRepository<Inchiriere>(
                DataFiles.GetPath("reservari.txt"),
                InchiriereTextSerializer.Deserialize,
                InchiriereTextSerializer.Serialize);
            _repoClienti = new TextFileRepository<Client>(
                DataFiles.GetPath("clienti.txt"),
                ClientTextSerializer.Deserialize,
                ClientTextSerializer.Serialize);

            TxtMasina.Text = $"{masina.Marca} {masina.Model} - {masina.PretPeZi:0.##} lei/zi";
            StartDate.SelectedDateChanged += Dates_SelectedDateChanged;
            EndDate.SelectedDateChanged += Dates_SelectedDateChanged;
        }

        private void Dates_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue)
            {
                return;
            }

            int zile = (EndDate.SelectedDate.Value - StartDate.SelectedDate.Value).Days;
            TxtPretTotal.Text = zile > 0
                ? $"Pret total: {zile * _masina.PretPeZi:0.##} lei ({zile} zile)"
                : "Perioada invalida.";
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string nume = TxtNume.Text.Trim();
            string prenume = TxtPrenume.Text.Trim();
            string cnp = TxtCNP.Text.Trim();

            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) || cnp.Length != 13)
            {
                MessageBox.Show("Completeaza corect datele personale (CNP 13 caractere).", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Selecteaza perioada.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int zile = (EndDate.SelectedDate.Value - StartDate.SelectedDate.Value).Days;
            if (zile <= 0)
            {
                MessageBox.Show("Perioada invalida.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Client client = new Client(nume, prenume, cnp);
            double total = zile * _masina.PretPeZi;
            Inchiriere inchiriere = new Inchiriere(_masina, client, StartDate.SelectedDate.Value, EndDate.SelectedDate.Value, total);

            bool conflict = _repoRezervari.GetAll().Any(r =>
                r.Masina.Id == _masina.Id &&
                inchiriere.Start < r.End &&
                inchiriere.End > r.Start);

            if (conflict)
            {
                MessageBox.Show("Masina nu este disponibila in perioada selectata.", "Conflict rezervare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _repoRezervari.Add(inchiriere);

            if (!_repoClienti.GetAll().Any(c => c.CNP == client.CNP))
            {
                _repoClienti.Add(client);
            }

            MessageBox.Show($"Rezervare confirmata. Total: {total:0.##} lei", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
