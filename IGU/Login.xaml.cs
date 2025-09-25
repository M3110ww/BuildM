using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuildM.IGU
{
    public partial class Login : Window
    {
        private string connStr = "Server=localhost;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;";

        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string contraseña = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Ingrese correo y contraseña.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                using var conn = new MySqlConnection(connStr);
                conn.Open();

                string sql = "SELECT rol FROM usuarios WHERE correo=@c AND contraseña=@p";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@c", correo);
                cmd.Parameters.AddWithValue("@p", contraseña);

                var rol = cmd.ExecuteScalar() as string;
                //hola
                if (rol == null)
                {
                    MessageBox.Show("Correo o contraseña incorrectos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Bienvenido, {rol}!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (rol == "ADMINISTRADOR")
                    {
                        var ventanaAdmin = new Administrador(); // ahora abre la nueva interfaz con menú
                        ventanaAdmin.Show();
                    }
                    else if (rol == "CLIENTE")
                    {
                        var ventanaCliente = new Solicitud(); // tu ventana de solicitud de materiales
                        ventanaCliente.Show();
                    }

                    this.Close(); // cerrar login
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegistro_Click(object sender, RoutedEventArgs e)
        {
            var registro = new Registro();
            registro.Show();
            this.Close();
        }
    }
}