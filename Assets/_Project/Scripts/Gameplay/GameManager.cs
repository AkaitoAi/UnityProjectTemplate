using AkaitoAi.Advertisement;
using AkaitoAi.Extensions;
using AkaitoAi.GameBase;
using AkaitoAi.Singleton;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if DOTWEEN
using DG.Tweening;
#endif

public class GameManager : Singleton<GameManager>
{
    [Header("Gameplay States")]
    public GameplayScreens state = new GameplayScreens();
    [SerializeField]
    private GameObject gameScreenCanvas, resumeScreen, pauseScreen,
        failScreen, winScreen, cutSceneScreen, settingScreen;

    [Space(10)]
    [Header("Additional")]
    public CanvasFader faderCanvas;
    public ObjectiveScreen objectiveScreen;
    public LevelTimer levelTimer;
    internal TimerController timeController;

    [Space(10)]
    public ModeLevelSetup[] modeLevelSetup;

    [Header("Game Setup")]
    internal SetupSO setupScriptable;
    internal GameObject selectedEnvironment;
    internal GameObject levelsParentObj;
    internal GameObject selectedLevel;
    internal GameObject selectedPlayer;

    [Header("Level Setup")]
    internal LevelSelectSO currentLevelSelectSO;
    internal LevelInfo levelInfo;
    private int levelWinReward = 1000;
    private int timeReward = 1000;
    private int reward;
    private int totalReward;
    internal int levelNumber;
    internal int selectedMode;
    internal int controlIndex;
    internal int muteSound;
    internal int musicMuteSound;
    internal int sfxMuteSound;
    internal int levelCoins;
    internal float sfxVolume;
    internal float musicVolume;
    internal float masterVolume;
    private bool showAds = false;
    private bool addCoins = false;
    private bool winPanelStateEnabled = false;
    private bool showWinPanel = false;
    private bool granted2xReward = false;
    private AudioSource counterAS;
    private float levelWinDelay;

    [Space(10)]
    [Header("Lerp Setup")]
    [SerializeField] private float counterDuration = .5f;
    [SerializeField] private TMP_Text totalWinRewardText;
    [SerializeField] private TMP_Text winTimeText;
    [SerializeField] private TMP_Text coinText;

    internal bool inResume = false;
    internal bool inCutScene = false;

    [Space(30)]
    public UnityEvent OnScreenNoneEvent;
    public UnityEvent OnResumeScreenEvent;

    EventBinding<OnLevelWon> levelWonEventBinding;
    EventBinding<OnLevelFailed> levelFailedEventBinding;
    EventBinding<OnEnvironmentSpawned> spawnedEnvironmentEventBinding;
    EventBinding<OnLevelSpawned> spawnedLevelEventBinding;
    EventBinding<OnPlayerSpawned> spawnedPlayerEventBinding;

    private void Awake()
    {
        Time.timeScale = 1f;

        //! Loads Setup which are configured in menu scene and assigning relevant variables
        setupScriptable = RuntimeSetup.GetInstance().Setup;

        levelNumber = setupScriptable.levelIndex;
        selectedMode = setupScriptable.modeIndex;
        controlIndex = PlayerPrefs.GetInt(setupScriptable.controlPref);
        sfxVolume = PlayerPrefs.GetFloat(setupScriptable.sFXVolumePref);
        musicVolume = PlayerPrefs.GetFloat(setupScriptable.bGVolumePref);
        masterVolume = PlayerPrefs.GetFloat(setupScriptable.masterVolumePref);
        muteSound = PlayerPrefs.GetInt(setupScriptable.muteAudioPref);
        musicMuteSound = PlayerPrefs.GetInt(setupScriptable.musicMutePref);
        sfxMuteSound = PlayerPrefs.GetInt(setupScriptable.sFXMutePref);

        //! Loads LevelDetails 
        currentLevelSelectSO = RuntimeSetup.GetInstance()?.Levels[selectedMode];
        //Resources.Load("ScriptableObjects/LevelSelect/LevelSelect_" + selectedMode) as LevelSelectSO;

        state = GameplayScreens.Resume;
        UpdateGameplayState();

        //TODO Sounds Calling
        //SoundManager.Instance?.PlayGameplayBG();
        AkaitoAi.SoundManager.Instance.menuBGAudioSource.Stop();
    }

