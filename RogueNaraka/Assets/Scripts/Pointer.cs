using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    public static Pointer instance = null;

    void Awake()
    {
        instance = this;
    }
    public float offset = 1;
	public void SetPointer(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetPosition(Vector2 position)
    {
        position.y += offset;
        transform.position = position;
    }

    public void PositionToMouse()
    {
        Vector2 pos = GameManager.GetMousePosition();
        pos.y += offset;
        transform.position = pos;
    }
}
