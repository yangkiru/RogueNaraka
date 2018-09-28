using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player instance = null;

    public bool isMoveToAttack;
    public bool isMoveToMouse;

    private bool win;

    private List<Enemy> enemies;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        enemies = boardManager.enemies;
    }

    protected override void OnDeath()
    {
        gameManager.OnEnd();
    }

    protected void Update()
    {
        if (!isPause)
        {
            CheckDeath();
            if (!isDeath)
            {
                StunFunc();
                KnockBackFunc();
                if (!SetTarget() && !win)
                    MoveToGoal();
                else
                {
                    if (isMoveToMouse && Input.GetMouseButton(0))
                        Move(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (isMoveToAttack)
                        MoveToAttack();
                    Attack();
                }
                if (target != null)
                    targetPosition = target.transform.position;
            }
            Animation();
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
                    if (closest.targetDistance > enemies[i].targetDistance)
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
        Move(boardManager.goalPoint);
        
        win = true;
        StartCoroutine(LevelUp());
        StartCoroutine(MoveToGoalRepeat());
    }

    private IEnumerator LevelUp()
    {
        float sqrDistance = 0;
        do
        {
            yield return new WaitForSeconds(0.1f);
            sqrDistance = Vector2.SqrMagnitude((Vector2)transform.position - boardManager.goalPoint);
        } while (sqrDistance > 0.1f);
        if(!_isDeath)
            gameManager.LevelUp();
    }

    private IEnumerator MoveToGoalRepeat()
    {
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(2);
            if (win && !_isDeath)
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
        win = false;
        agent.SetDestination(boardManager.spawnPoint);
        transform.position = boardManager.spawnPoint;
        attackCoolTime = 0;
    }

    public void Revive(float percent)
    {
        _health = stat.hp * percent * 0.01f;
        _isDeath = false;
    }

    public void SetDeath(bool value)
    {
        _isDeath = value;
        animator.SetBool("isDeath", value);
    }
}