    private void OnEnable()
    {
        levelWonEventBinding =
            new EventBinding<OnLevelWon>(LevelWin);
        EventBus<OnLevelWon>.Register(levelWonEventBinding);

        levelFailedEventBinding =
            new EventBinding<OnLevelFailed>(LevelFailed);
        EventBus<OnLevelFailed>.Register(levelFailedEventBinding);

        spawnedEnvironmentEventBinding = new EventBinding<OnEnvironmentSpawned>(InitEnvironment);
        EventBus<OnEnvironmentSpawned>.Register(spawnedEnvironmentEventBinding);
        CachedEvent<OnEnvironmentSpawned>.Subscribe(InitEnvironment, invokeWithLast: true);

        spawnedLevelEventBinding = new EventBinding<OnLevelSpawned>(InitLevel);
        EventBus<OnLevelSpawned>.Register(spawnedLevelEventBinding);

        spawnedPlayerEventBinding = new EventBinding<OnPlayerSpawned>(InitPlayer);
        EventBus<OnPlayerSpawned>.Register(spawnedPlayerEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnLevelWon>.Deregister(levelWonEventBinding);
        EventBus<OnLevelFailed>.Deregister(levelFailedEventBinding);
        EventBus<OnEnvironmentSpawned>.Deregister(spawnedEnvironmentEventBinding);
        CachedEvent<OnEnvironmentSpawned>.Unsubscribe(InitEnvironment);
        EventBus<OnLevelSpawned>.Deregister(spawnedLevelEventBinding);
        EventBus<OnPlayerSpawned>.Deregister(spawnedPlayerEventBinding);
    }

    private void InitEnvironment(OnEnvironmentSpawned env)
    {
        selectedEnvironment = env.currentEnvironment;
    }
    private void InitLevel(OnLevelSpawned level)
    {
        levelsParentObj = level.levelsParentGO;
        selectedLevel = level.currentLevel;
        levelWinReward = level.winReward;
        timeReward = level.timeReward;

        if (!level.currentLevel.TryGetComponent(out levelInfo))
        {
            Debug.LogError("Level Info not found!");

            return;
        }

        levelWinDelay = levelInfo.winDelay;

        if (levelInfo.showObjectiveInStart && levelInfo.dialogueSO != null)
        {
            if (objectiveScreen.okButton != null) objectiveScreen.okButton.onClick.AddListener(() => objectiveScreen.OnObjectiveOKButton());
            objectiveScreen.DisplayDialogue(levelInfo.dialogueSO.GetDialogue());
        }

        if (levelInfo.useTimer && levelTimer.timeController != null)
        {
            timeController = modeLevelSetup[selectedMode].timeController;
            timeController.time = levelInfo.time;
            levelTimer.timerContainer.SetActive(true);
        }
    }
    private void InitPlayer(OnPlayerSpawned player)
    {
        selectedPlayer = player.currentPlayer;

        EventBus<OnPlayerCamera>.Raise(new OnPlayerCamera { currentCamera = selectedPlayer });

        //selectedVehicleRCC = selectedVehicle.GetComponent<RCC_CarControllerV3>();
        //rccAudioListener = rccMainCamera.GetComponent<AudioListener>();

        //selectedVehicleIdleVolume = selectedVehicleRCC.idleEngineSoundVolume; // ! Store sound values of spawned vehicle
        //selectedVehicleMinVolume = selectedVehicleRCC.minEngineSoundVolume; // ! Store sound values of spawned vehicle
        //selectedVehicleMaxVolume = selectedVehicleRCC.maxEngineSoundVolume; // ! Store sound values of spawned vehicle

        //selectedVehicleRCC.Rigid.Sleep(); // ! sleeps the physcis 

        //selectedVehicleLastTransform = selectedVehicle.transform; // ! Set's last transform to player spawned player transform

        //rccCamera.TPSDistance = 15f;
        //rccCamera.cameraTPSHeight = 4f;

        //rccCamera.useOrbitInTPSCameraMode = false;
        //rccCamera.TPSLockY = false;
        //rccCamera.TPSOffset = new Vector3(0f, 0f, -6f);
        //rccCamera.orbitReset = false;
    }

    public void SetPlayerTransform(Transform _transform) // ! Reset's player position and rotation to desired position and rotation
    {
        //selectedVehicle.transform.position = new Vector3(_transform.position.x,
        //    selectedVehicle.transform.position.y, _transform.position.z); // ! Reset's position

        //selectedVehicle.transform.rotation = Quaternion.Euler(_transform.eulerAngles.x,
        //    _transform.eulerAngles.y, _transform.eulerAngles.z);  // ! Resets rotation

        //selectedVehicleRCC.Rigid.velocity = Vector3.zero; // ! Set's physics velocity to 0
        //selectedVehicleRCC.Rigid.angularVelocity = Vector3.zero; // ! Set's angular drag to 0
        //selectedVehicleRCC.Rigid.Sleep(); // ! Sleep's the physics
    }

    public void RCCVolume(float _idle, float _min, float _max) // ! Set's rcc vehicle engine volume
    {
        //selectedVehicleRCC.idleEngineSoundVolume = _idle;
        //selectedVehicleRCC.minEngineSoundVolume = _min;
        //selectedVehicleRCC.maxEngineSoundVolume = _max;
    }

    #region Gameplay States
    public void LevelPaused() // ! Level paused functionality
    {
        //TODO Ads Calling
        AdsWrapper.GetInstance()?.ShowInterstitialWithLoadingPanel(() =>
        {
            state = GameplayScreens.Pause;
            UpdateGameplayState();

            //TODO Firebase Calling
            AdsWrapper.GetInstance()?.FirebaseLog("Level_Pause_", "Mode_" +
                selectedMode.ToString(), "Level_" + levelNumber.ToString());
        });
    }

    public void LevelSetting() // ! Level paused functionality
    {
        state = GameplayScreens.Setting;
        UpdateGameplayState();

        //TODO Firebase Calling
        AdsWrapper.GetInstance()?.FirebaseLog("Level_Setting_", "Mode_" +
            selectedMode.ToString(), "Level_" + levelNumber.ToString());
    }

    public void LevelResume() // ! Level resumed functionality
    {
        state = GameplayScreens.Resume;
        UpdateGameplayState();
    }

    [ContextMenu("Level Failed")]
    public void LevelFailed() // ! Level failed functionality
    {
        //TODO Sound Calling
        //SoundManager.Instance.PlayNormalRandomFailedSound();

        AdsWrapper.GetInstance()?.ShowInterstitialWithLoadingPanel(() =>
        {
            if (!showAds)
            {
                showAds = true;

                state = GameplayScreens.Fail;
                UpdateGameplayState();

                //TODO Firebase Calling
                AdsWrapper.GetInstance()?.FirebaseLog("Level_Failed_", "Mode_" +
                    selectedMode.ToString(), "Level_" + levelNumber.ToString());
            }
        });
    }

    [ContextMenu("Level Win")]
    public void LevelWin() // ! Level Win/ Pass/ Passed/ Success/ Successfull functionality before delay
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.ChangeVolume(0f, sfxVolume);

        if (!addCoins)
        {
            //TODO Sound Calling
            //SoundManager.Instance.PlayNormalRandomWinSound();
            AkaitoAi.SoundManager.Instance?.PlayLevelWinSound();

            PlayerPrefs.SetInt(setupScriptable.levelCoinsPref, levelCoins);
            //reward = PlayerPrefs.GetInt(setupScriptable.levelCoinsPref) + levelWinReward;
            reward = levelWinReward;
            timeReward = levelWinReward;
            PlayerPrefs.SetInt(setupScriptable.totalCoinsPref,
                PlayerPrefs.GetInt(setupScriptable.totalCoinsPref) +
                PlayerPrefs.GetInt(setupScriptable.levelCoinsPref) + levelWinReward + timeReward);
            //totalReward = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref);
            totalReward = reward + timeReward;

            granted2xReward = false;
            addCoins = true;

            counterAS = winScreen.transform.GetChild(0).GetComponent<AudioSource>();
        }


        currentLevelSelectSO.UnlockLevel(levelNumber);
        //UnlockLevel();

        StartCoroutine(LevelWinStateDelay(levelWinDelay));

        IEnumerator LevelWinStateDelay(float _delay) // !Level Win/ Pass/ Passed/ Success/ Successfull functionality after delay
        {
            state = GameplayScreens.None;
            UpdateGameplayState();

            if (!winPanelStateEnabled) winPanelStateEnabled = true;

            yield return new WaitForSeconds(_delay);


            AdsWrapper.GetInstance()?.
                ShowInterstitialWithLoadingPanel(() =>
                {
                    if (winPanelStateEnabled && !showWinPanel && !showAds)
                    {
                        showAds = true;
                        showWinPanel = true;

                        levelTimer.TimeToggle(false, 1f);

                        state = GameplayScreens.Win;
                        UpdateGameplayState();

                        StartCoroutine(RewardCounter());

                        //TODO Sound Calling
                        AkaitoAi.SoundManager.Instance?.ChangeVolume(musicVolume, sfxVolume);

                        //TODO Firebase Calling
                        AdsWrapper.GetInstance()?.FirebaseLog("Level_Win_", "Mode_" +
                            selectedMode.ToString(), "Level_" + levelNumber.ToString());

                    }
                });
            IEnumerator RewardCounter()
            {
                coinText.text = "";
                winTimeText.text = "";
                totalWinRewardText.text = "";

                float timeElapsed = 0;

                yield return AkaitoAiExtensions.Seconds(1.25f);

#if DOTWEEN
                this.DOIntCounterPunch()
                   .From(0)
                   .To(reward)
                   .Duration(0.5f)
                   .OnValueChanged(v => coinText.text = v.ToString())
                   .Yoyo(coinText.transform, strength: 0.15f, upDur: 0.08f, downDur: 0.12f)
                   .Start()
                   .OnComplete(() =>
                   {
                       coinText.text = reward.ToString();
                       if (counterAS != null) counterAS.Stop();

                       StartCoroutine(TimeRewardCounter());
                   });

                if (counterAS != null) counterAS.Play();

                yield break;
#endif
                while (timeElapsed < counterDuration)
                {
                    float amountCount = Mathf.Lerp(0f, reward, timeElapsed / counterDuration);
                    coinText.text = Mathf.FloorToInt(amountCount).ToString();

                    //if (timerContainer)
                    //{
                    //    //float timeCount = Mathf.Lerp(0f, timeController.time, timeElapsed / counterDuration);
                    //    //winTimeText.text = Mathf.FloorToInt(timeCount).ToString();
                    //}

                    timeElapsed += Time.unscaledDeltaTime;

                    yield return null;
                }

                if (counterAS != null) counterAS.Stop();

                coinText.text = reward.ToString();

                //if (timerContainer) winTimeText.text = timeController.timerText.text;

                StartCoroutine(TimeRewardCounter());
            }

            IEnumerator TimeRewardCounter()
            {
                float timeElapsed = 0;

                yield return AkaitoAiExtensions.Seconds(.25f);

#if DOTWEEN
                this.DOIntCounterPunch()
                  .From(0)
                  .To(timeReward)
                  .Duration(0.5f)
                  .OnValueChanged(v => winTimeText.text = v.ToString())
                  .Yoyo(winTimeText.transform, strength: 0.15f, upDur: 0.08f, downDur: 0.12f)
                  .Start()
                  .OnComplete(() =>
                  {
                      winTimeText.text = timeReward.ToString();
                      if (counterAS != null) counterAS.Stop();

                      StartCoroutine(TotalRewardCounter());
                  });

                if (counterAS != null) counterAS.Play();

                yield break;
#endif

                while (timeElapsed < counterDuration)
                {
                    float timeCount = Mathf.Lerp(0f, timeReward, timeElapsed / counterDuration);
                    winTimeText.text = Mathf.FloorToInt(timeCount).ToString();
                    timeElapsed += Time.unscaledDeltaTime;

                    yield return null;
                }

                if (counterAS != null) counterAS.Stop();

                winTimeText.text = timeReward.ToString();

                StartCoroutine(TotalRewardCounter());
            }

            IEnumerator TotalRewardCounter()
            {
                float timeElapsed = 0;

                yield return AkaitoAiExtensions.Seconds(.25f);

#if DOTWEEN
                this.DOIntCounterPunch()
                  .From(0)
                  .To(totalReward)
                  .Duration(0.5f)
                  .OnValueChanged(v => totalWinRewardText.text = v.ToString())
                  .Yoyo(totalWinRewardText.transform, strength: 0.15f, upDur: 0.08f, downDur: 0.12f)
                  .Start()
                  .OnComplete(() =>
                  {
                      totalWinRewardText.text = totalReward.ToString();
                      if (counterAS != null) counterAS.Stop();

                  });

                if (counterAS != null) counterAS.Play();

                yield break;

#endif
                while (timeElapsed < counterDuration)
                {
                    float amountCount = Mathf.Lerp(reward, totalReward, timeElapsed / counterDuration);
                    totalWinRewardText.text = Mathf.FloorToInt(amountCount).ToString();
                    timeElapsed += Time.unscaledDeltaTime;

                    yield return null;
                }

                if (counterAS != null) counterAS.Stop();

                totalWinRewardText.text = totalReward.ToString();

            }
        }
    }

