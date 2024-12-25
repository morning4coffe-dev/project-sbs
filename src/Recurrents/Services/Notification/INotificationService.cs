namespace Recurrents.Services.Notifications;

public class InAppNotificationRequestedEventArgs : EventArgs { public required string NotificationText { get; set; } public int NotificationTime { get; set; } }

public interface INotificationService
{
    bool IsEnabledOnDevice();

    bool IsNotificationScheduledForDate(DateOnly date, string itemId);

    void ShowInAppNotification(string notification, bool autoHide = true);

    void ShowBasicToastNotification(string title, string description);

    //void ShowErrorNotification(Exception ex);

    void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time);
    void RemoveScheduledNotifications(string id = "");
}

public abstract class NotificationServiceBase : INotificationService
{
    public event EventHandler<InAppNotificationRequestedEventArgs>? InAppNotificationRequested;

    public void InvokeInAppNotificationRequested(InAppNotificationRequestedEventArgs args)
    {
        InAppNotificationRequested?.Invoke(this, args);
    }

    //TODO ShowInAppNotification
    //public void ShowErrorNotification(Exception ex) => ShowInAppNotification(string.Format(Resources.ErrorGeneric, ex), false);

    public abstract bool IsEnabledOnDevice();
    public abstract bool IsNotificationScheduledForDate(DateOnly date, string itemId);
    public abstract void ShowInAppNotification(string notification, bool autoHide = true);
    public abstract void ShowBasicToastNotification(string title, string description);
    public abstract void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time);
    public abstract void RemoveScheduledNotifications(string id = "");
}
