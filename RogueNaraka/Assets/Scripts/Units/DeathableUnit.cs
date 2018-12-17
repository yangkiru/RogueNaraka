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
            _isDeath = true;
            unit.animator.SetBool("isDeath", true);
        }

        IEnumerator DeathCorou()
        {
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
            if (unit != BoardManager.instance.player)
                BoardManager.instance.unitPool.EnqueueObjectPool(gameObject, true);
            else
                GameManager.instance.OnEnd();
        }

        public void Revive()
        {
            _isDeath = false;
            unit.animator.SetBool("isDeath", false);
        }
    }
}
