using AkaitoAi;
using AkaitoAi.GameBase;
using TMPro;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

public class CoinsUpdater : MonoBehaviour
{
    [SerializeField] private bool animateText = false;
    [GetComponent] [SerializeField]private TMP_Text _text;
    
    private SetupSO setupScriptable;
#if DOTWEEN
    private Tween _tween;
#endif

    //Bindings
    EventBinding<OnCoinsCharged> coinsChargedEventBinding;

    private void Awake()
    {
        setupScriptable = RuntimeSetup.GetInstance()?.Setup;

        coinsChargedEventBinding = new EventBinding<OnCoinsCharged>(UpdateCoins);
        EventBus<OnCoinsCharged>.Register(coinsChargedEventBinding);
        
        UpdateCoins();
    }

    private void UpdateCoins()
    {
        if (_text == null) return;

        int targetCoins = PlayerPrefs.GetInt(setupScriptable.totalCoinsPref);

        if (!_text.gameObject.activeInHierarchy)
        {
            _text.text = targetCoins.ToString();
            return;
        }

        if (!animateText)
        {
            _text.text = targetCoins.ToString();
            return;
        }

#if DOTWEEN
        int currentCoins = int.TryParse(_text.text, out int parsed) ? parsed : 0;

        _tween?.Kill();

        _tween = this.DOIntCounterPunch()
            .From(currentCoins)
            .To(targetCoins)
            .Duration(0.25f)
            .OnValueChanged(v => _text.text = v.ToString())
            .Yoyo(_text.transform, 0.15f, 0.08f, 0.12f)
            .Start()
            .OnComplete(() => _text.text = targetCoins.ToString());
#else
        _text.text = targetCoins.ToString();
#endif
    }


    private void OnDestroy()
    {
        EventBus<OnCoinsCharged>.Deregister(coinsChargedEventBinding);
    }
}
