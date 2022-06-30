using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;

public class _NotificationManager : MonoBehaviour
{
    const string ch_Id = "Channel_Id";
    const string _name = "Reminder";
    const string description = "Reminder Notification";

    AndroidNotificationChannel? notifChanel;

    private void Start()
    {
        AndroidNotificationCenter.Initialize();

        try
        {
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
        }
        catch
        {
            return;
        }
    }

    void CreateChannel()
    {
        AndroidNotificationChannel tempChanel = new AndroidNotificationChannel(
            ch_Id, _name, description, Importance.High
            );

        tempChanel.EnableLights = true;        
        tempChanel.EnableVibration = true;
        tempChanel.CanShowBadge = true;
        tempChanel.LockScreenVisibility = LockScreenVisibility.Public;        

        notifChanel = tempChanel;
        AndroidNotificationCenter.RegisterNotificationChannel(notifChanel.Value);
    }

    string small_Icon = "icon_small";
    string large_Icon = "icon_large";

    public void ScheduleNotification(DateTime fireTime)
    {
        if (fireTime.Date == DateTime.Today.Date && fireTime.TimeOfDay < DateTime.Now.TimeOfDay)
            return;

        AndroidNotificationCenter.CancelAllNotifications();

        if(notifChanel == null)        
            CreateChannel();
        
        SendNotification(fireTime);
    }

    void SendNotification(DateTime fireTime)
    {
        string title = "I Miss You";
        string text = "Come To PaPa , It Doesnt Hurt ...";

        AndroidNotification reminder = new AndroidNotification(
            title , text , fireTime
            );

        reminder.SmallIcon = small_Icon;
        reminder.LargeIcon = large_Icon;        

        AndroidNotificationCenter.SendNotification(reminder, ch_Id);        
    }
}
