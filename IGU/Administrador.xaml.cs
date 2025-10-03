using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuildM.IGU
{
    public partial class Administrador : Window
    {
        public Administrador()
        {
            InitializeComponent();

            // Al iniciar, mostramos Inventario por defecto
            frameContenido.Content = new Inventario();
        }

        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            frameContenido.Content = new Inventario();
        }

        private void BtnSolicitudes_Click(object sender, RoutedEventArgs e)
        {
            //frameContenido.Content = new Solicitudes();
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            //frameContenido.Content = new Reportes();
        }
    }
}