    public void Loading()
    {
        state = GameplayScreens.Loading;
        UpdateGameplayState();
    }

    public void UpdateGameplayState()
    {
        levelTimer.TimeToggle(true, 1f);

        if (!gameScreenCanvas.activeInHierarchy) gameScreenCanvas.SetActive(true);

        failScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseScreen.SetActive(false);
        resumeScreen.SetActive(false);
        cutSceneScreen.SetActive(false);
        settingScreen.SetActive(false);

        EventBus<OnGameplayScreen>.Raise(new OnGameplayScreen { screen = state });

        switch (state)
        {
            case GameplayScreens.None:
                {
                    if (gameScreenCanvas.activeInHierarchy) gameScreenCanvas.SetActive(false);

                    break;
                }

            case GameplayScreens.Resume:
                {
                    resumeScreen.SetActive(true);

                    if (inResume) break;

                    inResume = true;
                    inCutScene = false;

                    break;
                }

            case GameplayScreens.Pause:
                {
                    levelTimer.TimeToggle(false, 0f);

                    pauseScreen.SetActive(true);

                    break;
                }

            case GameplayScreens.Win:
                {
                    levelTimer.TimeToggle(false, 1f);

                    winScreen.SetActive(true);

                    break;
                }

            case GameplayScreens.Fail:
                {
                    levelTimer.TimeToggle(false, 1f);

                    failScreen.SetActive(true);

                    break;
                }

            case GameplayScreens.Loading:
                {

                    break;
                }

            case GameplayScreens.CutScene:
                {
                    cutSceneScreen.SetActive(true);

                    if (inCutScene) break;

                    inCutScene = true;
                    inResume = false;

                    break;
                }

            case GameplayScreens.Setting:
                {
                    levelTimer.TimeToggle(false, 0f);

                    settingScreen.SetActive(true);

                    break;
                }

            default: break;
        }
    }

