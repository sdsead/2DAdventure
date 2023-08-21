using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour,IInteractable
{
    public Vector3 positionToGo;

    public GameSceneSO sceneToGo;
    public SceneLoadEventSO loadEventSo;
    public void TriggerAction()
    {
        loadEventSo.RaiseLoadRequestEvent(sceneToGo, positionToGo,true);
    }
    
    
}
