using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    public static Pointer instance = null;
    public Transform cashedTransform;
    public Vector3 restPosition;
    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    public float offset = 1;
	public void SetPointer(bool value)
    {
        gameObject.SetActive(value);
        if (!value)
            cashedTransform.position = restPosition;
    }

    public void SetPosition(Vector2 position)
    {
        position.y += offset;
        cashedTransform.position = position;
    }

    public void PositionToMouse()
    {
        Vector2 pos = GameManager.instance.GetMousePosition();
        pos.y += offset;
        cashedTransform.position = pos;
    }
}
