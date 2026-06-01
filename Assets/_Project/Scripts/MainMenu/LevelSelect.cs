using UnityEngine;
using UnityEngine.UI;
using AkaitoAi.GameBase;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private LevelDetails[] modeLevels;

    internal SetupSO setupScriptable;

    public static int totalModeLevels;

    private UnityEngine.Events.UnityAction[] selectLevelActions;
    private UnityEngine.Events.UnityAction[] unlockLevelActions;

    //Bindings
    EventBinding<OnModeSelected> modeSelectedEventBinding;
    EventBinding<OnModeSkipLevelSelected> modeSkipLevelSelectedEventBinding;

    [System.Serializable]
    public struct LevelDetails
    {
        internal LevelSelectSO levelSelectSO;
        public Button[] buttons;
        public Button[] unlockButtons;
        
        public bool unlockAllLevels;

        public GameObject parentContainer;
        public RectTransform scrollViewRectTransform;
        public ScrollRect scrollRect;
        public RectTransform contentRectTransfrom;

        internal float oldPos;
    }

    private void ContentSetup()
    {
        if (modeLevels == null) return;

        LoadData();

        foreach (LevelDetails _mLevels in modeLevels)
        {
            if (_mLevels.contentRectTransfrom) _mLevels.contentRectTransfrom.gameObject.SetActive(false);
            if (_mLevels.scrollRect) _mLevels.scrollRect.gameObject.SetActive(false);
            if (_mLevels.parentContainer) _mLevels.parentContainer.SetActive(false);
        }

        LevelDetails currentMode = modeLevels[setupScriptable.modeIndex];
        if (currentMode.buttons.Length > 0 || currentMode.buttons != null)
        {
            if (selectLevelActions != null)
            {
                for (int i = 0; i < currentMode.buttons.Length; i++)
                {
                    currentMode.buttons[i].onClick.RemoveListener(selectLevelActions[i]);
                    currentMode.unlockButtons[i].onClick.RemoveListener(unlockLevelActions[i]);
                }
            }

            // Rebuild actions array
            selectLevelActions = new UnityEngine.Events.UnityAction[currentMode.buttons.Length];
            unlockLevelActions = new UnityEngine.Events.UnityAction[currentMode.unlockButtons.Length];

            // Assign OnModeSelect to modeButtons with their respective indexes
            for (int i = 0; i < currentMode.buttons.Length; i++)
            {
                // Create local copy of index for closure
                int index = i;

                selectLevelActions[i] = () => SelectLevel(index);
                unlockLevelActions[i] = () => LevelUnlockReward(index);

                currentMode.buttons[i].onClick.AddListener(selectLevelActions[i]);
                currentMode.unlockButtons[i].onClick.AddListener(unlockLevelActions[i]);
            }
        }

        modeLevels
            [setupScriptable.modeIndex].
            contentRectTransfrom.gameObject.SetActive(true);
        modeLevels
            [setupScriptable.modeIndex].
            scrollRect.gameObject.SetActive(true);

        if (currentMode.parentContainer)
            currentMode.parentContainer.SetActive(true);

        EnableModeLevelButtons(currentMode.levelSelectSO.totalLevels);
        SetLeft(currentMode.contentRectTransfrom, currentMode.levelSelectSO.ContentLeft);
        SetRight(currentMode.contentRectTransfrom, currentMode.levelSelectSO.ContentRight);

        CheckStatus();

        void EnableModeLevelButtons(int _levels)
        {
            foreach (Button levelBtn in currentMode.buttons)
                levelBtn.gameObject.SetActive(false);

            int length = _levels;
            for (int i = 0; i < length; i++)
                currentMode.buttons[i].gameObject.SetActive(true);
        }

        void SetLeft(RectTransform rt, float left) =>
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);

        void SetRight(RectTransform rt, float right) =>
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    private void LockLevel(int _level)
    {
        bool isUnlocked = modeLevels[setupScriptable.modeIndex].levelSelectSO.IsLevelUnlocked(_level);

        ToggleLevelLock(_level, isUnlocked);
        setupScriptable.levelIndex = _level;

        //if (_level > PlayerPrefs.GetInt(modeLevels[setupScriptable.sModeIndex].levelSelectSO.prefName))
        //{
        //    ToggleLevelLock(_level, false);
        //}
        //else
        //{
        //    setupScriptable.sLevelIndex = _level;
        //    ToggleLevelLock(_level, true);
        //}

        void ToggleLevelLock(int _index, bool _state)
        {
            if (modeLevels[setupScriptable.modeIndex].levelSelectSO.levelLock.use)
                modeLevels[setupScriptable.modeIndex].buttons[_index].
                    transform.GetChild(modeLevels[setupScriptable.modeIndex].levelSelectSO.levelLock.childIndex).gameObject.SetActive(!_state);

            if (modeLevels[setupScriptable.modeIndex].levelSelectSO.separateLevelNumber.use)
                modeLevels[setupScriptable.modeIndex].buttons[_index].
                    transform.GetChild(modeLevels[setupScriptable.modeIndex].levelSelectSO.separateLevelNumber.childIndex).gameObject.SetActive(_state);

            if (modeLevels[setupScriptable.modeIndex].buttons[_index].TryGetComponent(out Button _btn1)) _btn1.interactable = _state;

            TogglePlayedStatusScale(_index);

            void TogglePlayedStatusScale(int _levelIndex)
            {
                if (!modeLevels[setupScriptable.modeIndex].levelSelectSO.useLevelStatus) return;

                if (modeLevels[setupScriptable.modeIndex].buttons[_index].TryGetComponent(out Button _btn))
                {
                    if (!_btn.interactable) return;
                }

                modeLevels[setupScriptable.modeIndex].levelSelectSO.unlockedLevels =
                    PlayerPrefs.GetInt(modeLevels[setupScriptable.modeIndex].levelSelectSO.namePref);

                bool isPlayed = PlayerPrefs.GetInt(modeLevels[setupScriptable.modeIndex].levelSelectSO.isPlayedPref + _levelIndex) == 1 ? true : false;

                ToggleButtonShiny(isPlayed);
                ToggleButtonAnimation(isPlayed);
                TogglePlayStatus(isPlayed);

                return;

                if (_levelIndex == modeLevels[setupScriptable.modeIndex].levelSelectSO.unlockedLevels)
                {
                    ToggleButtonShiny(true);
                    ToggleButtonAnimation(true);
                    TogglePlayStatus(isPlayed);
                }

                if (_levelIndex < modeLevels[setupScriptable.modeIndex].levelSelectSO.unlockedLevels)
                {
                    ToggleButtonShiny(false);
                    ToggleButtonAnimation(false);
                    TogglePlayStatus(isPlayed);
                }

                void ToggleButtonShiny(bool _state)
                {
                    //if (modeLevels[setupScriptable.sModeIndex].buttons[_levelIndex].TryGetComponent<UIShiny>(out UIShiny uiShiny)) uiShiny.enabled = _state;
                }
                void ToggleButtonAnimation(bool _state)
                {
                    if (modeLevels[setupScriptable.modeIndex].buttons[_levelIndex].TryGetComponent(out Animation animation)) animation.enabled = _state;
                }

                void TogglePlayStatus(bool _state)
                {
                    if (!modeLevels[setupScriptable.modeIndex].levelSelectSO.levelComplete.use &&
                        !modeLevels[setupScriptable.modeIndex].levelSelectSO.levelToPlay.use)
                        return;

                    modeLevels[setupScriptable.modeIndex].buttons[_levelIndex].
                        transform.GetChild(modeLevels[setupScriptable.modeIndex].levelSelectSO.levelToPlay.childIndex).gameObject.SetActive(!_state);

                    modeLevels[setupScriptable.modeIndex].buttons[_levelIndex].
                        transform.GetChild(modeLevels[setupScriptable.modeIndex].levelSelectSO.levelComplete.childIndex).gameObject.SetActive(_state);
                }
            }
        }
    }

    private void LoadData()
    {
        if (modeLevels == null) return;

        setupScriptable =
            RuntimeSetup.GetInstance()?.Setup;

        int modeLevelsLength = modeLevels.Length;
        for (int i = 0; i < modeLevelsLength; i++)
        {
            modeLevels[i].levelSelectSO = RuntimeSetup.GetInstance()?.Levels[i];
                //Resources.Load(
                //    "ScriptableObjects/LevelSelect/LevelSelect_" + i
                //    )
                //as LevelSelectSO;

            PlayerPrefs.SetInt(modeLevels[i].levelSelectSO.isUnlockedPref + 0, 1);
        }

        totalModeLevels = modeLevels.Length;

        //! Unlocks All Levels
        if (modeLevels[setupScriptable.modeIndex].unlockAllLevels)
            PlayerPrefs.SetInt(modeLevels[setupScriptable.modeIndex].levelSelectSO.namePref,
                modeLevels[setupScriptable.modeIndex].levelSelectSO.totalLevels);
    }

    public void SelectLevel(int _index)
    {
        setupScriptable.levelIndex = _index;

        //string sceneToLoad = modeLevels[setupScriptable.modeIndex].levelSelectSO.useMoreThan2Scenes ? 
        //    $"Mode{setupScriptable.modeIndex}LevelScene{_index}" : $"Mode{setupScriptable.modeIndex}LevelScene";
        //EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromNames(new string[] { "2_GameplayScene", sceneToLoad }) });

        int sceneToLoad = modeLevels[setupScriptable.modeIndex].levelSelectSO.useMoreThan2Scenes ? _index + 3 : 3;
        EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromIndices(new int[] { 2, sceneToLoad }) });
    }

    public void ScrollRectSound()
    {
        if (modeLevels[setupScriptable.modeIndex].scrollRect.horizontalNormalizedPosition > 
            (modeLevels[setupScriptable.modeIndex].oldPos + modeLevels[setupScriptable.modeIndex].levelSelectSO.scrollSoundDelay))
        {
            modeLevels[setupScriptable.modeIndex].oldPos = modeLevels[setupScriptable.modeIndex].scrollRect.horizontalNormalizedPosition;

            //TODO Sound Calling
            AkaitoAi.SoundManager.Instance.PlayScrollRectSound();
        }
        else if (modeLevels[setupScriptable.modeIndex].scrollRect.horizontalNormalizedPosition < 
            (modeLevels[setupScriptable.modeIndex].oldPos - modeLevels[setupScriptable.modeIndex].levelSelectSO.scrollSoundDelay))
        {
            modeLevels[setupScriptable.modeIndex].oldPos = modeLevels[setupScriptable.modeIndex].scrollRect.horizontalNormalizedPosition;

            //TODO Sound Calling
            AkaitoAi.SoundManager.Instance.PlayScrollRectSound();
        }
    }

    public void LevelUnlockReward(int levelIndex)
    {
        LevelSelectSO levelSO = modeLevels[setupScriptable.modeIndex].levelSelectSO;

        levelSO.LevelUnlockReward(levelIndex, () =>
        {
            CheckStatus();
        });

        setupScriptable.levelIndex = levelIndex;
    }

    private void CheckStatus()
    {
        int length = modeLevels[setupScriptable.modeIndex].buttons.Length;
        for (int i = 0; i < length; i++)
            LockLevel(i);
    }

    private void OnEnable()
    {
        modeSelectedEventBinding = new EventBinding<OnModeSelected>(ContentSetup);
        EventBus<OnModeSelected>.Register(modeSelectedEventBinding);

        modeSkipLevelSelectedEventBinding = new EventBinding<OnModeSkipLevelSelected>(LoadData);
        EventBus<OnModeSkipLevelSelected>.Register(modeSkipLevelSelectedEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnModeSelected>.Deregister(modeSelectedEventBinding);
        EventBus<OnModeSkipLevelSelected>.Deregister(modeSkipLevelSelectedEventBinding);

    }
}
