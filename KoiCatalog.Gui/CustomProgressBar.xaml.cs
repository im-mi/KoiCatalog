using System.Windows;

namespace KoiCatalog.Gui
{
    public partial class CustomProgressBar
    {
        public string Overlay
        {
            get => (string)GetValue(OverlayProperty);
            set => SetValue(OverlayProperty, value);
        }
        public static readonly DependencyProperty OverlayProperty =
            DependencyProperty.Register(nameof(Overlay), typeof(string), typeof(CustomProgressBar));
        
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(CustomProgressBar));
        
        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(CustomProgressBar));
        
        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(CustomProgressBar));
        
        public CustomProgressBar()
        {
            InitializeComponent();
        }
    }
}
