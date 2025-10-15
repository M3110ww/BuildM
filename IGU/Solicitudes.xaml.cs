using BuildM.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BuildM.IGU
{
    public partial class Solicitudes : UserControl
    {
        public Solicitudes()
        {
            InitializeComponent();
            Loaded += (s, e) => CargarSolicitudes();
        }

        private void CargarSolicitudes()
        {
            var dao = new SolicitudDAO();
            List<Solicitudd> solicitudes = dao.ObtenerSolicitudes();

            ContenedorSolicitudes.Items.Clear();

            foreach (var solicitud in solicitudes)
            {
                if (string.IsNullOrWhiteSpace(solicitud.Estado))
                    solicitud.Estado = "Pendiente";

                var tarjeta = CrearTarjetaSolicitud(solicitud);
                tarjeta.Opacity = 0;

                ContenedorSolicitudes.Items.Add(tarjeta);

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                tarjeta.BeginAnimation(Border.OpacityProperty, fadeIn);
            }
        }

        private Border CrearTarjetaSolicitud(Solicitudd solicitud)
        {
            Border borde = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxWidth = 800,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    Opacity = 0.2
                }
            };

            DockPanel panel = new DockPanel { LastChildFill = false };

            // --- Controles a la derecha ---
            StackPanel derecha = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            ComboBox estadoCombo = new ComboBox
            {
                ItemsSource = new List<string> { "Pendiente", "Aprobado", "Rechazado" },
                SelectedItem = solicitud.Estado,
                Width = 120,
                Margin = new Thickness(10, 0, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.SemiBold
            };
            CambiarColorEstado(estadoCombo, solicitud.Estado);

            estadoCombo.SelectionChanged += (s, e) =>
            {
                if (estadoCombo.SelectedItem != null)
                {
                    string nuevoEstado = estadoCombo.SelectedItem.ToString();
                    CambiarColorEstado(estadoCombo, nuevoEstado);
                    solicitud.Estado = nuevoEstado;

                    var dao = new SolicitudDAO();
                    dao.ActualizarEstado(solicitud.Id, nuevoEstado);
                }
            };

            Button btnDetalles = new Button
            {
                Content = "Detalles",
                Background = new SolidColorBrush(Color.FromRgb(30, 58, 95)),
                Foreground = Brushes.White,
                Margin = new Thickness(10, 0, 0, 0),
                Padding = new Thickness(12, 4, 12, 4),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btnDetalles.Click += (s, e) =>
            {
                DetalleSolicitud ventana = new DetalleSolicitud(solicitud);
                ventana.ShowDialog();
            };

            derecha.Children.Add(estadoCombo);
            derecha.Children.Add(btnDetalles);
            DockPanel.SetDock(derecha, Dock.Right);
            panel.Children.Add(derecha);

            // --- Información a la izquierda ---
            StackPanel izquierda = new StackPanel { Margin = new Thickness(0, 0, 10, 0) };

            izquierda.Children.Add(new TextBlock
            {
                Text = $"Proyecto: {solicitud.NombreProyecto}",
                FontWeight = FontWeights.Bold,
                FontSize = 15,
                Foreground = Brushes.Black
            });

            izquierda.Children.Add(new TextBlock
            {
                Text = $"Responsable: {solicitud.Responsable}",
                Foreground = Brushes.Gray,
                FontSize = 13
            });

            izquierda.Children.Add(new TextBlock
            {
                Text = $"Fecha: {solicitud.Fecha:dd/MM/yyyy}",
                Foreground = Brushes.Gray,
                FontSize = 13
            });

            panel.Children.Add(izquierda);

            borde.Child = panel;
            return borde;
        }

        private void CambiarColorEstado(ComboBox combo, string estado)
        {
            Brush color = Brushes.LightGray;
            switch (estado)
            {
                case "Pendiente":
                    color = new SolidColorBrush(Color.FromRgb(255, 213, 79)); break; // amarillo
                case "Aprobado":
                    color = new SolidColorBrush(Color.FromRgb(76, 175, 80)); break; // verde
                case "Rechazado":
                    color = new SolidColorBrush(Color.FromRgb(244, 67, 54)); break; // rojo
            }
            combo.Background = color;
        }
    }
}
