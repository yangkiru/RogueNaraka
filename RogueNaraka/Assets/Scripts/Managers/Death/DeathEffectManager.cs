using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectManager : MonoBehaviour
{

    public ObjectPool pool;
    public GameObject effectPrefab;
    public RuntimeAnimatorController baseController;
    public static DeathEffectManager instance;
    // Use this for initialization
    void Awake()
    {
        instance = this;
        for (int i = 0; i < 200; i++)
        {
            GameObject obj = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity, pool.transform);
            this.pool.EnqueueObjectPool(obj);
        }
    }

    public void Play(Unit unit)
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

            ef.Init(unit.cachedTransform.position + (Vector3)offset, unit.cachedTransform.position, 0.3f, unit);
            //yield return new WaitForSeconds(0.1f);
        }
    }

    //IEnumerator PlayCorou(Transform trans)
    //{
        

    //}
}
