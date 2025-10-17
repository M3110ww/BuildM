using System.Windows;

namespace BuildM.IGU
{
    public partial class Administrador : Window
    {
        public Administrador()
        {
            InitializeComponent();

            frameContenido.Content = new Inventario();
        }

        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            frameContenido.Content = new Inventario();
            lblTitulo.Content = "INVENTARIO";
        }

        private void BtnSolicitudes_Click(object sender, RoutedEventArgs e)
        {
            frameContenido.Content = new Solicitudes();
            lblTitulo.Content = "SOLICITUDES";
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            frameContenido.Content = new Reportes();
            lblTitulo.Content = "REPORTES";
        }
    }
}