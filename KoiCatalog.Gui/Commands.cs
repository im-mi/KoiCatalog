using System.Windows.Input;

namespace KoiCatalog.Gui
{
    static class Commands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "E_xit",
            "Exit",
            typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand OpenDirectory = new RoutedUICommand
        (
            "_Open Directory...",
            "Open Directory",
            typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Refresh = new RoutedUICommand
        (
            "_Refresh",
            "Refresh",
            typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.R, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand DatabaseEventLog = new RoutedUICommand
        (
            "Event _Log",
            "Event Log",
            typeof(Commands)
        );

        public static readonly RoutedUICommand DatabaseEventLogClear = new RoutedUICommand
        (
            "_Clear Log",
            "Clear Log",
            typeof(Commands)
        );

        public static readonly RoutedUICommand Rebuild = new RoutedUICommand
        (
            "Re_build",
            "Rebuild",
            typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );

        public static readonly RoutedUICommand About = new RoutedUICommand
        (
            "_About",
            "About",
            typeof(Commands),
            new InputGestureCollection
            {
                new KeyGesture(Key.F1)
            }
        );

        public static readonly RoutedUICommand StartPage = new RoutedUICommand
        (
            "_Start Page",
            "Start Page",
            typeof(Commands)
        );

        public static readonly RoutedUICommand SelectInverse = new RoutedUICommand
        (
            "Select _Inverse",
            "Select Inverse",
            typeof(Commands)
        );

        public static readonly RoutedUICommand SelectNone = new RoutedUICommand
        (
            "Select _None",
            "Select None",
            typeof(Commands)
        );

        public static readonly RoutedUICommand NavigateTo = new RoutedUICommand
        (
            "_Navigate To",
            "Navigate To",
            typeof(Commands)
        );
    }
}
