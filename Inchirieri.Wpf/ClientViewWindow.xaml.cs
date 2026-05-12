using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class ClientViewWindow : Window
    {
        private System.Collections.Generic.List<Masina> _masini;
        private System.Collections.Generic.List<Client> _clienti = new System.Collections.Generic.List<Client>();
        private TextFileRepository<Client>? _repoClienti;

        public ClientViewWindow()
        {
            InitializeComponent();

            _masini = new System.Collections.Generic.List<Masina>
            {
                new Masina(1, "Dacia", "Logan", 100, true) { Culoare = CuloareMasina.Rosu, Optiuni = OptiuniMasina.AerConditionat },
                new Masina(2, "BMW", "X5", 300, true) { Culoare = CuloareMasina.Negru, Optiuni = OptiuniMasina.Navigatie | OptiuniMasina.CutieAutomata },
                new Masina(3, "Audi", "A4", 250, false) { Culoare = CuloareMasina.Albastru, Optiuni = OptiuniMasina.ScauneIncalzite },
                new Masina(4, "Toyota", "Corolla", 150, true) { Culoare = CuloareMasina.Alb, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Navigatie },
                new Masina(5, "Ford", "Focus", 120, true) { Culoare = CuloareMasina.Necunoscut, Optiuni = OptiuniMasina.Niciuna }
            };

            LstMasiniClient.ItemsSource = _masini;
            _repoClienti = new TextFileRepository<Client>("data/clienti.txt", ClientTextSerializer.Deserialize, ClientTextSerializer.Serialize);
            _clienti = _repoClienti.GetAll().ToList();
            LstClienti.ItemsSource = _clienti;
        }

        private void LstMasiniClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstMasiniClient.SelectedItem is Masina m)
            {
                TxtDetaliiClient.Text = $"ID: {m.Id}\nMarca: {m.Marca}\nModel: {m.Model}\nPret/zi: {m.PretPeZi} lei\nDisponibila: {m.Disponibila}\nCuloare: {m.Culoare}\nOptiuni: {m.Optiuni}";
                BtnReserve.IsEnabled = true;
                try { ImgClientDetalii.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(m.ImageUrl)); } catch { ImgClientDetalii.Source = null; }
            }
            else
            {
                BtnReserve.IsEnabled = false;
            }
        }

        private void BtnSearchClient_Click(object sender, RoutedEventArgs e)
        {
            var q = TxtCautareClient.Text?.Trim();
            if (string.IsNullOrEmpty(q))
                LstMasiniClient.ItemsSource = _masini;
            else
                LstMasiniClient.ItemsSource = _masini.Where(m => (m.Marca + " " + m.Model).ToLower().Contains(q.ToLower())).ToList();
        }

        private void BtnSearchClientEntity_Click(object sender, RoutedEventArgs e)
        {
            var q = TxtClientSearch.Text?.Trim();
            if (string.IsNullOrEmpty(q)) LstClienti.ItemsSource = _clienti;
            else LstClienti.ItemsSource = _clienti.Where(c => c.Nume.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        private void LstClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstClienti.SelectedItem is Client c)
            {
                // bind values to editor
                EdNume.Text = c.Nume;
                EdPrenume.Text = c.Prenume;
                EdCNP.Text = c.CNP;
            }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var nume = EdNume.Text?.Trim();
            var prenume = EdPrenume.Text?.Trim();
            var cnp = EdCNP.Text?.Trim();
            if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(prenume) || string.IsNullOrEmpty(cnp))
            {
                MessageBox.Show("Completează datele clientului.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var cl = new Client(nume, prenume, cnp);
            _repoClienti.Add(cl);
            _clienti.Add(cl);
            LstClienti.ItemsSource = null;
            LstClienti.ItemsSource = _clienti;
        }

        private void BtnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            if (!(LstClienti.SelectedItem is Client sel)) return;
            sel.Nume = EdNume.Text?.Trim();
            sel.Prenume = EdPrenume.Text?.Trim();
            sel.CNP = EdCNP.Text?.Trim();
            // persist: rewrite file
            System.IO.File.WriteAllLines("data/clienti.txt", _clienti.Select(c => ClientTextSerializer.Serialize(c)));
            LstClienti.Items.Refresh();
            MessageBox.Show("Client actualizat.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (!(LstClienti.SelectedItem is Client sel)) return;
            _clienti.Remove(sel);
            System.IO.File.WriteAllLines("data/clienti.txt", _clienti.Select(c => ClientTextSerializer.Serialize(c)));
            LstClienti.ItemsSource = null;
            LstClienti.ItemsSource = _clienti;
        }

        private void TxtCautareClient_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) BtnSearchClient_Click(sender, new RoutedEventArgs());
        }

        private void BtnReserve_Click(object sender, RoutedEventArgs e)
        {
            if (LstMasiniClient.SelectedItem is Masina m)
            {
                var dlg = new ReservationWindow(m) { Owner = this };
                dlg.ShowDialog();
            }
            else
            {
                MessageBox.Show("Selectează mai întâi o mașină pentru rezervare.", "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
