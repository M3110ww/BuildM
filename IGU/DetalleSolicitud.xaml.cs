using BuildM.Models;
using System.Windows;

namespace BuildM.IGU
{
    public partial class DetalleSolicitud : Window
    {
        private int idSolicitud;

        public DetalleSolicitud(int idSolicitud)
        {
            InitializeComponent();
            this.idSolicitud = idSolicitud;
            CargarDetalles();
        }

        private void CargarDetalles()
        {
            var dao = new DetalleDAO();
            List<Detalle> detalles = dao.ObtenerDetallesPorSolicitud(idSolicitud);
            TablaDetalles.ItemsSource = detalles;

            decimal total = dao.CalcularTotalPorSolicitud(idSolicitud);
            TxtTotal.Text = total.ToString("C");
        }
    }
}
