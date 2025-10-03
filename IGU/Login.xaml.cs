using BuildM.Models;
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
                MessageBox.Show("Ingrese correo y contraseña.", "Aviso");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT id_usuario, nombre, rol FROM usuarios WHERE correo=@c AND contraseña=@p";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@c", correo);
                        cmd.Parameters.AddWithValue("@p", contraseña);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Guardar datos en SesionUsuario
                                SesionUsuario.IdUsuario = reader.GetInt32("id_usuario");
                                SesionUsuario.Nombre = reader.GetString("nombre");
                                SesionUsuario.Rol = reader.GetString("rol");

                                MessageBox.Show($"Bienvenido,  ({SesionUsuario.Rol})");

                                if (SesionUsuario.Rol == "ADMINISTRADOR")
                                {
                                    var ventanaAdmin = new Administrador();
                                    ventanaAdmin.Show();
                                }
                                else if (SesionUsuario.Rol == "CLIENTE")
                                {
                                    var ventanaCliente = new Solicitud();
                                    ventanaCliente.Show();
                                }

                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Correo o contraseña incorrectos.", "Error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message, "Error");
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