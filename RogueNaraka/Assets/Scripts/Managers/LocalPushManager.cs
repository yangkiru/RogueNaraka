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

        public AndroidLocalPush SetLocalPush(string _title, string _text, DateTime _fireTime) {
            var newLocalPush = new AndroidLocalPush(_title, _text, _fireTime);
            this.localPushList.Add(newLocalPush);
            var id = AndroidNotificationCenter.SendNotification(newLocalPush.notification, "roguenaraka");
            newLocalPush.SetId(id);
            return newLocalPush;
        }

        public void CancelLocalPush(AndroidLocalPush _push) {
            this.localPushList.Remove(_push);
            AndroidNotificationCenter.CancelNotification(_push.id);
        }

        public class AndroidLocalPush {
            public AndroidNotification notification;
            public DateTime fireTime;
            public int id;

            internal AndroidLocalPush(string _title, string _text, DateTime _fireTime) {
                this.notification = new AndroidNotification(_title, _text, _fireTime);
                this.notification.LargeIcon = "icon_0";
                this.fireTime = _fireTime;
                this.id = -1;
            }

            internal void SetId(int _id) {
                this.id = _id;
            }
        }
    }
}