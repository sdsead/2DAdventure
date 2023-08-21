using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Enemy
{
    public float patrolRadius;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new BeePatrol();
        chaseState = new BeeChase();
    }

    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position, checkDistance, checkLayer);
        if (obj)
        {
            attacker = obj.transform;
        }

        return obj;
    }


    protected override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint, patrolRadius);
    }

    public override Vector3 GetNewPoint()
    {
        var tagetX = Random.Range(-patrolRadius,patrolRadius);
        var tagetY = Random.Range(-patrolRadius,patrolRadius);

        return spawnPoint + new Vector3(tagetX, tagetY, 0);
    }

    public override void Move()
    {
        
    }
}
