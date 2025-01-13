#if !__ANDROID__ && !WINDOWS
namespace Recurrents.Services.Notifications;

public class NotificationService : NotificationServiceBase
{
    public override bool IsEnabledOnDevice()
    {
        return false;
    }

    public override bool IsNotificationScheduledForDate(DateOnly date, string itemId)
    {
        return false;
    }

    public override void RemoveScheduledNotifications(string id = "")
    {
        
    }

    public override void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time)
    {
        
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        
    }

    public override void ShowInAppNotification(string notification, bool autoHide = true)
    {
        
    }
}
#endif
