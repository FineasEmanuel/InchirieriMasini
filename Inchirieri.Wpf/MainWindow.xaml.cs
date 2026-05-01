using System.Windows;
using System.Windows.Controls;
using Inchirieri.Data.Stocare;
using Inchirieri.Modele;
using System.Linq;
using System.Collections.Generic;

namespace Inchirieri.Wpf
{
    public partial class MainWindow : Window
    {
        private TextFileRepository<Masina> _repoMasini;
        private List<Masina> _masiniCache = new List<Masina>();

        public MainWindow()
        {
            InitializeComponent();

            _repoMasini = new TextFileRepository<Masina>("data/masini.txt", MasinaTextSerializer.Deserialize, MasinaTextSerializer.Serialize);

            // Seed default data if file empty
            _masiniCache = _repoMasini.GetAll().ToList();
            if (!_masiniCache.Any())
            {
                _masiniCache = new List<Masina>
                {
                    new Masina(1, "Dacia", "Logan", 100, true) { Culoare = CuloareMasina.Rosu, Optiuni = OptiuniMasina.AerConditionat },
                    new Masina(2, "BMW", "X5", 300, true) { Culoare = CuloareMasina.Negru, Optiuni = OptiuniMasina.Navigatie | OptiuniMasina.CutieAutomata },
                    new Masina(3, "Audi", "A4", 250, false) { Culoare = CuloareMasina.Albastru, Optiuni = OptiuniMasina.ScauneIncalzite },
                    new Masina(4, "Toyota", "Corolla", 150, true) { Culoare = CuloareMasina.Alb, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Navigatie },
                    new Masina(5, "Ford", "Focus", 120, true) { Culoare = CuloareMasina.Necunoscut, Optiuni = OptiuniMasina.Niciuna }
                };

                // persist initial data
                foreach (var m in _masiniCache)
                    _repoMasini.Add(m);
            }

            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";

            UpdateTotal();
        }

        private void MasinaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasinaCombo.SelectedItem is Masina m)
            {
                TxtDetalii.Text = $"ID: {m.Id}\nMarca: {m.Marca}\nModel: {m.Model}\nPret/zi: {m.PretPeZi}\nDisponibila: {m.Disponibila}\nCuloare: {m.Culoare}\nOptiuni: {m.Optiuni}";

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
            TxtTotalMasini.Text = _masiniCache.Count.ToString();
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
            _repoMasini.Add(m);
            MasinaCombo.Items.Refresh();
            UpdateTotal();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasinaCombo.SelectedItem is Masina selected)) return;

            if (!double.TryParse(TxtPret.Text, out double pret)) pret = selected.PretPeZi;

            _repoMasini.Update(x => x.Id == selected.Id, x => {
                x.Marca = TxtMarca.Text;
                x.Model = TxtModel.Text;
                x.PretPeZi = pret;
                x.Disponibila = ChkDisponibila.IsChecked == true;
                x.Culoare = RbCuloareRosu.IsChecked == true ? CuloareMasina.Rosu : RbCuloareAlb.IsChecked == true ? CuloareMasina.Alb : RbCuloareNegru.IsChecked == true ? CuloareMasina.Negru : CuloareMasina.Necunoscut;
                x.Optiuni = (ChkAer.IsChecked == true ? OptiuniMasina.AerConditionat : OptiuniMasina.Niciuna) | (ChkNavigatie.IsChecked == true ? OptiuniMasina.Navigatie : OptiuniMasina.Niciuna) | (ChkCutie.IsChecked == true ? OptiuniMasina.CutieAutomata : OptiuniMasina.Niciuna);
            });

            // refresh cache and UI
            _masiniCache = _repoMasini.GetAll().ToList();
            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.Items.Refresh();
            UpdateTotal();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasinaCombo.SelectedItem is Masina selected)) return;

            // remove from cache and rewrite file
            _masiniCache.RemoveAll(x => x.Id == selected.Id);
            // rewrite file with remaining items
            System.IO.File.WriteAllLines("data/masini.txt", _masiniCache.Select(m => MasinaTextSerializer.Serialize(m)));

            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.Items.Refresh();
            UpdateTotal();
        }
    }
}
