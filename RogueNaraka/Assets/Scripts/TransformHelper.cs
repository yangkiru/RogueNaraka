using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper : MonoBehaviour
{
    [SerializeField]
    Transform cachedTransform;

    public bool isMoveToMouse;
    public Vector3 movePos;

    public Camera cam;

    private void Reset()
    {
        cachedTransform = GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        if(isMoveToMouse){
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = cachedTransform.parent.transform.position.z;
            cachedTransform.position = pos;
        }
    }

    public void Move()
    {
        cachedTransform.position = movePos;
    }
}
