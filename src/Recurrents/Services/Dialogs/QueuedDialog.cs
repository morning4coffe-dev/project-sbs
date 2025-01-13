namespace Recurrents.Services.Dialogs;

public class QueuedDialog(ContentDialog dialog)
{
    public TaskCompletionSource<ContentDialogResult> CompletionSource { get; } = new TaskCompletionSource<ContentDialogResult>();

    public ContentDialog Dialog { get; } = dialog;
}
