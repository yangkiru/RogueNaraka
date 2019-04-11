using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper : MonoBehaviour
{
    [SerializeField]
    Transform cachedTransform;

    public bool isMoveToMouse;
    public Vector3 movePos;

    private void Reset()
    {
        cachedTransform = GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        if(isMoveToMouse)
            cachedTransform.position = GameManager.instance.GetMousePosition();
    }

    public void Move()
    {
        cachedTransform.position = movePos;
    }
}
