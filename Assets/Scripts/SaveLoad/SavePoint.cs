using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SavePoint : MonoBehaviour , IInteractable
{
    [Header("广播")] public VoidEventSO SaveDataEvent;
    
    private SpriteRenderer spriteRenderer;
    public GameObject lightObj;
    
    public Sprite lightSprite;
    public Sprite darkSprite;

    public bool isDone;

    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        
        lightObj.SetActive(isDone);
    }


    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);
            
            SaveDataEvent.OnRaiseEvent();
            this.gameObject.tag = "Untagged";
        }
    }
}
