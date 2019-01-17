using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orderable : MonoBehaviour
{
    [SerializeField]
    Order order;

    [SerializeField]
    SpriteRenderer render;

    float pos;
    float _pos;

    private void Reset()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void Init(Order order)
    {
        this.order = order;
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
    Top = 100, Mid = 0, Bottom = -100, Floor = -200
}