using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointTxtManager : MonoBehaviour {

    public ObjectPool txtPool;
    public GameObject txtObj;

    public static PointTxtManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        for (int i = 0; i < 500; i++)
        {
            SpawnTxt();
        }
    }

    public void SpawnTxt()
    {
        GameObject obj = Instantiate(txtObj, Vector3.zero, new Quaternion(0, 0, 0, 0));
        obj.transform.SetParent(transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        txtPool.EnqueueObjectPool(obj);
    }

    /// <summary>
    /// Text On
    /// </summary>
    /// <param name="cut">EX:"N2"</param>
    /// <returns></returns>
    public Text TxtOn(Vector2 pos, float value, string cut = null)
    {
        GameObject obj = txtPool.DequeueObjectPool();
        Text txt = obj ? obj.GetComponent<Text>() : null;
        if (txt)
        {
            txt.transform.position = pos;
            txt.gameObject.SetActive(true);
            if (value < 0)
                txt.text = value.ToString(cut);
            else
                txt.text = "+" + value.ToString(cut);
        }
        return txt;
    }

    public Text TxtOn(Transform tf, float value, Vector2 offset, string cut = null)
    {
        return TxtOn((Vector2)tf.position + offset, value, cut);
    }

    public Text TxtOn(Vector2 pos, float value, Color color, string cut = null)
    {
        Text txt = TxtOn(pos, value, cut);
        txt.color = color;
        return txt;
    }

    public Text TxtOn(Transform tf, float value, Color color, string cut = null)
    {
        return TxtOn(tf.position, value, color, cut);
    }

    public Text TxtOn(Transform tf, float value, Color color, Vector2 offset, string cut = null)
    {
        return TxtOn((Vector2)tf.position + offset, value, color, cut);
    }

    public Text TxtOnHead(float value, Transform tf, Color color)
    {
        Text txt = TxtOn(tf, value, color, "##0.##");
        if (txt)
        {
            StartCoroutine(Shoot(txt, 0.75f));
            StartCoroutine(AlphaDown(txt, 0.3f, 3));
            //StartCoroutine(MoveUp(txt, 0.5f, 0.01f));
        }
        return txt;
    }

    public void TxtOnSoul(float value, Transform tf, Vector2 offset)
    {
        Text txt = TxtOn(tf, value, Color.cyan, offset);
        if (txt)
        {
            StartCoroutine(MoveUp(txt, 1f, 0.03f));
            StartCoroutine(AlphaDown(txt, 0.3f, 3));
        }
    }

    private IEnumerator MoveUp(Text txt, float time, float speed)
    {
        float amount = 0.05f;
        for (float t = 0; t < time; t += amount)
        {
            txt.transform.position = new Vector2(txt.transform.position.x, txt.transform.position.y + speed);
            yield return new WaitForSecondsRealtime(amount);
        }
        txtPool.EnqueueObjectPool(txt.gameObject);
    }

    IEnumerator Shoot(Text txt, float time)
    {
        float rnd = Random.Range(-1f, 1f) * 0.01f;
        float acel = 0.1f;
        while (time > 0)
        {
            txt.transform.Translate(new Vector2(rnd * acel, 0.01f * acel));
            acel += 0.1f;
            time -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        txtPool.EnqueueObjectPool(txt.gameObject);
    }

    IEnumerator AlphaDown(Text txt, float delay, float speed)
    {
        yield return new WaitForSeconds(delay);
        while (txt.color.a > float.Epsilon)
        {
            yield return new WaitForSeconds(0.1f);
            Color color = txt.color;
            color.a = color.a -= 0.1f * speed;
            txt.color = color;
        }
    }
}
