using System.Windows;
using System.Windows.Controls;

namespace KoiCatalog.Gui
{
    public class Document : UserControl
    {
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Document), new PropertyMetadata(default(string)));

        public bool IsUserClosable
        {
            get => (bool)GetValue(IsUserClosableProperty);
            set => SetValue(IsUserClosableProperty, value);
        }
        public static readonly DependencyProperty IsUserClosableProperty =
            DependencyProperty.Register(nameof(IsUserClosable), typeof(bool), typeof(Document), new PropertyMetadata(true));
    }
}
