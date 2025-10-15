using System.Windows;
using BuildM.Models;

namespace BuildM.IGU
{
    public partial class DetalleSolicitud : Window
    {
        private Solicitudd solicitudActual;

        public DetalleSolicitud(Solicitudd solicitud)
        {
            InitializeComponent();
            solicitudActual = solicitud;
            CargarDatos();
        }

        private void CargarDatos()
        {
            // Aquí puedes mostrar la información de la solicitud en controles del XAML
            // Ejemplo: txtId.Text = solicitudActual.Id.ToString();
        }
    }
}
