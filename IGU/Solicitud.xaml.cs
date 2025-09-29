using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildM.IGU
{
    public partial class Solicitud : Window
    {
        public Solicitud()
        {
            InitializeComponent();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            string material = txtMaterial.Text.Trim();
            string cantidad = txtCantidad.Text.Trim();

            if (string.IsNullOrWhiteSpace(material) || string.IsNullOrWhiteSpace(cantidad))
            {
                MessageBox.Show("Debe ingresar material y cantidad.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Material: {material}\nCantidad: {cantidad}", "Datos ingresados",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}