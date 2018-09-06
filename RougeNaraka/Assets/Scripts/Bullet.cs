using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Rigidbody2D rigid;
    public Animator animator;
    public new SpriteRenderer renderer;

    [SerializeField][ReadOnly]
    private LayerMask layerMask;

    private Vector2 vec;
    private float damage;
    private float unitDamage;
    private float size;
    private float basicSpeed = 1;
    private float dotTime;
    private float spinVelocity;
    private float spinValue;
    private float shootSpeed;
    private float angle;
    [SerializeField][ReadOnly]
    private bool isFriendly;
    private bool isDestroy;
    private bool isSpin;
    private bool isMoving;
    private bool isSleep;

    [SerializeField][ReadOnly]
    private BulletData data;
    private EffectData[] effects;

    [SerializeField][ReadOnly]
    private List<Bullet> destroyChildren = new List<Bullet>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        switch (data.type)
        {
            case BULLET_TYPE.CIRCLECAST:
            case BULLET_TYPE.CIRCLEOVERLAP:
                Gizmos.DrawWireSphere(transform.position, size);
                break;
            case BULLET_TYPE.LINECAST:
            case BULLET_TYPE.LINECASTS:
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
        shootSpeed = 0;
        angle = data.angle;
    }

    public void SetEffects(EffectData[] ef)
    {
        effects = ef;
    }

    public void SetFriendly(bool value)
    {
        isFriendly = value;
        if(!value)
            SetMask(GameDatabase.instance.enemyMask);
        else
            SetMask(GameDatabase.instance.friendlyMask);
    }

    public void SpawnChildren()
    {
        for (int i = 0; i < data.children.Length; i++)
        {
            if (data.children[i].startTime != 0)
            {
                StartCoroutine(SpawnBulletTimer(data.children[i]));
            }
            else
                SpawnBullet(data.children[i], transform.position);
        }
    }

    private IEnumerator SpawnBulletTimer(BulletChild childData)
    {
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
        child.renderer.sortingOrder = renderer.sortingOrder + childData.sortingOrder;
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
        child.ActiveTimer(pos, childData.spawnPoint, vForce.normalized, childData.waitTime, childData.shootSpeed, childData.isShoot);
        //child.Attack(pos, vForce.normalized);
        if (childData.isRepeat && !isDestroy)
        {
            if (childData.startTime < 0)
                SpawnBullet(childData, transform.position);
            else
                StartCoroutine(SpawnBulletTimer(childData));
        }
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

    public void ActiveTimer(Vector2 start, Vector2 move, Vector2 v, float waitTime, float shootSpeed, bool isShoot)
    {
        if (waitTime > 0)
        {
            renderer.enabled = false;
            isSleep = true;
            StartCoroutine(ActiveTimerCoroutine(start, move, v, waitTime, shootSpeed, isShoot));
        }
        else
            Attack(start, move, v, shootSpeed, isShoot);
    }

    private IEnumerator ActiveTimerCoroutine(Vector2 start, Vector2 move, Vector2 v, float waitTime, float ShootSpeed, bool isShoot = true)
    {
        yield return new WaitForSeconds(waitTime);
        isSleep = false;
        renderer.enabled = true;
        Attack(start, move, v, shootSpeed, isShoot);
    }

    public void SetMask(LayerMask mask)
    {
        layerMask = mask;
    }

    public void Attack(Vector2 start, Vector2 move, Vector2 v, float shootSpeed, bool isShoot = true)
    {
        this.shootSpeed = shootSpeed;
        //Abilities 체크
        if (data.abilities != null)
        {
            for (int i = 0; i < data.abilities.Length; i++)
            {
                switch (data.abilities[i].ability)
                {
                    case ABILITY.KNOCKBACK:
                        break;
                    case ABILITY.GRAVITY:
                        break;
                    case ABILITY.CHAIN:
                        break;
                    case ABILITY.SPIN:
                        spinVelocity = data.abilities[i].value;
                        isSpin = true;
                        break;
                    case ABILITY.PIERCE:
                        break;
                    case ABILITY.TIME:
                        if(data.abilities[i].ability > 0)
                            StartCoroutine(TimeLimit(data.abilities[i].value));
                        break;
                }
            }
        }

        //switch (data.type)
        //{
        //    case BULLET_TYPE.CIRCLECAST:
        //        Spawn(start, v);
        //        Shoot(v);
        //        break;
        //    case BULLET_TYPE.CIRCLEOVERLAP:
        //        Spawn(start, v);
        //        break;
        //    case BULLET_TYPE.LINECAST:
        //        break;
        //    case BULLET_TYPE.LINECASTS:
        //        Teleport(start);
        //        SetVec(v);
        //        break;
        //    case BULLET_TYPE.TRIANGLE:
        //        Teleport(start);
        //        SetVec(v);
        //        RotateTo(v);
        //        break;
        //}

        Spawn(start, move, v);
        SetVec(v);
        RotateTo(v);
        if (isShoot)
            Shoot(v);
        SpawnChildren();
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
        //rigid.AddForce(v * data.speed * basicSpeed);
    }

    public void RotateTo(Vector2 v)
    {
        //Debug.Log("RotateTo:" + v.ToString());
        transform.Rotate(new Vector3(0, 0, Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + 180f));
    }

    public void Destroy()
    {
        StartCoroutine(DestroyCoroutine());
    }

    public void Destroy(float time)
    {
        StartCoroutine(TimeLimit(time));
    }

    private float Damage()
    {
        return damage * unitDamage;
    }

    private void Update()
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
                transform.Translate(new Vector3(-shootSpeed * basicSpeed * Time.deltaTime, 0, 0));
            }

            if (data.type == BULLET_TYPE.CIRCLECAST)//원형 데미지
            {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, size, Vector2.zero, 0, layerMask);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (!(hits[i].transform.gameObject.layer == GameDatabase.wallLayer))
                    {
                        //Debug.Log(name + " hit to " + hits[i].transform.name);
                        Unit unit = hits[i].transform.GetComponent<Unit>();
                        unit.GetDamage(Damage());
                        unit.AddEffect(effects);
                    }
                }
                if(hits.Length > 0)
                    Destroy();
            }
            else if (data.type == BULLET_TYPE.CIRCLEOVERLAP)//원형 도트 데미지
            {
                if (dotTime >= 0)
                {
                    dotTime -= Time.deltaTime;
                }
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, size, layerMask);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (!(hits[i].gameObject.layer == GameDatabase.wallLayer))
                    {
                        if (dotTime < 0)
                        {
                            Debug.Log(name + " dot hit to " + hits[i].transform.name);
                            Unit unit = hits[i].GetComponent<Unit>();
                            unit.GetDamage(Damage());
                            unit.AddEffect(effects);
                        }
                    }
                }
                if (dotTime < 0)
                    dotTime += data.dealSpeed;
            }
            else if (data.type == BULLET_TYPE.LINECAST)//직선 단일 데미지
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vec, size, layerMask);

                if (hit)
                {
                    if (hit.collider.gameObject.layer == GameDatabase.wallLayer)
                    {
                        Debug.Log(name + " hit to wall");
                    }
                    else
                    {
                        Debug.Log(name + " hit to " + hit.transform.name);
                        Unit unit = hit.transform.GetComponent<Unit>();
                        unit.GetDamage(Damage());
                        unit.AddEffect(effects);
                    }
                }
            }
            else if (data.type == BULLET_TYPE.LINECASTS)//직선 다중 데미지 (테스트 필요)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vec, size, layerMask);

                for (int i = 0; i < hits.Length; i++)
                {
                    Debug.Log(name + " hit to " + hits[i].transform.name);
                    Unit unit = hits[i].transform.GetComponent<Unit>();
                    unit.GetDamage(Damage());
                    unit.AddEffect(effects);
                }
            }
            else if (data.type == BULLET_TYPE.TRIANGLE)//삼각형 도트 데미지
            {

                float vecAngle = Vector2.Angle(Vector2.up, vec);
                if (vec.x < 0)
                    vecAngle = 360 - vecAngle;
                Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + (vec * size / 2), new Vector2(size, size), vecAngle, layerMask);
                if (dotTime >= 0)
                {
                    dotTime -= Time.deltaTime;
                }
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].gameObject.layer == GameDatabase.friendlyLayer ||
                        hits[i].gameObject.layer == GameDatabase.enemyLayer)
                    {
                        if (dotTime < 0)
                        {
                            
                            Unit unit = hits[i].GetComponent<Unit>();
                            Vector2 v = unit.transform.position - transform.position;
                            float unitAngle = Vector2.Angle(Vector2.up, v);
                            if (v.x < 0)
                                unitAngle = 360 - unitAngle;

                            //Debug.Log(hits[i].transform.name + " angle:" + vecAngle + " unit:" + unitAngle);
                            if (IsInsideAngle(vecAngle, angle, unitAngle))
                            {
                                Debug.Log(name + " dot hit to " + hits[i].transform.name);
                                unit.GetDamage(Damage());
                                unit.AddEffect(effects);
                            }
                            else
                                Debug.Log(name + " not inside of triangle" + hits[i].transform.name);
                        }
                    }
                }
                if (dotTime < 0)
                    dotTime += data.dealSpeed;
            }
            else if (data.type == BULLET_TYPE.SECTOR)//부채꼴 도트 데미지
            {
                float vecAngle = Vector2.Angle(Vector2.up, vec);
                if (vec.x < 0)
                    vecAngle = 360 - vecAngle;
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, size, layerMask);
                if (dotTime >= 0)
                {
                    dotTime -= Time.deltaTime;
                }
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].gameObject.layer == GameDatabase.friendlyLayer ||
                        hits[i].gameObject.layer == GameDatabase.enemyLayer)
                    {
                        if (dotTime < 0)
                        {

                            Unit unit = hits[i].GetComponent<Unit>();
                            Vector2 v = unit.transform.position - transform.position;
                            float unitAngle = Vector2.Angle(Vector2.up, v);
                            if (v.x < 0)
                                unitAngle = 360 - unitAngle;

                            //Debug.Log(hits[i].transform.name + " vecAngle:" + vecAngle + " unit:" + unitAngle + " angle: " + angle);
                            if (IsInsideAngle(vecAngle, angle, unitAngle))
                            {
                                //Debug.Log(name + " dot hit to " + hits[i].transform.name);
                                unit.GetDamage(Damage());
                                unit.AddEffect(effects);
                            }
                            else
                            {
                                //Debug.Log(name + " not inside of sector" + hits[i].transform.name);
                                //Debug.Log(hits[i].transform.name + " vecAngle:" + vecAngle + " unit:" + unitAngle + " angle: " + angle);
                            }
                        }
                    }
                }
                if (dotTime < 0)
                    dotTime += data.dealSpeed;
            }
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

    private void DestroyChildren()
    {
        for(int i = 0; i < destroyChildren.Count; i++)
        {
            Bullet child = destroyChildren[i];
            destroyChildren.RemoveAt(i);
            i--;
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
        rigid.velocity = Vector2.zero;
        vec = Vector2.zero;
        isMoving = false;
        isDestroy = true;
        animator.SetBool("isDestroy", true);
        DestroyChildren();//자식들 죽이는 부모
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
        BoardManager.instance.bulletPool.EnqueueObjectPool(gameObject);
    }
}
