using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeChase : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    
    private float attackRateCounter = 0;
    private bool isAttack;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        attack = enemy.transform.GetChild(0).GetComponent<Attack>();

        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        
        currentEnemy.anim.SetBool("chase",true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(EnemyState.Patrol);
        }

        attackRateCounter -= Time.deltaTime;
        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f, 0);
        
        //判断攻击距离
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackRange &&
            Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            isAttack = true;
            if(!currentEnemy.isHurt)
                currentEnemy.rb.velocity = Vector2.zero;
            if (attackRateCounter <= 0)
            {
                attackRateCounter = attack.attackRate;
                currentEnemy.anim.SetTrigger("attack");
            }
        }
        else
        {
            isAttack = false;
        }
        
        
        moveDir = (target - currentEnemy.transform.position).normalized;
        
        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        if(moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase",false);
    }
}
