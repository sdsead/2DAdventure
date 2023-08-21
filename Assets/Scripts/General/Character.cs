using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveable
{
    [Header("监听")] public VoidEventSO newGameEvent;
    
    public bool isPlayer;
    
    [Header("属性")] public float maxHealth;
    public float currentHealth;
    [Space(30)]
    [Header("体力条属性")]
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    [Header("无敌")]
    public float invulnerableDuration;
    [HideInInspector]public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    
    public void TakeDamage(Attack attack)
    {
        if (invulnerable)
            return;
        
        if (currentHealth - attack.damage > 0)
        {
            currentHealth -= attack.damage;
            TrigerInvulnerable();
            OnTakeDamage?.Invoke(attack.transform);
        }
        else
        {
            //死亡
            currentHealth = 0;
            OnDie?.Invoke();
        }
        if(isPlayer)
            OnHealthChange?.Invoke(this);
    }


    public void TrigerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("water"))
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie?.Invoke(); 
            }
            
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();

            //通知UI更新
            OnHealthChange?.Invoke(this);
        }
    }
}
