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
                txt.text = string.Format("+{0}",value.ToString(cut));
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
        if(txt)
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
        Text txt = TxtOn(tf, value, Color.white, offset);
        if (txt)
        {
            StartCoroutine(MoveUp(txt, 2f, 0.005f));
            StartCoroutine(AlphaDown(txt, 0.1f, 5));
        }
    }

    private IEnumerator MoveUp(Text txt, float time, float speed)
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

    IEnumerator Shoot(Text txt, float time)
    {
        float rnd = Random.Range(-0.01f, 0.01f);
        float acel = 1f;

        while (time > 0)
        {
            txt.transform.Translate(new Vector2(rnd, 0.01f * acel));
            acel += 0.01f;
            time -= 0.01f;
#if DELAY
            yield return GameManager.instance.delayPointOneReal;
#else
            float t = 0;
            while (t < 0.1f)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
#endif
        }
        txtPool.EnqueueObjectPool(txt.gameObject);
    }

    IEnumerator AlphaDown(Text txt, float delay, float speed)
    {
        yield return new WaitForSeconds(delay);
#if !DELAY
#endif
        while (txt.color.a > float.Epsilon)
        {
#if DELAY
            yield return GameManager.instance.delayPointOneReal;
#else
            float t = 0;
            while (t < 0.1f)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
#endif
            Color color = txt.color;
            color.a = color.a -= 0.1f * speed;
            txt.color = color;
        }
    }
}
