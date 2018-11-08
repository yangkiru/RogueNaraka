using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss0 : Enemy {
    public float bodySize;

    private bool isWakeUp;
    private bool isDetected;
    private bool isAttacking;
    private bool isWalk;

    private Vector2 start;
    private Vector2 vec;

    protected override void OnEnable()
    {
        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(3);
        isWakeUp = true;
        base.OnEnable();
    }

    protected override void OnDeath()
    {
        MoneyManager.instance.AddSoul(data.cost);
        //Debug.Log("cost:" + data.cost + " enemy dead");
        boardManager.enemyPool.EnqueueObjectPool(gameObject);
    }

    protected override void MoveCheck()
    {
        agent.maxSpeed = 0.5f + data.stat.spd / 3;
        float randX = Random.Range(-data.stat.spd * 2, data.stat.spd * 2);
        float randY = Random.Range(-data.stat.spd * 0.5f, data.stat.spd * 3.5f);

        float x = transform.position.x + randX;
        float y = transform.position.y + randY;

        
        if (Mathf.Abs(x) > boardManager.boardRange[1].x)
        {
            if (x > boardManager.boardRange[0].x)
                x = boardManager.boardRange[1].x;
            else
                x = boardManager.boardRange[0].x;
        }
        if (Mathf.Abs(y) > boardManager.boardRange[1].y)
        {
            if (y > boardManager.boardRange[0].y)
                y = boardManager.boardRange[1].y;
            else
                y = boardManager.boardRange[0].y;
        }
        //Debug.Log("Move to x:" + x + " y:" + y);
        isWalk = true;
        agent.SetDestination(new Vector2(x, y));
    }

    protected override void Attack()
    {
        Debug.Log("Attack");
        agent.Stop();

        isDetected = true;
        isAttacking = true;
        isWalk = false;
    }

    protected override void OnStunEnd()
    {
        base.OnStunEnd();
        Debug.Log("OnStunEnd");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, bodySize);
    }

    protected override void Update()
    {
        TargetUpdate();
        if (isWakeUp && !isAttackCool && !isStun)
        {
            if (!isDetected)
            {
                vec = targetPosition - (Vector2)transform.position;
                start = (Vector2)transform.position + vec.normalized * 2.5f;

                RaycastHit2D hit = Physics2D.Raycast(start, vec, 10, GameDatabase.instance.playerMask);//friendly layer detected
                if (hit)
                {
                    //Debug.Log("Dectected");
                    if (transform.position.y > targetPosition.y)
                        Attack();
                }
            }
            else
            {
                //Debug.Log("Going Attack");
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, bodySize, vec, 0.1f, GameDatabase.instance.playerMask);
                //RaycastHit2D hit = Physics2D.Raycast(transform.position, vec, bodySize, GameDatabase.instance.playerMask);
                if (!hit)
                {
                    Debug.Log("Dash");
                    Vector2 check = (Vector2)transform.position + vec.normalized * 0.1f;
                    if (Mathf.Abs(check.y) > boardManager.boardRange[0].y + bodySize && Mathf.Abs(check.x) < boardManager.boardRange[1].x - bodySize)
                    //if (transform.position.y > boardManager.boardRange[0].y + bodySize && transform.position.x > boardManager.boardRange[0].x + bodySize
                        //&& transform.position.x < boardManager.boardRange[1].x - bodySize)
                        transform.Translate(vec.normalized * data.stat.spd * Time.deltaTime);
                    else
                    {
                        Debug.Log("Target Lost");
                        AddEffect(EFFECT.STUN, 2, 2);
                        isAttacking = false;
                        isDetected = false;
                    }
                }
                else
                {
                    Debug.Log("Hit");
                    attackCoolTime = 3 - data.stat.spd * 0.1f;
                    isAttackCool = true;
                    isAttacking = false;
                    if (hit.transform.gameObject.layer == 8)//friendly hit
                    {
                        Debug.Log("Hit Player");
                        hit.transform.GetComponent<Unit>().GetDamage(data.stat.dmg);
                    }
                    AddEffect(EFFECT.STUN, 2, 2);
                    isDetected = false;
                }
            }
        }
        Animation();
    }

    protected override void Animation()
    {
        animator.SetBool("isAttacking", isAttacking && isWakeUp);
        animator.SetBool("isWalk", isWalk && isWakeUp);
        animator.SetFloat("y", agent.velocity.y);
        animator.SetBool("isStun", isStun);
    }
}
