using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomGUI : MonoBehaviour
{
    public static BottomGUI Instance = null;
    public GameObject Highlight;
    public bool IsMouseEnter;
    
    private void Awake(){
        Instance = this;
    }

    public void HighlightOn(){
        Highlight.SetActive(true);
    }

    public void HighlightOff(){
        Highlight.SetActive(false);
    }

    public void OnMouseEnter(){
        IsMouseEnter = true;
    }

    public void OnMouseExit(){
        IsMouseEnter = false;
    }
}
