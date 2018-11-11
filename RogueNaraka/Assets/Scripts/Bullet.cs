using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Animator animator;
    public new SpriteRenderer renderer;
    public DigitalRuby.AdvancedPolygonCollider.AdvancedPolygonCollider colliderRenderer;
    public Collider2D coll;

    private RevolveHolder revolveHolder = null;
    public Unit owner
    { get { return _owner; } }
    private Unit _owner;

    [SerializeField]
    private LayerMask layerMask;

    private Vector2 vec;
    private int knockBack;
    [SerializeField] [ReadOnly]
    private int pierce = 1;
    [SerializeField]
    private float guideIncrease;
    private float guideSpeed;
    private float damage;
    private float unitDamage;
    private float size;
    private float basicSpeed = 1;
    private float spinVelocity;
    private float spinValue;
    private float localSpeed;
    private float worldSpeed;
    private float angle;
    [SerializeField] [ReadOnly]
    private bool isFriendly;
    private bool isDestroy;
    private bool isSpin;
    private bool isMoving;
    private bool isSleep = false;
    private bool isDestroyWithOwner;

    public event System.Action<Bullet, Unit> OnDamaged;

    [SerializeField] [ReadOnly]
    private BulletData data;
    private EffectData[] effects;
    private List<int> records = new List<int>();
    private List<float> dotTimes = new List<float>();

    [SerializeField] [ReadOnly]
    private List<Bullet> destroyChildren = new List<Bullet>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        switch (data.type)
        {
            case BULLET_TYPE.CIRCLECAST:
                Gizmos.DrawWireSphere(transform.position, size);
                break;
            case BULLET_TYPE.LINECAST:
                Gizmos.DrawLine(transform.position, vec);
                break;
            case BULLET_TYPE.TRIANGLE:
                GL.PushMatrix();
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.left * size / 2, new Vector3(size, size, size));
                GL.PopMatrix();
                break;
            case BULLET_TYPE.SECTOR:
                Gizmos.DrawWireSphere(transform.position, size);
                float vecAngle = Vector2.Angle(Vector2.up, vec);
                if (vec.x < 0)
                    vecAngle = 360 - vecAngle;
                float angleNeg = vecAngle - angle;
                float anglePos = vecAngle + angle;

                Matrix4x4 origin = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(Vector2.zero, MathHelpers.DegreeToVector2(angle + 180).normalized * size);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(Vector2.zero, MathHelpers.DegreeToVector2(180 - angle).normalized * size);
                Gizmos.matrix = origin;
                break;
        }
    }

    public void Init(int id, float unitDmg, bool isFriendly = false)
    {
        data = GameDatabase.instance.bullets[id];
        Init(data, unitDmg, isFriendly);
    }
    public void Init(BulletData dt, float unitDmg, bool isFriendly = false)
    {
        data = dt;
        damage = data.dmg;
        unitDamage = unitDmg;
        name = data.name;
        SetFriendly(isFriendly);
        animator.runtimeAnimatorController = data.controller;
        size = data.size;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        renderer.color = data.color;
        renderer.enabled = true;
        isDestroy = false;
        isMoving = false;
        isSpin = false;
        spinVelocity = 0;
        spinValue = 0;
        localSpeed = 0;
        worldSpeed = 0;
        angle = data.angle;
        effects = dt.effects;
        pierce = 1;
        guideIncrease = 0;
        guideSpeed = 0;
        revolveHolder = null;
        _owner = null;
        isDestroyWithOwner = false;
        coll.enabled = false;
        colliderRenderer.enabled = false;
        switch(dt.sortingLayer)
        {
            case SORTING_LAYER.BOT:
                renderer.sortingLayerID = GameDatabase.botLayer;
                break;
            case SORTING_LAYER.MID:
                renderer.sortingLayerID = GameDatabase.midLayer;
                break;
            case SORTING_LAYER.TOP:
                renderer.sortingLayerID = GameDatabase.topLayer;
                break;
        }
    }

    public void SetFriendly(bool value)
    {
        isFriendly = value;
        if (!value)
            SetMask(GameDatabase.instance.enemyMask);
        else
            SetMask(GameDatabase.instance.friendlyMask);
    }

    public void SpawnChildren(BulletChild[] children)
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].startTime != 0)
            {
                StartCoroutine(SpawnBulletTimer(children[i]));
            }
            else
                SpawnBullet(children[i], transform.position);
        }
    }

    private IEnumerator SpawnBulletTimer(BulletChild childData)
    {
        if (childData.isFirst)
            childData.isFirst = false;
        else
            yield return new WaitForSeconds(childData.startTime);
        if (!isDestroy)
        {
            Vector2 pos = transform.position;
            yield return new WaitForSeconds(childData.waitTime);
            if (!isDestroy)
                SpawnBullet(childData, pos);
        }
    }

    public void SpawnBullet(BulletChild childData, Vector2 pos)
    {
        Bullet child = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        child.Init(childData.bulletId, unitDamage, isFriendly);
        child.gameObject.SetActive(true);

        child.Teleport(pos);
        if(childData.sortingOrder != 0)
            child.GetComponent<IsometricSpriteRenderer>().add = childData.sortingOrder;
        
        if (childData.isRevolveTarget)
        {
            if (revolveHolder)
            {
                child.revolveHolder = revolveHolder;
                revolveHolder.Add(child.gameObject);
            }
            else
            {
                child.revolveHolder = _owner.revolveHolder;
                child.revolveHolder.Add(child.gameObject);
            }

        }
        if (childData.isStick)
        {
            child.transform.SetParent(transform);
            child.MoveToZero();
        }
        if (childData.isEndWith)
        {
            destroyChildren.Add(child);
        }
        var vForce = Quaternion.AngleAxis(childData.angle, Vector3.back) * vec;
        child.ActiveTimer(pos, childData.spawnPoint, vForce.normalized, childData.waitTime, childData.localSpeed, childData.worldSpeed);
        //child.Attack(pos, vForce.normalized);
        if (childData.isRepeat && !isDestroy)
            StartCoroutine(SpawnBulletTimer(childData));
    }

    public void MoveToZero()
    {
        StartCoroutine(MoveToZeroCoroutine());
    }

    private IEnumerator MoveToZeroCoroutine()
    {
        yield return null;
        transform.localPosition = Vector3.zero;
    }

    public void ActiveTimer(Vector2 start, Vector2 move, Vector2 v, float waitTime, float localSpeed, float worldSpeed)
    {
        renderer.enabled = false;
        isSleep = true;
        StartCoroutine(ActiveTimerCoroutine(start, move, v, waitTime, localSpeed, worldSpeed));
    }

    private IEnumerator ActiveTimerCoroutine(Vector2 start, Vector2 move, Vector2 v, float waitTime, float localSpeed, float worldSpeed)
    {
        yield return new WaitForSeconds(waitTime);
        renderer.enabled = true;
        isSleep = false;
        Attack(start, move, v, localSpeed, worldSpeed);
    }

    public void SetMask(LayerMask mask)
    {
        layerMask = mask;
    }

    private void AbilityCheck()
    {
        if (data.abilities != null)
        {
            for (int i = 0; i < data.abilities.Length; i++)
            {
                switch (data.abilities[i].ability)
                {
                    case ABILITY.GRAVITY:
                        break;
                    case ABILITY.SPIN:
                        spinVelocity = data.abilities[i].value;
                        isSpin = true;
                        break;
                    case ABILITY.PIERCE:
                        pierce += (int)data.abilities[i].value;
                        break;
                    case ABILITY.TIME:
                        if (data.abilities[i].value > 0)
                            StartCoroutine(TimeLimit(data.abilities[i].value));
                        else
                            StartCoroutine(DestroyCoroutine());
                        break;
                    case ABILITY.GUIDE:
                        guideIncrease = data.abilities[i].value;
                        break;
                    case ABILITY.OWNER:
                        if (data.abilities[i].value > 0)
                            isDestroyWithOwner = true;
                        break;
                }
            }
        }
    }

    public void Attack(Vector2 start, Vector2 move, Vector2 v, float localSpeed, float worldSpeed, RevolveHolder revolve = null, Unit owner = null)
    {
        gameObject.SetActive(true);
        if (owner)
            this._owner = owner;
        this.localSpeed = localSpeed;
        this.worldSpeed = worldSpeed;
        //Abilities 체크
        AbilityCheck();

        Spawn(start, move, v);
        SetVec(v);
        RotateTo(v);
        if (localSpeed > 0 || worldSpeed > 0)
            Shoot(v);
        if (revolve)
        {
            revolveHolder = revolve;
            if (data.type != BULLET_TYPE.NONE)//NONE 인 경우는 holder.Add 생략
                revolve.Add(gameObject);
        }
        if (!data.shake.isOnHit && data.shake.power != 0)
        {
            Camera.main.GetComponent<CameraShake>().Shake(data.shake);
        }
        SpawnChildren(data.children);
    }

    public void Shoot(Vector2 v)
    {
        Move(v);
    }

    public void Spawn(Vector2 start, Vector2 move, Vector2 v)
    {
        Teleport(start);
        Quaternion origin = transform.rotation;
        RotateTo(v);
        transform.position = transform.position;
        transform.position += transform.up * move.x;
        transform.position += transform.right * -move.y;
        transform.rotation = origin;
    }
    public void Teleport(Vector2 position)
    {
        transform.position = position;
    }

    public void SetVec(Vector2 v)
    {
        vec = v;
    }

    public void Move(Vector2 v)
    {
        vec = v;
        isMoving = true;
    }

    public void RotateTo(Vector2 v)
    {
        //Debug.Log("RotateTo:" + v.ToString());
        transform.Rotate(new Vector3(0, 0, Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + 180f));
    }

    private float Damage()
    {
        return damage * unitDamage;
    }

    private void EffectFunc(Unit unit)
    {
        if (effects != null)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                EffectData temp = (EffectData)(effects[i].Clone());
                switch (effects[i].type)
                {
                    case EFFECT.KNOCKBACK:
                        temp = new EffectData(EFFECT.KNOCKBACK, MathHelpers.Vector2ToDegree(unit.transform.position - transform.position), effects[i].value);
                        break;
                }
                unit.AddEffect(temp);
            }
        }
    }

    private void DotTimeFunc()
    {
        for (int i = 0; i < dotTimes.Count; i++)
        {
            dotTimes[i] -= Time.deltaTime;
            if (dotTimes[i] <= 0)
            {
                records.RemoveAt(i);
                dotTimes.RemoveAt(i);
            }
        }
    }

    private void Update()
    {
        bool isDynamic = data.type == BULLET_TYPE.DYNAMIC;
        coll.enabled = isDynamic;
        colliderRenderer.enabled = isDynamic;
        if (isSleep)
        {
            if (isDestroyWithOwner && _owner.isDeath)
                Destroy();
        }
        else
        {
            if (transform.position.x > 50 || transform.position.x < -50 ||
                transform.position.y > 50 || transform.position.y < -50)
                Destroy();
            if (isSpin)
            {
                spinValue += spinVelocity * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, spinValue);
            }

            if (!isDestroy)
            {
                if (isMoving)
                {
                    transform.Translate(new Vector3(-localSpeed * basicSpeed * data.flightSpeed * Time.deltaTime, 0, 0));
                    transform.Translate(vec.normalized * worldSpeed * basicSpeed * data.flightSpeed *  Time.deltaTime, Space.World);
                    if (guideIncrease > 0)
                    {
                        Unit target = null;
                        if (layerMask == GameDatabase.instance.friendlyMask)
                        {
                            List<Enemy> enimies = BoardManager.instance.enemies;
                            if (enimies.Count > 0)
                            {
                                Enemy closest = enimies[0];
                                float distance = Vector2.SqrMagnitude(transform.position - enimies[0].transform.position);
                                for (int i = 1; i < enimies.Count; i++)
                                {
                                    float temp = Vector2.SqrMagnitude(transform.position - enimies[i].transform.position);
                                    if (temp < distance)
                                    {
                                        distance = temp;
                                        closest = enimies[i];
                                    }
                                }
                                target = closest;
                            }
                        }
                        else
                        {
                            target = Player.instance;
                        }
                        if (target)
                        {
                            Vector3 targetVec = transform.position - target.transform.position;
                            transform.right = Vector3.Slerp(transform.right, targetVec, guideSpeed * 0.01f);
                            guideSpeed += guideIncrease * 0.01f;
                        }
                    }
                }

                Collider2D[] hits = null;
                switch (data.type)
                {
                    case BULLET_TYPE.CIRCLECAST://원형 데미지
                        hits = Physics2D.OverlapCircleAll(transform.position, size, layerMask);

                        DotTimeFunc();
                        bool isDeal = false;
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].gameObject.layer != GameDatabase.wallLayer)
                            {
                                Unit unit = hits[i].GetComponent<Unit>();
                                int hash = unit.GetHashCode();
                                int index = records.FindIndex(d => d == hash);
                                if (index == -1)
                                {
                                    isDeal = true;
                                    //Debug.Log(name + " dot hit to " + hits[i].transform.name);
                                    unit.GetDamage(Damage());
                                    EffectFunc(unit);
                                    records.Add(hash);
                                    dotTimes.Add(data.dealSpeed);
                                    OnDamage(unit);
                                    //Debug.Log("records.Count:" + records.Count);
                                }
                            }
                            else if (hits[i].gameObject.layer == GameDatabase.wallLayer)
                            {
                                if (!revolveHolder)
                                {
                                    OnDamage(null);
                                    Destroy();
                                }
                            }
                        }
                        if (isDeal)
                        {
                            if (pierce < 999)
                                pierce -= hits.Length;
                            if (pierce <= 0)
                                Destroy();
                        }
                        break;
                    case BULLET_TYPE.LINECAST://직선 데미지
                        RaycastHit2D[] _hits = Physics2D.RaycastAll(transform.position, vec, size, layerMask);

                        DotTimeFunc();
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].gameObject.layer != GameDatabase.wallLayer)
                            {
                                Unit unit = _hits[i].transform.GetComponent<Unit>();
                                int hash = unit.GetHashCode();
                                int index = records.FindIndex(d => d == hash);
                                if (index == -1)
                                {
                                    isDeal = true;
                                    Debug.Log(name + " dot hit to " + hits[i].transform.name);
                                    unit.GetDamage(Damage());
                                    EffectFunc(unit);
                                    records.Add(hash);
                                    dotTimes.Add(data.dealSpeed);
                                    OnDamage(unit);
                                }
                            }
                        }

                        if (hits.Length > 0)
                        {
                            if (pierce < 999)
                                pierce -= hits.Length;
                            if (pierce <= 0)
                                Destroy();
                        }
                        break;
                    case BULLET_TYPE.TRIANGLE://삼각형 도트 데미지
                    case BULLET_TYPE.SECTOR:
                        float vecAngle = Vector2.Angle(Vector2.up, vec);
                        if (vec.x < 0)
                            vecAngle = 360 - vecAngle;
                        if (data.type == BULLET_TYPE.TRIANGLE)
                            hits = Physics2D.OverlapBoxAll((Vector2)transform.position + (vec * size / 2), new Vector2(size, size), vecAngle, layerMask);
                        else
                            hits = Physics2D.OverlapCircleAll(transform.position, size, layerMask);
                        DotTimeFunc();

                        for (int i = 0; i < hits.Length; i++)
                        {

                            if (hits[i].gameObject.layer != GameDatabase.wallLayer)
                            {
                                Unit unit = hits[i].GetComponent<Unit>();
                                int hash = unit.GetHashCode();
                                int index = records.FindIndex(d => d == hash);
                                if (index == -1)
                                {
                                    Vector2 v = unit.transform.position - transform.position;
                                    float unitAngle = Vector2.Angle(Vector2.up, v);
                                    if (v.x < 0)
                                        unitAngle = 360 - unitAngle;

                                    //Debug.Log(hits[i].transform.name + " angle:" + vecAngle + " unit:" + unitAngle);
                                    if (IsInsideAngle(vecAngle, angle, unitAngle))
                                    {
                                        Debug.Log(name + " dot hit to " + hits[i].transform.name);
                                        unit.GetDamage(Damage());
                                        EffectFunc(unit);
                                        records.Add(hash);
                                        dotTimes.Add(data.dealSpeed);
                                        OnDamage(unit);
                                    }
                                    else
                                        Debug.Log(name + " not inside of triangle" + hits[i].transform.name);
                                }
                            }
                        }
                        break;
                    case BULLET_TYPE.DYNAMIC:
                        DotTimeFunc();
                        break;
                }
            }
        }
    }

    private void OnTriggetEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeSelf)
        {
            if (data.type == BULLET_TYPE.DYNAMIC)
            {
                int layer = !isFriendly ? GameDatabase.friendlyLayer : GameDatabase.enemyLayer;
                if (collision.gameObject.layer == layer)
                {
                    Unit unit = collision.GetComponent<Unit>();
                    if (!unit.isDeath)
                    {
                        int hash = unit.GetHashCode();
                        int index = records.FindIndex(d => d == hash);
                        if (index == -1)
                        {
                            //Debug.Log(name + "  hit to " + hits[i].transform.name);
                            unit.GetDamage(Damage());
                            EffectFunc(unit);
                            records.Add(hash);
                            dotTimes.Add(data.dealSpeed);
                            OnDamage(unit);
                        }
                    }

                    if (pierce < 999)
                        pierce--;
                    if (pierce <= 0)
                        Destroy();
                }
            }
        }
    }
    
    void OnDamage(Unit unit)
    {
        if (data.shake.isOnHit && data.shake.power != 0)
            Camera.main.GetComponent<CameraShake>().Shake(data.shake);
        if (OnDamaged != null)
        {
            OnDamaged(this, unit);
        }
    }
            

    public bool IsInsideAngle(float mid, float range, float target)
    {
        float left = mid - range;
        float right = mid + range;
        if (left < 0)
        {
            if (target > right)
                if (target < 360 + left)
                    return false;
        }
        else if (right > 360)
        {
            if (target < left)
                if (target > right - 360)
                    return false;
        }
        else
        {
            if (target > right)
                return false;
            if (target < left)
                return false;
        }
        return true;
    }

    public void Destroy()
    {
        StartCoroutine(DestroyCoroutine());
    }

    public void Destroy(float time)
    {
        StartCoroutine(TimeLimit(time));
    }

    private void DestroyChildren()
    {
        for(int i = 0; i < destroyChildren.Count; i++)
        {
            Bullet child = destroyChildren[i];
            destroyChildren.RemoveAt(i);
            i--;
            if(child.gameObject.activeSelf)
                child.Destroy();
        }
    }

    private IEnumerator TimeLimit(float time)
    {
        //Debug.Log(name + " time Limit " + time);

        yield return new WaitForSeconds(time);
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        coll.enabled = false;
        colliderRenderer.enabled = false;
        vec = Vector2.zero;
        isMoving = false;
        isDestroy = true;
        animator.SetBool("isDestroy", true);
        DestroyChildren();//자식들 죽이는 부모
        SpawnChildren(data.onDestroy);
        //if (destroyTime <= 0)
        //    renderer.enabled = false;

        AnimatorStateInfo animationState;
        AnimatorClipInfo[] clips;
        do
        {
            yield return null;
            animationState = animator.GetCurrentAnimatorStateInfo(0);
        } while (!animationState.IsName("Destroy"));
        clips = animator.GetCurrentAnimatorClipInfo(0);
        
        float time = clips[0].clip.length / animationState.speed;
        //Debug.Log(clips[0].clip.name +" "+ clips[0].clip.length + "*" + animationState.speed + "=" + time);
        yield return new WaitForSeconds(time);

        //Debug.Log("Destroy");
        OnDamaged = null;
        BoardManager.instance.bulletPool.EnqueueObjectPool(gameObject, true);
    }

    public void OnDisable()
    {
        if (revolveHolder)
            revolveHolder.Remove(gameObject);
    }
}
