using AkaitoAi.Advertisement;
using AkaitoAi.Extensions;
using AkaitoAi.GameBase;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if DOTWEEN
using DG.Tweening;
#endif

public class SelectionShop : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button tryOnceButton;
    [SerializeField] private Button incrementButton, decrementButton;

    [Header("Total Coins")]
    [SerializeField] private TMP_Text coinsText;

    [SerializeField] private AkaitoAi.GameBase.Selectable selectables;
    [Space(10)]
    [SerializeField] private bool lockAllInStart = false;
    internal int currentSelectedIndex = 0;
    internal int perSelectedIndex = 0;
    internal Animator _animator;

    [Header("Buy Process")]
    [SerializeField] private GameObject lockedImageObj;
    [SerializeField] private GameObject priceContainer;
    [SerializeField] private bool useBuyCoinTextLerp;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private bool useDollarSignBeforePrice;

    [Header("Buy Error")]
    [SerializeField] private GameObject buyErrorObj;
    [SerializeField] private bool showCoinsRequired = false;
    [SerializeField] private TMP_Text buyErrorText;
    [SerializeField] private float showBuyErrorFor = 2.5f;

    [Header("Lerp Configration")]
    [SerializeField] private float priceLerpDuration = 3f;

    internal SetupSO setupScriptable;

    //Bindings
    EventBinding<OnCoinsCharged> coinsChargedEventBinding;

    private void OnEnable()
    {
        coinsChargedEventBinding = new EventBinding<OnCoinsCharged>(UpdateTotalCoins);
        EventBus<OnCoinsCharged>.Register(coinsChargedEventBinding);
    }
    private void OnDisable()
    {
        EventBus<OnCoinsCharged>.Deregister(coinsChargedEventBinding);
    }

    private void Start()
    {
        setupScriptable = RuntimeSetup.GetInstance().Setup;

        setupScriptable.selectIndex = PlayerPrefs.GetInt(setupScriptable.selectedPref, 0);

        for (int i = 0; i <= selectables.details.Length - 1; i++)
            PlayerPrefs.SetInt(setupScriptable.selectTryOncePref + i, 0);

        UpdateTotalCoins();

        EnableSlected();

        UpdateSpecs((SelectSpecsSO)selectables.details[currentSelectedIndex].specs);

        if (buyErrorObj) buyErrorObj.SetActive(false);

        CheckUnlocked();

        if (selectButton != null) selectButton.onClick.AddListener(OnSelectButton);
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyButton);
        if (tryOnceButton != null) tryOnceButton.onClick.AddListener(OnTryOnceRewardButton);
        if (incrementButton != null) incrementButton.onClick.AddListener(() => SwitchBetween(1));
        if (decrementButton != null) decrementButton.onClick.AddListener(() => SwitchBetween(-1));

        void EnableSlected() // Call it on the back of next panel
        {
            int length = selectables.details.Length;
            for (int i = 0; i < length; i++)
            {
                selectables.details[i].specs = RuntimeSetup.GetInstance()?.Specs[i];
                    //Resources.Load("ScriptableObjects/SelectSpecs/SelectSpecs_" + i) as ScriptableObject;

                selectables.details[i].Obj.SetActive(false);
            }
            if (!lockAllInStart) PlayerPrefs.SetInt(setupScriptable.selectBoughtPref + 0, 1);

            currentSelectedIndex = setupScriptable.selectIndex;

            selectables.details[currentSelectedIndex].Obj.SetActive(true);
        }
    }

    public void OnSelectButton()
    {
        setupScriptable.selectIndex = currentSelectedIndex;

        PlayerPrefs.SetInt(setupScriptable.selectedPref, currentSelectedIndex);
    }
    public void OnBuyButton()
    {
        bool hasMoney = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref)
            >= selectables.details[currentSelectedIndex].price;

        if (hasMoney)
        {
            PlayerPrefs.SetInt(setupScriptable.selectBoughtPref + currentSelectedIndex, 1);

            int totalCoins = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref);
            int chargedCoins = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref)
                - selectables.details[currentSelectedIndex].price;

            PlayerPrefs.SetInt(setupScriptable.totalCoinsPref, chargedCoins);

            // TODO Sound Calling
            AkaitoAi.SoundManager.Instance.PlayPurchaseSound();

            if(tryOnceButton != null) tryOnceButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            if (lockedImageObj) lockedImageObj.SetActive(false);
            selectButton.gameObject.SetActive(true);
            if(priceContainer) priceContainer.SetActive(false);

            if (!useBuyCoinTextLerp) StaticUpdateCoins();
            else
            {
                //StartCoroutine(UpdateCoins(totalCoins, chargedCoins));

#if DOTWEEN
                this.DOIntCounterPunch()
                    .From(chargedCoins)
                    .To(totalCoins)
                    .Duration(0.5f)
                    .OnValueChanged(v => coinsText.text = v.ToString())
                    .Yoyo(coinsText.transform, strength: 0.15f, upDur: 0.08f, downDur: 0.12f)
                    //.Punch(coinsText.transform, 0.5f)
                    .Start()
                    .OnComplete(() => coinsText.text = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref).ToString());
#else
                coinsText.text = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref).ToString();
