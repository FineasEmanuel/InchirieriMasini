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
                new Masina(1, "Dacia", "Logan", 100, true) { Culoare = CuloareMasina.Rosu, Optiuni = OptiuniMasina.AerConditionat },
                new Masina(2, "BMW", "X5", 300, true) { Culoare = CuloareMasina.Negru, Optiuni = OptiuniMasina.Navigatie | OptiuniMasina.CutieAutomata },
                new Masina(3, "Audi", "A4", 250, false) { Culoare = CuloareMasina.Albastru, Optiuni = OptiuniMasina.ScauneIncalzite },
                new Masina(4, "Toyota", "Corolla", 150, true) { Culoare = CuloareMasina.Alb, Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Navigatie },
                new Masina(5, "Ford", "Focus", 120, true) { Culoare = CuloareMasina.Necunoscut, Optiuni = OptiuniMasina.Niciuna }
            };

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
            // refresh UI
            MasinaCombo.ItemsSource = null;
            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";
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

            // refresh UI
            MasinaCombo.ItemsSource = null;
            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";
            UpdateTotal();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasinaCombo.SelectedItem is Masina selected)) return;

            // remove from cache
            _masiniCache.RemoveAll(x => x.Id == selected.Id);

            MasinaCombo.ItemsSource = null;
            MasinaCombo.ItemsSource = _masiniCache;
            MasinaCombo.DisplayMemberPath = "Marca";
            UpdateTotal();
        }
    }
}
