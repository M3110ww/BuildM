using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BuildM.IGU
{
    public partial class Solicitudes : Page
    {
        public Solicitudes()
        {
            InitializeComponent();
            CargarSolicitudes();
        }

        private void CargarSolicitudes()
        {
            // ⚠️ Aquí debes traer desde la base de datos
            List<Solicitud> solicitudes = SolicitudDAO.ObtenerSolicitudes();
            SolicitudesList.ItemsSource = solicitudes;
        }

        private void CambiarEstado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Solicitud solicitud)
            {
                MessageBox.Show($"Cambiar estado de {solicitud.NombreProyecto}");
                // Aquí actualizarías en la BD
                // SolicitudDAO.ActualizarEstado(solicitud.Id, "NuevoEstado");
            }
        }

        private void VerDetalles_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Solicitud solicitud)
            {
                MessageBox.Show($"Ver detalles de {solicitud.NombreProyecto}");
                // Aquí abrirías otra ventana/página de detalles
            }
        }
    }
}