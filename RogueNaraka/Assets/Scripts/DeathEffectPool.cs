using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectPool : MonoBehaviour
{

    public ObjectPool pool;
    public GameObject effectPrefab;
    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject obj = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity, pool.transform);
            pool.EnqueueObjectPool(obj);
        }
    }

    public void Play(Transform trans)
    {
        //StartCoroutine(PlayCorou(trans));
        int rnd = Random.Range(3, 6);

        for (int i = 0; i < rnd; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            DeathEffect ef = pool.DequeueObjectPool().GetComponent<DeathEffect>();
            //if (i == 0)
            //    ef.animator.SetBool("isSmall", false);
            //else
            //    ef.animator.SetBool("isSmall", true);
            ef.transform.position = trans.position + (Vector3)offset;
            ef.gameObject.SetActive(true);
            ef.StartCoroutine(ef.Stop(pool));
            //yield return new WaitForSeconds(0.1f);
        }
    }

    //IEnumerator PlayCorou(Transform trans)
    //{
        

    //}
}
