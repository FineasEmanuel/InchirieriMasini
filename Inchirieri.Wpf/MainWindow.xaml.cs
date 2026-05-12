using System.Windows;
using System.Windows.Controls;
using Inchirieri.Modele;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Inchirieri.Wpf
{
    public partial class MainWindow : Window
    {
        private List<Masina> _masiniCache = new List<Masina>();

        public MainWindow()
        {
            InitializeComponent();

            // Seed in-memory data for immediate UI functionality
            _masiniCache = new List<Masina>
            {
                new Masina(1, "Dacia", "Logan", 60, true) { Culoare = CuloareMasina.Rosu, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Bluetooth, ImageUrl = "https://via.placeholder.com/400x200.png?text=Dacia+Logan" },
                new Masina(2, "BMW", "X5", 150, true) { Culoare = CuloareMasina.Negru, Optiuni = OptiuniMasina.Navigatie | OptiuniMasina.CutieAutomata | OptiuniMasina.CameraMarsarier, ImageUrl = "https://via.placeholder.com/400x200.png?text=BMW+X5" },
                new Masina(3, "Audi", "A4", 140, false) { Culoare = CuloareMasina.Albastru, Optiuni = OptiuniMasina.ScauneIncalzite | OptiuniMasina.PilotAutomat, ImageUrl = "https://via.placeholder.com/400x200.png?text=Audi+A4" },
                new Masina(4, "Toyota", "Corolla", 70, true) { Culoare = CuloareMasina.Alb, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Navigatie, ImageUrl = "https://via.placeholder.com/400x200.png?text=Toyota+Corolla" },
                new Masina(5, "Ford", "Focus", 65, true) { Culoare = CuloareMasina.Necunoscut, Optiuni = OptiuniMasina.Niciuna, ImageUrl = "https://via.placeholder.com/400x200.png?text=Ford+Focus" },
                new Masina(6, "Hyundai", "i20", 50, true) { Culoare = CuloareMasina.Gri, Optiuni = OptiuniMasina.Bluetooth | OptiuniMasina.SenzoriParcare, ImageUrl = "https://via.placeholder.com/400x200.png?text=Hyundai+i20" },
                new Masina(7, "Kia", "Rio", 55, true) { Culoare = CuloareMasina.Verde, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Bluetooth, ImageUrl = "https://via.placeholder.com/400x200.png?text=Kia+Rio" },
                new Masina(8, "Skoda", "Fabia", 58, true) { Culoare = CuloareMasina.Galben, Optiuni = OptiuniMasina.Navigatie | OptiuniMasina.SenzoriParcare, ImageUrl = "https://via.placeholder.com/400x200.png?text=Skoda+Fabia" },
                new Masina(9, "Renault", "Clio", 52, true) { Culoare = CuloareMasina.Orange, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.CutieAutomata, ImageUrl = "https://via.placeholder.com/400x200.png?text=Renault+Clio" },
                new Masina(10, "Peugeot", "208", 57, true) { Culoare = CuloareMasina.Violet, Optiuni = OptiuniMasina.PachetSport | OptiuniMasina.Bluetooth, ImageUrl = "https://via.placeholder.com/400x200.png?text=Peugeot+208" }
            };

            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";
            MasiniList.ItemsSource = _masiniCache;

            UpdateTotal();
        }

        private void MasiniList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasiniList.SelectedItem is Masina m)
            {
                MasinaCombo.SelectedItem = m;
            }
        }

        private void BtnCalcTotal_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasinaCombo.SelectedItem is Masina masina))
            {
                MessageBox.Show("Selectează mai întâi o mașină.", "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Selectează data de început și data de sfârșit.", "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var start = StartDatePicker.SelectedDate.Value;
            var end = EndDatePicker.SelectedDate.Value;
            var zile = (end - start).Days;
            if (zile <= 0)
            {
                MessageBox.Show("Perioadă invalidă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var total = zile * masina.PretPeZi;
            TxtCost.Text = $"Total: {total} lei ({zile} zile x {masina.PretPeZi} lei)";
        }

        private void BtnSearchLeft_Click(object sender, RoutedEventArgs e)
        {
            var query = TxtCautareStanga.Text?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MasinaCombo.ItemsSource = _masiniCache;
            }
            else
            {
                var filtered = _masiniCache.Where(m => (m.Marca + " " + m.Model).IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                MasinaCombo.ItemsSource = filtered;
                if (!filtered.Any())
                {
                    MessageBox.Show("Nu exista aceasta masina", "Căutare", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            MasinaCombo.Items.Refresh();
            UpdateTotal();
        }

        private void TxtCautareStanga_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnSearchLeft_Click(sender, new RoutedEventArgs());
            }
        }

        private void MasinaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasinaCombo.SelectedItem is Masina m)
            {
                TxtDetalii.Text = $"ID: {m.Id}\nMarca: {m.Marca}\nModel: {m.Model}\nPret/zi: {m.PretPeZi} lei\nDisponibila: {m.Disponibila}\nCuloare: {m.Culoare}\nOptiuni: {m.Optiuni}";
                try
                {
                    ImgDetalii.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(m.ImageUrl));
                }
                catch
                {
                    ImgDetalii.Source = null;
                }

                // populate editor fields
                TxtMarca.Text = m.Marca;
                TxtModel.Text = m.Model;
                TxtPret.Text = m.PretPeZi.ToString();
                ChkDisponibila.IsChecked = m.Disponibila;

                RbCuloareNecunoscut.IsChecked = m.Culoare == CuloareMasina.Necunoscut;
                RbCuloareRosu.IsChecked = m.Culoare == CuloareMasina.Rosu;
                RbCuloareAlb.IsChecked = m.Culoare == CuloareMasina.Alb;
                RbCuloareNegru.IsChecked = m.Culoare == CuloareMasina.Negru;

                ChkAer.IsChecked = m.Optiuni.HasFlag(OptiuniMasina.AerConditionat);
                ChkNavigatie.IsChecked = m.Optiuni.HasFlag(OptiuniMasina.Navigatie);
                ChkCutie.IsChecked = m.Optiuni.HasFlag(OptiuniMasina.CutieAutomata);
            }
            else
            {
                TxtDetalii.Text = "Selecteaza o masina...";
            }
        }

        private void UpdateTotal()
        {
            int displayed = (MasinaCombo.ItemsSource as IEnumerable<Masina>)?.Count() ?? _masiniCache.Count;
            TxtTotalMasini.Text = $"{displayed} / {_masiniCache.Count}";
        }

        private int NextId() => _masiniCache.Any() ? _masiniCache.Max(m => m.Id) + 1 : 1;

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(TxtPret.Text, out double pret)) pret = 0;

            var m = new Masina(NextId(), TxtMarca.Text, TxtModel.Text, pret, ChkDisponibila.IsChecked == true)
            {
                Culoare = RbCuloareRosu.IsChecked == true ? CuloareMasina.Rosu : RbCuloareAlb.IsChecked == true ? CuloareMasina.Alb : RbCuloareNegru.IsChecked == true ? CuloareMasina.Negru : CuloareMasina.Necunoscut,
                Optiuni = (ChkAer.IsChecked == true ? OptiuniMasina.AerConditionat : OptiuniMasina.Niciuna) | (ChkNavigatie.IsChecked == true ? OptiuniMasina.Navigatie : OptiuniMasina.Niciuna) | (ChkCutie.IsChecked == true ? OptiuniMasina.CutieAutomata : OptiuniMasina.Niciuna)
            };

            _masiniCache.Add(m);
            // refresh UI
            MasinaCombo.ItemsSource = null;
            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";
            MasiniList.ItemsSource = null;
            MasiniList.ItemsSource = _masiniCache;
            UpdateTotal();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasinaCombo.SelectedItem is Masina selected)) return;

            if (!double.TryParse(TxtPret.Text, out double pret)) pret = selected.PretPeZi;

            // update in-memory
            selected.Marca = TxtMarca.Text;
            selected.Model = TxtModel.Text;
            selected.PretPeZi = pret;
            selected.Disponibila = ChkDisponibila.IsChecked == true;
            selected.Culoare = RbCuloareRosu.IsChecked == true ? CuloareMasina.Rosu : RbCuloareAlb.IsChecked == true ? CuloareMasina.Alb : RbCuloareNegru.IsChecked == true ? CuloareMasina.Negru : CuloareMasina.Necunoscut;
            selected.Optiuni = (ChkAer.IsChecked == true ? OptiuniMasina.AerConditionat : OptiuniMasina.Niciuna) | (ChkNavigatie.IsChecked == true ? OptiuniMasina.Navigatie : OptiuniMasina.Niciuna) | (ChkCutie.IsChecked == true ? OptiuniMasina.CutieAutomata : OptiuniMasina.Niciuna);

            // refresh UI while keeping selection
            MasinaCombo.Items.Refresh();
            MasiniList.Items.Refresh();
            MasinaCombo.SelectedItem = selected;
            TxtDetalii.Text = $"ID: {selected.Id}\nMarca: {selected.Marca}\nModel: {selected.Model}\nPret/zi: {selected.PretPeZi}\nDisponibila: {selected.Disponibila}\nCuloare: {selected.Culoare}\nOptiuni: {selected.Optiuni}";
            UpdateTotal();

            MessageBox.Show("Entitate actualizată cu succes.", "Actualizare", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasinaCombo.SelectedItem is Masina selected)) return;

            // remove from cache
            _masiniCache.RemoveAll(x => x.Id == selected.Id);

            MasinaCombo.ItemsSource = null;
            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";
            MasiniList.ItemsSource = null;
            MasiniList.ItemsSource = _masiniCache;
            UpdateTotal();
        }
    }
}
