using MySqlConnector;

namespace BuildM.Models
{
    public class DetalleDAO
    {
        private readonly string connStr = "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public List<Detalle> ObtenerDetallesPorSolicitud(int idSolicitud)
        {
            var lista = new List<Detalle>();

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                        SELECT id_detalle, id_solicitud, id_material, nombre_proyecto, responsable,
                               nombre_material, descripcion_material, cantidad, precio_unitario, subtotal
                        FROM detalle_solicitud
                        WHERE id_solicitud = @idSolicitud;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Detalle
                                {
                                    IdDetalle = reader.GetInt32("id_detalle"),
                                    IdSolicitud = reader.GetInt32("id_solicitud"),
                                    IdMaterial = reader.GetInt32("id_material"),
                                    NombreProyecto = reader.IsDBNull(reader.GetOrdinal("nombre_proyecto")) ? string.Empty : reader.GetString("nombre_proyecto"),
                                    Responsable = reader.IsDBNull(reader.GetOrdinal("responsable")) ? string.Empty : reader.GetString("responsable"),
                                    NombreMaterial = reader.IsDBNull(reader.GetOrdinal("nombre_material")) ? string.Empty : reader.GetString("nombre_material"),
                                    DescripcionMaterial = reader.IsDBNull(reader.GetOrdinal("descripcion_material")) ? string.Empty : reader.GetString("descripcion_material"),
                                    Cantidad = reader.GetInt32("cantidad"),
                                    PrecioUnitario = reader.GetDecimal("precio_unitario"),
                                    Subtotal = reader.GetDecimal("subtotal")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al obtener los detalles: " + ex.Message, "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return lista;
        }

        public decimal CalcularTotalPorSolicitud(int idSolicitud)
        {
            decimal total = 0;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT SUM(subtotal) FROM detalle_solicitud WHERE id_solicitud = @idSolicitud;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);

                        var result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                            total = Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al calcular el total: " + ex.Message, "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return total;
        }
    }
}
