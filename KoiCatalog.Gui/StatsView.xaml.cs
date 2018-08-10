using System.Windows;
using KoiCatalog.Data;

namespace KoiCatalog.Gui
{
    public partial class StatsView
    {
        public Stats Stats
        {
            get => (Stats)GetValue(StatsProperty);
            set => SetValue(StatsProperty, value);
        }
        public static readonly DependencyProperty StatsProperty =
            DependencyProperty.Register(nameof(Stats), typeof(Stats), typeof(StatsView));

        public StatsView()
        {
            InitializeComponent();
        }
    }
}
