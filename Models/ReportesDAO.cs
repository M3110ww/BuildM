using MySqlConnector;
using System;
using System.Data;
using System.Windows;

namespace BuildM.Models
{
    public class ReportesDAO
    {
        private readonly string connStr = "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public DataTable ObtenerStockBajo(int umbral = 20)
        {
            string query = @"SELECT id_material, nombre, descripcion, stock, costo
                             FROM materiales
                             WHERE stock <= @umbral
                             ORDER BY stock ASC;";
            var tabla = new DataTable();
            try
            {
                using var conn = new MySqlConnection(connStr);
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@umbral", umbral);
                using var adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ObtenerStockBajo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tabla;
        }

        public DataTable ObtenerMaterialesAgotados()
        {
            string query = @"SELECT id_material, nombre, descripcion, stock, costo
                             FROM materiales
                             WHERE stock = 0;";
            return EjecutarConsultaSimple(query);
        }

        public DataTable ObtenerMaterialesMasSolicitados()
        {
            string query = @"
                SELECT d.id_material, d.nombre_material AS nombre, SUM(d.cantidad) AS total_solicitado
                FROM detalle_solicitud d
                GROUP BY d.id_material, d.nombre_material
                ORDER BY total_solicitado DESC;";
            return EjecutarConsultaSimple(query);
        }

        public DataTable ObtenerValorInventarioPorMaterial()
        {
            string query = @"
                SELECT id_material, nombre, stock, costo, (stock * costo) AS valor_total
                FROM materiales
                ORDER BY valor_total DESC;";
            return EjecutarConsultaSimple(query);
        }

        public DataTable ObtenerSolicitudesPorEstado()
        {
            string query = @"
                SELECT estado, COUNT(*) AS cantidad
                FROM solicitudes
                GROUP BY estado;";
            return EjecutarConsultaSimple(query);
        }

        public DataTable ObtenerSolicitudesPorUsuario()
        {
            string query = @"
                SELECT u.id_usuario, u.nombre AS usuario, COUNT(s.id_solicitud) AS total_solicitudes
                FROM usuarios u
                LEFT JOIN solicitudes s ON s.id_usuario = u.id_usuario
                GROUP BY u.id_usuario, u.nombre
                ORDER BY total_solicitudes DESC;";
            return EjecutarConsultaSimple(query);
        }

        public DataTable ObtenerDetallesPorSolicitud(int idSolicitud)
        {
            string query = @"
                SELECT id_detalle, id_solicitud, id_material, nombre_material, descripcion_material, cantidad, precio_unitario, subtotal
                FROM detalle_solicitud
                WHERE id_solicitud = @idSolicitud;";
            var tabla = new DataTable();
            try
            {
                using var conn = new MySqlConnection(connStr);
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                using var adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ObtenerDetallesPorSolicitud: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tabla;
        }

        private DataTable EjecutarConsultaSimple(string query)
        {
            var tabla = new DataTable();
            try
            {
                using var conn = new MySqlConnection(connStr);
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                using var adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en consulta: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tabla;
        }
    }
}
