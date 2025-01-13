namespace Recurrents.Services.Dialogs;

public interface IDialogService
{
    Task<ContentDialogResult> ShowAsync(
        string title,
        string message,
        string submitText = "",
        ContentDialogButton defaultButton = ContentDialogButton.None,
        ICommand? submit = default);
}
