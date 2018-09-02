﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleWidthCamera : MonoBehaviour
{

    public int targetWidth = 720;
    public float pixelsToUnits = 32;

    void Update()
    {
        int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);

        Camera.main.orthographicSize = height / pixelsToUnits / 2;
    }
}