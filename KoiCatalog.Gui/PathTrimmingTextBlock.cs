using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KoiCatalog.Gui
{
    // Written by Mykhailo Seniutovych
    // Source: https://stackoverflow.com/a/47970287
    public class PathTrimmingTextBlock : TextBlock, INotifyPropertyChanged
    {
        #region Dependency properties
        //This property represents the Text of this textblock that can be bound to another viewmodel property, 
        //whenever this property is updated the Text property will be updated too.
        //We cannot bind to Text property directly because once we update Text, e.g., Text = "NewValue", the binding will be broken
        public string BoundedText
        {
            get { return GetValue(BoundedTextProperty).ToString(); }
            set { SetValue(BoundedTextProperty, value); }
        }

        public static readonly DependencyProperty BoundedTextProperty = DependencyProperty.Register(
            nameof(BoundedText), typeof(string), typeof(PathTrimmingTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(BoundedTextProperty_Changed)));

        //Every time the property BoundedText is updated two things should be done:
        //1) Text should be updated to be equal to new BoundedText
        //2) New path should be trimmed again
        private static void BoundedTextProperty_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            var pathTrimmingTextBlock = (PathTrimmingTextBlock)sender;
            pathTrimmingTextBlock.OnPropertyChanged(nameof(BoundedText));
            pathTrimmingTextBlock.Text = pathTrimmingTextBlock.BoundedText;
            pathTrimmingTextBlock.TrimPathAsync();
        }
        #endregion

        private const string Ellipsis = "...";


        public PathTrimmingTextBlock()
        {
            // This will make sure if the directory name is too long it will be trimmed with ellipsis on the right side
            TextTrimming = TextTrimming.CharacterEllipsis;

            //setting the event handler for every time this PathTrimmingTextBlock is rendered
            Loaded += new RoutedEventHandler(PathTrimmingTextBox_Loaded);
        }

        private void PathTrimmingTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            //asynchronously update Text, so that the window won't be frozen
            TrimPathAsync();
        }

        private void TrimPathAsync()
        {
            Task.Run(() => Dispatcher.Invoke(() => TrimPath()));
        }

        private void TrimPath()
        {
            var isWidthOk = false; //represents if the width of the Text is short enough and should not be trimmed 
            var widthChanged = false; //represents if the width of Text was changed, if the text is short enough at the begging it should not be trimmed
            var wasTrimmed = false; //represents if Text was trimmed at least one time

            //in this loop we will be checking the current width of textblock using FormattedText at every iteration,
            //if the width is not short enough to fit textblock it will be shrinked by one character, and so on untill it fits
            do
            {
                //widthChanged? Text + Ellipsis : Text - at first iteration we have to check if Text is not already short enough to fit textblock,
                //after widthChanged = true, we will have to measure the width of Text + Ellipsis, because ellipsis will be added to Text
                var formattedText = new FormattedText(widthChanged ? Text + Ellipsis : Text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                    FontSize,
                    Foreground);

                //check if width fits textblock RenderSize.Width, (cannot use Width here because it's not set during rendering,
                //and cannot use ActualWidth either because it is the initial width of Text not textblock itself)
                isWidthOk = formattedText.Width < RenderSize.Width;

                //if it doesn't fit trim it by one character
                if (!isWidthOk)
                {
                    wasTrimmed = TrimPathByOneChar();
                    widthChanged = true;
                }
                //continue loop
            } while (!isWidthOk && wasTrimmed);

            //Format Text with ellipsis, if width was changed (after previous loop we may have gotten a path like this "D:\Dire\Directory" 
            //it should be formatted to "D:\...\Directory")
            if (widthChanged)
            {
                FormatWithEllipsis();
            }
        }

        //Trim Text by one character before last slash, if Text doesn't have slashes it won't be trimmed with ellipsis in the middle,
        //instead it will be trimmed with ellipsis at the end due to having TextTrimming = TextTrimming.CharacterEllipsis; in the constructor
        private bool TrimPathByOneChar()
        {
            var lastSlashIndex = Text.LastIndexOf('\\');
            if (lastSlashIndex > 0)
            {
                Text = Text.Substring(0, lastSlashIndex - 1) + Text.Substring(lastSlashIndex);
                return true;
            }
            return false;
        }

        //"\Directory will become "...\Directory"
        //"Dire\Directory will become "...\Directory"\
        //"D:\Dire\Directory" will become "D:\...\Directory"
        private void FormatWithEllipsis()
        {
            var lastSlashIndex = Text.LastIndexOf('\\');
            if (lastSlashIndex == 0)
            {
                Text = Ellipsis + Text;
            }
            else if (lastSlashIndex > 0)
            {
                var secondastSlashIndex = Text.LastIndexOf('\\', lastSlashIndex - 1);
                if (secondastSlashIndex < 0)
                {
                    Text = Ellipsis + Text.Substring(lastSlashIndex);
                }
                else
                {
                    Text = Text.Substring(0, secondastSlashIndex + 1) + Ellipsis + Text.Substring(lastSlashIndex);
                }
            }
        }

        //starndard implementation of INotifyPropertyChanged to be able to notify BoundedText property change 
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
