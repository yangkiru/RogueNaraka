using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;
using RogueNaraka.EffectScripts;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class Boss0MoveableUnit : AutoMoveableUnit
    {
        TargetableUnit targetable;
        
        public STATE state;
        public override void Init(UnitData data)
        {
            base.Init(data);
            targetable = unit.targetable;
        }

        protected override void AutoMove()
        {
            switch(state)
            {
                case STATE.REST:
                    StartCoroutine(Rest());
                    break;
                case STATE.RUSH:
                    Rush();
                    break;
                case STATE.RANDOM:
                    Random();
                    break;
                case STATE.RETURN:
                    Return();
                    break;
            }
        }

        IEnumerator Rest()
        {
            float time = 3;
            Debug.Log("Rest");
            unit.collider.isTrigger = true;
            unit.tackleable.isTackle = false;
            leftDelay = 99999;
            do
            {
                yield return null;
                time -= Time.deltaTime;
            } while (time > 0);
            leftDelay = 0;
            state = STATE.RUSH;
        }

        //float originSpeed;
        Effect accelEffect;

        void Rush()
        {
            if (targetable && targetable.target)
            {
                Vector2 vec = targetable.target.cachedTransform.position - cashedTransform.position;
                Vector2 destination = (Vector2)cashedTransform.position + vec.normalized * 5;
                //unit.tackleable.isTackle = true;
                leftDelay = 99999;

                accelEffect = unit.effectable.AddEffect(EFFECT.Accel, 2f, 10);

                //originSpeed = unit.data.moveSpeed;
                AudioManager.instance.PlaySFX("boss0Rush");
                
                //moveable.SetDestination(destination, OnRushEnd);
                unit.animator.SetBool("isBeforeAttack", true);
            }
        }

        void OnRushEnd(bool isArrived)
        {
            leftDelay = 0;
            //unit.tackleable.isTackle = false;
            unit.effectable.AddEffect(EFFECT.Stun, 0, 2);
            AudioManager.instance.PlaySFX("weaponUpgrade");
            state = STATE.RANDOM;
            StartCoroutine(RandomCorou());
            unit.animator.SetBool("isBeforeAttack", false);
            if(accelEffect != null)
                accelEffect.Destroy();
            CameraShake.instance.Shake(0.2f, 0.25f, 0.01f);
        }

        void Random()
        {
            Vector2 rnd = new Vector2(UnityEngine.Random.Range(-distance, distance), UnityEngine.Random.Range(-distance, distance));
            //moveable.Move((Vector2)cashedTransform.position + rnd);
        }

        IEnumerator RandomCorou()
        {
            float t = 3;
            do
            {
                yield return null;
                t -= Time.deltaTime;
            } while (t > 0);
            state = STATE.RETURN;
        }

        void Return()
        {
            moveable.SetDestination(BoardManager.instance.bossPoint, OnReturnEnd);
            leftDelay = 99999;
        }

        void OnReturnEnd(bool isArrived)
        {
            state = STATE.RUSH;
            leftDelay = 0;
        }

        [System.Serializable]
        public enum STATE
        {
            REST, RUSH, RANDOM, RETURN
        }
    }
}