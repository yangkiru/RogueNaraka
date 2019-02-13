using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class AdEventScript : MonoBehaviour
{
    public AdEvent onReward;
    public AdEvent onStart;
}
[Serializable]
public class AdEvent : UnityEvent { }
