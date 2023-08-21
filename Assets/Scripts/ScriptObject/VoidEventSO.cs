using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/VoidEventSO")]
public class VoidEventSO : ScriptableObject
{
   public UnityAction OnEventRaised;

   public void OnRaiseEvent()
   {
      OnEventRaised?.Invoke();
   }
}
