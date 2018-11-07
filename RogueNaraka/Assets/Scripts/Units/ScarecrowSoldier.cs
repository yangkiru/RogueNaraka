using UnityEngine;
using System.Collections;

public class ScarecrowSoldier : Unit
{

    public override bool SetTarget()
    {
        try
        {
            if (boardManager.enemies.Count >= 1)
            {
                Enemy closest = null;
                if (boardManager.enemies.Count == 0)
                {
                    //Debug.Log("No Enemy");
                    target = null;
                    return false;
                }
                closest = boardManager.enemies[0];
                for (int i = 1; i < boardManager.enemies.Count; i++)
                {
                    if (closest.targetDistance > boardManager.enemies[i].targetDistance)
                        closest = boardManager.enemies[i];
                }
                target = closest;
                targetPosition = target.transform.position;
                if (target.targetDistance == 1000)
                    target = null;
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

    protected virtual void Update()
    {
        if (!isPause)
        {
            SyncData();
            CheckDeath();
            if (!isDeath)
            {
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

    protected override void AttackBullet(Bullet bullet, Vector2 vec, RevolveHolder holder)
    {
        bullet.Attack(transform.position, weapon.spawnPoint, Vector2.down, weapon.localSpeed, weapon.worldSpeed, holder, this);
    }
}
