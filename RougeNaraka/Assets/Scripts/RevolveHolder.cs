﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolveHolder : MonoBehaviour {

    public int segments;
    public Vector2 radius;
    public Vector2[] points;
    public Vector2[] lastPoints;
    public List<GameObject> list = new List<GameObject>();
    private float speed = 180;
    private float time = 0;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
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
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    [ContextMenu("RemoveAll")]
    public void RemoveAll()
    {
        StartCoroutine(RemoveAllCoroutine(1));
    }

    IEnumerator RemoveAllCoroutine(float t)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            list[0].SetActive(false);
            Remove(list[0]);
            yield return new WaitForSeconds(t);
        }
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
        Debug.Log("position = " + position);
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
