using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class ClientViewWindow : Window
    {
        private System.Collections.Generic.List<Masina> _masini;

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
        }

        private void LstMasiniClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstMasiniClient.SelectedItem is Masina m)
            {
                TxtDetaliiClient.Text = $"ID: {m.Id}\nMarca: {m.Marca}\nModel: {m.Model}\nPret/zi: {m.PretPeZi}\nDisponibila: {m.Disponibila}\nCuloare: {m.Culoare}\nOptiuni: {m.Optiuni}";
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
