using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatedText : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    [TextArea]
    public string[] contents;

    private void Reset()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (contents.Length > (int)GameManager.language)
        {
            text.text = contents[(int)GameManager.language];
        }
        else if(contents.Length > 0)
            text.text = contents[0];
    }
}
