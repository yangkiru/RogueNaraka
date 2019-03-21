using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectToTransform : MonoBehaviour {

    private RectTransform rectTransform = null;
    private void OnEnable()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        Vector2 pos = gameObject.transform.position;  // get the game object position
        Vector2 viewportPoint = GameManager.instance.mainCamera.WorldToViewportPoint(pos);  //convert game object position to VievportPoint

        // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
    }
}
