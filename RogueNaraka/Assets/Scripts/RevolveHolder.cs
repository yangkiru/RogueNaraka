using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolveHolder : MonoBehaviour {

    public int segments
    {
        get { return _segments; } set { _segments = value; }
    }
    public int _segments;
    public Vector2 radius;
    public Vector2[] points;
    public Vector2[] lastPoints;
    public List<GameObject> list = new List<GameObject>();
    public bool spin;
    private float speed = 180;
    private float time = 0;
    private int added = 0;
    private bool isAdding = false;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if(spin)
            transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
    }

    public void Init()
    {
        transform.localPosition = Vector3.zero;
        segments = 0;
        points = new Vector2[0];
        lastPoints = null;
        if(list.Count > 0)
        {
            int count = list.Count;
            for(int i = 0; i < count; i++)
            {
                list[i].SetActive(false);
            }
        }
        time = 0;
        speed = 180;
        spin = true;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    [ContextMenu("RemoveAll")]
    public void RemoveAll(float time)
    {
        StartCoroutine(RemoveAllCoroutine(time));
    }

    IEnumerator RemoveAllCoroutine(float t)
    {
        int count = list.Count;
        float time = t / count;
        for (int i = count-1; i >= 0; i--)
        {
            list[i].SetActive(false);
            Remove(list[i]);
            yield return new WaitForSeconds(time);
        }
        Init();
    }
    [ContextMenu("CreatePoints")]
    public void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        float angle = 0f;
        points = new Vector2[segments];
        for (int i = 0; i < (segments); i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius.x;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius.y;

            points[i] = new Vector2(x, y);
            angle += (360f / segments);
        }
    }
    
    public void Increase()
    {
        lastPoints = (Vector2[])points.Clone();
        segments++;
        CreatePoints();
    }

    public void Decrease()
    {
        lastPoints = (Vector2[])points.Clone();
        segments--;
        CreatePoints();
    }

    public void Add(GameObject obj, float rotation = 0)
    {
        Increase();
        list.Add(obj);
        obj.transform.SetParent(transform);
        StartCoroutine(AddMove());
        StartCoroutine(ResetRotation(obj, Quaternion.Euler(0, 0, rotation)));
    }

    public void Remove(GameObject obj)
    {
        int position = list.IndexOf(obj);
        if(list.Remove(obj))
        {
            Decrease();
            StartCoroutine(RemoveMove(position));
        }
    }

    private IEnumerator ResetRotation(GameObject obj, Quaternion rot)
    {
        yield return null;
        obj.transform.localRotation = rot;
    }

    private IEnumerator AddMove()
    {
        float time = 0;
        yield return new WaitForSeconds(0.0001f);
        isAdding = true;
        list[list.Count - 1].transform.localPosition = points[points.Length - 1];//마지막 놈 이동
        while (time <= 1f)
        {
            yield return null;
            time += Time.deltaTime;
            for(int i = 0; i < list.Count - 1;i++)//마지막 놈은 이동 제외
            {
                list[i].transform.localPosition = Vector2.Lerp(lastPoints[i], points[i], time * 2f);
            }
        }
        isAdding = false;
    }

    private IEnumerator RemoveMove(int position)
    {
        float time = 0;
        Vector2[] temp = new Vector2[points.Length];
        for (int i = 0, j = 0; i < lastPoints.Length; i++)//삭제될 놈의 포인트 제외 복사
        {
            if (i != position)
            {
                temp[j] = lastPoints[i];
                j++;
            }
        }
        while (time <= 1f)
        {
            yield return null;
            time += Time.deltaTime;
            for (int i = 0; i < list.Count; i++)//삭제된 놈은 이동 제외
            {
                list[i].transform.localPosition = Vector2.Lerp(temp[i], points[i], time * 2f);
            }
        }
    }
}
