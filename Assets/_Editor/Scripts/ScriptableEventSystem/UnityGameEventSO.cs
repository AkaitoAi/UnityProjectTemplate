using System.Collections.Generic;
using UnityEngine;
using AkaitoAi.Events;

[CreateAssetMenu(fileName = "UnityEvent", menuName = "ScriptableObjects/UnityEvent", order = 1)]
public class UnityGameEventSO : ScriptableObject
{
    private List<UnityGameEventListener> listeners = new List<UnityGameEventListener>();

    public void RegisterListener(UnityGameEventListener listener) => 
        listeners.Add(listener);
    public void UnRegisterListener(UnityGameEventListener listener) => 
        listeners.Remove(listener);

    public void Raise()
    {
        int ln = listeners.Count - 1;
        for (int i = ln; i >= 0; i--)
            listeners[i].OnEventRaised();
    }
}
