using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMProPage : MonoBehaviour
{
    public TextMeshProUGUI txt;
    private void Reset()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        txt.pageToDisplay = 1;
    }
    public void NextPage()
    {
        txt.pageToDisplay = ((txt.pageToDisplay) % txt.textInfo.pageCount + 1);
    }
}
