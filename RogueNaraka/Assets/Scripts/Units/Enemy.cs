using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit {
    Vector2 moveVector = Vector2.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        agent.OnDestinationReached += Move;
        agent.OnDestinationInvalid += Move;
        Move();
    }

    protected virtual void OnDisable()
    {
        agent.OnDestinationInvalid -= Move;
        agent.OnDestinationReached -= Move;
    }

    public override bool SetTarget()
    {
        try
        {
            if (!Player.instance.isDeath)
            {
                target = Player.instance;
                targetPosition = target.transform.position;
            }
            else
                target = null;
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override void OnCheckDeath()
    {
        boardManager.enemies.Remove(this);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        MoneyManager.instance.AddCollectedSoul(data.cost);
        //Debug.Log("cost:" + cost + " enemy dead");
        boardManager.enemyPool.EnqueueObjectPool(gameObject);
    }

    protected int MoneyFunction(int cost, float luck)
    {
        int money = UnityEngine.Random.Range(1, (int)((10 * cost) + luck));
        return money;
    }

    protected virtual void MoveCheck()
    {
        if (!target)
        {
            Move();
        }
        else if(!isDeath)
        {
            Vector2 v;
            float d;
            switch (move)
            {
                case MOVE_TYPE.RUSH:
                    //Debug.Log("RUSH");
                    v = targetPosition - (Vector2)transform.position;
                    attackable = true;
                    d = data.moveDistance;
                    float vM = v.magnitude;
                    if (d > vM)
                        d = vM;
                    Move((Vector2)transform.position + v.normalized * d);
                    break;
                case MOVE_TYPE.STATUE:
                    break;
                case MOVE_TYPE.DISTANCE:
                    v = (targetPosition - (Vector2)transform.position);
                    float vAngle = Vector2.Angle(Vector2.up, v);
                    if (v.x < 0)
                        vAngle = 360 - vAngle;
                    //Debug.Log("vAngle" + vAngle);
                    float distance = data.moveDistance;
                    if (targetDistance < data.minDistance)//move back
                    {
                        //Debug.Log("back");
                        float angle = vAngle - 180;
                        if (angle < 0)
                            angle += 360;
                        angle += (UnityEngine.Random.Range(-90, 91));
                        if (angle < 0)
                            angle += 360;
                        else if (angle > 360)
                            angle -= 360;

                        angle = 450 - angle;

                        v = MathHelpers.DegreeToVector2(angle);
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, v, distance, GameDatabase.instance.wallMask);
                        if (hit)
                        {
                            //Debug.Log("wall");
                            v = -v;
                        }

                    }
                    else if (targetDistance < data.maxDistance)//inside
                    {
                        //Debug.Log("Inside");
                        if (UnityEngine.Random.Range(0, 2) == 0)//move left
                        {
                            float angle = vAngle - 90;
                            if (angle < 0)
                                angle += 360;
                            //Debug.Log("vAngle : " + vAngle + "left : " + angle);
                            angle = 450 - angle;
                            v = MathHelpers.DegreeToVector2(angle);
                        }
                        else//move right
                        {
                            float angle = vAngle + 90;
                            if (angle > 360)
                                angle -= 360;
                            //Debug.Log("vAngle : " + vAngle + "right : " + angle);
                            angle = 450 - angle;
                            v = MathHelpers.DegreeToVector2(angle);
                        }
                    }
                    else //move forward
                    {
                        d = targetDistance - distance;//이동 후 타겟과의 거리
                        if (data.minDistance > d)
                        {
                            distance = distance - data.minDistance + d;//이동 후 거리가 최소 거리를 넘지 않게
                        }
                    }
                    //Debug.Log(moveVector.ToString());
                    Move((Vector2)transform.position + v.normalized * distance);
                    break;
                case MOVE_TYPE.RUN:
                    Vector2 vec = -(targetPosition - (Vector2)transform.position);
                    RaycastHit2D hitt = Physics2D.Raycast(transform.position, vec, data.moveDistance, GameDatabase.instance.wallMask);
                    if (hitt)
                    {
                        if (targetDistance < data.moveDistance)
                        {
                            Vector2 mid = new Vector2((boardManager.boardRange[0].x + boardManager.boardRange[1].x) / 2,
                                (boardManager.boardRange[0].y + boardManager.boardRange[1].y) / 2);
                            vec = mid - (Vector2)transform.position;
                        }
                    }
                    Move((Vector2)transform.position + vec.normalized * data.moveDistance);
                    break;
            }
        }
    }


    protected void Move()
    {
        StartCoroutine(MoveCoolTime(data.moveDelay));
    }

    private IEnumerator MoveCoolTime(float time)
    {
        attackable = true;
        yield return new WaitForSeconds(time);
        attackable = false;

        MoveCheck();
    }

    protected virtual void Update()
    {
        if (!isPause)
        {
            CheckDeath();
            if (!isDeath)
            {
                if (effects.Count > 0)//상태 이상
                {
                    StunFunc();
                    KnockBackFunc();
                }
                TargetUpdate();
                Attack();
            }
            Animation();
        }
    }

    protected virtual void TargetUpdate()
    {
        SetTarget();
        if (target != null)
            targetPosition = target.transform.position;
    }
}
