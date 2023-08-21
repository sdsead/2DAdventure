using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChase : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(EnemyState.Patrol);
        }
        
        if (!currentEnemy.physicCheck.isGround ||(currentEnemy.physicCheck.leftWall && currentEnemy.faceDir < 0) || (currentEnemy.physicCheck.rightWall && currentEnemy.faceDir > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("run", false);
    }
}


