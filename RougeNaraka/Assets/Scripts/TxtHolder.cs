using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TxtHolder {

    public List<Text> txts = new List<Text>();
    public int position;

    public void AddTxt(Text txt)
    {
        if (txts.Count >= 5)
        {
            Text old = txts[txts.Count - 1];
            RemoveTxt(old);
        }
        txts.Insert(0, txt);
        position = (position+1) % 5;
    }

    public void RemoveTxt(Text txt)
    {
        txts.Remove(txt);
        if (txts.Count == 0)
            position = 0;
        PointTxtManager.instance.txtPool.EnqueueObjectPool(txt.gameObject);
    }
}
