using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuildM.IGU
{
    public partial class Registro : Window
    {
        // 🔹 Ajusta tu cadena de conexión según tu configuración
        private string connStr = "Server=localhost;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public Registro()
        {
            InitializeComponent();
        }

        // Botón Registrar
        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string contraseña = txtPassword.Password.Trim();
            string rol = (cmbRol.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();

            // Validación rápida
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Debe completar todos los campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "INSERT INTO usuarios (nombre, correo, contraseña, rol) VALUES (@n, @c, @p, @r)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", nombre);
                        cmd.Parameters.AddWithValue("@c", correo);
                        cmd.Parameters.AddWithValue("@p", contraseña); // 🔐 luego podemos encriptar
                        cmd.Parameters.AddWithValue("@r", rol);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Opcional: volver al login después de registrar
                var login = new Login();
                login.Show();
                this.Close();
            }
            catch (MySqlException ex) when (ex.Number == 1062) // clave duplicada (correo único)
            {
                MessageBox.Show("El correo ya está registrado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Botón Cancelar
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Regresar al Login
            var login = new Login();
            login.Show();
            this.Close();
        }
    }
}