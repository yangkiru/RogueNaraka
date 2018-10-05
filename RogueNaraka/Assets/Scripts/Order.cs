using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour {

    public int orderID;
    private int _orderID = -1;
    public SpriteRenderer render;
	// Update is called once per frame
	void Update () {
        if (orderID != _orderID)
        {
            render.sortingLayerID = orderID;
            _orderID = orderID;
        }
	}
}
