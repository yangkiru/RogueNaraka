using UnityEngine;
using System.Collections;

public class Tracker : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log(string.Format("{0} {1}", name, "OnEnabled"));
    }
    void OnDisable()
    {
        Debug.Log(string.Format("{0} {1}", name, "OnDisabled"));
    }
}