    #endregion

    #region Button Events

    public void OnPauseButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        LevelPaused();
    }
    public void OnSettingButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        LevelSetting();
    }
    public void OnResumeButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        LevelResume();
    }
    public void OnHomeButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromIndices(new int[] { 1 }) });

        Loading();
    }
    public void OnBackFromSetting()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        //TODO Ads Calling
        AdsWrapper.GetInstance()?.ShowInterstitialWithLoadingPanel(LevelResume);
    }
    public void OnRestartButton()
    {
        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromIndices(SceneLoadCommandHelper.GetAllLoadedSceneIndices()) });

        Loading();
    }
    public void OnNextButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        levelTimer.TimeToggle(false, 1f);

        AdsWrapper.GetInstance()?.ShowInterstitialWithLoadingPanel(Behaviour);

        void Behaviour()
        {
            levelNumber++;

            if (levelNumber < currentLevelSelectSO.totalLevels)
            {
                PlayerPrefs.SetInt(currentLevelSelectSO.namePref, PlayerPrefs.GetInt(currentLevelSelectSO.namePref));
                setupScriptable.levelIndex = levelNumber;

            }
            else
            {
                setupScriptable.modeIndex = setupScriptable.modeIndex++;

                if (setupScriptable.modeIndex >= LevelSelect.totalModeLevels) setupScriptable.modeIndex = 0;

                setupScriptable.levelIndex = 0;

            }

            //string sceneToLoad = modeLevels[setupScriptable.modeIndex].levelSelectSO.useMoreThan2Scenes ? 
            //    $"Mode{setupScriptable.modeIndex}LevelScene{_index}" : $"Mode{setupScriptable.modeIndex}LevelScene";
            //EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromNames(new string[] { "2_GameplayScene", sceneToLoad }) });

            int sceneToLoad = currentLevelSelectSO.useMoreThan2Scenes ? levelNumber + 3 : 3;
            EventBus<OnLoadScene>.Raise(new OnLoadScene { command = SceneLoadCommandHelper.FromIndices(new int[] { 2, sceneToLoad }) });

            Loading();
        }
    }

    public void On2xRewardButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        if (granted2xReward) return;

        AdsWrapper.GetInstance()?.ShowRewardedAds(Reward, AdsWrapper.GetInstance().ShowAdNotAvailable);

        void Reward()
        {
            int reward = totalReward * 2;
            granted2xReward = true;

            PlayerPrefs.SetInt(setupScriptable.totalCoinsPref,
               PlayerPrefs.GetInt(setupScriptable.totalCoinsPref) + reward);

            StartCoroutine(Total2xRewardCounter(reward));
        }

        IEnumerator Total2xRewardCounter(int doubleReward)
        {
            float timeElapsed = 0;

#if DOTWEEN
            this.DOIntCounterPunch()
                 .From(totalReward)
                 .To(doubleReward)
                 .Duration(0.5f)
                 .OnValueChanged(v => totalWinRewardText.text = v.ToString())
                 .Yoyo(totalWinRewardText.transform, strength: 0.15f, upDur: 0.08f, downDur: 0.12f)
                 .Start()
                 .OnComplete(() =>
                 {
                     totalWinRewardText.text = doubleReward.ToString();
                     if (counterAS != null) counterAS.Stop();

                 });

            if (counterAS != null) counterAS.Play();


            yield break;
#endif
            while (timeElapsed < counterDuration)
            {
                float amountCount = Mathf.Lerp(totalReward, doubleReward, timeElapsed / counterDuration);
                totalWinRewardText.text = Mathf.FloorToInt(amountCount).ToString();
                timeElapsed += Time.unscaledDeltaTime;

                yield return null;
            }

            if (counterAS != null) counterAS.Stop();

            totalWinRewardText.text = doubleReward.ToString();
        }
    }
    public void OnSkipLevelRewardButton()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        AdsWrapper.GetInstance()?.ShowRewardedAds(Reward, AdsWrapper.GetInstance().ShowAdNotAvailable);

        void Reward()
        {
            currentLevelSelectSO.UnlockLevel(levelNumber);
        }
    }


    private int currentTOD = 0;
    [ContextMenu("Toggle Time Of Day")]
    public void OnToggleTimeOfDay()
    {
        if (currentTOD == ScenesToSwapBetween.All.Length - 1) currentTOD = 0;
        else currentTOD++;

        EventBus<OnSwapScene>.Raise(new OnSwapScene { command = SceneLoadCommandHelper.FromNames(ScenesToSwapBetween.All[currentTOD]) });
    }

    #endregion
}
