using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;
using RogueNaraka.UnitScripts.AutoMoveable;
using RogueNaraka.UnitScripts.Attackable;
using RogueNaraka.ShadowScripts;

namespace RogueNaraka.UnitScripts
{
    public class Unit : MonoBehaviour
    {
        #region field

        public MoveableUnit moveable;

        #region attackable

        public AttackableUnit attackable;
        public StopBeforeAttackableUnit stopBeforeAttackable;
        public StopAfterAttackableUnit stopAfterAttackable;
        public DontStopAttackableUnit dontStopAttackable;

        #endregion

        #region targetable

        public TargetableUnit targetable;
        public EnemyTargetableUnit enemyTargetable;
        public FriendlyTargetableUnit friendlyTargetable;

        #endregion

        #region autoMoveable

        public AutoMoveableUnit autoMoveable;
        public RandomMoveableUnit randomMoveable;
        public RushMoveableUnit rushMoveable;
        public RestRushMoveableUnit restRushMoveable;

        public FollowMoveableUnit followMoveable;
        public Boss0MoveableUnit boss0Moveable;

        #endregion

        #region etc-able

        public DamageableUnit damageable;
        public HpableUnit hpable;
        public MpableUnit mpable;
        public DeathableUnit deathable;
        public EffectableUnit effectable;
        public TimeLimitableUnit timeLimitable;
        public Orderable orderable;
        public FollowableUnit followable;

        #endregion

        #region etc

        public TackleableUnit tackleable;
        public Animator animator;
        public UnitData data;
        public Rigidbody2D rigid;
        public new SpriteRenderer renderer;
        public new Collider2D collider;
        public Transform cachedTransform;
        public Stat stat { get { return data.stat; } }
        public bool isStun;
        bool isDisabled;
        public ShadowController shadow;

        #endregion

        #endregion

        void Reset()
        {
            moveable = GetComponent<MoveableUnit>();
            stopBeforeAttackable = GetComponent<StopBeforeAttackableUnit>();
            stopAfterAttackable = GetComponent<StopAfterAttackableUnit>();
            dontStopAttackable = GetComponent<DontStopAttackableUnit>();

            enemyTargetable = GetComponent<EnemyTargetableUnit>();
            friendlyTargetable = GetComponent<FriendlyTargetableUnit>();
            randomMoveable = GetComponent<RandomMoveableUnit>();
            rushMoveable = GetComponent<RushMoveableUnit>();
            restRushMoveable = GetComponent<RestRushMoveableUnit>();
            followMoveable = GetComponent<FollowMoveableUnit>();
            boss0Moveable = GetComponent<Boss0MoveableUnit>();

            damageable = GetComponent<DamageableUnit>();
            hpable = GetComponent<HpableUnit>();
            mpable = GetComponent<MpableUnit>();
            deathable = GetComponent<DeathableUnit>();
            effectable = GetComponent<EffectableUnit>();
            timeLimitable = GetComponent<TimeLimitableUnit>();
            orderable = GetComponent<Orderable>();
            followable = GetComponent<FollowableUnit>();

            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            renderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
            tackleable = GetComponent<TackleableUnit>();

            cachedTransform = transform;
            shadow = GetComponentInChildren<ShadowController>();
        }

        public void OnDisable()
        {
            if (!Application.isPlaying)
                return;
            if (data.isFriendly)
            {
                if(BoardManager.instance.friendlies.Remove(this))
                    Debug.Log("Friendly Remove:" + name);
                else
                    Debug.Log("Friendly Remove Fail:" + name);
            }
            else
                BoardManager.instance.enemies.Remove(this);

            DisableAll();
            cachedTransform.localScale = Vector3.one;
            isDisabled = false;
        }

        public void SetStat(Stat stat)
        {
            data.stat = (Stat)stat.Clone();
        }

