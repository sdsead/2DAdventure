using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("监听")] public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;
    
    
    private PlayerControl playerControl;
    private Rigidbody2D rb;
    private PhysicCheck physicCheck;
    private CapsuleCollider2D coll;
    private PlayerAnimation playerAnimation;
    private Character character;
    
    public Vector2 inputDirection;
    [SerializeField]private float speed;
    private float walkSpeed;
    private float runSpeed;
    [SerializeField]private float jumpForce;
    [SerializeField]private float wallJumpForce;

    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;
    
    public bool isCrouch;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    public float hurtForce;
    public bool isHurt;
    public bool isAttack;
    public bool isDead;
    public bool wallJump;
    public bool isSlide;
    
    [Header("物理材质")] 
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;
    
    private void Awake()
    {
        physicCheck = GetComponent<PhysicCheck>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        playerControl = new PlayerControl();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();
        
        originalOffset = coll.offset;
        originalSize = coll.size;
        
        walkSpeed = speed / 2.5f;
        runSpeed = speed;
        playerControl.Enable();
        
        playerControl.Gameplay.Jump.started += Jump;

        #region 设置行走
        playerControl.Gameplay.Walk.performed += ctx =>
        {
            if (physicCheck.isGround)
                speed = walkSpeed;
        };
        playerControl.Gameplay.Walk.canceled += ctx =>
        {
            if (physicCheck.isGround)
                speed = runSpeed;
        };
        
        #endregion

        playerControl.Gameplay.Attack.started += PlayerAttack;

        playerControl.Gameplay.Slide.started += Slide;
    }
    
    private void OnEnable()
    {
        sceneLoadEvent.LoadRequestEvent += OnloadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }

    private void OnDisable()
    {
        playerControl.Disable();
        sceneLoadEvent.LoadRequestEvent -= OnloadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }

    private void Update()
    {
        inputDirection = playerControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
            Move();
    }
    
    private void OnAfterSceneLoadedEvent()
    {
        playerControl.Gameplay.Enable();
    }

    private void OnloadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        playerControl.Gameplay.Disable();
    }

    /// <summary>
    /// 读取游戏进度
    /// </summary>
    private void OnLoadDataEvent()
    {
        isDead = false;
    }
    
  
    void Move()
    {
        if(!isCrouch && !wallJump)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        
        
        //翻转
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;
        transform.localScale = new Vector3(faceDir, 1, 1);
        
        //下蹲
        isCrouch = inputDirection.y < -0.1 && physicCheck.isGround;
        if (isCrouch)
        {
            coll.offset = new Vector2(-0.05f, 0.75f);
            coll.size = new Vector2(0.68f, 1.5f);
        }
        else
        {
            coll.size = originalSize;
            coll.offset = originalOffset;
        }

    }
    
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicCheck.isGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            //打断滑铲
            isSlide = false;
            StopAllCoroutines();
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else if(physicCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x,2.5f) * wallJumpForce,ForceMode2D.Impulse);
            wallJump = true;
        }
    }
    
    /// <summary>
    /// 滑墙
    /// </summary>
    /// <param name="obj"></param>
    private void Slide(InputAction.CallbackContext obj)
    {
        if (character.currentPower < slidePowerCost)
            return;
        
        if (!isSlide && physicCheck.isGround)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x,
                transform.position.y);

            
            StartCoroutine(TriggerSlide(targetPos));
            
            
            character.OnSlide(slidePowerCost);
            
        }
    }

    IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if(!physicCheck.isGround)
                break;
            if (physicCheck.leftWall && transform.localScale.x < 0f || physicCheck.rightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }
            
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x )>= 0.1f);

        isSlide = false;
       
    }
    
    
    /// <summary>
    /// 受伤后击退效果 
    /// </summary>
    /// <param name="attacker"></param>
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        
        Vector2 Dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        
        rb.AddForce(Dir*hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        playerControl.Gameplay.Disable();
    }
    
    private void CheckState()
    {
        if (isDead || isSlide)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");

        coll.sharedMaterial = physicCheck.isGround ? normal : wall;
        
        if (physicCheck.onWall && !wallJump)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        
        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }
        

    }


    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.SetAttackTriger();
        isAttack = true;
    }
}
