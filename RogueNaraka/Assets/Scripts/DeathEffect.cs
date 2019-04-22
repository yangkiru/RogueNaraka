using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour {

    public Animator animator;

    public Transform cachedTransform;
    Unit unit;

    float speed;

    Vector2 move;

    private void Awake()
    {
        cachedTransform = transform;
    }

    public void Init(Vector2 position, Vector2 center, float speed, Unit unit = null)
    {
        if (unit)
        {
            this.unit = unit;
            animator.runtimeAnimatorController = !unit.data.deathEffectController ?
                DeathEffectManager.instance.baseController : unit.data.deathEffectController;
            if (animator.runtimeAnimatorController.animationClips[0].name.CompareTo("Empty") == 0)
                return;
        }
        cachedTransform.position = position;
        this.speed = speed;
        move = ((Vector2)cachedTransform.position - center).normalized * speed;
        gameObject.SetActive(true);
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
    }

    private void Update()
    {
        
        cachedTransform.Translate(move * Time.deltaTime);

        

    }

    /// <summary>
    /// Animation에서 호출
    /// </summary>
    public void EnqueueToPool()
    {
        unit = null;
        speed = 0;
        move = Vector2.zero;
        DeathEffectManager.instance.pool.EnqueueObjectPool(this.gameObject);
    }
}
