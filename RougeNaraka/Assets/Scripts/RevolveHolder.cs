using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolveHolder : MonoBehaviour {

    public GameObject[] test;
    public int segments;
    public Vector2 radius;
    public Vector2[] points = new Vector2[0];
    public Vector2[] lastPoints;
    public List<GameObject> list = new List<GameObject>();
    public float time;

    [ContextMenu("Test")]
    public void TestFunction()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        for(int i = 0; i < test.Length; i++)
        {
            yield return new WaitForSeconds(1);
            Add(test[i]);
            test[i].SetActive(true);
        }
    }

    [ContextMenu("RemoveFirst")]
    public void RemoveFirst()
    {
        list[0].SetActive(false);
        Remove(list[0]);
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

    public void Add(GameObject obj)
    {
        Increase();
        list.Add(obj);
        obj.transform.SetParent(transform);
        StartCoroutine(AddMove());
    }

    public void Remove(GameObject obj)
    {
        Decrease();
        int position = list.IndexOf(obj);
        list.Remove(obj);
        StartCoroutine(RemoveMove(position));
    }

    private IEnumerator AddMove()
    {
        list[list.Count - 1].transform.localPosition = points[points.Length - 1];//마지막 놈 이동
        float time = 0;
        while(time <= 1f)
        {
            yield return null;
            time += Time.deltaTime;
            for(int i = 0; i < list.Count - 1;i++)//마지막 놈은 이동 제외
            {
                list[i].transform.localPosition = Vector2.Lerp(lastPoints[i], points[i], time * 2f);
            }
        }
    }

    private IEnumerator RemoveMove(int position)
    {
        float time = 0;
        Vector2[] temp = new Vector2[points.Length];
        for (int i = 0, j = 0; i < lastPoints.Length; i++)
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
            for (int i = 0; i < list.Count - 1; i++)//삭제된 놈은 이동 제외
            {
                list[i].transform.localPosition = Vector2.Lerp(temp[i], points[i], time * 2f);
            }
        }
    }
}
