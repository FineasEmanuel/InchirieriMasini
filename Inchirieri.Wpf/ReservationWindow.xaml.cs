using System;
using System.Windows;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class ReservationWindow : Window
    {
        private Masina _masina;

        public ReservationWindow(Masina masina)
        {
            InitializeComponent();
            _masina = masina;
            TxtMasina.Text = $"{masina.Marca} {masina.Model} - {masina.PretPeZi} lei/zi";

            StartDate.SelectedDateChanged += Dates_SelectedDateChanged;
            EndDate.SelectedDateChanged += Dates_SelectedDateChanged;
        }

        private void Dates_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (StartDate.SelectedDate.HasValue && EndDate.SelectedDate.HasValue)
            {
                var zile = (EndDate.SelectedDate.Value - StartDate.SelectedDate.Value).Days;
                if (zile > 0)
                {
                    var total = zile * _masina.PretPeZi;
                    TxtPretTotal.Text = $"Preț total: {total} lei ({zile} zile)";
                }
                else
                {
                    TxtPretTotal.Text = "Perioadă invalidă";
                }
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(TxtNume.Text) || string.IsNullOrWhiteSpace(TxtPrenume.Text) || TxtCNP.Text.Length != 13)
            {
                MessageBox.Show("Completează corect datele personale (CNP 13 caractere)", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Selectează perioada.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var zile = (EndDate.SelectedDate.Value - StartDate.SelectedDate.Value).Days;
            if (zile <= 0)
            {
                MessageBox.Show("Perioadă invalidă.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // create client and inchiriere in memory and show confirmation
            var client = new Client(TxtNume.Text.Trim(), TxtPrenume.Text.Trim(), TxtCNP.Text.Trim());
            var total = zile * _masina.PretPeZi;
            var inchiriere = new Inchiriere(_masina, client, StartDate.SelectedDate.Value, EndDate.SelectedDate.Value, total);

            MessageBox.Show($"Rezervare confirmată. Total: {total} lei", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
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
