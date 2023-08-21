using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicCheck physicCheck;
    private PlayerController playerController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicCheck = GetComponent<PhysicCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimations();
    }

    private void SetAnimations()
    {
        anim.SetFloat("velocityX",Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY",rb.velocity.y);
        anim.SetBool("isGround",physicCheck.isGround);
        anim.SetBool("isCrouch",playerController.isCrouch);
        anim.SetBool("isDead",playerController.isDead);
        anim.SetBool("isAttack",playerController.isAttack);
        anim.SetBool("onWall",physicCheck.onWall);
        anim.SetBool("isSlide",playerController.isSlide);
    }
    
    public void SetHurtTrigger()
    {
        anim.SetTrigger("hurt");
    }

    public void SetAttackTriger()
    {
        anim.SetTrigger("attack");
    }
}
