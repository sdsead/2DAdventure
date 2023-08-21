public class SnailSkill : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = 0;
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.character.invulnerable = true;
        currentEnemy.character.invulnerableCounter = currentEnemy.lostTimeCounter;
        
        currentEnemy.anim.SetBool("hide",true);
        currentEnemy.anim.SetTrigger("skill");
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(EnemyState.Patrol);
        }

        currentEnemy.character.invulnerableCounter = currentEnemy.lostTimeCounter;
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("hide",false);
        currentEnemy.character.invulnerable = false;
    }
}
