using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AkaitoAi.Advertisement;
using AkaitoAi.GameBase;
using TMPro;
#if DOTWEEN
using DG.Tweening;
#endif

#region ===================== EVENT HUB =====================

public static class RewardEvents
{
    public static event Action<RewardType> OnRewardGranted;

    public static void RaiseRewardGranted(RewardType rewardType)
    {
        OnRewardGranted?.Invoke(rewardType);
    }
}

#endregion

#region ===================== ENUM =====================

public enum RewardType
{
    UnlockEverything,
    UnlockAllWeapons,
    UnlockAllCars,
    RemoveAds,
    UnlockAllModesAndLevels,
    Coins
}

#endregion

#region ===================== STRATEGY INTERFACE =====================

public interface IRewardStrategy
{
    void GrantReward();
    string GetDescription();
}

#endregion

#region ===================== STRATEGIES =====================

public class UnlockEverythingStrategy : IRewardStrategy
{
    private readonly List<IRewardStrategy> subStrategies;

    public UnlockEverythingStrategy(List<IRewardStrategy> subStrategies)
    {
        this.subStrategies = subStrategies;
    }

    public void GrantReward()
    {
        foreach (var strategy in subStrategies)
            strategy.GrantReward();

        // Fire ONCE
        RewardEvents.RaiseRewardGranted(RewardType.UnlockEverything);

        //NotificationEvents.OnNotify?.Invoke(new NotificationData("Unlocked EVERYTHING!",
        //    duplicateMode: DuplicateNotificationMode.ReplaceOldImmediately, layoutMode: NotificationLayoutMode.Queue));

        Debug.Log("Unlocked EVERYTHING!");
    }

    public string GetDescription() => "Unlock Everything";
}

public class UnlockAllCarsStrategy : IRewardStrategy
{
    public void GrantReward()
    {
        Debug.Log("Unlocked all cars!");
        RewardEvents.RaiseRewardGranted(RewardType.UnlockAllCars);

        //NotificationEvents.OnNotify?.Invoke(new NotificationData("Unlocked All Vehicles!",
        //    duplicateMode: DuplicateNotificationMode.ReplaceOldImmediately, layoutMode: NotificationLayoutMode.Queue));
    }

    public string GetDescription() => "Unlock All Cars";
}

public class RemoveAdsStrategy : IRewardStrategy
{
    public void GrantReward()
    {
        Debug.Log("Ads removed!");
        RewardEvents.RaiseRewardGranted(RewardType.RemoveAds);

        //NotificationEvents.OnNotify?.Invoke(new NotificationData("Removed Ads!",
        //    duplicateMode: DuplicateNotificationMode.ReplaceOldImmediately, layoutMode: NotificationLayoutMode.Queue));
    }

    public string GetDescription() => "Remove Ads";
}
public class UnlockAllWeaponsStrategy : IRewardStrategy
{
    public void GrantReward()
    {
        Debug.Log("Unlocked all weapons");
        RewardEvents.RaiseRewardGranted(RewardType.UnlockAllWeapons);

        //NotificationEvents.OnNotify?.Invoke(new NotificationData("Removed Ads!",
        //    duplicateMode: DuplicateNotificationMode.ReplaceOldImmediately, layoutMode: NotificationLayoutMode.Queue));
    }

    public string GetDescription() => "Unlocked All Weapons";
}

public class UnlockAllModesAndLevelsStrategy : IRewardStrategy
{
    public void GrantReward()
    {
        Debug.Log("Unlocked all modes and levels!");
        RewardEvents.RaiseRewardGranted(RewardType.UnlockAllModesAndLevels);

        //NotificationEvents.OnNotify?.Invoke(new NotificationData("Unlocked All Modes & Levels!",
        //    duplicateMode: DuplicateNotificationMode.ReplaceOldImmediately, layoutMode: NotificationLayoutMode.Queue));
    }

    public string GetDescription() => "Unlock All Modes & Levels";
}

public class CoinsRewardStrategy : IRewardStrategy
{
    private readonly int amount;

    public CoinsRewardStrategy(int amount)
    {
        this.amount = amount;
    }

    public void GrantReward()
    {
        string key = RuntimeSetup.GetInstance().Setup.totalCoinsPref;
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + amount);

        EventBus<OnCoinsCharged>.Raise(new OnCoinsCharged { });

        RewardEvents.RaiseRewardGranted(RewardType.Coins);

        Debug.Log($"Granted {amount} coins!");

        //NotificationEvents.OnNotify?.Invoke(new NotificationData($"Granted {amount} Coins!",
        //    duplicateMode: DuplicateNotificationMode.ReplaceOldImmediately, layoutMode: NotificationLayoutMode.Queue));
    }

    public string GetDescription() => $"{amount} Coins";
}

#endregion

#region ===================== STRATEGY FACTORY =====================

