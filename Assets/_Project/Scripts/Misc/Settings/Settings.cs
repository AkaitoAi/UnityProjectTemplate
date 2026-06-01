using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN
using DG.Tweening;
#endif
//using RichTap;

public class Settings : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider gpMusicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Status sfxOnStatus, sfxOffStatus;
    [SerializeField] private Status musicOnStatus, musicOffStatus;

    [Header("Controls")]
    [SerializeField] private Status arrowStatus;
    [SerializeField] private Status steeringStatus;
    [SerializeField] private Status tiltStatus;
    [SerializeField] private Setting controlSetting;

    [Header("Graphics")]
    [SerializeField] bool useAutoSetAA = false;
    [SerializeField] bool useAutoDetectGraphicsSettings = false;
    [SerializeField] WaitForSeconds loadingTime = new WaitForSeconds(2f);
    [SerializeField] private GameObject graphicsLoadingContainerScreen;
    [SerializeField] private GameObject settingsContainerScreen;
    [SerializeField] private Slider cameraFarSlider;
    [SerializeField] private Slider trafficDensitySlider;
    [SerializeField] private Slider PedestriansDensitySlider;
    [SerializeField] private Status smoothStatus;
    [SerializeField] private Status balanceStatus;
    [SerializeField] private Status hdStatus;
    [SerializeField] private Status shadowOnStatus, shadowOffStatus;
    [SerializeField] private Setting graphicsSetting;
    [SerializeField] private Setting shadowSetting;
    private int deviceMem;

    [Header("General")]
    [SerializeField] private Setting mapSetting;
    [SerializeField] private Setting speedMeterSetting;
    [SerializeField] private Setting vibrationSetting;
    [SerializeField] private Setting steeringSensitivitySetting;
    [SerializeField] private Text trafficDensityText, pedestriansDensityText, cameraFarText;

    [Header("Panels")]
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private Status volumeTabStatus;
    [SerializeField] private Status controlTabStatus;
    [SerializeField] private Status graphicTabStatus;
    [SerializeField] private Status generalTabStatus;

    [Serializable]
    public struct Status
    {
        public GameObject selected;
        public GameObject unSelected;
    }

    [Serializable]
    public class Setting
    {
        public Image statusImage;
        public Sprite[] sprites;
        internal int index = 0;
    }

    private SetupSO setupScriptable;

    private void Start()
    {
        Init();
    }

    EventBinding<OnSettingSave> settingSaveEventBinding;
    private void OnEnable()
    {
        settingSaveEventBinding = new EventBinding<OnSettingSave>(Init);
        EventBus<OnSettingSave>.Register(settingSaveEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnSettingSave>.Deregister(settingSaveEventBinding);
    }

    private void Init()
    {
        setupScriptable = RuntimeSetup.GetInstance().Setup;

        deviceMem = SystemInfo.systemMemorySize;

        if (PlayerPrefs.GetInt(setupScriptable.setupPref) == 1)
        {
            if (musicSlider) musicSlider.value = PlayerPrefs.GetFloat(setupScriptable.bGVolumePref);
            if (sfxSlider) sfxSlider.value = PlayerPrefs.GetFloat(setupScriptable.sFXVolumePref);
            if (gpMusicSlider) gpMusicSlider.value = PlayerPrefs.GetFloat(setupScriptable.bGGPVolumePref);

            if (cameraFarSlider) cameraFarSlider.value = PlayerPrefs.GetFloat(setupScriptable.cameraFarPref);
            if (trafficDensitySlider) trafficDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.trafficDensityPref);
            if (PedestriansDensitySlider) PedestriansDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.pedestriansDensityPref);

            if (trafficDensityText) trafficDensityText.text = trafficDensitySlider.value.ToString();
            if (pedestriansDensityText) pedestriansDensityText.text = PedestriansDensitySlider.value.ToString();
            if (cameraFarText) cameraFarText.text = cameraFarSlider.value.ToString();

            OnToggleControls(PlayerPrefs.GetInt(setupScriptable.controlPref));
        }
        else
        {
            if (musicSlider) PlayerPrefs.SetFloat(setupScriptable.bGVolumePref, musicSlider.value);
            if (sfxSlider) PlayerPrefs.SetFloat(setupScriptable.sFXVolumePref, sfxSlider.value);
            if (gpMusicSlider) PlayerPrefs.SetFloat(setupScriptable.bGGPVolumePref, gpMusicSlider.value);

            PlayerPrefs.SetInt(setupScriptable.musicMutePref, 1);
            PlayerPrefs.SetInt(setupScriptable.sFXMutePref, 1);

            PlayerPrefs.SetInt(setupScriptable.mapPref, 1);
            PlayerPrefs.SetInt(setupScriptable.speedMeterPref, 0);
            PlayerPrefs.SetInt(setupScriptable.vibrationPref, 1);


            OnToggleControls(0);

            if (useAutoDetectGraphicsSettings) AutoDetectGraphicSettings();
            if (useAutoSetAA) AutoAA();

            PlayerPrefs.SetInt(setupScriptable.setupPref, 1);
        }

        //SoundUIUpdate("MusicMute", s_Music_ON, us_Music_ON, s_Music_OFF, us_Music_OFF);
        //SoundUIUpdate("SFXMute", s_SFX_ON, us_SFX_ON, s_SFX_OFF, us_SFX_OFF);
        SetQualityLevel();
        CheckSelectedQualityUI();
        //CheckSelectedShadowUI();

        //SetVolume();

        CheckSelectedUI(mapSetting, setupScriptable.mapPref);
        CheckSelectedUI(speedMeterSetting, setupScriptable.speedMeterPref);
        CheckSelectedUI(vibrationSetting, setupScriptable.vibrationPref);
    }

    #region General Setup

    public void OnMapIncrement()
    {
        Increment(mapSetting, OnToggleMap);
    }

    public void OnMapDecrement()
    {
        Decrement(mapSetting, OnToggleMap);
    }

    public void OnToggleMap(int index)
    {
        PlayerPrefs.SetInt(setupScriptable.mapPref, index);

        CheckSelectedUI(mapSetting, setupScriptable.mapPref);
    }

    public void OnSpeedMeterIncrement()
    {
        Increment(speedMeterSetting, OnSpeedMeterType);
    }

    public void OnSpeedMeterDecrement()
    {
        Decrement(speedMeterSetting, OnSpeedMeterType);
    }

    public void OnSpeedMeterType(int index)
    {
        PlayerPrefs.SetInt(setupScriptable.speedMeterPref, index);

        //if (index == 0)
        //    RCC_Settings.Instance.units = RCC_Settings.Units.KMH;
        //else if (index == 1)
        //    RCC_Settings.Instance.units = RCC_Settings.Units.MPH;

        CheckSelectedUI(speedMeterSetting, setupScriptable.speedMeterPref);
    }

    public void OnVibrationIncrement()
    {
        Increment(vibrationSetting, OnToggleVibration);
    }

    public void OnVibrationDecerement()
    {
        Decrement(vibrationSetting, OnToggleVibration);
    }

    public void OnToggleVibration(int index)
    {
        PlayerPrefs.SetInt(setupScriptable.vibrationPref, index);

        //if (index == 1) RichtapEffectSource.Instance?.Play(RichTap.Common.RichtapPreset.RT_CLICK);

        CheckSelectedUI(vibrationSetting, setupScriptable.vibrationPref);
    }

    public void OnSteeringSensitivityIncrement()
    {

    }
    public void OnSteeringSensitivityDecrement()
    {

    }

    #endregion

    #region Sound Setup

    //TODO Sound Calling
    public void OnMusicSlider() => VolumeSlider(AkaitoAi.SoundManager.Instance?.menuBGAudioSource, musicSlider, setupScriptable.bGVolumePref); // ! Set's pref with respect to changed slider value
    public void OnSFXSlider() => VolumeSlider(AkaitoAi.SoundManager.Instance?.sfxAudioSource, sfxSlider, setupScriptable.sFXVolumePref); // ! Set's pref with respect to changed slider value
    public void OnGameplayMusicSlider() => VolumeSlider(AkaitoAi.SoundManager.Instance?.gameplayBGAudioSource, gpMusicSlider, setupScriptable.bGGPVolumePref); // ! Set's pref with respect to changed slider value
    private void VolumeSlider(AudioSource _audioSource, Slider _volumeSlider, string _pref) // ! Set's pref with respect to changed slider value
    {
        _audioSource.volume = _volumeSlider.value;
        PlayerPrefs.SetFloat(_pref, _volumeSlider.value);
        _volumeSlider.value = PlayerPrefs.GetFloat(_pref);
    }

    public void OnMusicMute()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        if (PlayerPrefs.GetInt(setupScriptable.musicMutePref) == 1)
        {
            MuteAudio(AkaitoAi.SoundManager.Instance.menuBGAudioSource, setupScriptable.musicMutePref, 0);
            SoundUIUpdate(setupScriptable.musicMutePref,
                musicOnStatus.selected, musicOnStatus.unSelected,
                musicOffStatus.selected, musicOffStatus.unSelected);

            return;
        }

        if (PlayerPrefs.GetInt(setupScriptable.musicMutePref) == 0)
        {
            MuteAudio(AkaitoAi.SoundManager.Instance?.menuBGAudioSource, setupScriptable.musicMutePref, 1);
            SoundUIUpdate(setupScriptable.musicMutePref,
                musicOnStatus.selected, musicOnStatus.unSelected,
                musicOffStatus.selected, musicOffStatus.unSelected);

            return;
        }
    }

    public void OnSFXMute()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        if (PlayerPrefs.GetInt(setupScriptable.sFXMutePref) == 1)
        {
            MuteAudio(AkaitoAi.SoundManager.Instance?.sfxAudioSource, setupScriptable.sFXMutePref, 0);
            SoundUIUpdate(setupScriptable.sFXMutePref,
                sfxOnStatus.selected, sfxOnStatus.unSelected,
                sfxOffStatus.selected, sfxOffStatus.unSelected);

            return;
        }

        if (PlayerPrefs.GetInt(setupScriptable.sFXMutePref) == 0)
        {
            MuteAudio(AkaitoAi.SoundManager.Instance?.sfxAudioSource, setupScriptable.sFXMutePref, 1);
            SoundUIUpdate(setupScriptable.sFXMutePref,
                sfxOnStatus.selected, sfxOnStatus.unSelected,
                sfxOffStatus.selected, sfxOffStatus.unSelected);

            return;
        }
    }

    private void SoundUIUpdate(string _pref, GameObject _onSelected, GameObject _onUnselected,
        GameObject _offSelected, GameObject _offUnselected)
    {
        if (PlayerPrefs.GetInt(_pref) == 1)
        {
            _onSelected.SetActive(true);
            _onUnselected.SetActive(false);
            _offSelected.SetActive(false);
            _offUnselected.SetActive(true);
            return;
        }

        if (PlayerPrefs.GetInt(_pref) == 0)
        {
            _onSelected.SetActive(false);
            _onUnselected.SetActive(true);
            _offSelected.SetActive(true);
            _offUnselected.SetActive(false);

            return;
        }
    }

    private void MuteAudio(AudioSource _aSrc, string _pref, int _value)
    {
        PlayerPrefs.SetInt(_pref, _value);

        _aSrc.volume = _value;
    }

    public void OnSetAudio(int _value)
    {
        PlayerPrefs.SetInt(setupScriptable.muteAudioPref, _value);

        // Sound Calling
        SetGameAudio();
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();
    }
    public void SetGameAudio()
    {
        int soundValue = PlayerPrefs.GetInt(setupScriptable.muteAudioPref);

        if (soundValue == 0)
        {
            AkaitoAi.SoundManager.Instance.menuBGAudioSource.mute = false;
            AkaitoAi.SoundManager.Instance.sfxAudioSource.mute = false;
            return;
        }

        if (soundValue == 1)
        {
            AkaitoAi.SoundManager.Instance.menuBGAudioSource.mute = true;
            AkaitoAi.SoundManager.Instance.sfxAudioSource.mute = true;
            return;
        }
    }

    private void SetVolume()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.ChangeVolume(
            PlayerPrefs.GetFloat(setupScriptable.bGVolumePref),
            PlayerPrefs.GetFloat(setupScriptable.sFXVolumePref));

        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.ChangeVolume(
            PlayerPrefs.GetFloat(setupScriptable.bGVolumePref),
            PlayerPrefs.GetFloat(setupScriptable.sFXVolumePref));

        //TODO Sound Calling
        if (AkaitoAi.SoundManager.Instance == null) return;

        AkaitoAi.SoundManager.Instance.menuBGAudioSource.volume =
            PlayerPrefs.GetInt(setupScriptable.musicMutePref);
        AkaitoAi.SoundManager.Instance.sfxAudioSource.volume =
            PlayerPrefs.GetInt(setupScriptable.sFXMutePref);

        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance.menuBGAudioSource.volume = PlayerPrefs.GetInt(setupScriptable.musicMutePref);
        AkaitoAi.SoundManager.Instance.sfxAudioSource.volume = PlayerPrefs.GetInt(setupScriptable.sFXMutePref);
    }

    #endregion

    #region Controls Setup

    public void OnControlIncrement()
    {
        Increment(controlSetting, OnToggleControls);
    }

    public void OnControlDecrement()
    {
        Decrement(controlSetting, OnToggleControls);
    }

    public void OnToggleControls(int _controls)
    {
        //TODO Sound Calling
        //SoundManager.Instance.PlayOnButtonSound();

        PlayerPrefs.SetInt(setupScriptable.controlPref, _controls);

        CheckControlUI();
    }

    private void CheckControlUI() // ! Check's selected contols
    {
        if (generalPanel != null)
        {
            controlSetting.index = PlayerPrefs.GetInt(setupScriptable.controlPref);
            controlSetting.statusImage.sprite = controlSetting.sprites[controlSetting.index];
            controlSetting.statusImage.SetNativeSize();

#if DOTWEEN
            controlSetting.statusImage.rectTransform.DOKill();
            controlSetting.statusImage.rectTransform.localScale = Vector2.one;
            controlSetting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif
        }

        if (arrowStatus.selected == null || steeringStatus.selected == null ||
            steeringStatus.unSelected || arrowStatus.unSelected) return;

        if (PlayerPrefs.GetInt(setupScriptable.controlPref) == 0)
        {
            //RCC.SetMobileController(RCC_Settings.MobileController.TouchScreen);

            arrowStatus.selected.SetActive(true);
            arrowStatus.unSelected.SetActive(false);

            steeringStatus.selected.SetActive(false);
            steeringStatus.unSelected.SetActive(true);

            //tiltStatus.selected.SetActive(false);
            //tiltStatus.unSelected.SetActive(true);

            return;
        }

        if (PlayerPrefs.GetInt(setupScriptable.controlPref) == 1)
        {
            //RCC.SetMobileController(RCC_Settings.MobileController.SteeringWheel);

            arrowStatus.selected.SetActive(false);
            arrowStatus.unSelected.SetActive(true);

            steeringStatus.selected.SetActive(true);
            steeringStatus.unSelected.SetActive(false);

            //tiltStatus.selected.SetActive(false);
            //tiltStatus.unSelected.SetActive(true);

            return;
        }

        if (PlayerPrefs.GetInt(setupScriptable.controlPref) == 2)
        {
            //RCC.SetMobileController(RCC_Settings.MobileController.Gyro);

            //arrowStatus.selected.SetActive(false);
            //arrowStatus.unSelected.SetActive(true);

            //steeringStatus.selected.SetActive(false);
            //steeringStatus.unSelected.SetActive(true);

            //tiltStatus.selected.SetActive(true);
            //tiltStatus.unSelected.SetActive(false);

            return;
        }
    }

    #endregion

    #region Graphics Setup

    // Sets Quality setting according to the index of the button
    public void OnQualityIncrement()
    {
        Increment(graphicsSetting,
            OnChangeQualitySetting);
    }

    public void OnQualityDecrement()
    {
        Decrement(graphicsSetting,
            OnChangeQualitySetting);
    }

    public void OnShadowIncrement()
    {
        Increment(shadowSetting,
            OnCastShadows);
    }
    public void OnShadowDecrement()
    {
        Decrement(shadowSetting,
            OnCastShadows);
    }

    public void OnCastShadows(int index)
    {
        PlayerPrefs.SetInt(setupScriptable.shadowSettingPref, index);

        QualitySettings.shadows = (ShadowQuality)index;

        CheckSelectedShadowUI();
    }

    public void OnChangeQualitySetting(int _qualityIndex)
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance.PlayOnButtonSound();

        //if (PlayerPrefs.GetInt("AutoDetectGraphicsSettings") == 1)
        //    StartCoroutine(SettingsLoadingScreen());

        PlayerPrefs.SetInt(setupScriptable.qualitySettingPref, _qualityIndex);

        QualitySettings.SetQualityLevel(_qualityIndex, true);
        QualitySettings.GetQualityLevel();

        CheckSelectedQualityUI();
    }
    IEnumerator SettingsLoadingScreen()
    {
        graphicsLoadingContainerScreen.SetActive(true);
        settingsContainerScreen.SetActive(false);

        yield return loadingTime;

        graphicsLoadingContainerScreen.SetActive(false);
        settingsContainerScreen.SetActive(true);
    }
    private void SetQualityLevel()
    {
        int qualityIndex = PlayerPrefs.GetInt(setupScriptable.qualitySettingPref);

        QualitySettings.SetQualityLevel(qualityIndex, true);
        QualitySettings.GetQualityLevel();
    }
    public void OnCameraFarSlider()
    {
        SetupSlider(cameraFarSlider, setupScriptable.cameraFarPref);

        if (cameraFarText) cameraFarText.text = cameraFarSlider.value.ToString();
    }
    public void OnTrafficDensitySlider()
    {
        SetupSlider(trafficDensitySlider, setupScriptable.trafficDensityPref);

        if (trafficDensityText) trafficDensityText.text = trafficDensitySlider.value.ToString();
    }
    public void OnPedestriansDensitySlider()
    {
        SetupSlider(PedestriansDensitySlider, setupScriptable.pedestriansDensityPref);

        if (pedestriansDensityText) pedestriansDensityText.text = PedestriansDensitySlider.value.ToString();
    }

    // Automatically Sets AntiAliasing according to phone memory 
    public void AutoAA()
    {
        if (deviceMem <= 3072)
        {
            QualitySettings.antiAliasing = 2;

            return;
        }

        if (deviceMem > 3072)
        {
            QualitySettings.antiAliasing = 4;

            return;
        }
    }

    public void CheckSelectedShadowUI()
    {
        return;

        if (generalPanel != null)
        {

            if (shadowSetting.statusImage == null) return;

            shadowSetting.index = PlayerPrefs.GetInt(setupScriptable.shadowSettingPref);
            shadowSetting.statusImage.sprite = shadowSetting.sprites[shadowSetting.index];
            shadowSetting.statusImage.SetNativeSize();

#if DOTWEEN
            shadowSetting.statusImage.rectTransform.DOKill();
            shadowSetting.statusImage.rectTransform.localScale = Vector2.one;
            shadowSetting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif
        }

        int shadowIndex = PlayerPrefs.GetInt(setupScriptable.shadowSettingPref);

        if (shadowIndex == 1)
        {
            shadowOnStatus.selected.SetActive(true);
            shadowOnStatus.unSelected.SetActive(false);

            shadowOffStatus.selected.SetActive(false);
            shadowOffStatus.unSelected.SetActive(true);

            return;
        }

        if (shadowIndex == 0)
        {
            shadowOnStatus.selected.SetActive(false);
            shadowOnStatus.unSelected.SetActive(true);

            shadowOffStatus.selected.SetActive(true);
            shadowOffStatus.unSelected.SetActive(false);

            return;
        }
    }

    public void CheckSelectedQualityUI() // Call it on settings button in main menu
    {
        if (generalPanel != null)
        {
            if (graphicsSetting.statusImage == null) return;

            graphicsSetting.index = PlayerPrefs.GetInt(setupScriptable.qualitySettingPref);
            graphicsSetting.statusImage.sprite = graphicsSetting.sprites[graphicsSetting.index];
            graphicsSetting.statusImage.SetNativeSize();

#if DOTWEEN
            graphicsSetting.statusImage.rectTransform.DOKill();
            graphicsSetting.statusImage.rectTransform.localScale = Vector2.one;
            graphicsSetting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif

            if (graphicsSetting.index == 0)
            {
                shadowSetting.index = 0;
            }
            else
            {
                shadowSetting.index = 1;
            }

            shadowSetting.statusImage.sprite = shadowSetting.sprites[shadowSetting.index];
            shadowSetting.statusImage.SetNativeSize();

#if DOTWEEN
            shadowSetting.statusImage.rectTransform.DOKill();
            shadowSetting.statusImage.rectTransform.localScale = Vector2.one;
            shadowSetting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif
        }

        int qualityIndex = PlayerPrefs.GetInt(setupScriptable.qualitySettingPref);

        if (qualityIndex == 0)
        {
            smoothStatus.selected.SetActive(true);
            smoothStatus.unSelected.SetActive(false);

            balanceStatus.selected.SetActive(false);
            balanceStatus.unSelected.SetActive(true);

            hdStatus.selected.SetActive(false);
            hdStatus.unSelected.SetActive(true);

            OnCastShadows(0);

            return;
        }

        if (qualityIndex == 1)
        {
            smoothStatus.selected.SetActive(false);
            smoothStatus.unSelected.SetActive(true);

            balanceStatus.selected.SetActive(true);
            balanceStatus.unSelected.SetActive(false);

            hdStatus.selected.SetActive(false);
            hdStatus.unSelected.SetActive(true);

            OnCastShadows(1);

            return;
        }

        if (qualityIndex == 2)
        {
            smoothStatus.selected.SetActive(false);
            smoothStatus.unSelected.SetActive(true);

            balanceStatus.selected.SetActive(false);
            balanceStatus.unSelected.SetActive(true);

            hdStatus.selected.SetActive(true);
            hdStatus.unSelected.SetActive(false);

            OnCastShadows(1);

            return;
        }
    }
    private void AutoDetectGraphicSettings()
    {
        if (deviceMem <= 4096)
        {
            AutoChangeQualitySetting(0);
            OnCastShadows(0);

            PlayerPrefs.SetFloat(setupScriptable.cameraFarPref, 150f);
            if (cameraFarSlider) cameraFarSlider.value = PlayerPrefs.GetFloat(setupScriptable.cameraFarPref);

            PlayerPrefs.SetFloat(setupScriptable.trafficDensityPref, 1f);
            if (trafficDensitySlider) trafficDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.trafficDensityPref);

            PlayerPrefs.SetFloat(setupScriptable.pedestriansDensityPref, 1f);
            if (PedestriansDensitySlider) PedestriansDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.pedestriansDensityPref);

            if (trafficDensityText) trafficDensityText.text = trafficDensitySlider.value.ToString();
            if (pedestriansDensityText) pedestriansDensityText.text = PedestriansDensitySlider.value.ToString();
            if (cameraFarText) cameraFarText.text = cameraFarSlider.value.ToString();

            return;
        }

        if (deviceMem > 4096 && deviceMem <= 8192)
        {
            AutoChangeQualitySetting(1);
            OnCastShadows(1);

            PlayerPrefs.SetFloat(setupScriptable.cameraFarPref, 250f);
            if (cameraFarSlider) cameraFarSlider.value = PlayerPrefs.GetFloat(setupScriptable.cameraFarPref);

            PlayerPrefs.SetFloat(setupScriptable.trafficDensityPref, 3f);
            if (trafficDensitySlider) trafficDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.trafficDensityPref);

            PlayerPrefs.SetFloat(setupScriptable.pedestriansDensityPref, 3f);
            if (PedestriansDensitySlider) PedestriansDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.pedestriansDensityPref);

            if (trafficDensityText) trafficDensityText.text = trafficDensitySlider.value.ToString();
            if (pedestriansDensityText) pedestriansDensityText.text = PedestriansDensitySlider.value.ToString();
            if (cameraFarText) cameraFarText.text = cameraFarSlider.value.ToString();

            return;
        }

        if (deviceMem > 8192)
        {
            AutoChangeQualitySetting(2);
            OnCastShadows(1);

            PlayerPrefs.SetFloat(setupScriptable.cameraFarPref, 400f);
            if (cameraFarSlider) cameraFarSlider.value = PlayerPrefs.GetFloat(setupScriptable.cameraFarPref);

            PlayerPrefs.SetFloat(setupScriptable.trafficDensityPref, 5f);
            if (trafficDensitySlider) trafficDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.trafficDensityPref);

            PlayerPrefs.SetFloat(setupScriptable.pedestriansDensityPref, 5f);
            if (PedestriansDensitySlider) PedestriansDensitySlider.value = PlayerPrefs.GetFloat(setupScriptable.pedestriansDensityPref);

            if (trafficDensityText) trafficDensityText.text = trafficDensitySlider.value.ToString();
            if (pedestriansDensityText) pedestriansDensityText.text = PedestriansDensitySlider.value.ToString();
            if (cameraFarText) cameraFarText.text = cameraFarSlider.value.ToString();

            return;
        }

        void AutoChangeQualitySetting(int _qualityIndex)
        {
            PlayerPrefs.SetInt(setupScriptable.qualitySettingPref, _qualityIndex);

            QualitySettings.SetQualityLevel(_qualityIndex, true);
            QualitySettings.GetQualityLevel();
        }
    }

    #endregion

    #region Panels
    public void OnSettingTypeButton(string btnName)
    {
        if (generalPanel == null) return;

        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        switch (btnName)
        {
            case "Volume":

                volumePanel.SetActive(true);
                controlPanel.SetActive(false);
                graphicsPanel.SetActive(false);
                generalPanel.SetActive(false);

                volumeTabStatus.selected.SetActive(true);
                volumeTabStatus.unSelected.SetActive(false);

                controlTabStatus.selected.SetActive(false);
                controlTabStatus.unSelected.SetActive(true);

                graphicTabStatus.selected.SetActive(false);
                graphicTabStatus.unSelected.SetActive(true);

                generalTabStatus.selected.SetActive(false);
                generalTabStatus.unSelected.SetActive(true);

                break;

            case "Control":

                volumePanel.SetActive(false);
                controlPanel.SetActive(true);
                graphicsPanel.SetActive(false);
                generalPanel.SetActive(false);

                volumeTabStatus.selected.SetActive(false);
                volumeTabStatus.unSelected.SetActive(true);

                controlTabStatus.selected.SetActive(true);
                controlTabStatus.unSelected.SetActive(false);

                graphicTabStatus.selected.SetActive(false);
                graphicTabStatus.unSelected.SetActive(true);

                generalTabStatus.selected.SetActive(false);
                generalTabStatus.unSelected.SetActive(true);

                break;

            case "Graphic":

                volumePanel.SetActive(false);
                controlPanel.SetActive(false);
                graphicsPanel.SetActive(true);
                generalPanel.SetActive(false);

                volumeTabStatus.selected.SetActive(false);
                volumeTabStatus.unSelected.SetActive(true);

                controlTabStatus.selected.SetActive(false);
                controlTabStatus.unSelected.SetActive(true);

                graphicTabStatus.selected.SetActive(true);
                graphicTabStatus.unSelected.SetActive(false);

                generalTabStatus.selected.SetActive(false);
                generalTabStatus.unSelected.SetActive(true);

                break;

            case "General":

                volumePanel.SetActive(false);
                controlPanel.SetActive(false);
                graphicsPanel.SetActive(false);
                generalPanel.SetActive(true);

                volumeTabStatus.selected.SetActive(false);
                volumeTabStatus.unSelected.SetActive(true);

                controlTabStatus.selected.SetActive(false);
                controlTabStatus.unSelected.SetActive(true);

                graphicTabStatus.selected.SetActive(false);
                graphicTabStatus.unSelected.SetActive(true);

                generalTabStatus.selected.SetActive(true);
                generalTabStatus.unSelected.SetActive(false);

                break;

            default: break;
        }
    }
    public void OnOKButton()
    {
        OnSettingTypeButton("General");

        EventBus<OnSettingSave>.Raise(new OnSettingSave { });
    }
    private void Increment(Setting setting, Action<int> action)
    {
        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        setting.index++;

        if (setting.index >= setting.sprites.Length)
            setting.index = 0;

        setting.statusImage.sprite = setting.sprites[setting.index];
        setting.statusImage.SetNativeSize();

#if DOTWEEN
        setting.statusImage.rectTransform.DOKill();
        setting.statusImage.rectTransform.localScale = Vector2.one;
        setting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif

        action?.Invoke(setting.index);
    }
    private void Decrement(Setting setting, Action<int> action)
    {
        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        setting.index--;

        if (setting.index < 0)
            setting.index = setting.sprites.Length - 1;

        setting.statusImage.sprite = setting.sprites[setting.index];
        setting.statusImage.SetNativeSize();

#if DOTWEEN
        setting.statusImage.rectTransform.DOKill();
        setting.statusImage.rectTransform.localScale = Vector2.one;
        setting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif

        action?.Invoke(setting.index);
    }
    private void SetupSlider(Slider slider, string pref)
    {
        PlayerPrefs.SetFloat(pref, slider.value);

        slider.value = PlayerPrefs.GetFloat(pref);
    }
    public void CheckSelectedUI(Setting setting, string prefName)
    {
        if (setting.statusImage == null) return;

        setting.index = PlayerPrefs.GetInt(prefName);
        setting.statusImage.sprite = setting.sprites[setting.index];
        setting.statusImage.SetNativeSize();

#if DOTWEEN
        setting.statusImage.rectTransform.DOKill();
        setting.statusImage.rectTransform.localScale = Vector2.one;
        setting.statusImage.rectTransform.DOPunchScale(Vector2.one * 0.1f, 0.4f, 0);
#endif
    }

    #endregion
}

public struct OnSettingSave : IEvent { }
