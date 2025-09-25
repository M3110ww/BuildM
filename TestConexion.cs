using BuildM;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuildM
{
    class Testconexion
    {
        public static void Probar()
        {
            string connStr = "Server=localhost;Port=3306;Database=BuildManager;Uid=root;Pwd=1013105926;SslMode=None;AllowPublicKeyRetrieval=True;"; ;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("Conexión exitosa a MySQL");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }
    }
}