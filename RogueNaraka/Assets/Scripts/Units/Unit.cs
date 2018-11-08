using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Unit : MonoBehaviour {

    public bool attackable;

    ///인스펙터 캐싱
    public PolyNav.PolyNavAgent agent;
    public Animator animator;
    public Shadow shadow;
    public ParticleSystem particle;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigid;
    public RevolveHolder revolveHolder;
    public Transform effectHolder;
    public List<Effect> effects
    { get { return _effects; } }
    protected List<Effect> _effects = new List<Effect>();

    [SerializeField]
    protected UnitData _data;
    public UnitData data
    { get { return _data; } }

    [SerializeField]
    protected float attackDistance;

    public float health
    { get { return _health; } }
    public float mana
    { get { return _mana; } }

    [SerializeField]
    protected float _health;
    [SerializeField]
    protected float _mana;
    [SerializeField]
    protected float poison;
    [SerializeField]
    protected float slow;
    [SerializeField]
    protected float ice;

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

    public void AddStat(STAT type, float amount)
    {
        SetStat(type, GetStat(type) + amount);
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

    public float GetStat(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return _data.stat.dmg;
            case STAT.SPD:
                return _data.stat.spd;
            case STAT.TEC:
                return _data.stat.tec;
            case STAT.HP:
                return _data.stat.hp;
            case STAT.MP:
                return _data.stat.mp;
            case STAT.HPREGEN:
                return _data.stat.hpRegen;
            case STAT.MPREGEN:
                return _data.stat.mpRegen;
            default:
                Debug.Log("ERROR!");
                return -9999;
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

    //Animator로 전달 할 값들
    protected bool isBeforeAttack;
    protected bool isAfterAttack;
    protected IEnumerator BeforeAttackCool()
    {
        float t = weapon.beforeAttackDelay;
        isBeforeAttack = true;
        while (t > 0)
        {
            yield return null;
            float _ice = ice * 0.1f * KnowledgeData.GetNegative(knowledge.ice);
            t -= Time.deltaTime * (1 - _ice);
        }
        Bullet bullet = boardManager.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        if (!bullet)
            bullet = boardManager.SpawnBulletObj().GetComponent<Bullet>();
        bullet.Init(_weapon.startBulletId[_weapon.level], _data.stat.dmg, _data.isFriendly);
        Vector2 vec = (targetPosition - (Vector2)transform.position).normalized;
        RevolveHolder holder = null;
        if (weapon.type == ATTACK_TYPE.REVOLVE)
        {
            holder = revolveHolder;
            attackCoolTime = 1000;
        }
        AttackBullet(bullet, vec, holder);
        isBeforeAttack = false;
        isAfterAttack = true;
    }

    protected virtual void AttackBullet(Bullet bullet, Vector2 vec, RevolveHolder holder)
    {
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
                    float _ice = ice * 0.1f * KnowledgeData.GetNegative(knowledge.ice);
                    if (t < 1000)
                        t -= Time.deltaTime * (1 - _ice);
                    if (isDeath)
                        break;
                }
                isAttackCool = false;
                isAfterAttack = false;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 데미지 입는 함수
    /// </summary>
    /// <param name="damage"></param>
    public virtual float GetDamage(float damage, bool isTxtOnHead = true)
    {
        if (damage < 0)
            damage = 0;
        float additional = poison * KnowledgeData.GetHalf(knowledge.poison);
        //Debug.Log(name + " knowledge.poison:" + knowledge.poison + " KnowledgeData.GetHalf(knowledge.poison):" + KnowledgeData.GetHalf(knowledge.poison));
        if (additional <= 0)
            additional = 0;
        //else Debug.Log("additional:" + additional);
        float sum = damage + additional;
        _health -= sum;
        if (isTxtOnHead && damage != 0)
            PointTxtManager.instance.TxtOnHead(-sum, transform, Color.white);
        return sum;
    }

    public void HealHealth(float amount)
    {
        if (_health + amount > _data.stat.hp)
            amount = _data.stat.hp - _health;
        AddHealth(amount);
        if(amount > 0)
            PointTxtManager.instance.TxtOnHead(amount, transform, Color.green);
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
                        StartCoroutine(RandomMoveCorou());
                    break;
                case ATTACK_TYPE.NONTARGET:
                    break;
            }
        }
    }
    [SerializeField]
    protected bool isRandomMoved = false;
    private IEnumerator RandomMoveCorou()
    {
        if (isAutoMove && !isWin)
        {
            isRandomMoved = true;
            Vector2 rnd = new Vector2(
                UnityEngine.Random.Range(-_data.moveDistance, _data.moveDistance),
                UnityEngine.Random.Range(-_data.moveDistance, _data.moveDistance));
            Move((Vector2)transform.position + rnd.normalized);
            yield return new WaitForSeconds(data.moveDelay);
            isRandomMoved = false;
        }
    }
    //상태이상 효율
    public KnowledgeData knowledge
    {
        get { return new KnowledgeData(data.knowledge, 1); }
    }

    protected IEnumerator SlowFunc(Effect slow)
    {
        this.slow += slow.data.value;
        while (slow.data.time  > 0)
        {
            slow.data.time -= Time.deltaTime * KnowledgeData.GetAdditional(knowledge.slow) * 0.5f;
            yield return null;
        }
        this.slow -= slow.data.value;
    }

    protected IEnumerator PoisonFunc(Effect poison)
    {
        this.poison += poison.data.value;
        while (poison.data.time > 0)
        {
            poison.data.time -= Time.deltaTime * KnowledgeData.GetAdditional(knowledge.poison) * 0.5f;
            yield return null;
        }
        this.poison -= poison.data.value;
    }

    protected IEnumerator IceFunc(Effect ice)
    {
        float time = 0;
        this.ice += ice.data.value;
        while (ice.data.time > 0)
        {
            ice.data.time -= Time.deltaTime * KnowledgeData.GetAdditional(knowledge.ice);
            yield return null;
        }
        this.ice -= ice.data.value;
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
            while (time < 0.5f * knowledge.fire)
            {
                yield return null;
                time += Time.deltaTime;
            }
            time = 0;
            GetDamage(Mathf.Max(fire.data.value - ice, 0));
            yield return null;
        }
    }

    public void KnockBack(Vector2 vec, float amount = 1)
    {
        AddEffect(EFFECT.KNOCKBACK, MathHelpers.Vector2ToDegree(vec), amount);
    }

    protected void KnockBackFunc(Effect knockBack)
    {
        Vector2 vec = MathHelpers.DegreeToVector2(knockBack.data.value);
        if (knowledge.knockBack < 2)
            rigid.AddForce(vec.normalized * knockBack.data.time * (KnowledgeData.GetNegative(knowledge.knockBack)));
        //Debug.Log("angle:" + knockBacks[i].data.value + " vec:" + vec.x + "," + vec.y);
        RemoveEffect(knockBack);
    }

    protected IEnumerator StunFunc(Effect stun)
    {
        while(stun.data.time > 0)
        {
            isStun = true;
            stun.data.time -= Time.deltaTime * KnowledgeData.GetAdditional(knowledge.stun);//시간 추가 감소
            yield return null;
        }
        RemoveEffect(stun);
        if (GetEffect(EFFECT.STUN) == null)
        {
            OnStunEnd();
            isStun = false;
        }
    }

    protected virtual void OnStunEnd() { }
    //상태이상
    public Effect AddEffect(EffectData data)
    {
        Effect effect = boardManager.effectPool.DequeueObjectPool().GetComponent<Effect>();
        effect.transform.SetParent(effectHolder);
        effect.SetData(data);
        effect.SetOwner(this);
        effect.gameObject.SetActive(true);
        effect.Active(true);//이펙트 타이머 활성화
        _effects.Add(effect);
        switch(data.type)
        {
            case EFFECT.FIRE:
                StartCoroutine(FireFunc(effect));
                break;
            case EFFECT.KNOCKBACK:
                KnockBackFunc(effect);
                break;
            case EFFECT.POISON:
                StartCoroutine(PoisonFunc(effect));
                break;
            case EFFECT.STUN:
                StartCoroutine(StunFunc(effect));
                break;
            case EFFECT.ICE:
                StartCoroutine(IceFunc(effect));
                break;
            case EFFECT.SLOW:
                StartCoroutine(SlowFunc(effect));
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
        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i].data.type == type)
            {
                result = _effects[i];
                return result;
            }
        }
        return null;
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
            bool isBA = false, isAA = false;
            animator.SetBool("isDeath", isDeath);
            if (HasParameter("isBeforeAttack", animator))
            {
                animator.SetBool("isBeforeAttack", isBeforeAttack);
                isBA = true;
            }
            if (HasParameter("isAfterAttack", animator))
            {
                animator.SetBool("isAfterAttack", isAfterAttack);
                isAA = true;
            }
            if (shadow.gameObject.activeSelf)
            {
                shadow.animator.SetBool("isDeath", isDeath);
                if(isBA)shadow.animator.SetBool("isBeforeAttack", isBeforeAttack);
                if(isAA)shadow.animator.SetBool("isAfterAttack", isAfterAttack);
            }
        }
        Vector2 velocity = agent.velocity;
        if (!isStun)
        {
            bool isWalk = false;
            if (velocity.x != 0 || velocity.y != 0)
                isWalk = true;
            else
                isWalk = false;
            if (HasParameter("isWalk", animator))
            {
                animator.SetBool("isWalk", isWalk);
                if (shadow.gameObject.activeSelf)
                    shadow.animator.SetBool("isWalk", isWalk);
            }
            if (HasParameter("isStun", animator))
            {
                animator.SetBool("isStun", isStun);
                if (shadow.gameObject.activeSelf)
                    shadow.animator.SetBool("isStun", isStun);
            }
            if (target)
            {
                animator.SetFloat("x", targetPosition.x - transform.position.x);
                animator.SetFloat("y", targetPosition.y - transform.position.y);
                if (shadow.gameObject.activeSelf)
                {
                    shadow.animator.SetFloat("x", targetPosition.x - transform.position.x);
                    shadow.animator.SetFloat("y", targetPosition.y - transform.position.y);
                }
            }
            else
            {
                animator.SetFloat("x", velocity.x);
                animator.SetFloat("y", velocity.y);
                if (shadow.gameObject.activeSelf)
                {
                    shadow.animator.SetFloat("x", velocity.x);
                    shadow.animator.SetFloat("y", velocity.y);
                }
            }
        }
        
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        for(int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name.CompareTo(paramName) == 0) return true;
        }
        return false;
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
        attackCoolTime = (weapon.beforeAttackDelay + weapon.afterAttackDelay) * (1 - ((_data.stat.spd - 1) * 0.05f));//총 10%씩 증가
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
        float _slow = slow * 0.1f * KnowledgeData.GetNegative(knowledge.slow);
        float _ice = ice * 0.1f * KnowledgeData.GetNegative(knowledge.ice);
        SetSpeed(data.moveSpeed * (1 + (0.1f * (_data.stat.spd - 1))) * Mathf.Max(0,(1 - _slow - _ice)));//slow마다 10%, ice마다 10% 속도 느려짐
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
