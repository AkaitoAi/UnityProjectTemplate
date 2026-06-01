using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AkaitoAi.Singleton;
using AkaitoAi.GameBase;

public class MenuScreenManager : Singleton<MenuScreenManager>
{
    [SerializeField] private List<MenuScreenController> canvasControllerList;
    private MenuScreenController lastActiveCanvas;
    private static bool freshStart = true;

    private void Awake()
    {
        //canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(cc => cc.gameObject.SetActive(false));

        if (freshStart) { freshStart = false; SwitchCanvas(ScreenType.PlayerProfile); }
        else SwitchCanvas(ScreenType.MainMenu);
    }

    public void SwitchCanvas(ScreenType _cType)
    {
        if (lastActiveCanvas != null)
            lastActiveCanvas.gameObject.SetActive(false);

        MenuScreenController desiredCanvas = canvasControllerList.Find(cc => cc.screenType == _cType);

        if (desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            lastActiveCanvas = desiredCanvas;
        }
        else { Debug.LogWarning("The desired canvas was not found!"); }
    }
}
