using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace RogueNaraka.UnityEventTracker
{
    public class EventTracker
    {
        static public void TrackEvent(UnityEvent e, GameObject obj)
        {
            for (int i = 0; i < e.GetPersistentEventCount(); i++)
            {
                Debug.Log(string.Format("Tracker # Name:{0} Method:{1} Target:{2}", obj.name, e.GetPersistentMethodName(i), e.GetPersistentTarget(i)), obj);
            }
        }

        static public void TrackEvent(UnityEvent e, MonoBehaviour mono)
        {
            for (int i = 0; i < e.GetPersistentEventCount(); i++)
            {
                Debug.Log(string.Format("Tracker # Name:{0} Method:{1} Target:{2}", mono.name, e.GetPersistentMethodName(i), e.GetPersistentTarget(i)), mono);
            }
        }

        static public void TrackEvent(UnityEvent e)
        {
            for (int i = 0; i < e.GetPersistentEventCount(); i++)
            {
                Debug.Log(string.Format("Tracker # Method:{0} Target:{1}", e.GetPersistentMethodName(i), e.GetPersistentTarget(i)));
            }
        }
    }
}