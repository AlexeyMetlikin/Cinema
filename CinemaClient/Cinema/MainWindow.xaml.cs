using Cinema.Infrastructure.Entities;
using Cinema.ViewModel;
using System.Windows;

namespace Cinema
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new CinemaHallViewModel(new CinemaApi("http://localhost:9090/"));
        }
    }
}
