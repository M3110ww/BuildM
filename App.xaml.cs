using BuildM;
using System.Configuration;
using System.Data;
using System.Windows;

namespace BuildM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Testconexion.Probar();

            var login = new IGU.Login();
            this.MainWindow = login;
            login.Show();
        }
    }
}
