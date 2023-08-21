using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public PlayAudioEventSO playAudioEvent;
    public AudioClip AudioClip;

    public bool playOnEnable;

    private void OnEnable()
    {
        if(playOnEnable)
            PlayAudioClip();
    }

    public void PlayAudioClip()
    {
        playAudioEvent.RaisedEvent(AudioClip);
    }
}