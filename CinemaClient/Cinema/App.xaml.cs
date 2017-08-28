
using Cinema.Infrastructure.Entities;
using System.Windows;

namespace Cinema
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}
