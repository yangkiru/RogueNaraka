using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

    public bool reset;
    public bool setStage;
    public int stage;
    private void OnValidate()
    {
        if (reset)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Reset");
            reset = false;
        }
        if (setStage)
        {
            setStage = false;
            stage = 0;
            PlayerPrefs.SetInt("stage", stage);
        }
    }
}
