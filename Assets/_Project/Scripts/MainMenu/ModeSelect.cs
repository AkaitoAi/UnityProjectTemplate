using AkaitoAi.Advertisement;
using UnityEngine;
using AkaitoAi.GameBase;

public class ModeSelect : MonoBehaviour
{
    [SerializeField] private int[] onSkipLevelScenesToLoad = new int[] { 2, 3 };

    public void OnModeSelect(int _index)
    {
        RuntimeSetup.GetInstance().Setup.modeIndex = _index;

        EventBus<OnModeSelected>.Raise(new OnModeSelected { });

        AdsWrapper.GetInstance()?.FirebaseLog("Mode_" + _index);
    }

    //! Loads data only
    public void OnModeSkipLevel(int _index)
    {
        RuntimeSetup.GetInstance().Setup.modeIndex = _index;
        RuntimeSetup.GetInstance().Setup.levelIndex = 0; //! Forces to load level 1 (child 0) in gameplay

        EventBus<OnModeSkipLevelSelected>.Raise(new OnModeSkipLevelSelected { });

        EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromIndices(onSkipLevelScenesToLoad) });

        AdsWrapper.GetInstance()?.FirebaseLog("Mode_" + _index);
    }
}
