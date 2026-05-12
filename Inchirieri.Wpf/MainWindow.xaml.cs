using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Inchirieri.Data.Stocare;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly TextFileRepository<Masina> _masiniRepo;
        private List<Masina> _masiniCache = new();
        private List<Masina> _masiniAfisate = new();

        public MainWindow()
        {
            InitializeComponent();

            _masiniRepo = new TextFileRepository<Masina>(
                DataFiles.GetPath("masini.txt"),
                MasinaTextSerializer.Deserialize,
                MasinaTextSerializer.Serialize);

            CuloareComboBox.ItemsSource = Enum.GetValues<CuloareMasina>();
            CuloareComboBox.SelectedItem = CuloareMasina.Necunoscut;

            IncarcaMasini();
            AplicaFiltre();
            CurataEditor();
        }

        private void IncarcaMasini()
        {
            _masiniCache = _masiniRepo.GetAll().ToList();

            if (_masiniCache.Count == 0)
            {
                _masiniCache = CreeazaMasiniInitiale();
                _masiniRepo.SaveAll(_masiniCache);
            }
        }

        private static List<Masina> CreeazaMasiniInitiale()
        {
            return new List<Masina>
            {
                new Masina(1, "Dacia", "Logan", 100, true)
                {
                    Culoare = CuloareMasina.Rosu,
                    Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Bluetooth | OptiuniMasina.CinciLocuri,
                    ImageUrl = "https://placehold.co/640x360/png?text=Dacia+Logan"
                },
                new Masina(2, "BMW", "X5", 300, true)
                {
                    Culoare = CuloareMasina.Negru,
                    Optiuni = OptiuniMasina.Navigatie | OptiuniMasina.CutieAutomata | OptiuniMasina.CameraMarsarier,
                    ImageUrl = "https://placehold.co/640x360/png?text=BMW+X5"
                },
                new Masina(3, "Audi", "A4", 250, false)
                {
                    Culoare = CuloareMasina.Albastru,
                    Optiuni = OptiuniMasina.ScauneIncalzite | OptiuniMasina.PilotAutomat,
                    ImageUrl = "https://placehold.co/640x360/png?text=Audi+A4"
                },
                new Masina(4, "Toyota", "Corolla", 150, true)
                {
                    Culoare = CuloareMasina.Alb,
                    Optiuni = OptiuniMasina.AerConditionat | OptiuniMasina.Navigatie | OptiuniMasina.CinciLocuri,
                    ImageUrl = "https://placehold.co/640x360/png?text=Toyota+Corolla"
                },
                new Masina(5, "Volkswagen", "Golf", 180, true)
                {
                    Culoare = CuloareMasina.Gri,
                    Optiuni = OptiuniMasina.CutieAutomata | OptiuniMasina.SenzoriParcare,
                    ImageUrl = "https://placehold.co/640x360/png?text=Volkswagen+Golf"
                }
            };
        }

        private void AplicaFiltre(Masina? masinaDeSelectat = null)
        {
            string query = TxtCautareStanga.Text.Trim();
            IEnumerable<Masina> rezultate = _masiniCache;

            if (!string.IsNullOrWhiteSpace(query))
            {
                rezultate = rezultate.Where(m =>
                    m.Marca.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Model.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Id.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            if (ChkFiltruDisponibile.IsChecked == true)
            {
                rezultate = rezultate.Where(m => m.Disponibila);
            }

            if (ChkFiltruAutomata.IsChecked == true)
            {
                rezultate = rezultate.Where(m => m.Optiuni.HasFlag(OptiuniMasina.CutieAutomata));
            }

            if (ChkFiltruAer.IsChecked == true)
            {
                rezultate = rezultate.Where(m => m.Optiuni.HasFlag(OptiuniMasina.AerConditionat));
            }

            _masiniAfisate = rezultate.OrderBy(m => m.Marca).ThenBy(m => m.Model).ToList();
            MasiniList.ItemsSource = null;
            MasiniList.ItemsSource = _masiniAfisate;

            if (masinaDeSelectat != null)
            {
                MasiniList.SelectedItem = _masiniAfisate.FirstOrDefault(m => m.Id == masinaDeSelectat.Id);
            }

            UpdateTotal();
        }

        private void MasiniList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasiniList.SelectedItem is Masina masina)
            {
                IncarcaMasinaInEditor(masina);
            }
        }

        private void IncarcaMasinaInEditor(Masina masina)
        {
            TxtMarca.Text = masina.Marca;
            TxtModel.Text = masina.Model;
            TxtPret.Text = masina.PretPeZi.ToString(CultureInfo.CurrentCulture);
            TxtImageUrl.Text = masina.ImageUrl;
            ChkDisponibila.IsChecked = masina.Disponibila;
            CuloareComboBox.SelectedItem = masina.Culoare;

            ChkAer.IsChecked = masina.Optiuni.HasFlag(OptiuniMasina.AerConditionat);
            ChkNavigatie.IsChecked = masina.Optiuni.HasFlag(OptiuniMasina.Navigatie);
            ChkCutie.IsChecked = masina.Optiuni.HasFlag(OptiuniMasina.CutieAutomata);
            ChkIluminare.IsChecked = masina.Optiuni.HasFlag(OptiuniMasina.IluminareAmbientala);
            Chk5Locuri.IsChecked = masina.Optiuni.HasFlag(OptiuniMasina.CinciLocuri);
            Chk7Locuri.IsChecked = masina.Optiuni.HasFlag(OptiuniMasina.SapteLocuri);

            TxtDetalii.Text =
                $"ID: {masina.Id}\n" +
                $"Masina: {masina.Marca} {masina.Model}\n" +
                $"Pret/zi: {masina.PretPeZi:0.##} lei\n" +
                $"Status: {masina.StatusDisponibilitate}\n" +
                $"Culoare: {masina.Culoare}\n" +
                $"Optiuni: {masina.Optiuni}";

            IncarcaImagine(masina.ImageUrl);
            AscundeMesajEditor();
        }

        private void IncarcaImagine(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uri))
            {
                ImgDetalii.Source = new BitmapImage(uri);
            }
            else
            {
                ImgDetalii.Source = null;
            }
        }

        private void BtnSearchLeft_Click(object sender, RoutedEventArgs e)
        {
            AplicaFiltre();
        }

        private void TxtCautareStanga_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AplicaFiltre();
            }
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            AplicaFiltre(MasiniList.SelectedItem as Masina);
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            TxtCautareStanga.Clear();
            ChkFiltruDisponibile.IsChecked = false;
            ChkFiltruAutomata.IsChecked = false;
            ChkFiltruAer.IsChecked = false;
            AplicaFiltre();
        }

        private void BtnCalcTotal_Click(object sender, RoutedEventArgs e)
        {
            if (MasiniList.SelectedItem is not Masina masina)
            {
                MessageBox.Show("Selecteaza mai intai o masina.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Selecteaza data de inceput si data de sfarsit.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int zile = (EndDatePicker.SelectedDate.Value - StartDatePicker.SelectedDate.Value).Days;
            if (zile <= 0)
            {
                MessageBox.Show("Perioada este invalida.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double total = zile * masina.PretPeZi;
            TxtCost.Text = $"Total: {total:0.##} lei ({zile} zile x {masina.PretPeZi:0.##} lei)";
        }

        private void BtnOpenReservation_Click(object sender, RoutedEventArgs e)
        {
            DeschideRezervare();
        }

        private void DeschideRezervare()
        {
            if (MasiniList.SelectedItem is not Masina masina)
            {
                MessageBox.Show("Selecteaza o masina pentru rezervare.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new ReservationWindow(masina) { Owner = this };
            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show("Rezervarea a fost salvata.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private int NextId()
        {
            return _masiniCache.Count == 0 ? 1 : _masiniCache.Max(m => m.Id) + 1;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!CitesteMasinaDinEditor(NextId(), out Masina masina))
            {
                return;
            }

            _masiniCache.Add(masina);
            SalveazaMasini();
            AplicaFiltre(masina);
            AfiseazaMesajEditor("Masina a fost adaugata.", false);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MasiniList.SelectedItem is not Masina selected)
            {
                AfiseazaMesajEditor("Selecteaza o masina pentru actualizare.", true);
                return;
            }

            if (!CitesteMasinaDinEditor(selected.Id, out Masina masinaEditata))
            {
                return;
            }

            selected.Marca = masinaEditata.Marca;
            selected.Model = masinaEditata.Model;
            selected.PretPeZi = masinaEditata.PretPeZi;
            selected.Disponibila = masinaEditata.Disponibila;
            selected.Culoare = masinaEditata.Culoare;
            selected.Optiuni = masinaEditata.Optiuni;
            selected.ImageUrl = masinaEditata.ImageUrl;

            SalveazaMasini();
            AplicaFiltre(selected);
            IncarcaMasinaInEditor(selected);
            AfiseazaMesajEditor("Masina a fost actualizata.", false);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MasiniList.SelectedItem is not Masina selected)
            {
                AfiseazaMesajEditor("Selecteaza o masina pentru stergere.", true);
                return;
            }

            MessageBoxResult confirmare = MessageBox.Show(
                $"Stergi {selected.Marca} {selected.Model}?",
                "Confirmare stergere",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmare != MessageBoxResult.Yes)
            {
                return;
            }

            _masiniCache.RemoveAll(m => m.Id == selected.Id);
            SalveazaMasini();
            AplicaFiltre();
            CurataEditor();
            AfiseazaMesajEditor("Masina a fost stearsa.", false);
        }

        private bool CitesteMasinaDinEditor(int id, out Masina masina)
        {
            masina = new Masina(id, string.Empty, string.Empty, 0, true);
            string marca = TxtMarca.Text.Trim();
            string model = TxtModel.Text.Trim();

            if (string.IsNullOrWhiteSpace(marca) || string.IsNullOrWhiteSpace(model))
            {
                AfiseazaMesajEditor("Marca si modelul sunt obligatorii.", true);
                return false;
            }

            if (!double.TryParse(TxtPret.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out double pret) &&
                !double.TryParse(TxtPret.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out pret))
            {
                AfiseazaMesajEditor("Pretul pe zi trebuie sa fie numeric.", true);
                return false;
            }

            if (pret <= 0)
            {
                AfiseazaMesajEditor("Pretul pe zi trebuie sa fie mai mare decat 0.", true);
                return false;
            }

            masina = new Masina(id, marca, model, pret, ChkDisponibila.IsChecked == true)
            {
                Culoare = CuloareComboBox.SelectedItem is CuloareMasina culoare ? culoare : CuloareMasina.Necunoscut,
                Optiuni = CitesteOptiuni(),
                ImageUrl = TxtImageUrl.Text.Trim()
            };

            return true;
        }

        private OptiuniMasina CitesteOptiuni()
        {
            OptiuniMasina optiuni = OptiuniMasina.Niciuna;

            if (ChkAer.IsChecked == true) optiuni |= OptiuniMasina.AerConditionat;
            if (ChkNavigatie.IsChecked == true) optiuni |= OptiuniMasina.Navigatie;
            if (ChkCutie.IsChecked == true) optiuni |= OptiuniMasina.CutieAutomata;
            if (ChkIluminare.IsChecked == true) optiuni |= OptiuniMasina.IluminareAmbientala;
            if (Chk5Locuri.IsChecked == true) optiuni |= OptiuniMasina.CinciLocuri;
            if (Chk7Locuri.IsChecked == true) optiuni |= OptiuniMasina.SapteLocuri;

            return optiuni;
        }

        private void SalveazaMasini()
        {
            _masiniRepo.SaveAll(_masiniCache.OrderBy(m => m.Id));
        }

        private void CurataEditor()
        {
            TxtMarca.Clear();
            TxtModel.Clear();
            TxtPret.Clear();
            TxtImageUrl.Clear();
            ChkDisponibila.IsChecked = true;
            CuloareComboBox.SelectedItem = CuloareMasina.Necunoscut;
            ChkAer.IsChecked = false;
            ChkNavigatie.IsChecked = false;
            ChkCutie.IsChecked = false;
            ChkIluminare.IsChecked = false;
            Chk5Locuri.IsChecked = false;
            Chk7Locuri.IsChecked = false;
            TxtDetalii.Text = "Selecteaza o masina din catalog.";
            TxtCost.Text = string.Empty;
            ImgDetalii.Source = null;
            AscundeMesajEditor();
        }

        private void UpdateTotal()
        {
            TxtTotalMasini.Text = $"{_masiniAfisate.Count} / {_masiniCache.Count}";
        }

        private void AfiseazaMesajEditor(string mesaj, bool eroare)
        {
            EditorMessageText.Text = mesaj;
            EditorMessageText.Foreground = eroare
                ? (Brush)FindResource("DangerBrush")
                : (Brush)FindResource("SuccessBrush");
            EditorMessagePanel.Background = eroare
                ? new SolidColorBrush(Color.FromRgb(255, 241, 241))
                : new SolidColorBrush(Color.FromRgb(235, 248, 241));
            EditorMessagePanel.BorderBrush = eroare
                ? new SolidColorBrush(Color.FromRgb(240, 187, 187))
                : new SolidColorBrush(Color.FromRgb(174, 222, 197));
            EditorMessagePanel.Visibility = Visibility.Visible;
        }

        private void AscundeMesajEditor()
        {
            EditorMessagePanel.Visibility = Visibility.Collapsed;
            EditorMessageText.Text = string.Empty;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuMasinaNoua_Click(object sender, RoutedEventArgs e)
        {
            MasiniList.SelectedIndex = -1;
            CurataEditor();
        }

        private void MenuRezerva_Click(object sender, RoutedEventArgs e)
        {
            DeschideRezervare();
        }

        private void MenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            IncarcaMasini();
            AplicaFiltre();
            AfiseazaMesajEditor("Datele au fost reincarcate din fisier.", false);
        }

        private void MenuDespre_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "AutoRent Desk - aplicatie WPF pentru inchirieri masini, clienti si rezervari.",
                "Despre",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
