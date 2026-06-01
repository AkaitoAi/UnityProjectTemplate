using System;
using UnityEngine;

public class BuildingInteractable : MonoBehaviour, ITriggerEnterable, ITriggerExitable
{
    [field: SerializeField] public Building _Building { get; private set; }

    public static event Action<Building, Transform> OnEnterBuildingIntection;
    public static event Action OnExitBuildingInteraction;

    [Serializable] public struct Building
    {
        public Transform enterPoint, exitPoint;

        public UnityGameEventSO OnEnterEvent, OnExitEvent;
    }

    public void TriggerEnter(Interactor interactor)
    {
        OnEnterBuildingIntection?.Invoke(_Building, interactor.transform);
    }

    public void TriggerExit(Interactor interactor)
    {
        OnExitBuildingInteraction?.Invoke();
    }
}
