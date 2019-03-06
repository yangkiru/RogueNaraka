using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        for (int i = 0; i < 200; i++)
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
    public TextMeshProUGUI TxtOn(Vector2 pos, float value, string cut = null)
    {
        GameObject obj = txtPool.DequeueObjectPool();
        TextMeshProUGUI txt = obj ? obj.GetComponent<TextMeshProUGUI>() : null;
        if (txt)
        {
            txt.transform.position = pos;
            txt.gameObject.SetActive(true);
            if (value < 0)
                txt.text = value.ToString(cut);
            else
                txt.text = string.Format("+{0}",value.ToString(cut));
        }
        return txt;
    }

    public TextMeshProUGUI TxtOn(Transform tf, float value, Vector2 offset, string cut = null)
    {
        return TxtOn((Vector2)tf.position + offset, value, cut);
    }

    public TextMeshProUGUI TxtOn(Vector2 pos, float value, Color color, string cut = null)
    {
        TextMeshProUGUI txt = TxtOn(pos, value, cut);
        if(txt)
            txt.color = color;
        return txt;
    }

    public TextMeshProUGUI TxtOn(Transform tf, float value, Color color, string cut = null)
    {
        return TxtOn(tf.position, value, color, cut);
    }

    public TextMeshProUGUI TxtOn(Transform tf, float value, Color color, Vector2 offset, string cut = null)
    {
        return TxtOn((Vector2)tf.position + offset, value, color, cut);
    }

    public TextMeshProUGUI TxtOnHead(float value, Transform tf, Color color)
    {
        TextMeshProUGUI txt = TxtOn((Vector2)tf.position + new Vector2(0, 0.3f), value, color, "##0.##");
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
        TextMeshProUGUI txt = TxtOn(tf, value, Color.white, offset);
        if (txt)
        {
            StartCoroutine(MoveUp(txt, 3f, 0.01f));
            StartCoroutine(AlphaDown(txt, 1f, 5));
        }
    }

    private IEnumerator MoveUp(TextMeshProUGUI txt, float time, float speed)
    {
        float amount = 0.05f;
        for (float t = 0; t < time; t += amount)
        {
            txt.transform.position = new Vector2(txt.transform.position.x, txt.transform.position.y + speed);
            float _t = 0;
            while (_t < amount)
            {
                _t += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        txtPool.EnqueueObjectPool(txt.gameObject);
    }

    IEnumerator Shoot(TextMeshProUGUI txt, float time)
    {
        float rnd = Random.Range(-0.01f, 0.01f);
        float acel = Random.Range(0.75f, 1f);

        while (time > 0)
        {
            txt.transform.Translate(new Vector2(rnd, 0.01f * acel));
            acel += 0.01f;
            time -= 0.01f;

            float t = 0;
            while (t < 0.01f)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        txtPool.EnqueueObjectPool(txt.gameObject);
    }

    IEnumerator AlphaDown(TextMeshProUGUI txt, float delay, float speed)
    {
        while(delay > 0)
        {
            yield return null;
            delay -= Time.unscaledDeltaTime;
        }

        while (txt.color.a > float.Epsilon)
        {
            float t = 0;
            while (t < 0.1f)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            Color color = txt.color;
            color.a = color.a -= 0.1f * speed;
            txt.color = color;
        }
    }

    IEnumerator Pump(TextMeshProUGUI txt, float time, float power)
    {
        txt.rectTransform.localScale = Vector3.one * power;
        yield return null;
    }
}
