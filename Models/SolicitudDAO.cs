using MySqlConnector;
using System;
using System.Collections.Generic;

namespace BuildM.Models
{
    public class SolicitudDAO
    {
        private readonly string connStr = "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public List<Solicitudd> ObtenerSolicitudes()
        {
            var lista = new List<Solicitudd>();

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT id_solicitud, nombre_proyecto, responsable, fecha, IFNULL(estado, 'Pendiente') AS estado FROM solicitudes;";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Solicitudd
                            {
                                Id = reader.GetInt32("id_solicitud"),
                                NombreProyecto = reader.GetString("nombre_proyecto"),
                                Responsable = reader.GetString("responsable"),
                                Fecha = reader.GetDateTime("fecha"),
                                Estado = reader.GetString("estado")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al obtener solicitudes: " + ex.Message);
            }

            return lista;
        }

        public void ActualizarEstado(int idSolicitud, string nuevoEstado)
        {
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "UPDATE solicitudes SET estado = @estado WHERE id_solicitud = @id;";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@id", idSolicitud);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al actualizar el estado: " + ex.Message);
            }
        }
    }
}
