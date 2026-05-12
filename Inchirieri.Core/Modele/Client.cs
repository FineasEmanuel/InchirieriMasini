using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inchirieri.Modele
{
    public class Client : INotifyPropertyChanged
    {
        private string _nume;
        private string _prenume;
        private string _cnp;

        public string Nume
        {
            get => _nume;
            set { _nume = value; OnPropertyChanged(); }
        }

        public string Prenume
        {
            get => _prenume;
            set { _prenume = value; OnPropertyChanged(); }
        }

        public string CNP
        {
            get => _cnp;
            set { _cnp = value; OnPropertyChanged(); }
        }

        public Client(string nume, string prenume, string cnp)
        {
            _nume = nume;
            _prenume = prenume;
            _cnp = cnp;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
