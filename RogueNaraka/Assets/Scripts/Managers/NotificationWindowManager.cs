using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RogueNaraka.SingletonPattern;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.NotificationScripts {
    public class NotificationWindowManager : MonoSingleton<NotificationWindowManager> {
        const float START_POS_Y = 120.0f;
        const float END_POS_Y = -10.0f;
        const float MOVE_SPEED = 150.0f;

        const float TEXT_POS_X_IF_IS_IMAGE = 40.0f;
        const float TEXT_POS_X_IF_IS_NOT_IMAGE = 0.0f;

        public RectTransform WindowRectTransform;
        public Image WindowImage;
        public TextMeshProUGUI WindowText;
        public GameObject HighScoreBanner;

        private Queue<NotificationData> notificationDataQueue = new Queue<NotificationData>();
        private bool isPlaying;

        void Start() {
            #if !UNITY_EDITOR
                LocalPushManager.Instance.Initialize();
            #endif
        }

        void LateUpdate() {
            if(!isPlaying && this.notificationDataQueue.Count > 0) {
                StartCoroutine(PlayNotificationWindowCoroutine());
            }
        }

        public void EnqueueNotificationData(string _text, Sprite _sprite = null) {
            this.notificationDataQueue.Enqueue(new NotificationData(_sprite, _text));
        }

        public void ActiveHighScoreBanner() {
            this.HighScoreBanner.SetActive(true);
        }

        private IEnumerator PlayNotificationWindowCoroutine() {
            this.isPlaying = true;
            this.WindowRectTransform.gameObject.SetActive(true);
            WaitForSecondsRealtime waitForSecondsToPauseWindow = new WaitForSecondsRealtime(3.0f);

            NotificationData curNotification;

            while(this.notificationDataQueue.Count != 0) {
                curNotification = this.notificationDataQueue.Dequeue();
                if(curNotification.sprite == null) {
                    this.WindowImage.gameObject.SetActive(false);
                    this.WindowText.rectTransform.anchoredPosition = new Vector2(TEXT_POS_X_IF_IS_NOT_IMAGE, 0.0f);
                } else {
                    this.WindowImage.gameObject.SetActive(true);
                    this.WindowImage.sprite = curNotification.sprite;
                    this.WindowText.rectTransform.anchoredPosition = new Vector2(TEXT_POS_X_IF_IS_IMAGE, 0.0f);
                }
                this.WindowText.text = curNotification.text;

                //Out
                while(this.WindowRectTransform.anchoredPosition.y > END_POS_Y) {
                    this.WindowRectTransform.anchoredPosition += new Vector2(0.0f, -MOVE_SPEED * TimeManager.Instance.UnscaledDeltaTime);
                    yield return null;
                }
                this.WindowRectTransform.anchoredPosition = new Vector2(0.0f, END_POS_Y);

                //Pause
                yield return waitForSecondsToPauseWindow;

                //In
                while(this.WindowRectTransform.anchoredPosition.y < START_POS_Y) {
                    this.WindowRectTransform.anchoredPosition += new Vector2(0.0f, MOVE_SPEED * TimeManager.Instance.UnscaledDeltaTime);
                    yield return null;
                }
                this.WindowRectTransform.anchoredPosition = new Vector2(0.0f, START_POS_Y);
            }

            this.WindowRectTransform.gameObject.SetActive(false);
            this.isPlaying = false;
        }

        private struct NotificationData {
            public Sprite sprite;
            public string text;

            public NotificationData(Sprite _sprite, string _text) {
                this.sprite = _sprite;
                this.text = _text;
            }
        }
    }
}