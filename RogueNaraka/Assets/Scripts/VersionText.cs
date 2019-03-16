using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    public TextMeshProUGUI txt;

    private void Awake()
    {
        txt.text = string.Format("{0} ver", Application.version);
    }
}
