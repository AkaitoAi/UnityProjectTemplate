using UnityEngine;
using UnityEngine.UI;
using AkaitoAi.GameBase;

[RequireComponent(typeof(Button))]
public class MenuScreenSwitcher : MonoBehaviour
{
    public ScreenType desiredScreenType;

    private MenuScreenManager menuCanvasManager;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
        menuCanvasManager = MenuScreenManager.GetInstance();
    }

    private void OnButtonClicked() => menuCanvasManager.SwitchCanvas(desiredScreenType);
}