        public void Init(UnitData data)
        {
            //Debug.Log(data.name + " Init");
            this.data = (UnitData)data.Clone();
            name = this.data.name;
            if (this.data.isFriendly)
                gameObject.layer = GameDatabase.friendlyLayer;
            else
                gameObject.layer = GameDatabase.enemyLayer;

            deathable.Init();
            animator.runtimeAnimatorController = this.data.controller;

            moveable.Init(this.data);
            moveable.enabled = true;

            if (attackable)
                attackable.enabled = false;
            if (this.data.weapon >= 0)
            {
                switch (GameDatabase.instance.weapons[this.data.weapon].type)
                {
                    case ATTACK_TYPE.STOP_BEFORE:
                        attackable = stopBeforeAttackable;
                        break;
                    case ATTACK_TYPE.STOP_AFTER:
                        attackable = stopAfterAttackable;
                        break;
                    case ATTACK_TYPE.DONT_STOP:
                        attackable = dontStopAttackable;
                        break;
                }
                attackable.Init(this.data);
                attackable.enabled = true;
            }

            DisableTargetables();
            if (this.data.isFriendly)
                targetable = enemyTargetable;
            else
                targetable = friendlyTargetable;
            targetable.enabled = true;

            DisableAutoMoveables();
            switch(this.data.move)
            {
                case MOVE_TYPE.RANDOM:
                    autoMoveable = randomMoveable;
                    break;
                case MOVE_TYPE.RUSH:
                    autoMoveable = rushMoveable;
                    break;
                case MOVE_TYPE.REST_RUSH:
                    autoMoveable = restRushMoveable;
                    break;
                case MOVE_TYPE.FOLLOW:
                    autoMoveable = followMoveable;
                    break;
                case MOVE_TYPE.BOSS0:
                    autoMoveable = boss0Moveable;
                    break;
                default:
                    autoMoveable = null;
                    break;
            }
            if (autoMoveable)
            {
                autoMoveable.Init(this.data);
                autoMoveable.enabled = true;
            }

            hpable.Init(this.data.stat);
            mpable.Init(this.data.stat);
            hpable.enabled = true;
            mpable.enabled = true;

            timeLimitable.enabled = false;
            timeLimitable.Init(this.data.limitTime);

            followable.Init(this.data);

            orderable.Init(data.order);

            effectable.Init();

            if (this.data.effects != null)
            {
                for (int i = 0; i < this.data.effects.Length; i++)
                    effectable.AddEffect(this.data.effects[i]);
            }

            if (this.data.color == Color.clear)
                renderer.color = Color.white;
            else
                renderer.color = this.data.color;
            collider.isTrigger = false;
            collider.enabled = true;

            tackleable.Init(data);

            //그림자 세팅
            this.shadow.Initialize(this.data.shadowZAngle, this.data.shadowXFlip, this.data.shadowPos);
        }

        public void Spawn(Vector3 position)
        {
            cachedTransform.position = position;

            gameObject.SetActive(true);
            if (data.isFriendly && !BoardManager.instance.friendlies.Contains(this))
            {
                //Debug.Log("Friendly:" + name);
                BoardManager.instance.friendlies.Add(this);
            }
            else if (!data.isFriendly && !BoardManager.instance.enemies.Contains(this))
            {
                //Debug.Log("Enemy:" + name);
                BoardManager.instance.enemies.Add(this);
            }
            tackleable.OnSpawn();
            if (timeLimitable.time != 0)
                timeLimitable.enabled = true;
            if (hpable.currentHp <= 0)
                deathable.Death();
        }

        public void Teleport(Vector3 position)
        {
            cachedTransform.position = position;
        }

        public void PlaySFX(string name)
        {
            AudioManager.instance.PlaySFX(name);
        }

        void DisableAutoMoveables()
        {
            randomMoveable.enabled = false;
            rushMoveable.enabled = false;
            followMoveable.enabled = false;
        }

        void DisableTargetables()
        {
            enemyTargetable.enabled = false;
            friendlyTargetable.enabled = false;
        }

        public void DisableAll()
        {
            if (isDisabled)
                return;
            isDisabled = true;
            hpable.enabled = false;
            mpable.enabled = false;
            moveable.enabled = false;
            if(autoMoveable) autoMoveable.enabled = false;
            if(attackable) attackable.enabled = false;
            if(targetable) targetable.enabled = false;
        }

        public void Kill(bool isTxt = true)
        {
            if (isTxt)
                damageable.Damage(hpable.currentHp);
            else
                hpable.AddHp(-stat.hp);
        }
    }
}