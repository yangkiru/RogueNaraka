using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class DeathableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;

        public bool isDeath { get { return _isDeath; } }
        bool _isDeath;

        IEnumerator deathCorou;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init()
        {
            _isDeath = false;
        }

        public void Death()
        {
            if (deathCorou == null)
            {
                Debug.Log(name + " death");
                deathCorou = DeathCorou();
                StartCoroutine(deathCorou);
            }
        }

        IEnumerator DeathCorou()
        {
            _isDeath = true;
            unit.animator.SetBool("isDeath", true);
            AnimatorStateInfo state;
            do
            {
                yield return null;
                state = unit.animator.GetCurrentAnimatorStateInfo(0);
            } while (!state.IsName("Death"));

            unit.DisableAll();

            do
            {
                yield return null;
            } while (state.normalizedTime < 1 && unit.animator.IsInTransition(0));

            DeathEffectPool.instance.Play(transform);

            if (unit == BoardManager.instance.player)
                GameManager.instance.StartCoroutine(GameManager.instance.OnEnd());
            else if (!unit.data.isFriendly)
                MoneyManager.instance.AddUnrefinedSoul(unit.data.cost);
            BoardManager.instance.unitPool.EnqueueObjectPool(gameObject, false);
            deathCorou = null;
        }

        public void Revive()
        {
            _isDeath = false;
            unit.animator.SetBool("isDeath", false);
        }
    }
}
