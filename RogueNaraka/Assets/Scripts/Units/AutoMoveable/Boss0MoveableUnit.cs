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

        int rndCount;

        public STATE state;
        public override void Init(UnitData data)
        {
            base.Init(data);
            targetable = unit.targetable;
            leftDelay = 5.0f;
        }

        protected override void AutoMove()
        {
            switch(state)
            {
                case STATE.REST:
                    Rest();
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

        void Rest()
        {
            unit.tackleable.isTackle = false;
            leftDelay = 1;
            state = STATE.RUSH;
        }

        Effect accelEffect;

        void Rush()
        {
            if (targetable && targetable.target)
            {
                Vector2 vec = targetable.target.cachedTransform.position - cashedTransform.position;
                Vector2 destination = (Vector2)cashedTransform.position + vec.normalized * unit.data.moveDistance;

                leftDelay = 5;

                accelEffect = unit.effectable.AddEffect(EFFECT.Accel, 2f, 10);

                AudioManager.instance.PlaySFX("boss0Rush");
                
                moveable.SetDestination(destination, OnRushEnd);
                unit.animator.SetBool("isBeforeAttack", true);
            }
        }

        void OnRushEnd(bool isArrived)
        {
            leftDelay = 0;
            unit.effectable.AddEffect(EFFECT.Stun, 0, 2);
            AudioManager.instance.PlaySFX("weaponUpgrade");
            state = STATE.RANDOM;
            
            unit.animator.SetBool("isBeforeAttack", false);
            if (accelEffect != null)
            {
                accelEffect.Destroy();
                accelEffect = null;
            }
            CameraShake.instance.Shake(0.2f, 0.25f, 0.01f);
            rndCount = 2;
        }

        void Random()
        {
            float distance = this.distance * 0.5f;
            Vector2 dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            moveable.SetDestination((Vector2)cashedTransform.position + dir * distance);
            Debug.Log("Random" + dir);
            if (--rndCount <= 0)
                state = STATE.RETURN;
        }

        void Return()
        {
            moveable.SetDestination(BoardManager.instance.bossPoint, OnReturnEnd);
            leftDelay = 3;
        }

        void OnReturnEnd(bool isArrived)
        {
            state = STATE.REST;
        }

        [System.Serializable]
        public enum STATE
        {
            REST, RUSH, RANDOM, RETURN
        }
    }
}