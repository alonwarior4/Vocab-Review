///*
//Copyright (c) 2020 Razeware LLC

//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or
//sell copies of the Software, and to permit persons to whom
//the Software is furnished to do so, subject to the following
//conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//Notwithstanding the foregoing, you may not use, copy, modify,
//merge, publish, distribute, sublicense, create a derivative work,
//and/or sell copies of the Software in any work that is designed,
//intended, or marketed for pedagogical or instructional purposes
//related to programming, coding, application development, or
//information technology. Permission for such use, copying,
//modification, merger, publication, distribution, sublicensing,
//creation of derivative works, or sale is expressly withheld.

//This project and source code may use libraries or frameworks
//that are released under various Open-Source licenses. Use of
//those libraries and frameworks are governed by their own
//individual licenses.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//DEALINGS IN THE SOFTWARE.
//*/

//using UnityEngine;
////1
//using System;	//for DateTime
////2
//using NotificationSamples;

//public class NotificationManager : MonoBehaviour
//{
//    //public static NotificationManager instance;
//    private GameNotificationsManager manager;
//    public const string ChannelId = "game_channel0";
//    public const string ReminderChannelId = "reminder_channel1";

//    void Awake()
//    {
//        if (!instance)
//            instance = this;

//        manager = GetComponent<GameNotificationsManager>();        
//    }

//    void Start()
//    {
//        var c1 = new GameNotificationChannel(ChannelId, "Game Alerts", "Game notifications");
//        var c3 = new GameNotificationChannel(ReminderChannelId, "Reminders", "Reminder notifications");

//        manager.Initialize(c1, c3);

//        manager.DismissAllNotifications();
//    }

//    void SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null,
//                                 bool reschedule = false, string channelId = null,
//                                 string smallIcon = null, string largeIcon = null)
//    {
//        IGameNotification notification = manager.CreateNotification();
//        if (notification == null)
//        {
//            return;
//        }        

//        notification.Title = title;
//        notification.Body = body;
//        notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : ChannelId;
//        notification.DeliveryTime = deliveryTime;
//        notification.SmallIcon = smallIcon;
//        notification.LargeIcon = largeIcon;

//        if (badgeNumber != null)
//        {
//            notification.BadgeNumber = badgeNumber;
//        }

//        PendingNotification notificationToDisplay = manager.ScheduleNotification(notification);     
//        notificationToDisplay.Reschedule = reschedule;
//    }

//    private string retentionIconName = "icon_small";
//    private string retentionLargeIconName = "icon_large";

//    private void ScheduleNotification(DateTime date)
//    {
//        string title = "I Miss You!";
//        string body = string.Concat("Come to PaPa , It Doesnt Hurt ...");
//        DateTime deliverTime = date;
//        string channel = ReminderChannelId;

//        SendNotification(title, body, deliverTime, channelId: channel, smallIcon: retentionIconName, largeIcon: retentionLargeIconName);
//    }

//    public void ScheduleCurrentNotifications(DateTime date)
//    {
//        if (date.Date == DateTime.Today.Date && date.TimeOfDay < DateTime.Now.TimeOfDay)
//            return;

//        manager.DismissAllNotifications();
//        manager.CancelAllNotifications();
//        manager.Platform.CancelAllScheduledNotifications();        
//        ScheduleNotification(date);
//    }
//}
