using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour {

    public Animator animator;
	void OnEnable()
    {
        if (Random.Range(0, 2) == 0)
            animator.SetBool("isSmall", false);
        else
            animator.SetBool("isSmall", true);
    }

    public IEnumerator Stop(ObjectPool pool)
    {
        yield return new WaitForSeconds(2);

        pool.EnqueueObjectPool(gameObject);
    }
}
