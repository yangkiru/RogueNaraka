using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBuyPnl : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI txt;
    public Button btn;
    public SkillData data;

    public void Init(SkillData dt)
    {
        data = (SkillData)dt.Clone();
        img.sprite = data.spr;
        int lang = (int)GameManager.language;
        string name = data.nameLang?[lang];
        txt.text = name != null ? name : dt.name;
    }
}
