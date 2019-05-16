using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class Escapeable : MonoBehaviour
{
    public EscapeEvent onEscape;
    bool isQuitting = false;
    public void OnEscape()
    {
        if (onEscape != null)
            onEscape.Invoke();
    }

    private void OnEnable()
    {
        EscapeManager.Instance.Stack.Push(this);
    }

    private void OnDisable()
    {
        if (!isQuitting && EscapeManager.Instance.Stack.Count != 0)
            EscapeManager.Instance.Stack.Pop();
    }

    public void OnClick(Button btn)
    {
        if (btn.onClick != null)
            btn.onClick.Invoke();
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
