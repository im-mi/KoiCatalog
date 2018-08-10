using System.Windows;
using System.Windows.Media;

namespace KoiCatalog.Gui
{
    public partial class ColorControl
    {
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorControl));

        public ColorControl()
        {
            InitializeComponent();
        }
    }
}
