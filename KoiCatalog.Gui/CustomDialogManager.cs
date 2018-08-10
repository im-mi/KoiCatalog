using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace KoiCatalog.Gui
{
    public static class CustomDialogManager
    {
        /// <summary>
        /// Creates a <see cref="CustomMessageDialog"/> inside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the <see cref="CustomMessageDialog"/>.</param>
        /// <param name="message">The message contained within the <see cref="CustomMessageDialog"/>.</param>
        /// <param name="style">The type of buttons to use.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        public static Task<MessageDialogResult> ShowCustomMessageAsync(
            this MetroWindow window, string title, object message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            settings = settings ?? window.MetroDialogOptions;
            var dialog = new CustomMessageDialog(window, settings)
            {
                Message = message,
                Title = title,
                ButtonStyle = style
            };

            var tcs = new TaskCompletionSource<MessageDialogResult>();

            dialog.WaitForButtonPressAsync().ContinueWith(async task =>
            {
                await window.Invoke(async () =>
                {
                    await window.HideMetroDialogAsync(dialog);
                    // The focus may have changed and altered the command-routing path, so suggest that command conditions be re-evaluated.
                    // Note that this may no longer necessary since making the "current document" content control
                    // focusable (which fixed a bunch of command condition re-evaluation issues).
                    CommandManager.InvalidateRequerySuggested();
                });
                tcs.TrySetResult(task.Result);
            });

            window.ShowMetroDialogAsync(dialog, settings ?? window.MetroDialogOptions);

            return tcs.Task;
        }
    }
}
