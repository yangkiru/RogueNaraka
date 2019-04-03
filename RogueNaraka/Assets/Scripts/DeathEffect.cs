using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour {

    public Animator animator;

    public Transform cachedTransform;
    Unit unit;



    private void Awake()
    {
        cachedTransform = transform;
    }

    public void Init(Unit unit)
    {
        this.unit = unit;
        animator.runtimeAnimatorController = !unit.data.deathEffectController ?
            DeathEffectManager.instance.baseController : unit.data.deathEffectController;
    }

    void OnEnable()
    {
        if (unit == null)
            return;
        AnimatorControllerParameter[] parameters = animator.parameters;

        for (int i = 0; i < parameters.Length; i++)
            if (parameters[i].name.CompareTo("isSmall") == 0)
            {
                if (Random.Range(0, 2) == 0)
                    animator.SetBool("isSmall", false);
                else
                    animator.SetBool("isSmall", true);
                //return;
            }


        moveCorou = MoveCorou(Random.Range(0, 0.3f), unit);
        StartCoroutine(moveCorou);
    }

    IEnumerator moveCorou;

    IEnumerator MoveCorou(float speed, Unit unit)
    {
        Debug.Log(speed);
        Vector2 dir = (cachedTransform.position - unit.cachedTransform.position).normalized;
        Vector2 move = dir * speed;
        while (true)
        {
            yield return null;
            cachedTransform.Translate(move * Time.deltaTime);
        }
    }

    /// <summary>
    /// Animation에서 호출
    /// </summary>
    public void EnqueueToPool()
    {
        if (moveCorou != null)
        {
            StopCoroutine(moveCorou);
            moveCorou = null;
        }
        unit = null;
        DeathEffectManager.instance.pool.EnqueueObjectPool(this.gameObject);
    }
}
