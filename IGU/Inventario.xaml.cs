using BuildM.Models;
using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BuildM.IGU
{
    public partial class Inventario : Page
    {
        private string connStr = "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";
        private ObservableCollection<Material> materiales = new ObservableCollection<Material>();

        public Inventario()
        {
            InitializeComponent();
            LoadMaterials();

            Loaded += (s, e) =>
            {
                var win = Window.GetWindow(this);
                if (win != null)
                {
                    win.WindowState = WindowState.Maximized;
                    win.ResizeMode = ResizeMode.NoResize;
                }
            };
        }

        private void LoadMaterials()
        {
            materiales.Clear();
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT id_material, nombre, descripcion, stock, costo FROM materiales";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            materiales.Add(new Material
                            {
                                IdMaterial = rdr.GetInt32("id_material"),
                                Nombre = rdr.GetString("nombre"),
                                Descripcion = rdr.GetString("descripcion"),
                                Stock = rdr.GetInt32("stock"),
                                Costo = rdr.GetDecimal("costo")
                            });
                        }
                    }
                }
                dgMateriales.ItemsSource = null;
                dgMateriales.ItemsSource = materiales;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materiales: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Ingrese el nombre."); return; }
            if (!int.TryParse(txtStock.Text, out int stock)) { MessageBox.Show("Stock inválido."); return; }
            if (!decimal.TryParse(txtCosto.Text, out decimal costo)) { MessageBox.Show("Costo inválido."); return; }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO materiales (nombre, descripcion, stock, costo) VALUES (@n,@d,@s,@c); SELECT LAST_INSERT_ID();", conn);
                    cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@s", stock);
                    cmd.Parameters.AddWithValue("@c", costo);

                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    materiales.Add(new Material { IdMaterial = newId, Nombre = txtNombre.Text.Trim(), Descripcion = txtDescripcion.Text.Trim(), Stock = stock, Costo = costo });
                    ClearInputs();
                    MessageBox.Show("Material agregado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgMateriales.SelectedItem is Material selected)
            {
                if (!int.TryParse(txtStock.Text, out int stock)) { MessageBox.Show("Stock inválido."); return; }
                if (!decimal.TryParse(txtCosto.Text, out decimal costo)) { MessageBox.Show("Costo inválido."); return; }

                try
                {
                    using (var conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        var cmd = new MySqlCommand("UPDATE materiales SET nombre=@n, descripcion=@d, stock=@s, costo=@c WHERE id_material=@id", conn);
                        cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                        cmd.Parameters.AddWithValue("@s", stock);
                        cmd.Parameters.AddWithValue("@c", costo);
                        cmd.Parameters.AddWithValue("@id", selected.IdMaterial);
                        cmd.ExecuteNonQuery();
                    }

                    selected.Nombre = txtNombre.Text.Trim();
                    selected.Descripcion = txtDescripcion.Text.Trim();
                    selected.Stock = stock;
                    selected.Costo = costo;
                    dgMateriales.Items.Refresh();

                    MessageBox.Show("Material actualizado.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un material para actualizar.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMateriales.SelectedItem is Material selected)
            {
                var res = MessageBox.Show($"¿Eliminar el material '{selected.Nombre}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;

                try
                {
                    using (var conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        var cmdRef = new MySqlCommand("DELETE FROM detalle_solicitud WHERE id_material=@id", conn);
                        cmdRef.Parameters.AddWithValue("@id", selected.IdMaterial);
                        cmdRef.ExecuteNonQuery();

                        var cmd = new MySqlCommand("DELETE FROM materiales WHERE id_material=@id", conn);
                        cmd.Parameters.AddWithValue("@id", selected.IdMaterial);
                        cmd.ExecuteNonQuery();
                    }

                    materiales.Remove(selected);
                    ClearInputs();
                    MessageBox.Show("Material eliminado.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un material para eliminar.");
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadMaterials();
        }

        private void dgMateriales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMateriales.SelectedItem is Material sel)
            {
                txtNombre.Text = sel.Nombre;
                txtDescripcion.Text = sel.Descripcion;
                txtStock.Text = sel.Stock.ToString();
                txtCosto.Text = sel.Costo.ToString("F2");
            }
        }

        private void ClearInputs()
        {
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtStock.Clear();
            txtCosto.Clear();
            dgMateriales.UnselectAll();
        }
    }
}
