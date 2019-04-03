using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnAwake : MonoBehaviour
{
    public GameObject[] objs;
    
    private void Awake()
    {
        for (int i = 0; i < objs.Length; i++)
            objs[i].SetActive(true);
    }
}
