//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Enemy : Unit {
//    Vector2 moveVector = Vector2.zero;

//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        agent.OnDestinationReached += OnMoveEnd;
//        agent.OnDestinationInvalid += OnMoveEnd;
//        StartCoroutine(MoveCorou());
//    }

//    protected virtual void OnDisable()
//    {
//        agent.OnDestinationInvalid -= OnMoveEnd;
//        agent.OnDestinationReached -= OnMoveEnd;
//        boardManager.enemies.Remove(this);
//    }

//    public override bool SetTarget()
//    {
//        try
//        {
//            if (boardManager.scarecorws.Count > 0)
//            {
//                Unit closest = boardManager.scarecorws[0];
//                //for(int i = 1; i < boardManager.scarecorws.Count;i++)
//                //{
//                //    if(Vector2.SqrMagnitude(closest.transform.position - transform.position) > Vector2.SqrMagnitude(boardManager.scarecorws[i].transform.position - transform.position))
//                //    {
//                //        closest = boardManager.scarecorws[i];
//                //    }
//                //}
//                target = closest;
//                targetPosition = target.transform.position;
//            }
//            else if (!Player.instance.isDeath)
//            {
//                target = Player.instance;
//                targetPosition = target.transform.position;
//            }
//            else
//                target = null;
//            return true;
//        }
//        catch
//        {
//            return false;
//        }
//    }

//    protected override void OnCheckDeath()
//    {
//        boardManager.enemies.Remove(this);
//    }

//    protected override void OnDeath()
//    {
//        base.OnDeath();
//        MoneyManager.instance.AddUnrefinedSoul(data.cost);
//        //Debug.Log("cost:" + data.cost + " enemy dead");
//        boardManager.enemyPool.EnqueueObjectPool(gameObject);
//    }

//    protected virtual void MoveCheck()
//    {
//        if(!isDeath)
//        {
//            switch (data.move)
//            {
//                case MOVE_TYPE.REST_RUSH://맞으면 이동
//                    if(target && health < data.stat.hp)
//                    {
//                        Vector2 v = targetPosition - (Vector2)transform.position;
//                        attackable = true;
//                        float d = data.moveDistance;
//                        float vM = v.magnitude;
//                        if (d > vM)
//                            d = vM;
//                        Move((Vector2)transform.position + v.normalized * d);
//                    }
//                    break;
//                case MOVE_TYPE.RUSH:
//                    //Debug.Log("RUSH");
//                    if (target)
//                    {
//                        Vector2 v = targetPosition - (Vector2)transform.position;
//                        attackable = true;
//                        float d = data.moveDistance;
//                        float vM = v.magnitude;
//                        if (d > vM)
//                            d = vM;
//                        Move((Vector2)transform.position + v.normalized * d);
//                    }
//                    break;
//                case MOVE_TYPE.STATUE:
//                    //Debug.Log("STATUE");
//                    break;
//                case MOVE_TYPE.DISTANCE:
//                    //Debug.Log("DISTANCE");
//                    if (target)
//                    {
//                        Vector2 v = (targetPosition - (Vector2)transform.position);
//                        float vAngle = Vector2.Angle(Vector2.up, v);
//                        if (v.x < 0)
//                            vAngle = 360 - vAngle;
//                        //Debug.Log("vAngle" + vAngle);
//                        float distance = data.moveDistance;
//                        if (targetDistance < data.minDistance)//move back
//                        {
//                            //Debug.Log("back");
//                            float angle = vAngle - 180;
//                            if (angle < 0)
//                                angle += 360;
//                            angle += (UnityEngine.Random.Range(-90, 91));
//                            if (angle < 0)
//                                angle += 360;
//                            else if (angle > 360)
//                                angle -= 360;

//                            angle = 450 - angle;

//                            v = MathHelpers.DegreeToVector2(angle);
//                        }
//                        else if (targetDistance < data.maxDistance)//inside
//                        {
//                            //Debug.Log("Inside");
//                            if (UnityEngine.Random.Range(0, 2) == 0)//move left
//                            {
//                                float angle = vAngle - 90;
//                                if (angle < 0)
//                                    angle += 360;
//                                //Debug.Log("vAngle : " + vAngle + "left : " + angle);
//                                angle = 450 - angle;
//                                v = MathHelpers.DegreeToVector2(angle);
//                            }
//                            else//move right
//                            {
//                                float angle = vAngle + 90;
//                                if (angle > 360)
//                                    angle -= 360;
//                                //Debug.Log("vAngle : " + vAngle + "right : " + angle);
//                                angle = 450 - angle;
//                                v = MathHelpers.DegreeToVector2(angle);
//                            }
//                        }
//                        else //move forward
//                        {
//                            float d = targetDistance - distance;//이동 후 타겟과의 거리
//                            if (data.minDistance > d)
//                            {
//                                distance = distance - data.minDistance + d;//이동 후 거리가 최소 거리를 넘지 않게
//                            }
//                        }
//                        //Debug.Log(moveVector.ToString());
//                        Move((Vector2)transform.position + v.normalized * distance);
//                    }
//                    break;
//                case MOVE_TYPE.RUN:
//                    //Debug.Log("RUN");
//                    if (target)
//                    {
//                        Vector2 v = -(targetPosition - (Vector2)transform.position);
//                        RaycastHit2D hitt = Physics2D.Raycast(transform.position, v, data.moveDistance, GameDatabase.instance.wallMask);
//                        if (hitt)
//                        {
//                            if (targetDistance < data.moveDistance)
//                            {
//                                Vector2 mid = new Vector2((boardManager.boardRange[0].x + boardManager.boardRange[1].x) / 2,
//                                    (boardManager.boardRange[0].y + boardManager.boardRange[1].y) / 2);
//                                v = mid - (Vector2)transform.position;
//                            }
//                        }
//                        Move((Vector2)transform.position + v.normalized * data.moveDistance);
//                    }
//                    break;

//            }
//        }
//    }
//    bool isMoveEnd;
//    bool isReadyToMove;
//    void OnMoveEnd()
//    {
//        isMoveEnd = true;
//    }

//    IEnumerator MoveCorou()
//    {
//        WaitForSeconds delay = new WaitForSeconds(data.moveDelay);
//        float moveDelay = data.moveDelay;
//        while(true)
//        {
//            if(isMoveEnd)
//            {
//                if (moveDelay != data.moveDelay)//update
//                {
//                    moveDelay = data.moveDelay;
//                    delay = new WaitForSeconds(data.moveDelay);
//                }
//                yield return delay;
//                isMoveEnd = false;
//            }
//            if(!isStun)
//                MoveCheck();
//#if DELAY
//            yield return GameManager.instance.delayPointOne;
//#else
//            yield return new WaitForSeconds(0.1f);
//#endif
//        }
//    }

//    protected virtual void Update()
//    {
//        if (!isPause)
//        {
//            SyncData();
//            CheckDeath();
//            if (!isDeath)
//            {
//                Attack();
//            }
//            Animation();
//        }
//    }
//}
