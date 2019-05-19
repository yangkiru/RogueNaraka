using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using RogueNaraka.UnityEventTracker;
namespace RogueNaraka.Escapeable
{
    public class Escapeable : MonoBehaviour
    {
        public EscapeEvent onEscape;

        bool isQuitting = false;
        public void OnEscape()
        {
            if (onEscape != null)
            {
                //EventTracker.TrackEvent(onEscape, this.gameObject);
                onEscape.Invoke();
            }
        }

        private void OnEnable()
        {
            EscapeManager.Instance.Stack.Push(this);
        }

        private void OnDisable()
        {
            if (!isQuitting && EscapeManager.Instance.Stack.Count != 0)
            {
                if (EscapeManager.Instance.Stack.Peek() == this)
                    EscapeManager.Instance.Stack.Pop();
                else
                    EscapeManager.Instance.Stack.Remove(this);
            }
            
        }

        public void OnClick(Button btn)
        {
            if (btn.onClick != null)
                btn.onClick.Invoke();
        }

        public void ClearOnEscape(Escapeable escapeable)
        {
            escapeable.onEscape.RemoveAllListeners();
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        [Serializable]
        public class EscapeEvent : UnityEvent
        {
        }
    }
}