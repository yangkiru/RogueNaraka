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
        for (int i = 0; i < 50; i++)
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

    public Text TxtOnHead(float value, Transform tf, Color color, TxtHolder holder)
    {
        Text txt = TxtOn(tf, value, color, new Vector2(2, 0.5f + holder.position * 0.25f), "N2");
        holder.AddTxt(txt);
        StartCoroutine(MoveUp(txt, 0.5f, 0.01f, holder));
        return txt;
    }
    public Text TxtOnHead(float value, Transform tf, Color color)
    {
        Text txt = TxtOn(tf, value, color, new Vector2(2, 0.5f), "N2");
        StartCoroutine(MoveUp(txt, 0.5f, 0.01f));
        return txt;
    }

    public void TxtOnGold(float value, Transform tf, TxtHolder holder)
    {
        Debug.Log("ScreenSize:" + Screen.width + " " + Screen.height);
        Text txt = TxtOn(tf, value, Color.yellow, new Vector2(0.5f, 0.3f + holder.position * 0.25f));
        holder.AddTxt(txt);
        StartCoroutine(MoveUp(txt, 0.5f, 0.01f, holder));
    }

    public void TxtOnSoul(float value, Transform tf, TxtHolder holder)
    {
        Text txt = TxtOn(tf, value, Color.cyan, new Vector2(0.5f, 0.3f + holder.position * 0.25f));
        holder.AddTxt(txt);
        StartCoroutine(MoveUp(txt, 0.5f, 0.01f, holder));
    }

    private IEnumerator MoveUp(Text txt, float time, float speed, TxtHolder holder = null)
    {
        float amount = 0.05f;
        for(float t = 0; t < time; t += amount)
        {
            txt.transform.position = new Vector2(txt.transform.position.x, txt.transform.position.y + speed);
            yield return new WaitForSecondsRealtime(amount);
        }
        if (holder != null)
            holder.RemoveTxt(txt);
        else
            txtPool.EnqueueObjectPool(txt.gameObject);
    }
}
