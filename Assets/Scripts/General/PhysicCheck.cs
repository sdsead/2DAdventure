using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicCheck : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    
    public bool isGround;
    public bool leftWall;
    public bool rightWall;
    public bool onWall;
    public bool isPlayer;
    
    
    [SerializeField]
    public float radius;
    [SerializeField]private LayerMask layerMask;
    
    [SerializeField]private Vector2 bottomOffset;
    [SerializeField]private Vector2 rightOffset;
    [SerializeField]private Vector2 leftOffset;

    private void Awake()
    {
        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        Check();
    }

    private void Check()
    {
        //地面检测
        if(onWall)
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x,bottomOffset.y), radius,layerMask);
        else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x,bottomOffset.y), radius,layerMask);
         
        
        
        leftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, radius,layerMask);
        rightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, radius,layerMask);

        if (isPlayer)
        {
            if(rb.velocity.y < 0f)
                onWall = (leftWall && playerController.inputDirection.x < 0f || rightWall && playerController.inputDirection.x > 0f) && !isGround;
        }
            

    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x,bottomOffset.y), radius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, radius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, radius);
    }
}
