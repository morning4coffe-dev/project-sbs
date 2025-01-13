#if X
namespace Recurrents.Services.Notifications;

public class NotificationService : NotificationServiceBase
{
    public NotificationService()
    {
    }

    public override bool IsEnabledOnDevice()
    {
        return false;
    }
    public override bool IsNotificationScheduledForDate(DateOnly date, string itemId)
    {
        return false;
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {

    }

    public override async void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time)
    {

    }

    public override void RemoveScheduledNotifications(string id)
    {

    }

    public override void ShowBasicToastNotification(string title, string description)
    {

    }
}
#endif
