using BuildM.Models;
using MySqlConnector;
using System.Windows;

namespace BuildM.IGU
{
    public partial class Solicitud : Window
    {
        public class Material
        {
            public int IdMaterial { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public int Stock { get; set; }
            public decimal Costo { get; set; }
            public string DisplayText => $"{Nombre} - {Descripcion} - ${Costo:N2}";

            public override string ToString() => DisplayText;
        }

        public class ItemSolicitud
        {
            public int IdMaterial { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public decimal Precio { get; set; }
            public decimal Subtotal => Cantidad * Precio;
        }

        private List<Material> materialesDisponibles = new List<Material>();
        private List<ItemSolicitud> solicitudActual = new List<ItemSolicitud>();

        private string connStr = "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public Solicitud()
        {
            InitializeComponent();
            CargarMaterialesBD();
        }

        private void CargarMaterialesBD()
        {
            materialesDisponibles.Clear();

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT id_material, nombre, descripcion, stock, costo FROM materiales";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var mat = new Material
                        {
                            IdMaterial = reader.GetInt32("id_material"),
                            Nombre = reader.GetString("nombre"),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? string.Empty : reader.GetString("descripcion"),
                            Stock = reader.GetInt32("stock"),
                            Costo = reader.GetDecimal("costo")
                        };
                        materialesDisponibles.Add(mat);
                    }
                }
            }

            cmbMateriales.ItemsSource = materialesDisponibles;
            cmbMateriales.SelectedIndex = -1;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMateriales.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un material.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mat = (Material)cmbMateriales.SelectedItem;

            if (cantidad > mat.Stock)
            {
                MessageBox.Show("No hay suficiente stock.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existing = solicitudActual.FirstOrDefault(x => x.IdMaterial == mat.IdMaterial);
            if (existing != null)
            {
                existing.Cantidad += cantidad;
            }
            else
            {
                solicitudActual.Add(new ItemSolicitud
                {
                    IdMaterial = mat.IdMaterial,
                    Nombre = mat.Nombre,
                    Descripcion = mat.Descripcion,
                    Cantidad = cantidad,
                    Precio = mat.Costo
                });
            }

            dgSolicitud.ItemsSource = null;
            dgSolicitud.ItemsSource = solicitudActual;
            ActualizarTotal();
        }

        private void BtnQuitar_Click(object sender, RoutedEventArgs e)
        {
            var sel = dgSolicitud.SelectedItem as ItemSolicitud;
            if (sel == null)
            {
                MessageBox.Show("Seleccione una fila para quitar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            solicitudActual.Remove(sel);
            dgSolicitud.ItemsSource = null;
            dgSolicitud.ItemsSource = solicitudActual;
            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            decimal total = solicitudActual.Sum(x => x.Subtotal);
            lblTotal.Content = $"Total: ${total:N2}";
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            string nombreProyecto = txtNombreProyecto.Text?.Trim();
            if (string.IsNullOrWhiteSpace(nombreProyecto))
            {
                MessageBox.Show("Ingrese el nombre del proyecto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!solicitudActual.Any())
            {
                MessageBox.Show("No hay materiales en la solicitud.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("¿Está seguro de enviar la solicitud?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlSolicitud = @"INSERT INTO solicitudes (fecha, nombre_proyecto, responsable, estado, id_usuario)
                                        VALUES (NOW(), @nombre_proyecto, @responsable, 'EN ESPERA', @id_usuario)";
                        using (var cmdSol = new MySqlCommand(sqlSolicitud, conn, tx))
                        {
                            cmdSol.Parameters.AddWithValue("@nombre_proyecto", nombreProyecto);
                            cmdSol.Parameters.AddWithValue("@responsable", SesionUsuario.Nombre);
                            cmdSol.Parameters.AddWithValue("@id_usuario", SesionUsuario.IdUsuario);
                            cmdSol.ExecuteNonQuery();
                        }

                        long idSolicitud = 0;
                        using (var cmdGet = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, tx))
                        {
                            idSolicitud = Convert.ToInt64(cmdGet.ExecuteScalar());
                        }

                        foreach (var item in solicitudActual)
                        {
                            string sqlDetalle = @"INSERT INTO detalle_solicitud
                                           (id_solicitud, id_material, nombre_proyecto, responsable, nombre_material, descripcion_material, cantidad, precio_unitario, subtotal)
                                           VALUES
                                           (@idSolicitud, @idMaterial, @nombreProyecto, @responsable, @nombreMaterial, @descripcionMaterial, @cantidad, @precioUnitario, @subtotal)";
                            using (var cmdDet = new MySqlCommand(sqlDetalle, conn, tx))
                            {
                                cmdDet.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                                cmdDet.Parameters.AddWithValue("@idMaterial", item.IdMaterial);
                                cmdDet.Parameters.AddWithValue("@nombreProyecto", nombreProyecto);
                                cmdDet.Parameters.AddWithValue("@responsable", SesionUsuario.Nombre);
                                cmdDet.Parameters.AddWithValue("@nombreMaterial", item.Nombre);
                                cmdDet.Parameters.AddWithValue("@descripcionMaterial", item.Descripcion);
                                cmdDet.Parameters.AddWithValue("@cantidad", item.Cantidad);
                                cmdDet.Parameters.AddWithValue("@precioUnitario", item.Precio);
                                cmdDet.Parameters.AddWithValue("@subtotal", item.Subtotal);
                                cmdDet.ExecuteNonQuery();
                            }

                            string sqlStock = "UPDATE materiales SET stock = stock - @cantidad WHERE id_material = @idMaterial";
                            using (var cmdStock = new MySqlCommand(sqlStock, conn, tx))
                            {
                                cmdStock.Parameters.AddWithValue("@cantidad", item.Cantidad);
                                cmdStock.Parameters.AddWithValue("@idMaterial", item.IdMaterial);
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        MessageBox.Show("Solicitud enviada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        solicitudActual.Clear();
                        dgSolicitud.ItemsSource = null;
                        ActualizarTotal();
                        txtNombreProyecto.Clear();
                        CargarMaterialesBD();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MessageBox.Show("Error al enviar la solicitud: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}