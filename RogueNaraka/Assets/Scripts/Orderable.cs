using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orderable : MonoBehaviour
{
    [SerializeField]
    Order order;

    [SerializeField]
    SpriteRenderer render;

    float pos = 0;
    float _pos = 0;

    private void Reset()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void Init(Order order)
    {
        this.order = order;
        render.sortingOrder = (int)order;
    }

    private void Update()
    {
        pos = transform.position.y;
        if (pos != _pos)
            render.sortingOrder = (int)order + (int)(transform.position.y * -10);
        _pos = pos;
    }
}

[System.Serializable]
public enum Order
{
    Ceiling = 200, Top = 100, Mid = 0, Bottom = -100, Floor = -200, Floor_ = -300
}