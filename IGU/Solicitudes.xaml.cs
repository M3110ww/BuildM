using BuildM.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BuildM.IGU
{
    public partial class Solicitudes : Page
    {
        public Solicitudes()
        {
            InitializeComponent();
            CargarSolicitudes();
        }

        private void CargarSolicitudes()
        {
            var dao = new SolicitudDAO();
            List<Solicitudd> solicitudes = dao.ObtenerSolicitudes();

            ContenedorSolicitudes.Children.Clear();

            foreach (var solicitud in solicitudes)
            {
                if (string.IsNullOrEmpty(solicitud.Estado))
                    solicitud.Estado = "EN ESPERA";

                Border card = CrearTarjetaSolicitud(solicitud);
                ContenedorSolicitudes.Children.Add(card);

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
                card.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
        }

        private Border CrearTarjetaSolicitud(Solicitudd solicitud)
        {
            Border card = new Border
            {
                CornerRadius = new CornerRadius(8),
                Background = Brushes.White,
                Margin = new Thickness(0, 6, 0, 6),
                Padding = new Thickness(16),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 6,
                    ShadowDepth = 2,
                    Opacity = 0.2
                }
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Información general
            StackPanel info = new StackPanel();
            info.Children.Add(new TextBlock
            {
                Text = $"Proyecto: {solicitud.NombreProyecto}",
                FontWeight = FontWeights.Bold,
                FontSize = 15,
                Margin = new Thickness(0, 0, 0, 2)
            });
            info.Children.Add(new TextBlock
            {
                Text = $"Responsable: {solicitud.Responsable}",
                FontSize = 13,
                Foreground = Brushes.Gray
            });
            info.Children.Add(new TextBlock
            {
                Text = $"Fecha: {solicitud.Fecha:dd/MM/yyyy}",
                FontSize = 13,
                Foreground = Brushes.Gray
            });
            Grid.SetColumn(info, 0);
            grid.Children.Add(info);

            ComboBox comboEstado = new ComboBox
            {
                Width = 130,
                Margin = new Thickness(10, 0, 10, 0),
                FontSize = 13,
                ItemsSource = new List<string> { "EN ESPERA", "ACEPTADO", "RECHAZADO" },
                SelectedItem = solicitud.Estado,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            comboEstado.Foreground = ObtenerColorTextoEstado(solicitud.Estado);

            comboEstado.SelectionChanged += (s, e) =>
            {
                if (comboEstado.SelectedItem is string nuevoEstado)
                {
                    solicitud.Estado = nuevoEstado;
                    comboEstado.Foreground = ObtenerColorTextoEstado(nuevoEstado);

                    try
                    {
                        var dao = new SolicitudDAO();
                        dao.ActualizarEstado(solicitud.Id, nuevoEstado);

                        MessageBox.Show($"Estado actualizado a '{nuevoEstado}' correctamente.",
                                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar el estado: {ex.Message}",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            };

            Grid.SetColumn(comboEstado, 1);
            grid.Children.Add(comboEstado);

            Button btnDetalles = new Button
            {
                Content = "Detalles",
                Background = new SolidColorBrush(Color.FromRgb(51, 122, 255)),
                Foreground = Brushes.White,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 0, 5, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            Grid.SetColumn(btnDetalles, 2);
            grid.Children.Add(btnDetalles);

            btnDetalles.Click += (s, e) =>
            {
                try
                {
                    DetalleSolicitud ventanaDetalles = new DetalleSolicitud(solicitud.Id);
                    ventanaDetalles.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir los detalles: " + ex.Message,
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            card.Child = grid;
            return card;
        }

        private SolidColorBrush ObtenerColorTextoEstado(string estado)
        {
            return estado switch
            {
                "ACEPTADO" => new SolidColorBrush(Color.FromRgb(0, 128, 0)),
                "RECHAZADO" => new SolidColorBrush(Color.FromRgb(192, 0, 0)),
                "EN ESPERA" => new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                _ => Brushes.Black
            };
        }
    }
}
