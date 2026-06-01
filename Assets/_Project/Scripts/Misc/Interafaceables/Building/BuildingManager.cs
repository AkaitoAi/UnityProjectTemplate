using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Button button;

    private bool inBuilding = true;

    public static event Action<bool> OnBuildingStateChanged;

    private void Start()
    {
        button.onClick.RemoveAllListeners();
        button.gameObject.SetActive(false);
    }

    //private void DisableAll()
    //{
    //    foreach (var field in typeof(Buttons).GetFields())
    //    {
    //        GameObject obj = field.GetValue(buttons) as GameObject;
    //        if (obj != null) obj.SetActive(false);
    //    }
    //}

    private void OnEnable()
    {
        BuildingInteractable.OnEnterBuildingIntection += OnEnterBuildingInteractable;
        BuildingInteractable.OnExitBuildingInteraction += OnExitBuildingInteractable;
    }

    private void OnDisable()
    {
        BuildingInteractable.OnEnterBuildingIntection -= OnEnterBuildingInteractable;
        BuildingInteractable.OnExitBuildingInteraction -= OnExitBuildingInteractable;
    }

    private void OnEnterBuildingInteractable(BuildingInteractable.Building building, Transform player)
    {
        button.gameObject.SetActive(true);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            
            GameManager.GetInstance()?.faderCanvas.FadeIn(() => {

                button.gameObject.SetActive(false);
                
                inBuilding = !inBuilding;

                Transform currentPoint = inBuilding ? building.exitPoint : building.enterPoint;

                player.transform.position = new Vector3(currentPoint.position.x, player.position.y, currentPoint.position.z);
                player.transform.rotation = currentPoint.rotation;

                (inBuilding ? (Action)(() => building.OnExitEvent?.Raise()) : () => building.OnEnterEvent?.Raise())();

                OnBuildingStateChanged?.Invoke(inBuilding);

                GameManager.GetInstance()?.faderCanvas.FadeOut();

            });
        });
    }

    private void OnExitBuildingInteractable()
    { 
        button.gameObject.SetActive(false);

        button.onClick.RemoveAllListeners();
    }
}
