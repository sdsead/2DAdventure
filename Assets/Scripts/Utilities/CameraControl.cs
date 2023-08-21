using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public VoidEventSO afterSceneLoadedEvent;
    
    private CinemachineConfiner2D confiner2D;
    [SerializeField]private CinemachineImpulseSource cinemachineImpulseSource;
    public VoidEventSO shakeEvent;
    
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    /*private void Start()
    {
        GetNewCameraBounds();
    }*/

    private void OnEnable()
    {
        shakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoaded;
    }

    private void OnDisable()
    {
        shakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoaded;
    }

    private void OnAfterSceneLoaded()
    {
        GetNewCameraBounds();
    }


    private void OnCameraShakeEvent()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }

    public void GetNewCameraBounds()
    {
        var obj = GameObject.FindWithTag("Bounds");
        if (obj == null)
        {
            return;
        }

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        //清除缓存
        confiner2D.InvalidateCache();
    }
}

