using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Unit : MonoBehaviour {

    public bool attackable;

    public Stat stat
    { get { return _stat; } }
    [SerializeField][ReadOnly]
    protected Stat _stat;
    public Stat maxStat
    { get { return _maxStat; } }
    [SerializeField][ReadOnly]
    protected Stat _maxStat;

    public PolyNav.PolyNavAgent agent;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigid;

    public List<Effect> effects
    { get { return _effects; } }
    protected List<Effect> _effects = new List<Effect>();

    protected int cost;
    protected int id;
    [SerializeField][ReadOnly]
    protected float attackDistance;
    protected float moveTime;
    protected float moveDistance;
    protected float minDistance;
    protected float maxDistance;

    public float health
    { get { return _health; } }
    public float mana
    { get { return _mana; } }

    [SerializeField][ReadOnly]
    protected float _health;
    [SerializeField][ReadOnly]
    protected float _mana;

    protected bool isAttackCool = false;
    [SerializeField][ReadOnly]
    protected bool isFriendly;
    protected bool isStun;
    public bool isDeath
    { get { return _isDeath; } }
    [SerializeField][ReadOnly]
    protected bool _isDeath;
    protected bool isPause
    { get { return gameManager.isPause; } }

    /// <summary>
    /// 자동으로 움직이는 것을 멈출지
    /// </summary>
    public bool isAutoMove
    { get { return _isAutoMove; } set { _isAutoMove = value; } }
    protected bool _isAutoMove;

    public Weapon weapon
    { get { return _weapon; } }
    [SerializeField][ReadOnly]
    protected Weapon _weapon;

    protected MOVE_TYPE move;


    private Vector2 lastVelocity;
    private float minSpeed = 0.1f;//For Animator Min speed

    protected GameManager gameManager
    { get { return GameManager.instance; } }
    protected BoardManager boardManager
    { get { return BoardManager.instance; } }

    [SerializeField][ReadOnly]
    protected Unit target = null;
    [SerializeField][ReadOnly]
    protected Vector2 targetPosition;

    public float targetDistance
    { get { return _targetDistance; } }
    [SerializeField][ReadOnly]
    protected float _targetDistance = 1000;

    [SerializeField][ReadOnly]
    protected float attackCoolTime;

    /// <summary>
    /// 텍스트가 안 겹치게 하는 홀더
    /// </summary>
    protected TxtHolder txtHolder = new TxtHolder();

    protected UnityEngine.UI.Text[] texts = new UnityEngine.UI.Text[5];

    //upadate target, targetPosition
    public abstract bool SetTarget();

    protected virtual void OnEnable()
    {
        _targetDistance = 1000;
        StartCoroutine(AttackCool());
        StartCoroutine(DistanceCoroutine());
        StartCoroutine(Regen());
    }

    /// <summary>
    /// Enemy 전용
    /// </summary>
    /// <param name="data"></param>
    public void SyncData(UnitData data)
    {
        _isDeath = false;
        id = data.id;
        cost = data.cost;
        name = data.name;
        SyncData(data.stat, data.isFriendly);
        animator.runtimeAnimatorController = data.controller;
        spriteRenderer.color = data.color;
        move = data.move;//Enemy Only
        moveTime = data.moveTime;//Enemy Only
        moveDistance = data.moveDistance;//Enemy Only
        minDistance = data.minDistance;//Enemy Only
        maxDistance = data.maxDistance;//Enemy Only
        _targetDistance = 1000;

        if (data.weapon.startBulletId.Length != 0)
            EquipWeapon(data.weapon);

        _isAutoMove = true;
    }

    public void Move(Vector2 pos)
    {
        agent.SetDestination(pos);
    }

    /// <summary>
    /// Player전용
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="isFriendly"></param>
    public void SyncData(Stat stat, bool isFriendly = false, bool isHealthMana = true)
    {
        GetComponent<Collider2D>().enabled = true;
        _stat = new Stat(stat);
        if (isHealthMana)
        {
            _health = _stat.hp;
            _mana = _stat.mp;
        }
        SyncData();
        this.isFriendly = isFriendly;
    }

    public void SyncData()
    {
        SyncAttackCool();
        SyncSpeed();
    }

    public void TxtOnHead(float amount, Transform transform, Color color)
    {
        PointTxtManager.instance.TxtOnHead(amount, transform, color, txtHolder);
    }

    public void SetHealth(float value)
    {
        _health = value;
    }

    public void AddHealth(float amount)
    {
        if (_health + amount < stat.hp)
            _health += amount;
        else
        {
            float remain = _health + amount - stat.hp;
            _health = stat.hp;
        }
    }

    public void SetMana(float value)
    {
        _mana = value;
    }

    public void AddMana(float amount)
    {
        if (_mana + amount < stat.mp)
            _mana += amount;
        else
        {
            float remain = _mana + amount - stat.mp;
            _mana = stat.mp;
        }
    }

    public void HealMana(float amount)
    {
        if (_health + amount > stat.hp)
            amount = stat.hp - _mana;
        AddMana(amount);
        TxtOnHead(amount, transform, Color.blue);
    }

    public bool UseMana(float amount)
    {
        if (_mana - amount >= 0)
        {
            AddMana(-amount);
            TxtOnHead(-amount, transform, Color.blue);
            return true;
        }
        else
            return false;
    }

    protected void CheckDeath()
    {
        if(!isDeath && _health <= 0)
        {
            _isDeath = true;
            agent.Stop();
            GetComponent<Collider2D>().enabled = false;
            OnCheckDeath();
            StartCoroutine(WaitForDeath());
        }
    }

    protected virtual void OnCheckDeath(){ }

    protected IEnumerator WaitForDeath()
    {
        //Debug.Log(name +"Death");
        AnimatorStateInfo animationState;
        AnimatorClipInfo[] clips;
        do
        {
            yield return null;
            animationState = animator.GetCurrentAnimatorStateInfo(0);
            clips = animator.GetCurrentAnimatorClipInfo(0);
        } while (!animationState.IsName("Death"));
        
        float time = clips[0].clip.length * animationState.speed;
        //Debug.Log(clips[0].clip.name + " " + clips[0].clip.length + "*" + animationState.speed + "=" + time);
        yield return new WaitForSeconds(time);
        OnDeath();
    }

    /// <summary>
    /// 죽음처리하고 죽음 애니메이션을 재생하고 호출하는 함수
    /// WaitForDeath에서 호출 함
    /// Player에서는 GameManager의 OnEnd()호출, Enemy에서는 오브젝트 풀 인큐
    /// </summary>
    protected abstract void OnDeath();

    public void EquipWeapon(int index, int level)
    {
        _weapon = GameDatabase.instance.weapons[index];
        Debug.Log(name + " Equiped " + _weapon.name);
        _weapon.level = level;
    }

    public void EquipWeapon(Weapon w)
    {
        _weapon = w;
        //Debug.Log(name + " Equiped " + _weapon.name);
    }

    private IEnumerator DistanceCoroutine()
    {
        while(true)
        {
            if(target != null)
                _targetDistance = Vector2.Distance((Vector2)transform.position, targetPosition);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetStat(Stat s)
    {
        _stat = s;
    }

    public void SetStat(STAT type, float value)
    {
        switch (type)
        {
            case STAT.DMG:
                _stat.dmg = value;
                break;
            case STAT.SPD:
                _stat.spd = value;
                break;
            case STAT.LUCK:
                _stat.luck = value;
                break;
            case STAT.HP:
                _stat.hp = value;
                break;
            case STAT.MP:
                _stat.mp = value;
                break;
            case STAT.HPREGEN:
                _stat.hpRegen = value;
                break;
            case STAT.MPREGEN:
                _stat.mpRegen = value;
                break;
        }
    }

    public bool AddStat(STAT type, float amount)
    {
        switch (type)
        {
            case STAT.DMG:
                if (_stat.dmg + amount <= _maxStat.dmg)
                {
                    _stat.dmg += amount;
                    return true;
                }
                else return false;
            case STAT.SPD:
                if (_stat.spd + amount <= _maxStat.spd)
                {
                    _stat.spd += amount;
                    return true;
                }
                else return false;
            case STAT.LUCK:
                if (_stat.luck + amount <= _maxStat.luck)
                {
                    _stat.luck += amount;
                    return true;
                }
                else return false;
            case STAT.HP:
                if (_stat.hp + amount <= _maxStat.hp)
                {
                    _stat.hp += amount;
                    _health += amount;
                    return true;
                }
                else return false;
            case STAT.MP:
                if (_stat.mp + amount <= _maxStat.mp)
                {
                    _stat.mp += amount;
                    _mana += amount;
                    return true;
                }
                else return false;
            case STAT.HPREGEN:
                if (_stat.hpRegen + amount <= _maxStat.hpRegen)
                {
                    _stat.hpRegen += amount;
                    return true;
                }
                else return false;
            case STAT.MPREGEN:
                if (_stat.mpRegen + amount <= _maxStat.mpRegen)
                {
                    _stat.mpRegen += amount;
                    return true;
                }
                else return false;
        }
        return false;
    }

    public void SetMaxStat(Stat s)
    {
        _maxStat = s;
    }

    public void SetMaxStat(STAT type, float value)
    {
        switch (type)
        {
            case STAT.DMG:
                _maxStat.dmg = value;
                break;
            case STAT.SPD:
                _maxStat.spd = value;
                break;
            case STAT.LUCK:
                _maxStat.luck = value;
                break;
            case STAT.HP:
                _maxStat.hp = value;
                break;
            case STAT.MP:
                _maxStat.mp = value;
                break;
            case STAT.HPREGEN:
                _maxStat.hpRegen = value;
                break;
            case STAT.MPREGEN:
                _maxStat.mpRegen = value;
                break;
        }
    }

    public void AddMaxStat(STAT type, float amount)
    {
        switch (type)
        {
            case STAT.DMG:
                _maxStat.dmg += amount;
                break;
            case STAT.SPD:
                _maxStat.spd += amount;
                break;
            case STAT.LUCK:
                _maxStat.luck += amount;
                break;
            case STAT.HP:
                _maxStat.hp += amount;
                break;
            case STAT.MP:
                _maxStat.mp += amount;
                break;
            case STAT.HPREGEN:
                _maxStat.hpRegen += amount;
                break;
            case STAT.MPREGEN:
                _maxStat.mpRegen += amount;
                break;
        }
    }

    protected virtual void Attack()
    {
        if (attackable && target)
        {
            int bulletId = _weapon.startBulletId[_weapon.level];

            SyncData();

            if (_weapon.type == ATTACK_TYPE.CLOSE)
            {
                attackDistance = GameDatabase.instance.bullets[bulletId].size + _weapon.spawnPoint.y;
            }
            else
            {
                attackDistance = 1000;
            }
            if (!isAttackCool && _targetDistance <= attackDistance)
            {
                //Debug.Log(name + "Attacking");
                isAttackCool = true;
                Bullet bullet = boardManager.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
                if (!bullet)
                    bullet = boardManager.SpawnBulletObj().GetComponent<Bullet>();
                bullet.Init(bulletId, stat.dmg, isFriendly);
                bullet.gameObject.SetActive(true);
                Vector2 vec = (targetPosition - (Vector2)transform.position).normalized;
                bullet.Attack(transform.position, weapon.spawnPoint, vec, weapon.localSpeed, weapon.worldSpeed);
            }
            //else
                //Debug.Log("targetDistance : " + targetDistance + " attackDistance : " + attackDistance);
        }
    }

    protected virtual IEnumerator AttackCool()
    {
        while(true)
        {
            if (isAttackCool && !isStun)
            {
                yield return new WaitForSeconds(attackCoolTime);
                isAttackCool = false;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 데미지 입는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void GetDamage(float damage)
    {
        _health -= damage;
        TxtOnHead(-damage, transform, Color.red);
    }

    public void HealHealth(float amount)
    {
        if (_health + amount > stat.hp)
            amount = stat.hp - _health;
        AddHealth(amount);
        TxtOnHead(amount, transform, Color.green);
    }

    public void MoveToAttack()
    {
        if (!isStun && _isAutoMove)
        {
            switch (_weapon.type)
            {
                case ATTACK_TYPE.CLOSE:
                    Vector2 vec = (targetPosition - (Vector2)transform.position).normalized * attackDistance * 0.6f;
                    Move(targetPosition - vec);
                    break;
                case ATTACK_TYPE.TARGET:
                    break;
                case ATTACK_TYPE.NONTARGET:
                    break;
            }
        }
    }
    //상태이상
    public void KnockBack(Vector2 vec, float amount = 1)
    {
        AddEffect(EFFECT.KNOCKBACK, MathHelpers.Vector2ToDegree(vec), amount);
    }

    protected void KnockBackFunc()
    {
        Effect[] knockBacks = GetEffects(EFFECT.KNOCKBACK);
        for(int i = 0; i < knockBacks.Length; i++)
        {
            Vector2 vec = MathHelpers.DegreeToVector2(knockBacks[i].data.value);
            rigid.AddForce(vec.normalized * knockBacks[i].data.time * 10);
            //Debug.Log("angle:" + knockBacks[i].data.value + " vec:" + vec.x + "," + vec.y);
        }

        for (int i = knockBacks.Length - 1; i >= 0; i--)
        {
            RemoveEffect(knockBacks[i]);
        }
    }

    public void Stun(float time)
    {
        AddEffect(EFFECT.STUN, 0, time);
    }

    protected void StunFunc()
    {
        Effect[] stuns = GetEffects(EFFECT.STUN);
        Effect highest = null;
        if (stuns.Length > 0)
            highest = stuns[0];
        for (int i = 1; i < stuns.Length; i++)
        {
            if (highest.data.time < stuns[i].data.time)
                highest = stuns[i];
            else
                RemoveEffect(stuns[i]);
        }

        if (highest != null)
        {
            agent.Stop();
            isStun = true;
        }
        else if(isStun)
        {
            OnStunEnd();
            isStun = false;
        }
    }

    protected virtual void OnStunEnd() { }
    //상태이상

    public void AddEffect(EffectData data)
    {
        Effect effect = gameObject.AddComponent<Effect>();
        effect.SetData(data);
        effect.Active(true);
        _effects.Add(effect);
    }

    public void AddEffect(EFFECT type, float value, float time, bool isInfinity = false)
    {
        Effect effect = gameObject.AddComponent<Effect>();
        effect.SetData(new EffectData(type, value, time, isInfinity));
        effect.Active(true);
        _effects.Add(effect);
    }

    public void AddEffect(EffectData[] datas)
    {
        if (datas != null)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                AddEffect(datas[i]);
            }
        }
    }

    public void RemoveEffect(Effect ef)
    {
        _effects.Remove(ef);
        ef.DestroySelf();
    }

    public void RemoveDebuff()
    {
        for(int i = 0; i < _effects.Count; i++)
        {
            if(!_effects[i].isBuff)
            {
                Effect temp = _effects[i];
                _effects.RemoveAt(i);
                temp.DestroySelf();
                i--;
            }
        }
    }

    public Effect GetEffect(EFFECT type)
    {
        Effect result = null;
        int count = 0;
        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i].data.type == type)
            {
                result = _effects[i];
                count++;
            }
        }
        if (count > 1)
            Debug.Log("There are many Effects. Plz use GetEffects(string name)");
        return result;
    }

    public Effect[] GetEffects(EFFECT type)
    {
        List<Effect> list = new List<Effect>();
        for(int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i].data.type == type)
                list.Add(_effects[i]);
        }

        return list.ToArray();
    }

    protected virtual void Animation()
    {
        if (isDeath)
        {
            animator.SetBool("isDeath", true);
            return;
        }
        else
            animator.SetBool("isDeath", false);
        Vector2 velocity = agent.velocity;
        if (!isStun)
        {
            if (velocity.x >= minSpeed || velocity.x <= -minSpeed || velocity.y >= minSpeed || velocity.x <= -minSpeed)
            {
                animator.SetBool("isWalk", true);
                lastVelocity = velocity;
                animator.SetFloat("x", velocity.x);
                animator.SetFloat("y", velocity.y);
            }
            else
            {
                animator.SetBool("isWalk", false);
                if (isAttackCool)
                {
                    animator.SetFloat("x", targetPosition.x - transform.position.x);
                    animator.SetFloat("y", targetPosition.y - transform.position.y);
                }
                else
                {
                    animator.SetFloat("x", lastVelocity.x);
                    animator.SetFloat("y", lastVelocity.y);
                }
            }
        }
    }

    public void SetAttackCool(float value)
    {
        attackCoolTime = value;
    }

    public float GetAttackCool()
    {
        return attackCoolTime;
    }

    public float GetOriginAttackCool()
    {
        return attackCoolTime;
    }

    public void SyncAttackCool()
    {
        attackCoolTime = 1 - stat.spd * 0.01f;
    }

    public void SetSpeed(float value)
    {
        agent.maxSpeed = value;
    }

    public float GetOriginSpeed()
    {
        return 1 + stat.spd * 0.01f;
    }

    public float GetSpeed()
    {
        return agent.maxSpeed;
    }

    public void SyncSpeed()
    {
        SetSpeed(1 + stat.spd * 0.01f);
    }

    protected IEnumerator Regen()
    {
        float time = 1f;
        while(true)
        {
            yield return new WaitForSeconds(time);
            if (_health > 0)
            {
                AddHealth(stat.hpRegen * 0.1f);
                AddMana(stat.mpRegen * 0.1f);
            }
        }
    }
    [ContextMenu("Suicide")]
    public void Suicide()
    {
        GetDamage(_health);
    }
}
