namespace Recurrents.Services.Dialogs;

public class DialogService(IStringLocalizer localizer) : IDialogService
{
    private readonly IStringLocalizer _localizer = localizer;

    private readonly Queue<QueuedDialog> _dialogQueue = new();
    private bool _isProcessing;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task<ContentDialogResult> ShowAsync(
        string title,
        string message,
        string submitText = "",
        ContentDialogButton defaultButton = ContentDialogButton.None,
        ICommand? submitCommand = default)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = submitText,
            PrimaryButtonCommand = submitCommand,
            DefaultButton = defaultButton,
            CloseButtonText = _localizer["Cancel"]
        };

        return await ShowDialogAsync(dialog);
    }

    private async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
    {
        var queuedDialog = new QueuedDialog(dialog);
        _dialogQueue.Enqueue(queuedDialog);

        await ProcessQueueAsync();

        return await queuedDialog.CompletionSource.Task;
    }

    private async Task ProcessQueueAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (!_isProcessing)
            {
                _isProcessing = true;
                while (_dialogQueue.Count > 0)
                {
                    var queuedDialog = _dialogQueue.Dequeue();
                    queuedDialog.Dialog.XamlRoot = ((App)Application.Current).XamlRoot;
                    var result = await queuedDialog.Dialog.ShowAsync();
                    queuedDialog.CompletionSource.SetResult(result);
                }
            }
        }
        finally
        {
            _isProcessing = false;
            _semaphoreSlim.Release();
        }
    }
}
