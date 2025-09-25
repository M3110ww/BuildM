using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            // TODO: lógica para agregar materiales
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            // TODO: lógica para enviar la solicitud
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // cerrar ventana de solicitud
        }
    }
}
