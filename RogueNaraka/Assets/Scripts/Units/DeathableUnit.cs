﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.AutoMoveable;

namespace RogueNaraka.UnitScripts
{
    public class DeathableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;

        public bool isDeath { get { return _isDeath; } }
        bool _isDeath;

        IEnumerator deathCorou;

        public event System.Action onDeath;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init()
        {
            _isDeath = false;

            if (unit == BoardManager.instance.player)
                onDeath = PlayerOnDeath;
            else if (unit == BoardManager.instance.boss)
                onDeath = BossOnDeath;
            else if (!unit.data.isFriendly)
                onDeath = EnemyOnDeath;
        }

        public void PlayerOnDeath()
        {
            DeathManager.instance.OnDeath();
        }

        public void EnemyOnDeath()
        {
            MoneyManager.instance.AddUnrefinedSoul((int)(unit.data.cost * RageManager.instance.soul));
            LevelUpManager.instance.RequestEndStageCorou();
        }

        public void BossOnDeath()
        {
            Fillable.bossHp.gameObject.SetActive(false);
            //SoulParticle을 생성
            int _soul = (int)(unit.data.cost * RageManager.instance.soul);
            int count = (int)(_soul * 0.25f);
            int diff = _soul - (count * 4);
            for (int i = 0; i < count; i++)
            {
                SoulParticle soulParticle = BoardManager.instance.soulPool.DequeueObjectPool().GetComponent<SoulParticle>();
                int soul = 0;
                if (i < count - 1)
                    soul = 4;
                else
                {
                    soul = 4 + diff;
                }
                soulParticle.Init(unit.cashedTransform.position, soul);
            }
            
            AudioManager.instance.GetRandomMainMusic();
            RageManager.instance.Rage();
            BoardManager.instance.boss = null;
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

        private void OnDisable()
        {
            deathCorou = null;
        }

        IEnumerator DeathCorou()
        {
            _isDeath = true;
            unit.animator.SetBool("isDeath", true);
            unit.collider.enabled = false;
            unit.DisableAll();
            string sfx = unit.data.deathSFX;
            if (sfx.CompareTo(string.Empty) != 0)
            {
                //AudioManager.instance.StartCoroutine(AudioManager.instance.PlaySound(deathSFX, unit.cashedTransform));
                AudioManager.instance.PlaySFX(sfx);
            }
            if(unit.autoMoveable is FollowMoveableUnit)
            {
                unit.followMoveable.OnDeath();
            }
            unit.followable.OnDeath();
            AnimatorStateInfo state;
            do
            {
                yield return null;
                state = unit.animator.GetCurrentAnimatorStateInfo(0);
            } while (state.normalizedTime < 1 || !state.IsName("Death"));

            do
            {
                yield return null;
            } while (state.normalizedTime < 1 && unit.animator.IsInTransition(0));

            if (onDeath != null)
            {
                onDeath.Invoke();
            }

            if (unit.data.isDeathEffect)
            {
                DeathEffectPool.instance.Play(transform);
            }

            if (!unit.data.isCorpse)
                BoardManager.instance.unitPool.EnqueueObjectPool(gameObject, false);
            else
            {
                unit.OnDisable();
                BoardManager.instance.corpses.Add(unit);
            }
            deathCorou = null;
        }

        public void Revive()
        {
            _isDeath = false;
            unit.animator.SetBool("isDeath", false);
        }

        public void StopMusic()
        {
            AudioManager.instance.StopMusic();
        }
    }
}