#endif

            }

            EventBus<OnCoinsCharged>.Raise(new OnCoinsCharged { });

            //priceText.color = Color.green;
            priceText.text = "Purchased";
        }
        else
        {
            // TODO Sound Calling
            AkaitoAi.SoundManager.Instance.PlayPurchaseFailedSound();

            if (!buyErrorObj) return;

            buyButton.interactable = false;
            buyErrorObj.SetActive(true);
            if (lockedImageObj) lockedImageObj.SetActive(false);

            if (showCoinsRequired)
            {
                int needCoins = selectables.details[currentSelectedIndex].price - PlayerPrefs.GetInt(setupScriptable.totalCoinsPref);
                buyErrorText.text = "You Need More Than $" + needCoins + " To Buy This Vehicle!";
            }
            else
            {
                buyErrorText.text = "You Have Not Enough Coins!";
            }

            StartCoroutine(DisableBuyErrorObj());
        }

        IEnumerator UpdateCoins(int _totalCoins, int _chargedCoins)
        {
            float timeElapsed = 0;
            while (timeElapsed < priceLerpDuration)
            {
                float price = Mathf.Lerp(_totalCoins, _chargedCoins, timeElapsed / priceLerpDuration);

                coinsText.text = Mathf.FloorToInt(price).ToString();

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            coinsText.text = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref).ToString();
        }


        void StaticUpdateCoins() => 
            coinsText.text = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref).ToString();
        
        IEnumerator DisableBuyErrorObj()
        {
            yield return AkaitoAiExtensions.Seconds(showBuyErrorFor);

            buyButton.interactable = true;
            buyErrorObj.SetActive(false);
            if (lockedImageObj) lockedImageObj.SetActive(true);
        }

    }

    private void SwitchBetween(int direction)
    {
        // TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        if (selectables.details == null || selectables.details.Length == 0) return;

        selectables.details[currentSelectedIndex].Obj.SetActive(false);

        currentSelectedIndex = AkaitoAiExtensions.GetIndexWithDirectionWrap(direction, currentSelectedIndex, selectables.details.Length);

        //int length = selectables.details.Length;
        //for (int i = 0; i < length; i++)
        //    selectables.details[i].Obj.SetActive(false);

        selectables.details[currentSelectedIndex].Obj.SetActive(true);

        UpdateSpecs((SelectSpecsSO)selectables.details[currentSelectedIndex].specs);

        CheckUnlocked();
    }

    private void CheckUnlocked()
    {
        bool isUnlocked = PlayerPrefs.GetInt(setupScriptable.selectBoughtPref + currentSelectedIndex) == 1 ||
            PlayerPrefs.GetInt(setupScriptable.selectTryOncePref + currentSelectedIndex) == 1;

        if (isUnlocked)
        {
            if (tryOnceButton != null) tryOnceButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            if (lockedImageObj) lockedImageObj.SetActive(false);
            if (priceContainer) priceContainer.SetActive(false);
            selectButton.gameObject.SetActive(true);

            priceText.text = "Purchased";
        }
        else
        {
            if (tryOnceButton != null) tryOnceButton.gameObject.SetActive(true);
            if (priceContainer) priceContainer.SetActive(true);
            buyButton.gameObject.SetActive(true);
            if (lockedImageObj) lockedImageObj.SetActive(true);
            selectButton.gameObject.SetActive(false);
            StartCoroutine(TextLerp(currentSelectedIndex));
        }

        //if (PlayerPrefs.GetInt(setupScriptable.selectBoughtPref + currentSelectedIndex) == 1) priceText.color = Color.green;
        //else if (PlayerPrefs.GetInt(setupScriptable.totalCoinsPref) >= selectables.details[currentSelectedIndex].price
        //    && PlayerPrefs.GetInt(setupScriptable.selectBoughtPref + currentSelectedIndex) == 0)
        //{
        //    priceText.color = Color.yellow;
        //    //GetBuyButtonShiny().enabled = true;
        //}
        //else
        //{
        //    priceText.color = Color.red;
        //    //GetBuyButtonShiny().enabled = false;
        //}

        //UIShiny GetBuyButtonShiny()
        //{
        //    if (buyButton.TryGetComponent<UIShiny>(out UIShiny _btnShiny))
        //        return _btnShiny;

        //    return null;
        //}

        IEnumerator TextLerp(int _priceIndex)
        {
            float timeElapsed = 0;
            while (timeElapsed < priceLerpDuration)
            {
                float price = Mathf.Lerp(0f, selectables.details[currentSelectedIndex].price, timeElapsed / priceLerpDuration);

                if (!useDollarSignBeforePrice) priceText.text = Mathf.FloorToInt(price).ToString();
                else priceText.text = "$" + Mathf.FloorToInt(price).ToString();

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            if (PlayerPrefs.GetInt(setupScriptable.selectBoughtPref + currentSelectedIndex) == 0)
            {
                if (!useDollarSignBeforePrice) priceText.text = selectables.details[currentSelectedIndex].price.ToString();
                else priceText.text = "$" + selectables.details[currentSelectedIndex].price.ToString();
            }
            else
            {
                priceText.text = "Purchased";
            }
        }
    }

    public void OnTryOnceRewardButton()
    {
        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance.PlayOnButtonSound();

        AdsWrapper.GetInstance()?.ShowRewardedAds(Reward, AdsWrapper.GetInstance().ShowAdNotAvailable);

        void Reward()
        {
            PlayerPrefs.SetInt(setupScriptable.selectTryOncePref + currentSelectedIndex, 1);

            CheckUnlocked();
        }
    }

    private void UpdateTotalCoins()
    {
        if (coinsText != null) coinsText.text = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref).ToString(); //! Shows Total money text
    }

    public void UpdateSpecs(SelectSpecsSO specs)
    {
        if (specs == null) return;

        switch (selectables.ui.type)
        {
            case SpecsUIType.None:
                selectables.ui.UpdateFiller(specs);
                selectables.ui.UpdateText(specs);
                break;

            case SpecsUIType.Lerp:
                StartCoroutine(selectables.ui.FillerLerp(specs));
                StartCoroutine(selectables.ui.TextLerp(specs));
                break;
            
            case SpecsUIType.Tween:
                selectables.ui.FillerTween(specs);
                selectables.ui.TextTween(specs);
                break;
            
            default:  break;
        }
    }
}
