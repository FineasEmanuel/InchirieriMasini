using System.Linq;
using System.Windows;
using Inchirieri.Data.Stocare;
using Inchirieri.Modele;

namespace Inchirieri.Wpf
{
    public partial class LoginWindow : Window
    {
        private readonly TextFileRepository<Angajat> _repoAngajati;
        private readonly TextFileRepository<Client> _repoClienti;

        public LoginWindow()
        {
            InitializeComponent();

            _repoAngajati = new TextFileRepository<Angajat>(
                DataFiles.GetPath("angajati.txt"),
                AngajatTextSerializer.Deserialize,
                AngajatTextSerializer.Serialize);
            _repoClienti = new TextFileRepository<Client>(
                DataFiles.GetPath("clienti.txt"),
                ClientTextSerializer.Deserialize,
                ClientTextSerializer.Serialize);

            RbAngajat.Checked += Role_Checked;
            RbClient.Checked += Role_Checked;

            if (!_repoAngajati.GetAll().Any())
            {
                _repoAngajati.Add(new Angajat("admin", "1234"));
            }
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
                string username = TxtUsername.Text.Trim();
                string parola = PwdParola.Password.Trim();
                Angajat? angajat = _repoAngajati.GetAll()
                    .FirstOrDefault(a => a.Username == username && a.Parola == parola);

                if (angajat != null)
                {
                    var window = new MainWindow();
                    window.Show();
                    Close();
                    return;
                }

                MessageBox.Show("Autentificare angajat esuata.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string cnp = TxtCNP.Text.Trim();
            Client? client = _repoClienti.GetAll().FirstOrDefault(c => c.CNP == cnp);
            if (client != null)
            {
                var window = new ClientViewWindow();
                window.Show();
                Close();
                return;
            }

            MessageBox.Show("Clientul nu este inregistrat. Apasa 'Creeaza cont' pentru inregistrare.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (RbAngajat.IsChecked == true)
            {
                string username = TxtUsername.Text.Trim();
                string parola = PwdParola.Password.Trim();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(parola))
                {
                    MessageBox.Show("Completeaza username si parola.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_repoAngajati.GetAll().Any(a => a.Username == username))
                {
                    MessageBox.Show("Exista deja un angajat cu acest username.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _repoAngajati.Add(new Angajat(username, parola));
                MessageBox.Show("Angajat creat.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string nume = TxtNume.Text.Trim();
            string prenume = TxtPrenume.Text.Trim();
            string cnp = TxtCNP.Text.Trim();
            if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(prenume) || cnp.Length != 13)
            {
                MessageBox.Show("Completeaza corect datele clientului (CNP 13 caractere).", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_repoClienti.GetAll().Any(c => c.CNP == cnp))
            {
                MessageBox.Show("Exista deja un client cu acest CNP.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _repoClienti.Add(new Client(nume, prenume, cnp));
            MessageBox.Show("Client creat.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
