using AkaitoAi.Advertisement;
using AkaitoAi.GameBase;
using AkaitoAi.Singleton;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private TMP_Text coinsText;

    internal SetupSO setupScriptable;

    [Header("Panel Switch Bar")]
    public Animator transitionAnimator;
    public string openAnimation, closeAnimation;

    [Header("For Developer")]
    [SerializeField] private bool addCoins = false;
    [SerializeField] private int addCoinsAmount = 9999;
    [SerializeField] private bool clearData = false;

    public UnityEvent OnAwakeEvent;

    //Bindings
    EventBinding<OnCoinsCharged> coinsChargedEventBinding;

    private void Awake()
    {
        Time.timeScale = 1f;

        setupScriptable = RuntimeSetup.GetInstance().Setup;

        if (clearData) PlayerPrefs.DeleteAll(); // ! Checks and Clear all saved data
        if (addCoins) PlayerPrefs.SetInt(setupScriptable.totalCoinsPref, addCoinsAmount); // ! Checks and add money from inspector

        OnAwakeEvent?.Invoke();

        UpdateTotalCoins();

        //TODO Sounds Calling 
        AkaitoAi.SoundManager.Instance?.gameplayBGAudioSource.Stop();
        if (AkaitoAi.SoundManager.Instance?.menuBGAudioSource.clip == null ||
            AkaitoAi.SoundManager.Instance?.menuBGAudioSource.isPlaying == false)
        AkaitoAi.SoundManager.Instance?.PlayMenuBG();
    }

    private void Start()
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.gameObject.SetActive(true);
            transitionAnimator.Play(openAnimation);
        }

        //TODO Ads Calling
        //AdsWrapper.GetInstance()?.HideAllBanners();
        AdsWrapper.GetInstance()?.ShowSmallBannerTopLeft();
        AdsWrapper.GetInstance()?.ShowSmallBannerTopRight();

        //TODO Firebase Calling
        AdsWrapper.GetInstance()?.FirebaseLog("Main_Menu");
    }
    public void OnExitEnterButton()
    {
        //TODO Ads Calling
        //EventBus<OnEnterLeftSidedAd>.Raise(new OnEnterLeftSidedAd { });
    }
    public void OnExitNoButton()
    {
        //TODO Ads Calling
        //EventBus<OnExitLeftSidedAd>.Raise(new OnExitLeftSidedAd { });
    }

    public void OpenURL(string _url)
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance.PlayOnButtonSound();

        Application.OpenURL(_url);
    }

    private void UpdateTotalCoins()
    {
        if (coinsText != null) coinsText.text = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref).ToString(); //! Shows Total money text
    }

    private void OnEnable()
    {
        coinsChargedEventBinding = new EventBinding<OnCoinsCharged>(UpdateTotalCoins);
        EventBus<OnCoinsCharged>.Register(coinsChargedEventBinding);
    }
    private void OnDisable()
    {
        EventBus<OnCoinsCharged>.Deregister(coinsChargedEventBinding);
    }
}
