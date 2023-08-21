public class BoarPatrol : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;

    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(EnemyState.Chase);
        }
        
        if (!currentEnemy.physicCheck.isGround ||(currentEnemy.physicCheck.leftWall && currentEnemy.faceDir < 0) || (currentEnemy.physicCheck.rightWall && currentEnemy.faceDir > 0))
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk",false);
        }
        else
        {
            currentEnemy.anim.SetBool("walk",true);
        }
    }
    

    

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        
        currentEnemy.anim.SetBool("walk", false);
    }
}
