using AkaitoAi.Advertisement;
using AkaitoAi.GameBase;


//using RichTap;
using UnityEngine;
using UnityEngine.UI;

public class RewardedADButton : MonoBehaviour
{
    [SerializeField] private int rewardAmount = 500;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Reward);
    }

    private void Reward()
    {
        //TODO Sound Calling
        //SoundManager.Instance?.PlayOnButtonSound();

        AdsWrapper.GetInstance()?.ShowRewardedAds(() => 
        {
            //TODO Sound Calling
            //SoundManager.Instance?.PlayRewardGrantSound();

            //TODO Vibration
            //RichtapEffectSource.Instance?.Play(RichTap.Common.RichtapPreset.RT_AWARD);

            PlayerPrefs.SetInt(MenuManager.GetInstance()?.setupScriptable.totalCoinsPref,
                PlayerPrefs.GetInt(MenuManager.GetInstance()?.setupScriptable.totalCoinsPref) + rewardAmount);

            EventBus<OnCoinsCharged>.Raise(new OnCoinsCharged { });
        }, AdsWrapper.GetInstance().ShowAdNotAvailable);
    }

}
