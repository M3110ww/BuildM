using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace BuildM.IGU
{
    public partial class Solicitud : Window
    {
        public class Material
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public int Stock { get; set; }
            public double Precio { get; set; }
        }

        public class ItemSolicitud
        {
            public int IdMaterial { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public int Cantidad { get; set; }
            public double Precio { get; set; }
            public double Subtotal => Cantidad * Precio;
        }

        private List<Material> materialesDisponibles = new List<Material>();
        private List<ItemSolicitud> solicitudActual = new List<ItemSolicitud>();

        string connStr = "server=localhost;database=buildmanager;uid=root;pwd=1013105926;";

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
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    materialesDisponibles.Add(new Material
                    {
                        Id = reader.GetInt32("id_material"),
                        Nombre = reader.GetString("nombre"),
                        Stock = reader.GetInt32("stock"),
                        Descripcion = reader.GetString("descripcion"),
                        Precio = reader.GetDouble("costo")
                    });
                }
            }

            // 🔹 Mostrar nombre + descripción + stock + precio
            cmbMateriales.ItemsSource = materialesDisponibles
            .Select(m => $"{m.Nombre} - {m.Descripcion} - Precio: ${m.Precio}")
            .ToList();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMateriales.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Seleccione un material y cantidad.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var seleccionado = materialesDisponibles[cmbMateriales.SelectedIndex];

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cantidad > seleccionado.Stock)
            {
                MessageBox.Show("No hay suficiente stock disponible.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var item = new ItemSolicitud
            {
                IdMaterial = seleccionado.Id,
                Nombre = seleccionado.Nombre,
                Cantidad = cantidad,
                Precio = seleccionado.Precio
            };

            solicitudActual.Add(item);
            dgSolicitud.ItemsSource = null;
            dgSolicitud.ItemsSource = solicitudActual;

            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            double total = solicitudActual.Sum(x => x.Subtotal);
            lblTotal.Content = $"Total: ${total}";
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (!solicitudActual.Any())
            {
                MessageBox.Show("No hay materiales en la solicitud.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("¿Está seguro de enviar la solicitud?", "Confirmación",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 🔹 Insertar solicitud principal
                        string querySolicitud = "INSERT INTO solicitudes (fecha, total) VALUES (NOW(), @total)";
                        MySqlCommand cmdSolicitud = new MySqlCommand(querySolicitud, conn, transaction);
                        cmdSolicitud.Parameters.AddWithValue("@total", solicitudActual.Sum(x => x.Subtotal));
                        cmdSolicitud.ExecuteNonQuery();

                        long idSolicitud = cmdSolicitud.LastInsertedId;

                        // 🔹 Insertar detalles de solicitud
                        foreach (var item in solicitudActual)
                        {
                            string queryDetalle = @"INSERT INTO detalle_solicitud 
                                                    (idSolicitud, id_material, cantidad, precio, subtotal)
                                                    VALUES (@idSolicitud, @id_material, @cantidad, @precio, @subtotal)";
                            MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, conn, transaction);
                            cmdDetalle.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                            cmdDetalle.Parameters.AddWithValue("@id_material", item.IdMaterial);
                            cmdDetalle.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@precio", item.Precio);
                            cmdDetalle.Parameters.AddWithValue("@subtotal", item.Subtotal);
                            cmdDetalle.ExecuteNonQuery();

                            // 🔹 Actualizar stock
                            string updateStock = "UPDATE materiales SET stock = stock - @cantidad WHERE id_material = @id_material";
                            MySqlCommand cmdStock = new MySqlCommand(updateStock, conn, transaction);
                            cmdStock.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            cmdStock.Parameters.AddWithValue("@id_material", item.IdMaterial);
                            cmdStock.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Solicitud enviada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        solicitudActual.Clear();
                        dgSolicitud.ItemsSource = null;
                        ActualizarTotal();
                        CargarMaterialesBD();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
