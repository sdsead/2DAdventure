using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{

    public UnityAction<Color, float, bool> OnEnventRaised;
    public void FadeIn(float duration)
    {
        RaisedEvent(Color.black, duration,true);
    }

    public void FadeOut(float duration)
    {
        RaisedEvent(Color.clear,duration,false);
    }

    public void RaisedEvent(Color taget, float duration, bool fadeIn)
    {
        OnEnventRaised?.Invoke(taget,duration,fadeIn);
    }
}
