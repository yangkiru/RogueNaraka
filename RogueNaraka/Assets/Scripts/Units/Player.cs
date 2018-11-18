using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player instance = null;

    public bool isMoveToMouse;

    private float _collectedDmg;

    private List<Enemy> enemies;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        enemies = boardManager.enemies;
        StartCoroutine(CollectedDmgFunc());
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        gameManager.StartCoroutine(gameManager.OnEnd());
    }

    protected void Update()
    {
        if (!isPause && boardManager.isReady)
        {
            CheckDeath();
            if (!isDeath)
            {
                SyncData();
                if (!isWin && boardManager.enemies.Count == 0)
                    MoveToGoal();
                else
                {
                    if (isMoveToMouse && Input.GetMouseButton(0))
                        Move(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (isAutoMove && !isWin)
                        MoveToAttack();
                    Attack();
                }
            }
            Animation();
        }
    }

    public override float GetDamage(float damage, bool isTxtOnHead = true)
    {
        float sum = base.GetDamage(damage, false);
        _collectedDmg += sum;
        return sum;
    }


    protected IEnumerator CollectedDmgFunc()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return delay;
            if (_collectedDmg > 0)
            {
                PointTxtManager.instance.TxtOnHead(-_collectedDmg, transform, Color.red);
                _collectedDmg = 0;
            }
        }
    }

    public override bool SetTarget()
    {
        switch(weapon.type)
        {
            case ATTACK_TYPE.CLOSE:
            case ATTACK_TYPE.NONTARGET:
            case ATTACK_TYPE.TARGET:
            case ATTACK_TYPE.REVOLVE:
                //Debug.Log("player CloseType");
                Enemy closest = null;
                if (enemies.Count == 0)
                {
                    //Debug.Log("No Enemy");
                    target = null;
                    return false;
                }
                closest = enemies[0];
                for (int i = 1; i < enemies.Count; i++)
                {
                    if (Vector2.SqrMagnitude(closest.transform.position - transform.position) > Vector2.SqrMagnitude(boardManager.enemies[i].transform.position - transform.position))
                        closest = enemies[i];
                }
                target = closest;
                targetPosition = target.transform.position;
                if (target.targetDistance == 1000)
                    target = null;
                break;
        }
        return true;
    }

    private void MoveToGoal()
    {
        Debug.Log("MoveToGoal");
        agent.Stop();
        Move(boardManager.goalPoint);
        
        isWin = true;
        StartCoroutine(LevelUp());
        StartCoroutine(MoveToGoalRepeat());
    }

    private IEnumerator LevelUp()
    {
        float sqrDistance = 0;
#if !DELAY
        WaitForSeconds delay = new WaitForSeconds(0.1f); ;
#endif
        do
        {
#if DELAY
            yield return GameManager.instance.delayPointOne;
#else
            yield return delay;
#endif
            sqrDistance = Vector2.SqrMagnitude((Vector2)transform.position - boardManager.goalPoint);
        } while (sqrDistance > 0.1f);
        if(!_isDeath)
            LevelUpManager.instance.LevelUp();
    }

    private IEnumerator MoveToGoalRepeat()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        yield return null;
        while (true)
        {
            yield return delay ;
            if (isWin && !_isDeath)
            {
                if (isAutoMove)
                    Move(boardManager.goalPoint);
            }
            else
                break;
        }
    }

    public void Respawn()
    {
        isWin = false;
        agent.Stop();
        transform.position = boardManager.spawnPoint;
        StartCoroutine(WaitForRespawn());
    }

    IEnumerator WaitForRespawn()
    {
        WaitForSeconds delay = new WaitForSeconds(1.5f);
        bool isAuto = _isAutoMove;
        bool isAttack = attackable;
        _isAutoMove = false;
        attackable = false;
        yield return delay;
        _isAutoMove = isAuto;
        attackable = isAttack;
    }

    public void Revive(float percent)
    {
        _health = data.stat.hp * percent * 0.01f;
        _isDeath = false;
    }

    public void SetDeath(bool value)
    {
        _isDeath = value;
        animator.SetBool("isDeath", value);
    }
}
