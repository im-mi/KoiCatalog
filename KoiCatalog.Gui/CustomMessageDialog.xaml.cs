using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ControlzEx;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace KoiCatalog.Gui
{
    /// <summary>
    /// A modification of <see cref="MessageDialog"/> that adds support for arbitrary content.
    /// </summary>
    public partial class CustomMessageDialog
    {
        internal CustomMessageDialog()
            : this(null)
        {
        }

        internal CustomMessageDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal CustomMessageDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();

            PART_MessageScrollViewer.Height = DialogSettings.MaximumBodyHeight;
        }

        internal Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Focus();

                var defaultButtonFocus = DialogSettings.DefaultButtonFocus;

                //Ensure it's a valid option
                if (!IsApplicable(defaultButtonFocus))
                {
                    defaultButtonFocus = ButtonStyle == MessageDialogStyle.Affirmative
                        ? MessageDialogResult.Affirmative
                        : MessageDialogResult.Negative;
                }

                //kind of acts like a selective 'IsDefault' mechanism.
                switch (defaultButtonFocus)
                {
                    case MessageDialogResult.Affirmative:
                        PART_AffirmativeButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                        KeyboardNavigationEx.Focus(PART_AffirmativeButton);
                        break;
                    case MessageDialogResult.Negative:
                        PART_NegativeButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                        KeyboardNavigationEx.Focus(PART_NegativeButton);
                        break;
                    case MessageDialogResult.FirstAuxiliary:
                        PART_FirstAuxiliaryButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                        KeyboardNavigationEx.Focus(PART_FirstAuxiliaryButton);
                        break;
                    case MessageDialogResult.SecondAuxiliary:
                        PART_SecondAuxiliaryButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                        KeyboardNavigationEx.Focus(PART_SecondAuxiliaryButton);
                        break;
                }
            }));

            var tcs = new TaskCompletionSource<MessageDialogResult>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            RoutedEventHandler firstAuxHandler = null;
            KeyEventHandler firstAuxKeyHandler = null;

            RoutedEventHandler secondAuxHandler = null;
            KeyEventHandler secondAuxKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers?.Invoke();
                tcs.TrySetResult(ButtonStyle == MessageDialogStyle.Affirmative ? MessageDialogResult.Affirmative : MessageDialogResult.Negative);
            });

            cleanUpHandlers = () =>
            {
                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;
                PART_FirstAuxiliaryButton.Click -= firstAuxHandler;
                PART_SecondAuxiliaryButton.Click -= secondAuxHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
                PART_FirstAuxiliaryButton.KeyDown -= firstAuxKeyHandler;
                PART_SecondAuxiliaryButton.KeyDown -= secondAuxKeyHandler;

                KeyDown -= escapeKeyHandler;

                cancellationTokenRegistration.Dispose();
            };

            negativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);
                }
            };

            affirmativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);
                }
            };

            firstAuxKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);
                }
            };

            secondAuxKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);
                }
            };

            negativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.Negative);

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.Affirmative);

                e.Handled = true;
            };

            firstAuxHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);

                e.Handled = true;
            };

            secondAuxHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);

                e.Handled = true;
            };

            escapeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(DialogSettings.DialogResultOnCancel ?? (ButtonStyle == MessageDialogStyle.Affirmative ? MessageDialogResult.Affirmative : MessageDialogResult.Negative));
                }
                else if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);
                }
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;
            PART_FirstAuxiliaryButton.KeyDown += firstAuxKeyHandler;
            PART_SecondAuxiliaryButton.KeyDown += secondAuxKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;
            PART_FirstAuxiliaryButton.Click += firstAuxHandler;
            PART_SecondAuxiliaryButton.Click += secondAuxHandler;

            KeyDown += escapeKeyHandler;

            return tcs.Task;
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(object), typeof(CustomMessageDialog), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty =
            DependencyProperty.Register(nameof(AffirmativeButtonText), typeof(object), typeof(CustomMessageDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty =
            DependencyProperty.Register(nameof(NegativeButtonText), typeof(object), typeof(CustomMessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty FirstAuxiliaryButtonTextProperty =
            DependencyProperty.Register(nameof(FirstAuxiliaryButtonText), typeof(object), typeof(CustomMessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty SecondAuxiliaryButtonTextProperty =
            DependencyProperty.Register(nameof(SecondAuxiliaryButtonText), typeof(object), typeof(CustomMessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register(nameof(ButtonStyle), typeof(MessageDialogStyle), typeof(CustomMessageDialog), new PropertyMetadata(MessageDialogStyle.Affirmative, ButtonStylePropertyChangedCallback));

        private static void ButtonStylePropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var md = (CustomMessageDialog)s;

            SetButtonState(md);
        }

        private static void SetButtonState(CustomMessageDialog md)
        {
            if (md.PART_AffirmativeButton == null)
                return;

            switch (md.ButtonStyle)
            {
                case MessageDialogStyle.Affirmative:
                {
                    md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                    md.PART_NegativeButton.Visibility = Visibility.Collapsed;
                    md.PART_FirstAuxiliaryButton.Visibility = Visibility.Collapsed;
                    md.PART_SecondAuxiliaryButton.Visibility = Visibility.Collapsed;
                }
                    break;
                case MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegative:
                {
                    md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                    md.PART_NegativeButton.Visibility = Visibility.Visible;

                    if (md.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary || md.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary)
                    {
                        md.PART_FirstAuxiliaryButton.Visibility = Visibility.Visible;
                    }

                    if (md.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary)
                    {
                        md.PART_SecondAuxiliaryButton.Visibility = Visibility.Visible;
                    }
                }
                    break;
            }

            md.AffirmativeButtonText = md.DialogSettings.AffirmativeButtonText;
            md.NegativeButtonText = md.DialogSettings.NegativeButtonText;
            md.FirstAuxiliaryButtonText = md.DialogSettings.FirstAuxiliaryButtonText;
            md.SecondAuxiliaryButtonText = md.DialogSettings.SecondAuxiliaryButtonText;

            switch (md.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    md.PART_AffirmativeButton.SetResourceReference(StyleProperty, "AccentedDialogHighlightedSquareButton");
                    md.PART_NegativeButton.SetResourceReference(StyleProperty, "AccentedDialogHighlightedSquareButton");
                    md.PART_FirstAuxiliaryButton.SetResourceReference(StyleProperty, "AccentedDialogHighlightedSquareButton");
                    md.PART_SecondAuxiliaryButton.SetResourceReference(StyleProperty, "AccentedDialogHighlightedSquareButton");
                    break;
            }
        }

        protected override void OnLoaded()
        {
            SetButtonState(this);
        }

        public MessageDialogStyle ButtonStyle
        {
            get => (MessageDialogStyle)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        public object Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public object AffirmativeButtonText
        {
            get => GetValue(AffirmativeButtonTextProperty);
            set => SetValue(AffirmativeButtonTextProperty, value);
        }

        public object NegativeButtonText
        {
            get => GetValue(NegativeButtonTextProperty);
            set => SetValue(NegativeButtonTextProperty, value);
        }

        public object FirstAuxiliaryButtonText
        {
            get => GetValue(FirstAuxiliaryButtonTextProperty);
            set => SetValue(FirstAuxiliaryButtonTextProperty, value);
        }

        public object SecondAuxiliaryButtonText
        {
            get => GetValue(SecondAuxiliaryButtonTextProperty);
            set => SetValue(SecondAuxiliaryButtonTextProperty, value);
        }

        private void OnKeyCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetDataObject(this.Message);
        }

        private bool IsApplicable(MessageDialogResult value)
        {
            switch (value)
            {
                case MessageDialogResult.Affirmative:
                    return PART_AffirmativeButton.IsVisible;
                case MessageDialogResult.Negative:
                    return PART_NegativeButton.IsVisible;
                case MessageDialogResult.FirstAuxiliary:
                    return PART_FirstAuxiliaryButton.IsVisible;
                case MessageDialogResult.SecondAuxiliary:
                    return PART_SecondAuxiliaryButton.IsVisible;
            }

            return false;
        }

        private void CustomMessageDialog_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Prevent escaping the message dialog by using alt, etc..
            // Todo: Prevent all types of keys from escaping the dialog.
            if (e.Key == Key.System)
            {
                e.Handled = true;
            }
        }
    }
}
