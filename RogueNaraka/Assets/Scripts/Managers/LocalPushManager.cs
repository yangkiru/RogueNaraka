using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.NotificationScripts {
    public class LocalPushManager : Singleton<LocalPushManager> {
        public override void Destroy() {
            instance = null;
        }

        private List<AndroidLocalPush> localPushList = new List<AndroidLocalPush>();

        public void Initialize() {
            var channel = new AndroidNotificationChannel() {
                Id = "roguenaraka",
                Name = "roguenaraka",
                Importance = Importance.High,
                Description = "RogueNaraka Local Push"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public void SetLocalPush(string _title, string _text, DateTime _fireTime) {
            var newLocalPush = new AndroidLocalPush(_title, _text, _fireTime);
            this.localPushList.Add(newLocalPush);
            AndroidNotificationCenter.SendNotification(newLocalPush.notification, "roguenaraka");
        }

        public struct AndroidLocalPush {
            public AndroidNotification notification;
            public bool isPushed;
            public DateTime fireTime;

            internal AndroidLocalPush(string _title, string _text, DateTime _fireTime) {
                this.notification = new AndroidNotification(_title, _text, _fireTime);
                this.notification.LargeIcon = "icon_0";
                this.isPushed = false;
                this.fireTime = _fireTime;
            }
        }
    }
}