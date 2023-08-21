using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    private PlayerControl playerControl;
    public Transform playrTrans;
    private Animator anim;
    public GameObject sign;
    private bool canPress;

    private IInteractable targetItem;

    private void Awake()
    {
        anim = sign.GetComponent<Animator>();

        playerControl = new PlayerControl();
        playerControl.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerControl.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;
            switch (d.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                
                case XInputControllerWindows:
                    anim.Play("xbox");
                    break;
            }
        }
    }

    private void Update()
    {
        sign.GetComponent<SpriteRenderer>().enabled = canPress;
        sign.transform.localScale = playrTrans.localScale;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
