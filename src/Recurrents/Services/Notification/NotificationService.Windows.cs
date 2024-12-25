#if WINDOWS
using System;
using CommunityToolkit.WinUI.Notifications;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace Recurrents.Services.Notifications;

public class NotificationService : NotificationServiceBase
{
    public override bool IsEnabledOnDevice()
    {
        ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();
        return notifier.Setting is NotificationSetting.Enabled;
    }

    public override bool IsNotificationScheduledForDate(DateOnly date, string itemId)
    {
        ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();
        IReadOnlyList<ScheduledToastNotification> scheduledToasts = notifier.GetScheduledToastNotifications();
        string notificationId = GetNotificationId(itemId, date);

        return scheduledToasts.Any(toast => toast.Id == notificationId);
    }

    private string GetNotificationId(string itemId, DateOnly date)
    {
        return $"{itemId}_{date:yyyy_MM_dd}";
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {
        InvokeInAppNotificationRequested(new InAppNotificationRequestedEventArgs
        {
            NotificationText = notification,
            NotificationTime = autoHide ? 1500 : 0
        });
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        CreateNotification("", title, description).Show();
    }

    public override void ScheduleNotification(string itemId, string title, string text, DateOnly day, TimeOnly time)
    {
        var date = new DateTime(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
        if (date < DateTime.Now)
        {
            return;
        }

        string notificationId = GetNotificationId(itemId, day);
        CreateNotification(notificationId, title, text).Schedule(date);
    }

    private static ToastContentBuilder CreateNotification(string id, string title, string text)
    {
        return new ToastContentBuilder()
            .AddInlineImage(new Uri("ms-appx:///Assets/Icons/recurrents_icon.png"))
            .AddText(title)
            .AddText(text)
            .AddArgument("action", "viewItem")
            .AddArgument("itemId", id)
            .AddToastInput(new ToastSelectionBox("snoozeTime")
            {
                DefaultSelectionBoxItemId = "15",
                Items =
                {
                    new ToastSelectionBoxItem("5", "5 minutes"),
                    new ToastSelectionBoxItem("15", "15 minutes"),
                    new ToastSelectionBoxItem("60", "1 hour"),
                    new ToastSelectionBoxItem("240", "4 hours"),
                    new ToastSelectionBoxItem("1440", "1 day")
                }
            })
            .AddButton(new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" })
            .SetToastScenario(ToastScenario.Reminder);
    }

    public override void RemoveScheduledNotifications(string id = "")
    {
        ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();
        IReadOnlyList<ScheduledToastNotification> scheduledToasts = notifier.GetScheduledToastNotifications();

        if (string.IsNullOrEmpty(id))
        {
            foreach (var toRemove in scheduledToasts)
            {
                notifier.RemoveFromSchedule(toRemove);
            }
            return;
        }

        foreach (var toRemove in scheduledToasts)
        {
            if (toRemove.Id == id)
            {
                notifier.RemoveFromSchedule(toRemove);
            }
        }
    }
}
#endif
