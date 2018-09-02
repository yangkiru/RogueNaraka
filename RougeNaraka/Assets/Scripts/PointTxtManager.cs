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
        for(int i = 0; i < 50; i++)
        {
            SpawnTxt();
        }

        GameObject obj = Instantiate(txtObj, Vector3.zero, new Quaternion(0, 0, 0, 0));
    }

    public void SpawnTxt()
    {
        GameObject obj = Instantiate(txtObj, Vector3.zero, new Quaternion(0, 0, 0, 0));
        obj.transform.SetParent(transform);
        txtPool.EnqueueObjectPool(obj);
    }

    /// <summary>
    /// Text On
    /// </summary>
    /// <param name="cut">EX:"N2"</param>
    /// <returns></returns>
    public Text TxtOn(Vector2 pos, float value, string cut = null)
    {
        Text txt = txtPool.DequeueObjectPool().GetComponent<Text>();
        txt.transform.position = pos;
        txt.gameObject.SetActive(true);
        if (value < 0)
            txt.text = value.ToString(cut);
        else
            txt.text = "+" + value.ToString(cut);   
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

    public void TxtOnHead(float value, Transform tf, Color color)
    {
        Text txt = TxtOn(tf, value, color, new Vector2(2, 0.5f), "N2");
        StartCoroutine(MoveUp(txt.transform, 0.5f, 0.01f));
    }

    public void TxtOnGold(float value, Transform tf)
    {
        Text txt = TxtOn(tf, value, Color.yellow, new Vector2(0.5f, 0.3f));
        StartCoroutine(MoveUp(txt.transform, 1f, 0.005f));
    }

    public void TxtOnSoul(float value, Transform tf)
    {
        Text txt = TxtOn(tf, value, Color.cyan, new Vector2(0.5f, 0.3f));
        StartCoroutine(MoveUp(txt.transform, 1f, 0.005f));
    }

    private IEnumerator MoveUp(Transform tf, float time, float speed)
    {
        float amount = 0.05f;
        for(float t = 0; t < time; t += amount)
        {
            tf.position = new Vector2(tf.position.x, tf.position.y + speed);
            yield return new WaitForSecondsRealtime(amount);
        }
        txtPool.EnqueueObjectPool(tf.gameObject);
    }
}
