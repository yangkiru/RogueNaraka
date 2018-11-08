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
                    if (Vector2.SqrMagnitude(closest.transform.position - transform.position) > Vector2.SqrMagnitude(boardManager.enemies[i].transform.position - transform.position))
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
                SetTarget();
                Attack();
            }
            Animation();
        }
    }

    protected override void AttackBullet(Bullet bullet, Vector2 vec, RevolveHolder holder)
    {
        Debug.Log("Bullet Attack!");
        bullet.Attack(transform.position, weapon.spawnPoint, Vector2.down, weapon.localSpeed, weapon.worldSpeed, holder, this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        boardManager.scarecorws.Add(this);
    }

    public override float GetDamage(float damage, bool isTxtOnHead = true)
    {
        float _damage = base.GetDamage(damage, false);

        if (isTxtOnHead)
            PointTxtManager.instance.TxtOnHead(-_damage, transform, Color.red);
        return _damage;
    }

    public IEnumerator TimeLimit(float time)
    {
        yield return new WaitForSeconds(time);
        Suicide();
    }

    protected override void OnDeath()
    {
        Debug.Log("Death");
        boardManager.scarecorws.Remove(this);
        base.OnDeath();
        Destroy(gameObject);
    }
}
