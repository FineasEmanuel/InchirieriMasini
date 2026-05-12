using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Inchirieri.Data.Stocare;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class ClientViewWindow : Window
    {
        private readonly TextFileRepository<Masina> _repoMasini;
        private readonly TextFileRepository<Client> _repoClienti;
        private List<Masina> _masini = new();
        private List<Client> _clienti = new();

        public ClientViewWindow()
        {
            InitializeComponent();

            _repoMasini = new TextFileRepository<Masina>(
                DataFiles.GetPath("masini.txt"),
                MasinaTextSerializer.Deserialize,
                MasinaTextSerializer.Serialize);
            _repoClienti = new TextFileRepository<Client>(
                DataFiles.GetPath("clienti.txt"),
                ClientTextSerializer.Deserialize,
                ClientTextSerializer.Serialize);

            IncarcaDate();
        }

        private void IncarcaDate()
        {
            _masini = _repoMasini.GetAll().Where(m => m.Disponibila).OrderBy(m => m.Marca).ThenBy(m => m.Model).ToList();
            _clienti = _repoClienti.GetAll().OrderBy(c => c.Nume).ThenBy(c => c.Prenume).ToList();

            LstMasiniClient.ItemsSource = _masini;
            LstClienti.ItemsSource = _clienti;
        }

        private void LstMasiniClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstMasiniClient.SelectedItem is Masina masina)
            {
                TxtDetaliiClient.Text =
                    $"ID: {masina.Id}\n" +
                    $"Masina: {masina.Marca} {masina.Model}\n" +
                    $"Pret/zi: {masina.PretPeZi:0.##} lei\n" +
                    $"Culoare: {masina.Culoare}\n" +
                    $"Optiuni: {masina.Optiuni}";
                BtnReserve.IsEnabled = true;
                IncarcaImagine(masina.ImageUrl);
            }
            else
            {
                BtnReserve.IsEnabled = false;
                TxtDetaliiClient.Text = "Selecteaza o masina...";
                ImgClientDetalii.Source = null;
            }
        }

        private void IncarcaImagine(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uri))
            {
                ImgClientDetalii.Source = new BitmapImage(uri);
            }
            else
            {
                ImgClientDetalii.Source = null;
            }
        }

        private void BtnSearchClient_Click(object sender, RoutedEventArgs e)
        {
            string query = TxtCautareClient.Text.Trim();
            LstMasiniClient.ItemsSource = string.IsNullOrWhiteSpace(query)
                ? _masini
                : _masini.Where(m =>
                    m.Marca.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Model.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void BtnSearchClientEntity_Click(object sender, RoutedEventArgs e)
        {
            string query = TxtClientSearch.Text.Trim();
            LstClienti.ItemsSource = string.IsNullOrWhiteSpace(query)
                ? _clienti
                : _clienti.Where(c =>
                    c.Nume.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    c.Prenume.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    c.CNP.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void LstClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstClienti.SelectedItem is Client client)
            {
                EdNume.Text = client.Nume;
                EdPrenume.Text = client.Prenume;
                EdCNP.Text = client.CNP;
            }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            if (!CitesteClient(out Client client))
            {
                return;
            }

            if (_clienti.Any(c => c.CNP == client.CNP))
            {
                MessageBox.Show("Exista deja un client cu acest CNP.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _clienti.Add(client);
            SalveazaClienti(client);
        }

        private void BtnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            if (LstClienti.SelectedItem is not Client selected)
            {
                return;
            }

            if (!CitesteClient(out Client client))
            {
                return;
            }

            selected.Nume = client.Nume;
            selected.Prenume = client.Prenume;
            selected.CNP = client.CNP;
            SalveazaClienti(selected);
            MessageBox.Show("Client actualizat.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (LstClienti.SelectedItem is not Client selected)
            {
                return;
            }

            if (MessageBox.Show("Stergi clientul selectat?", "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            _clienti.Remove(selected);
            SalveazaClienti();
        }

        private bool CitesteClient(out Client client)
        {
            client = new Client(string.Empty, string.Empty, string.Empty);
            string nume = EdNume.Text.Trim();
            string prenume = EdPrenume.Text.Trim();
            string cnp = EdCNP.Text.Trim();

            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume) || cnp.Length != 13)
            {
                MessageBox.Show("Completeaza nume, prenume si CNP valid de 13 caractere.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            client = new Client(nume, prenume, cnp);
            return true;
        }

        private void SalveazaClienti(Client? selectat = null)
        {
            _repoClienti.SaveAll(_clienti.OrderBy(c => c.Nume).ThenBy(c => c.Prenume));
            _clienti = _repoClienti.GetAll().OrderBy(c => c.Nume).ThenBy(c => c.Prenume).ToList();
            LstClienti.ItemsSource = null;
            LstClienti.ItemsSource = _clienti;

            if (selectat != null)
            {
                LstClienti.SelectedItem = _clienti.FirstOrDefault(c => c.CNP == selectat.CNP);
            }
        }

        private void TxtCautareClient_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnSearchClient_Click(sender, new RoutedEventArgs());
            }
        }

        private void BtnReserve_Click(object sender, RoutedEventArgs e)
        {
            if (LstMasiniClient.SelectedItem is Masina masina)
            {
                var dialog = new ReservationWindow(masina) { Owner = this };
                if (dialog.ShowDialog() == true)
                {
                    MessageBox.Show("Rezervarea a fost salvata.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Selecteaza mai intai o masina pentru rezervare.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