public static class RewardStrategyFactory
{
    public static IRewardStrategy CreateStrategy(RewardType type, int coinAmount)
    {
        return type switch
        {
            RewardType.UnlockEverything => new UnlockEverythingStrategy(
                new List<IRewardStrategy>
                {
                    new UnlockAllCarsStrategy(),
                    new UnlockAllWeaponsStrategy(),
                    new RemoveAdsStrategy(),
                    new UnlockAllModesAndLevelsStrategy(),
                    new CoinsRewardStrategy(coinAmount > 0 ? coinAmount : 1000)
                }),

            RewardType.UnlockAllCars => new UnlockAllCarsStrategy(),
            RewardType.UnlockAllWeapons => new UnlockAllWeaponsStrategy(),
            RewardType.RemoveAds => new RemoveAdsStrategy(),
            RewardType.UnlockAllModesAndLevels => new UnlockAllModesAndLevelsStrategy(),
            RewardType.Coins => new CoinsRewardStrategy(coinAmount),

            _ => null
        };
    }
}

#endregion

#region ===================== REWARD DATA =====================

[Serializable]
public class Reward
{
    public string rewardName;
    public RewardType rewardType;
    public int adsRequired = 3;
    public bool isPermanent = true;

    public Button actionButton;
    public TMP_Text adsCountText;

    [Header("Coins Only")]
    public int coinAmount = 1000;

    [Space]
    public UnityEvent OnInitializeEvent;
    public UnityEvent OnWatchedEvent;
    public UnityEvent OnClaimedEvent;

    private int adsWatched;
    private IRewardStrategy rewardStrategy;
    private string prefsKey;
    private string claimedKey;

    public void Initialize()
    {
        prefsKey = "RewardShop_" + rewardName;
        claimedKey = prefsKey + "_claimed";

        adsWatched = PlayerPrefs.GetInt(prefsKey, 0);
        bool alreadyClaimed = PlayerPrefs.GetInt(claimedKey, 0) == 1;

        rewardStrategy = RewardStrategyFactory.CreateStrategy(rewardType, coinAmount);

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnButtonPressed);

        RewardEvents.OnRewardGranted += OnGlobalRewardGranted;

        if (alreadyClaimed && isPermanent)
            ForceClaimedState();
        else
            UpdateUI();

        OnInitializeEvent?.Invoke();
    }

    public void Dispose()
    {
        RewardEvents.OnRewardGranted -= OnGlobalRewardGranted;
    }

    private void OnButtonPressed()
    {
        if (isPermanent && PlayerPrefs.GetInt(claimedKey, 0) == 1)
            return;

        //TODO Sound Calling
        //SoundManager.Instance?.PlayOnButtonSound();

        if (adsWatched < adsRequired)
        {
            AdsWrapper.GetInstance()?.ShowRewardedAds(WatchAd, null);
        }
        else
        {
            ClaimReward();
            OnClaimedEvent?.Invoke();

            //TODO Sound Calling
            //SoundManager.Instance?.PlayRewardSound();
        }
    }

    private void WatchAd()
    {
        adsWatched++;
        SaveProgress();
        UpdateUI();
        OnWatchedEvent?.Invoke();
    }

    private void ClaimReward()
    {
        rewardStrategy?.GrantReward();

        if (isPermanent)
        {
            ForceClaimedState();
        }
        else
        {
            adsWatched = 0;
            SaveProgress();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (adsWatched < adsRequired)
        {
            adsCountText.text = $"{adsWatched}/{adsRequired}";
            actionButton.interactable = true;
        }
        else
        {
            adsCountText.text = "CLAIM";
            actionButton.interactable = true;
        }

#if DOTWEEN
        adsCountText.transform.DOPunchScale(
                Vector3.one * 0.12f,
                0.2f,
                6,
                0.7f
            );
#endif
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(prefsKey, adsWatched);
        PlayerPrefs.Save();
    }

    private void OnGlobalRewardGranted(RewardType grantedType)
    {
        if (!isPermanent)
            return;

        if (grantedType == RewardType.UnlockEverything &&
            rewardType != RewardType.UnlockEverything)
        {
            ForceClaimedState();
        }

        if (grantedType == rewardType)
        {
            ForceClaimedState();
        }
    }

    private void ForceClaimedState()
    {
        PlayerPrefs.SetInt(claimedKey, 1);
        PlayerPrefs.Save();

        actionButton.interactable = false;
        adsCountText.text = "CLAIMED";
    }
}

#endregion

#region ===================== SHOP MANAGER =====================

public class RewardShopManager : MonoBehaviour
{
    [SerializeField] private Reward[] rewards;

    private void Start()
    {
        foreach (var reward in rewards)
            reward.Initialize();
    }

    private void OnDestroy()
    {
        foreach (var reward in rewards)
            reward.Dispose();
    }
}

#endregion
