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

        const int maxPhase = 7;
        public int phase
        {
            get
            {
                int value = (int)((1 - unit.hpable.currentHp / unit.hpable.maxHp) * maxPhase);
                lastPhase = value;
                return value;
            }
        }
        int lastPhase = -1;

        public STATE state;
        public override void Init(UnitData data)
        {
            base.Init(data);
            targetable = unit.targetable;
            leftDelay = 5.0f;
            state = STATE.REST;
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
                    switch (phase)
                    {
                        case 0: Return(2);
                            break;
                        case 1: Return(1);
                            break;
                        default: Return(0.1f);
                            break;
                    }
                    
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
                Vector2 vec = targetable.target.cachedTransform.position - unit.cachedTransform.position;
                Vector2 destination = (Vector2)unit.cachedTransform.position + vec.normalized * unit.data.moveDistance * 2;

                leftDelay = 5;

                accelEffect = unit.effectable.AddEffect(EFFECT.Accel, 2f, 10);

                AudioManager.instance.PlaySFX("boss0Rush");
                
                moveable.SetDestination(destination, OnRushEnd);
                unit.animator.SetBool("isBeforeAttack", true);
            }
        }

        void OnRushEnd(bool isArrive)
        {
            leftDelay = 0;
            if (isArrive)
            {
                unit.effectable.AddEffect(EFFECT.Stun, 0, 3);
                AudioManager.instance.PlaySFX("weaponUpgrade");
            }
            
            unit.animator.SetBool("isBeforeAttack", false);
            if (accelEffect != null)
            {
                accelEffect.Destroy();
                accelEffect = null;
            }
            CameraShake.instance.Shake(0.2f, 0.25f, 0.01f);
            switch(phase)
            {
                case 0:case 1:case 2:
                    rndCount = 3;
                    state = STATE.RANDOM;
                    break;
                case 3:
                    rndCount = 2;
                    state = STATE.RANDOM;
                    break;
                case 4:
                    rndCount = 1;
                    state = STATE.RANDOM;
                    break;
                case 5:
                    state = STATE.RETURN;
                    break;
                case 6:
                    state = STATE.RUSH;
                    break;
            }
        }

        void Random()
        {
            if (--rndCount <= 0)
                state = STATE.RETURN;
            float distance = this.distance * 0.5f;
            Vector2 dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            moveable.SetDestination((Vector2)unit.cachedTransform.position + dir * distance);
            Debug.Log("Random" + dir);
        }

        void Return(float restTime)
        {
            moveable.SetDestination(BoardManager.instance.bossPoint, OnReturnEnd);
            leftDelay = restTime;
        }

        void OnReturnEnd(bool isArrive)
        {
            state = STATE.REST;
        }

        private void LateUpdate()
        {
            int lastPhase = this.lastPhase;
            int phase = this.phase;
            if (lastPhase != -1 && lastPhase != phase)
            {
                switch(phase)
                {
                    case 6:
                        state = STATE.RUSH;
                        break;
                    default:
                        state = STATE.RETURN;

                        break;
                }
                AudioManager.instance.PlaySFX("boss0Spawn", 1 + phase * 0.1f);
            }
        }

        [System.Serializable]
        public enum STATE
        {
            REST, RUSH, RANDOM, RETURN
        }
    }
}