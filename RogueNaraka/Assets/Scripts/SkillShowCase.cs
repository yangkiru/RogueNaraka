using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillShowCase : MonoBehaviour {
    public RollManager rollManager
    {
        get { return RollManager.instance; }
    }
    
    public int position;

    public bool IsSelected()
    {
        int selected = rollManager.selected;
        int length = rollManager.datas.Length;
        return selected == position || (selected + 1) % length == position || (selected + 2) % length == position;
    }
}
