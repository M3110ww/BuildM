using MySqlConnector;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace BuildM.IGU
{
    public partial class Reportes : Page
    {
        private readonly string connStr =
            "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public Reportes()
        {
            InitializeComponent();
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbReportes.SelectedItem is not ComboBoxItem selectedItem)
            {
                MessageBox.Show("Seleccione un tipo de reporte.");
                return;
            }

            string reporteSeleccionado = selectedItem.Content.ToString();
            lblTituloReporte.Text = reporteSeleccionado;

            try
            {
                using var conn = new MySqlConnection(connStr);
                conn.Open();

                string query = reporteSeleccionado switch
                {
                    "Materiales agotados" =>
                        "SELECT id_material, nombre, descripcion, stock, costo " +
                        "FROM materiales WHERE stock <= 15 ORDER BY stock ASC;",

                    "Solicitudes en espera" =>
                        "SELECT id_solicitud, fecha, nombre_proyecto, responsable, estado " +
                        "FROM solicitudes WHERE estado = 'EN ESPERA';",

                    "Solicitudes aceptadas" =>
                        "SELECT id_solicitud, fecha, nombre_proyecto, responsable, estado " +
                        "FROM solicitudes WHERE estado = 'ACEPTADO';",

                    "Solicitudes rechazadas" =>
                        "SELECT id_solicitud, fecha, nombre_proyecto, responsable, estado " +
                        "FROM solicitudes WHERE estado = 'RECHAZADO';",

                    _ => ""
                };

                if (string.IsNullOrEmpty(query))
                {
                    MessageBox.Show("Seleccione un reporte válido.");
                    return;
                }

                using var cmd = new MySqlCommand(query, conn);
                using var adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new();
                adapter.Fill(dt);

                dgReportes.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message);
            }
        }
    }
}
