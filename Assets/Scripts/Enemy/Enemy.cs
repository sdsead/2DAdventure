using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector]public Animator anim;
    [HideInInspector]public Rigidbody2D rb;
    [HideInInspector]public PhysicCheck physicCheck;
    [HideInInspector]public Character character;
    
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public int faceDir; //向右为 1
    public Vector3 spawnPoint;

    public int hurtForce;
    public Transform attacker;
    
    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    
    [Header("计时器")] 
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;

    public float lostTime;
    public float lostTimeCounter;
    
    
    
    [Header("检测")] 
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask checkLayer;
    
    
    
    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;
    
    
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicCheck = GetComponent<PhysicCheck>();
        character = GetComponent<Character>();

        spawnPoint = transform.position;
        currentSpeed = normalSpeed;
    }


    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = (int)-transform.localScale.x;
        
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isDead && !wait)
            Move();
        
        currentState.PhysicsUpdate();
    }


    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("SnailPreMove") && !anim.GetCurrentAnimatorStateInfo(0).IsName("SnailRecover"))
            rb.velocity = new Vector2(currentSpeed * faceDir * Time.deltaTime, rb.velocity.y);

    }

    /// <summary>
    /// 计时器
    /// </summary>
    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir, 1, 1);
            }        
        }

        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }else if (FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }

    
    public virtual bool FoundPlayer()
    {

        var temp = Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, new Vector2(faceDir,0),checkDistance, checkLayer);
        
        return temp;

    }

    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }
    
    
    public void SwitchState(EnemyState state)
    {
        var newState = state switch
        {
            EnemyState.Patrol => patrolState,
            EnemyState.Chase => chaseState,
            EnemyState.Skill => skillState,
            _ => null
        };
        
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }
    
    
    public void OnTakeDamage(Transform attack)
    {
        attacker = attack;

        if (attack.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (attack.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        isHurt = true;
        anim.SetTrigger("hurt");

        Vector2 dir = new Vector2(transform.position.x - attack.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        
        StartCoroutine(OnHurt(dir));

    }


    IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }


    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetTrigger("dead");
        isDead = true;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }


    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0) ,0.2f);
    }
    
    
    
}
