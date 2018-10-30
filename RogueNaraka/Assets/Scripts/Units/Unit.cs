using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Unit : MonoBehaviour {

    public bool attackable;

    public Stat maxStat
    { get { return _maxStat; } }
    protected Stat _maxStat;

    ///인스펙터 캐싱
    public PolyNav.PolyNavAgent agent;
    public Animator animator;
    public Shadow shadow;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigid;
    public RevolveHolder revolveHolder;

    public List<Effect> effects
    { get { return _effects; } }
    protected List<Effect> _effects = new List<Effect>();

    [SerializeField]
    protected UnitData _data;
    public UnitData data
    { get { return _data; } }

    protected float attackDistance;

    public float health
    { get { return _health; } }
    public float mana
    { get { return _mana; } }

    protected float _health;
    protected float _mana;

    protected bool isWin;
    protected bool isAttackCool = false;
    protected bool isStun;
    public bool isDeath
    { get { return _isDeath; } }
    protected bool _isDeath;
    protected bool isPause
    { get { return gameManager.isPause; } }

    /// <summary>
    /// 자동으로 움직이는 것을 멈출지
    /// </summary>
    public bool isAutoMove
    { get { return _isAutoMove; } set { //Debug.Log("Before:"+_isAutoMove+" After:"+value);
            _isAutoMove = value; } }
    [SerializeField]
    protected bool _isAutoMove = true;

    public Weapon weapon
    { get { return _weapon; } }
    [SerializeField]
    protected Weapon _weapon;

    private Vector2 lastVelocity;
    private float minSpeed = 0.1f;//For Animator Min speed

    protected GameManager gameManager
    { get { return GameManager.instance; } }
    protected BoardManager boardManager
    { get { return BoardManager.instance; } }

    [SerializeField]
    protected Unit target = null;
    protected Vector2 targetPosition;

    public float targetDistance
    { get { return _targetDistance; } }
    [SerializeField]
    protected float _targetDistance = 1000;
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

    public void SyncData(UnitData data, bool isHealthMana = true)
    {
        _isDeath = false;
        name = data.name;
        animator.runtimeAnimatorController = data.controller;
        spriteRenderer.color = data.color;

        _data = data;
        _targetDistance = 1000;
        isAttackCool = false;

        if (data.weaponId != -1)
            EquipWeapon(data.weaponId, data.weaponLevel);

        isAutoMove = true;
        if(revolveHolder)
        {
            revolveHolder.Init();
        }

        isRandomMoved = false;
        GetComponent<Collider2D>().enabled = true;
        if (isHealthMana)
        {
            _health = data.stat.hp;
            _mana = data.stat.mp;
        }
        shadow.Init();
        SyncData();
    }

    public void SyncData()
    {
        SyncAttackCool();
        SyncSpeed();
    }

    public void Move(Vector2 pos)
    {
        Move(pos, null);
    }

    public void Move(Vector2 pos, Action<bool> callback)
    {
        agent.SetDestination(pos, callback);
    }

    IEnumerator stopMoveCoroutine = null;
    /// <summary>
    /// PolyNavAgent를 강제로 정지시키는 함수
    /// true를 전달하면 코루틴을 탈출함
    /// </summary>
    /// <param name="value"></param>
    public void Move(bool value)
    {
        _isAutoMove = value;
        if(!value && stopMoveCoroutine == null)
        {
            stopMoveCoroutine = StopMove();
            StartCoroutine(stopMoveCoroutine);
        }

    }

    private IEnumerator StopMove()
    {
        while (!_isAutoMove)
        {
            agent.ResetVelocity();
            yield return null;
        }
    }

    public void SetHealth(float value)
    {
        _health = value;
    }

    public void AddHealth(float amount)
    {
        if (_health + amount < _data.stat.hp)
            _health += amount;
        else
        {
            float remain = _health + amount - _data.stat.hp;
            _health = _data.stat.hp;
        }
    }

    public void SetMana(float value)
    {
        _mana = value;
    }

    public void AddMana(float amount)
    {
        if (_mana + amount < _data.stat.mp)
            _mana += amount;
        else
        {
            float remain = _mana + amount - _data.stat.mp;
            _mana = _data.stat.mp;
        }
    }

    public void HealMana(float amount)
    {
        if (_health + amount > _data.stat.hp)
            amount = _data.stat.hp - _mana;
        AddMana(amount);
        PointTxtManager.instance.TxtOnHead(amount, transform, Color.blue);
    }

    public bool UseMana(float amount)
    {
        if (_mana - amount >= 0)
        {
            AddMana(-amount);
            PointTxtManager.instance.TxtOnHead(-amount, transform, Color.blue);
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
    protected virtual void OnDeath()
    {
        if(revolveHolder)
        {
            //int count = revolveHolder.transform.childCount;
            //for (int i = 0; i < count; i++)
            //{
            //    boardManager.bulletPool.EnqueueObjectPool(revolveHolder.transform.GetChild(0).gameObject);
            //}
            if (revolveHolder.transform.childCount > 0)
            {
                revolveHolder.spin = false;
                revolveHolder.RemoveAll(3);
            }
        }
    }

    public void EquipWeapon(int index, int level)
    {
        _weapon = GameDatabase.instance.weapons[index];
        //Debug.Log(name + " Equiped " + _weapon.name);
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
        _data.stat = s;
    }

    public void SetStat(STAT type, float value)
    {
        switch (type)
        {
            case STAT.DMG:
                _data.stat.dmg = value;
                break;
            case STAT.SPD:
                _data.stat.spd = value;
                break;
            case STAT.TEC:
                _data.stat.tec = value;
                break;
            case STAT.HP:
                _data.stat.hp = value;
                break;
            case STAT.MP:
                _data.stat.mp = value;
                break;
            case STAT.HPREGEN:
                _data.stat.hpRegen = value;
                break;
            case STAT.MPREGEN:
                _data.stat.mpRegen = value;
                break;
        }
    }

    public bool AddStat(STAT type, float amount)
    {
        switch (type)
        {
            case STAT.DMG:
                if (_data.stat.dmg + amount <= _maxStat.dmg)
                {
                    _data.stat.dmg += amount;
                    return true;
                }
                else return false;
            case STAT.SPD:
                if (_data.stat.spd + amount <= _maxStat.spd)
                {
                    _data.stat.spd += amount;
                    return true;
                }
                else return false;
            case STAT.TEC:
                if (_data.stat.tec + amount <= _maxStat.tec)
                {
                    _data.stat.tec += amount;
                    return true;
                }
                else return false;
            case STAT.HP:
                if (_data.stat.hp + amount <= _maxStat.hp)
                {
                    _data.stat.hp += amount;
                    _health += amount;
                    return true;
                }
                else return false;
            case STAT.MP:
                if (_data.stat.mp + amount <= _maxStat.mp)
                {
                    _data.stat.mp += amount;
                    _mana += amount;
                    return true;
                }
                else return false;
            case STAT.HPREGEN:
                if (_data.stat.hpRegen + amount <= _maxStat.hpRegen)
                {
                    _data.stat.hpRegen += amount;
                    return true;
                }
                else return false;
            case STAT.MPREGEN:
                if (_data.stat.mpRegen + amount <= _maxStat.mpRegen)
                {
                    _data.stat.mpRegen += amount;
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
            case STAT.TEC:
                _maxStat.tec = value;
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
            case STAT.TEC:
                _maxStat.tec += amount;
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
        if (attackable && target && boardManager.isReady)
        {
            int bulletId = _weapon.startBulletId[_weapon.level];

            SyncData();

            if (_weapon.type == ATTACK_TYPE.CLOSE)
            {
                attackDistance = GameDatabase.instance.bullets[_weapon.startBulletId[_weapon.level]].size + _weapon.spawnPoint.y;
            }
            else if (_weapon.type == ATTACK_TYPE.REVOLVE)
            {
                revolveHolder.gameObject.SetActive(true);
                attackDistance = (revolveHolder.radius.x + revolveHolder.radius.y) / 2f;
            }
            else
            {
                attackDistance = 1000;
            }
            if (!isAttackCool && (_targetDistance <= attackDistance || _weapon.type == ATTACK_TYPE.REVOLVE))
            {
                //Debug.Log(name + "Attacking");

                //bullet.Attack(transform.position, weapon.spawnPoint, vec, weapon.localSpeed, weapon.worldSpeed, holder, this);
                isAttackCool = true;
                StartCoroutine(BeforeAttackCool());
            }
            //else
                //Debug.Log("targetDistance : " + targetDistance + " attackDistance : " + attackDistance);
        }
    }

    protected IEnumerator BeforeAttackCool()
    {
        float t = weapon.beforeAttackDelay;
        while (t > 0)
        {
            yield return null;
            t -= Time.deltaTime;
        }
        Bullet bullet = boardManager.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        if (!bullet)
            bullet = boardManager.SpawnBulletObj().GetComponent<Bullet>();
        bullet.Init(_weapon.startBulletId[_weapon.level], _data.stat.dmg, _data.isFriendly);
        bullet.gameObject.SetActive(true);
        Vector2 vec = (targetPosition - (Vector2)transform.position).normalized;
        RevolveHolder holder = null;
        if (weapon.type == ATTACK_TYPE.REVOLVE)
        {
            holder = revolveHolder;
            attackCoolTime = 1000;
        }
        bullet.Attack(transform.position, weapon.spawnPoint, vec, weapon.localSpeed, weapon.worldSpeed, holder, this);
    }

    protected virtual IEnumerator AttackCool()
    {
        while(true)
        {
            if (isAttackCool && !isStun)
            {
                float t = attackCoolTime;
                while (t > 0)
                {
                    yield return null;
                    if(t < 1000)
                        t -= Time.deltaTime;
                    if (isDeath)
                        break;
                }
                isAttackCool = false;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 데미지 입는 함수
    /// </summary>
    /// <param name="damage"></param>
    public virtual void GetDamage(float damage)
    {
        if (damage > 0)
        {
            _health -= damage;
            PointTxtManager.instance.TxtOnHead(-damage, transform, Color.red, txtHolder);
        }
    }

    public void HealHealth(float amount)
    {
        if (_health + amount > _data.stat.hp)
            amount = _data.stat.hp - _health;
        AddHealth(amount);
        PointTxtManager.instance.TxtOnHead(amount, transform, Color.green, txtHolder);
    }

    public void MoveToAttack()
    {
        if (!isStun && _isAutoMove)
        {
            switch (_weapon.type)
            {
                case ATTACK_TYPE.CLOSE:
                case ATTACK_TYPE.REVOLVE:
                    if (target != null)
                    {
                        Vector2 vec = (targetPosition - (Vector2)transform.position);
                        Move(targetPosition - vec.normalized * attackDistance);
                    }
                    break;
                case ATTACK_TYPE.TARGET:
                    if(!isRandomMoved)
                        RandomMove();
                    break;
                case ATTACK_TYPE.NONTARGET:
                    break;
            }
        }
    }
    [SerializeField]
    private bool isRandomMoved = false;
    private void RandomMove(bool value = true)
    {
        if (!isRandomMoved && !isWin)
            isRandomMoved = true;
        else if (isWin)
            isRandomMoved = false;
        if(!isWin)
            StartCoroutine(RandomMoveCorou());
    }
    private IEnumerator RandomMoveCorou()
    {
        yield return new WaitForSeconds(data.moveDelay);
        if (isAutoMove && !isWin)
        {
            Vector2 rnd = new Vector2(
                UnityEngine.Random.Range(-_data.moveDistance, _data.moveDistance),
                UnityEngine.Random.Range(-_data.moveDistance, _data.moveDistance));
            Move((Vector2)transform.position + rnd.normalized, RandomMove);
        }
        else isRandomMoved = false;
    }
    //상태이상
    public EffectStat effectStat
    {
        get { return _effectStat; }
    }
    private EffectStat _effectStat;

    public void Fire(float damage, float time)
    {
        Effect effect = AddEffect(EFFECT.FIRE, damage, time);
    }

    /// <summary>
    /// AddEffect에서 호출
    /// </summary>
    /// <param name="fire"></param>
    /// <returns></returns>
    protected IEnumerator FireFunc(Effect fire)
    {
        float time = 0;
        while (fire.data.time > 0)
        {
            while (time < 0.5f * (1 + effectStat.fireResistance * 0.01f))
            {
                yield return null;
                time += Time.deltaTime;
                fire.data.time -= Time.deltaTime;
            }
            time = 0;
            GetDamage(fire.data.value);
        }
    }

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

    public Effect AddEffect(EffectData data)
    {
        Effect effect = gameObject.AddComponent<Effect>();
        effect.SetData(data);
        effect.Active(true);
        _effects.Add(effect);
        switch(data.type)
        {
            case EFFECT.FIRE:
                StartCoroutine(FireFunc(effect));
                break;
        }
        return effect;
    }

    public Effect AddEffect(EFFECT type, float value, float time, bool isInfinity = false)
    {
        return AddEffect(new EffectData(type, value, time, isInfinity));
    }

    public Effect[] AddEffect(EffectData[] datas)
    {
        if (datas != null)
        {
            Effect[] _effects = new Effect[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                _effects[i] = AddEffect(datas[i]);
            }
            return _effects;
        }
        return null;
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
            shadow.animator.SetBool("isDeath", true);
            return;
        }
        else
        {
            animator.SetBool("isDeath", false);
            shadow.animator.SetBool("isDeath", false);
        }
        Vector2 velocity = agent.velocity;
        if (!isStun)
        {
            if (velocity.x != 0 || velocity.y != 0)
            {
                animator.SetBool("isWalk", true);
                shadow.animator.SetBool("isWalk", true);
            }
            else
            {
                animator.SetBool("isWalk", false);
                shadow.animator.SetBool("isWalk", false);
            }
            if (target)
            {
                animator.SetFloat("x", targetPosition.x - transform.position.x);
                animator.SetFloat("y", targetPosition.y - transform.position.y);
                shadow.animator.SetFloat("x", targetPosition.x - transform.position.x);
                shadow.animator.SetFloat("y", targetPosition.y - transform.position.y);
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
        attackCoolTime = (weapon.beforeAttackDelay + weapon.afterAttackDelay) * (1 - (_data.stat.spd - 1) * 0.01f);
    }

    public void SetSpeed(float value)
    {
        agent.maxSpeed = value;
    }

    public float GetOriginSpeed()
    {
        return 1 + _data.stat.spd * 0.01f;
    }

    public float GetSpeed()
    {
        return agent.maxSpeed;
    }

    public void SyncSpeed()
    {
        SetSpeed(data.moveSpeed * (1 + ((_data.stat.spd - 1) * 0.1f)));
    }

    protected IEnumerator Regen()
    {
        float time = 1f;
        while(true)
        {
            yield return new WaitForSeconds(time);
            if (_health > 0)
            {
                AddHealth(_data.stat.hpRegen * 0.1f);
                AddMana(_data.stat.mpRegen * 0.1f);
            }
        }
    }
    [ContextMenu("Suicide")]
    public void Suicide()
    {
        GetDamage(_health);
    }

    [ContextMenu("InitShadow")]
    private void InitShadow()
    {
        shadow.Init();
    }
}
