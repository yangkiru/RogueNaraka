using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMProPage : MonoBehaviour
{
    public TextMeshProUGUI txt;
    public Button rightBtn;
    public Button leftBtn;

    string lastTxt = string.Empty;
    private void Reset()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        txt.pageToDisplay = 1;
        if (leftBtn)
            leftBtn.gameObject.SetActive(false);
        if (rightBtn)
            rightBtn.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (lastTxt.CompareTo(txt.text) != 0)
        {
            if (rightBtn)
                rightBtn.gameObject.SetActive(txt.textInfo.pageCount > 1);
            if (leftBtn)
                leftBtn.gameObject.SetActive(false);
            lastTxt = txt.text;
        }
    }

    public void NextPage(bool isCycle)
    {
        if(isCycle)
            txt.pageToDisplay = ((txt.pageToDisplay) % txt.textInfo.pageCount + 1);
        else
            txt.pageToDisplay = txt.pageToDisplay + 1;

        if (rightBtn)
            rightBtn.gameObject.SetActive(txt.pageToDisplay != txt.textInfo.pageCount);
        if(leftBtn)
            leftBtn.gameObject.SetActive(txt.pageToDisplay != 1);
    }

    public void PrevPage()
    {
        txt.pageToDisplay = txt.pageToDisplay - 1;
        if (leftBtn)
            leftBtn.gameObject.SetActive(txt.pageToDisplay != 1);
        if (rightBtn)
            rightBtn.gameObject.SetActive(txt.pageToDisplay != txt.textInfo.pageCount);

    }
}
