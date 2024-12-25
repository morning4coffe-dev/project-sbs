#if __ANDROID__

#nullable disable

using Android;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Uno.Foundation.Logging;
using Uno.Logging;

namespace Recurrents.Services.Notifications;

public class NotificationService : NotificationServiceBase
{
    private readonly Context _context = Android.App.Application.Context;
    internal const string DefaultChannelId = "Recurrents-default-channel";
    internal const string DefaultChannelName = "Notifications";

    public NotificationService()
    {
        EnsureNotificationChannel();
    }

    public override bool IsEnabledOnDevice()
    {
        return Build.VERSION.SdkInt >= BuildVersionCodes.N ||
               NotificationManagerCompat.From(_context).AreNotificationsEnabled();
    }

    private string GetNotificationId(string itemId, DateOnly date)
    {
        return $"{itemId}_{date:yyyy_MM_dd}";
    }

    public override bool IsNotificationScheduledForDate(DateOnly date, string itemId)
    {
        string notificationId = GetNotificationId(itemId, date);
        Intent notificationIntent = new(_context, typeof(NotificationReceiver));

        PendingIntent pendingIntent = PendingIntent.GetBroadcast(
            _context,
            notificationId.GetHashCode(),
            notificationIntent,
            PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable);

        return pendingIntent != null;
    }

    private async Task<bool> TryRequestPermissions()
    {
        var currentActivity = await ApplicationActivity.GetCurrent(CancellationToken.None);

        if (ActivityCompat.CheckSelfPermission(currentActivity, Manifest.Permission.PostNotifications) == Android.Content.PM.Permission.Granted)
        {
            return true;
        }

        ActivityCompat.RequestPermissions(currentActivity, new[] { Manifest.Permission.PostNotifications }, 1);

        return ActivityCompat.CheckSelfPermission(currentActivity, Manifest.Permission.PostNotifications) == Android.Content.PM.Permission.Granted;
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {
        InvokeInAppNotificationRequested(new InAppNotificationRequestedEventArgs
        {
            NotificationText = notification,
            NotificationTime = autoHide ? 1500 : 0
        });
    }

    public override async void ScheduleNotification(string itemId, string title, string text, DateOnly day, TimeOnly time)
    {
        if (!await TryRequestPermissions())
        {
            this.Log().Error("Permission denied for scheduling notifications.");
            return;
        }

        var notificationDateTime = new DateTime(day.Year, day.Month, day.Day, time.Hour, time.Minute, 0);
        if (notificationDateTime <= DateTime.Now)
        {
            this.Log().Error("Cannot schedule a notification in the past.");
            return;
        }

        string notificationId = GetNotificationId(itemId, day);
        var (manager, intent) = CreateAlarm(notificationId, title, text, notificationDateTime);

        long triggerAtMillis = (long)(notificationDateTime - DateTime.Now).TotalMilliseconds;
        manager?.SetExact(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + triggerAtMillis, intent);
    }

    private (AlarmManager, PendingIntent) CreateAlarm(string id, string title, string text, DateTime notificationDateTime)
    {
        Intent notificationIntent = new(_context, typeof(NotificationReceiver));
        notificationIntent.PutExtra("id", id);
        notificationIntent.PutExtra("title", title);
        notificationIntent.PutExtra("text", text);

        PendingIntent pendingIntent = PendingIntent.GetBroadcast(
            _context,
            id.GetHashCode(),
            notificationIntent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        AlarmManager alarmManager = (AlarmManager)_context.GetSystemService(Context.AlarmService);
        return (alarmManager, pendingIntent);
    }

    public override void RemoveScheduledNotifications(string itemId)
    {
        Intent notificationIntent = new(_context, typeof(NotificationReceiver));
        PendingIntent pendingIntent = PendingIntent.GetBroadcast(
            _context,
            itemId.GetHashCode(),
            notificationIntent,
            PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable);

        if (pendingIntent != null)
        {
            AlarmManager alarmManager = (AlarmManager)_context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent);
        }
    }

    public override async void ShowBasicToastNotification(string title, string description)
    {
        if (!await TryRequestPermissions())
        {
            this.Log().Error("Permission denied for displaying toast notifications.");
            return;
        }

        var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
        var notificationBuilder = new NotificationCompat.Builder(_context, DefaultChannelId)
            .SetSmallIcon(Resource.Drawable.recurrents_notification_icon)
            .SetColor(Android.Graphics.Color.Argb(255, 28, 133, 34))
            .SetContentTitle(title)
            .SetContentText(description)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetAutoCancel(true);

        notificationManager.Notify(Guid.NewGuid().GetHashCode(), notificationBuilder.Build());
    }

    private void EnsureNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
            var channel = new NotificationChannel(DefaultChannelId, DefaultChannelName, NotificationImportance.High)
            {
                Description = "Default notification channel for Recurrents"
            };
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}

[BroadcastReceiver(Enabled = true)]
public class NotificationReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        string id = intent.GetStringExtra("id");
        string title = intent.GetStringExtra("title");
        string text = intent.GetStringExtra("text");

        var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
        var notificationBuilder = new NotificationCompat.Builder(context, NotificationService.DefaultChannelId)
            .SetSmallIcon(Resource.Drawable.recurrents_notification_icon)
            .SetColor(Android.Graphics.Color.Argb(255, 28, 133, 34))
            .SetContentTitle(title)
            .SetContentText(text)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetAutoCancel(true);

        notificationManager.Notify(id.GetHashCode(), notificationBuilder.Build());
    }
}

#endif
