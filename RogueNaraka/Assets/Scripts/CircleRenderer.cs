using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CircleRenderer : MonoBehaviour {

    public int segments;
    public Vector2 radius;
    public float startWidth;
    public float endWidth;
    public LineRenderer line;
    public bool isSpin;
    public float spinAmount;

    private void Awake()
    {
        Material result = line.material;
        result.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
        line.material = result;
    }

    [ContextMenu("CreatPoints")]
    public void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        float angle = 20f;
        line.positionCount = segments + 1;
        for (int i=0;i<(segments+1);i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius.x;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius.y;

            line.SetPosition(i, new Vector3(x, y, z));
            angle += (360f / segments);
        }
    }

    public void SetEnable(bool value)
    {
        line.enabled = value;
    }

    public void SetCircle(Vector2 radius, Vector2 pos)
    {
        Move(pos);
        SetCircle(radius);
    }

    public void SetCircle(Vector2 radius)
    {
        this.radius = radius;
        CreatePoints();
    }

    public void SetCircle(float radius)
    {
        SetCircle(new Vector2(radius, radius));
    }
    
    public void SetCircle(float radius, Vector2 pos)
    {
        Move(pos);
        SetCircle(radius);
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void SetWidth(float start, float end)
    {
        startWidth = start;
        endWidth = end;
        SyncWidth();
    }

    public void Init()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        SyncWidth();
        line.enabled = false;
    }

    public void Move(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    public bool GetEnabled()
    {
        return line.enabled;
    }

    [ContextMenu("SyncWidth")]
    public void SyncWidth()
    {
        line.startWidth = startWidth;
        line.endWidth = endWidth;
    }


    public void MoveCircleToMouse()
    {
        Vector3 mp = GameManager.instance.GetMousePosition();
        Move(mp);
    }
}
