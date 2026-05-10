
using System.Windows;
using Inchirieri.Data.Stocare;
using Inchirieri.Modele;
using System.Linq;

namespace Inchirieri.Wpf
{
    public partial class LoginWindow : Window
    {
        private readonly TextFileRepository<Angajat> _repoAngajati;
        private readonly TextFileRepository<Client> _repoClienti;

        public LoginWindow()
        {
            InitializeComponent();

            _repoAngajati = new TextFileRepository<Angajat>("data/angajati.txt", AngajatTextSerializer.Deserialize, AngajatTextSerializer.Serialize);
            _repoClienti = new TextFileRepository<Client>("data/clienti.txt", ClientTextSerializer.Deserialize, ClientTextSerializer.Serialize);

            RbAngajat.Checked += Role_Checked;
            RbClient.Checked += Role_Checked;
        }

        private void Role_Checked(object sender, RoutedEventArgs e)
        {
            PanelAngajat.Visibility = RbAngajat.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            PanelClient.Visibility = RbClient.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (RbAngajat.IsChecked == true)
            {
                var username = TxtUsername.Text.Trim();
                var parola = PwdParola.Password.Trim();
                var ang = _repoAngajati.GetAll().FirstOrDefault(a => a.Username == username && a.Parola == parola);
                if (ang != null)
                {
                    var wnd = new MainWindow();
                    wnd.Show();
                    Close();
                    return;
                }

                MessageBox.Show("Autentificare angajat eșuat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var cnp = TxtCNP.Text.Trim();
                var client = _repoClienti.GetAll().FirstOrDefault(c => c.CNP == cnp);
                if (client != null)
                {
                    var wnd = new ClientViewWindow();
                    wnd.Show();
                    Close();
                    return;
                }

                MessageBox.Show("Clientul nu este înregistrat. Apasă 'Creează cont' pentru a te înregistra.", "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (RbAngajat.IsChecked == true)
            {
                var username = TxtUsername.Text.Trim();
                var parola = PwdParola.Password.Trim();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(parola))
                {
                    MessageBox.Show("Completează username și parolă.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _repoAngajati.Add(new Angajat(username, parola));
                MessageBox.Show("Angajat creat.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var nume = TxtNume.Text.Trim();
                var prenume = TxtPrenume.Text.Trim();
                var cnp = TxtCNP.Text.Trim();
                if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(prenume) || cnp.Length != 13)
                {
                    MessageBox.Show("Completează corect datele clientului (CNP 13 caractere).", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _repoClienti.Add(new Client(nume, prenume, cnp));
                MessageBox.Show("Client creat.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
