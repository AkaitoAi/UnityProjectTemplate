using AkaitoAi.GameBase;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AkaitoAi.Advertisement
{
    public class DailyReward7 : MonoBehaviour
    {
        private const int TotalDays = 7;
        private const int SecondsInADay = 86400; // 24 hours in seconds

        [SerializeField] private int secondsToWait = SecondsInADay;
        [SerializeField] private Reward[] rewardButtons = new Reward[TotalDays];
        [SerializeField] private TMP_Text displayText;

        [SerializeField] private bool allowBothClaimOptions = false; // Option to enable both claim modes
        private ulong lastRewarded;
        private float secondsLeft;

        [SerializeField] private GameObject dailyRewardPanel, rewardedAdBtn, claimNowBtn;
        [SerializeField] private string coinPref = "TotalCoins";

        [SerializeField] private RewardGranted rewardGranted;

        public static bool CanGetRewardNow { get; private set; }

        [Serializable]
        public struct RewardGranted
        {
            public GameObject obj;

            public Image image;
            
            public Text text;

            public Sprite[] rewardSprites;
        }

        [Serializable]
        public struct Reward
        {
            public Button button;
            public int reward;
            public UnityEvent onAvailable;    // Event for when reward is available
            public UnityEvent onClaimed;      // Event for when reward is claimed
            public UnityEvent onNotAvailable; // Event for when reward is not available

            public void SetInteractable(bool interactable)
            {
                button.interactable = interactable;
                if (interactable) onAvailable?.Invoke(); // Trigger available effect
                else onNotAvailable?.Invoke(); // Trigger not available effect
            }
        }

        private void OnEnable() => Initialize();

        private void Start()
        {
            coinPref = RuntimeSetup.GetInstance()?.Setup.totalCoinsPref;

            lastRewarded = ulong.Parse(PlayerPrefs.GetString("LastRewarded", "0"));
            CheckRewardAvailability();
            AddButtonListeners();
        }

        private void Update() => UpdateRewardState();

        private void Initialize()
        {
            foreach (var reward in rewardButtons)
                reward.SetInteractable(false);

            for (int day = 0; day < TotalDays; day++)
            {
                if (PlayerPrefs.GetInt("ClaimDay" + day) == 1)
                    rewardButtons[day].onClaimed?.Invoke(); // Trigger claimed effect
            }
        }

        private void AddButtonListeners()
        {
            // Claim Button Listener
            if (claimNowBtn.TryGetComponent<Button>(out Button claimBtn))
                claimBtn.onClick.AddListener(OnGetRewardButton);

            if (rewardedAdBtn.TryGetComponent<Button>(out Button adBtn))
                adBtn.onClick.AddListener(OnRewardAdButton);

            // Reward Button Listeners (if allowed)
            if (allowBothClaimOptions)
            {
                for (int i = 0; i < rewardButtons.Length; i++)
                {
                    int day = i; // Capture the current day
                    rewardButtons[i].button.onClick.AddListener(() => OnGetRewardButton(day));
                }
            }
        }

        private void CheckRewardAvailability()
        {
            if (dailyRewardPanel == null) return;

            // Determine if rewards can be claimed and update UI accordingly
            if (CanGetReward())
            {
                dailyRewardPanel.SetActive(true);
                rewardButtons[PlayerPrefs.GetInt("Day")].SetInteractable(true);
            }
            else rewardedAdBtn.SetActive(true);
        }

        private void UpdateRewardState()
        {
            if (CanGetReward())
            {
                SetDisplayText("Claim Now!");
                rewardButtons[PlayerPrefs.GetInt("Day")].SetInteractable(true);
            }
            else
            {
                SetDisplayText(GetRemainingTimeText());
                rewardButtons[PlayerPrefs.GetInt("Day")].SetInteractable(false);
            }
        }

        public void OnGetRewardButton()
        {
            int currentDay = PlayerPrefs.GetInt("Day");
            OnGetRewardButton(currentDay);
        }

        public void OnGetRewardButton(int day)
        {
            if (!CanGetReward()) return;

            ShowRewardMessage(day, rewardButtons[day].reward);
            UpdatePlayerRewards(day);
            UpdateClaimState(day);
        }

        private void ShowRewardMessage(int day, int reward)
        {
            if (rewardGranted.obj == null) return;

            if (rewardGranted.text == null)
            {
                if (rewardGranted.obj.transform.GetChild(0).GetChild(0).TryGetComponent<Text>(out Text textComp))
                {
                    textComp.text = $"YOU HAVE GOT {reward}$ COINS";
                }
                Instantiate(rewardGranted.obj);
            }
            else
            {
                rewardGranted.obj.SetActive(true);
                rewardGranted.text.text = $"YOU HAVE GOT {reward}$ COINS";
                rewardGranted.image.sprite = rewardGranted.rewardSprites[day];
                rewardGranted.image.SetNativeSize();

                if (rewardGranted.obj.activeInHierarchy)
                    StartCoroutine(DiableAfterDelay());

            }

            //reward.ToString() + rewardType;
            //"YOU GOT YOUR DAY " + PlayerPrefs.GetInt("Day").ToString() + "  REWARD";

            IEnumerator DiableAfterDelay()
            {
                yield return new WaitForSeconds(5f);

                rewardGranted.obj.SetActive(false);
            }
        }

        private void UpdatePlayerRewards(int day)
        {
            PlayerPrefs.SetInt(coinPref, PlayerPrefs.GetInt(coinPref) + rewardButtons[day].reward);
            lastRewarded = (ulong)DateTime.Now.Ticks;
            PlayerPrefs.SetString("LastRewarded", lastRewarded.ToString());

            EventBus<OnCoinsCharged>.Raise(new OnCoinsCharged { });
        }

        private void UpdateClaimState(int day)
        {
            PlayerPrefs.SetInt("ClaimDay" + day, 1);

            // Cycle days and reset if needed
            if (day >= TotalDays - 1)
            {
                PlayerPrefs.SetInt("Day", 0);
                //ResetAllClaimDays();
            }
            else PlayerPrefs.SetInt("Day", day + 1);

            DisableButtonEffects();
            rewardedAdBtn.SetActive(true);
            rewardButtons[day].onClaimed?.Invoke(); // Trigger claimed effect
        }

        private void DisableButtonEffects()
        {
            int currentDay = PlayerPrefs.GetInt("Day");

            for (int i = 0; i <= currentDay; i++)
            {
                rewardButtons[i].SetInteractable(false);

                if (PlayerPrefs.GetInt("ClaimDay" + i) == 1)
                    rewardButtons[i].onClaimed?.Invoke(); // Trigger claimed effect
            }

            //for (int day = 0; day < TotalDays; day++)
            //{
            //    if (PlayerPrefs.GetInt("ClaimDay" + day) == 1)
            //        rewardButtons[day].onClaimed?.Invoke(); // Trigger claimed effect
            //}
        }

        private bool CanGetReward()
        {
            // Calculate the difference in ticks as a long
            long difference = DateTime.Now.Ticks - (long)lastRewarded;

            // Convert the difference from ticks to seconds
            secondsLeft = secondsToWait - (difference / TimeSpan.TicksPerSecond);

            // Check if the reward can be claimed
            if (secondsLeft <= 0)
            {
                ResetRewardUI();

                CanGetRewardNow = true;

                return true;
            }

            CanGetRewardNow = false;
            return false;
        }

        private void ResetRewardUI()
        {
            rewardedAdBtn.SetActive(false);

            if (PlayerPrefs.GetInt("Day") == 0)
                ResetAllClaimDays();
        }

        private string GetRemainingTimeText()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(secondsLeft);
            return $"{timeSpan.Hours:D2}h {timeSpan.Minutes:D2}m {timeSpan.Seconds:D2}s";
        }

        private void SetDisplayText(string text)
        {
            if (displayText != null) 
                displayText.text = text;
        }

        private void ResetAllClaimDays()
        {
            for (int i = 0; i < TotalDays; i++)
                PlayerPrefs.SetInt("ClaimDay" + i, 0);
        }

        public void OnRewardAdButton()
        {
            AdsWrapper.GetInstance()
                .ShowRewardedAds(Reward
                , () => NoInternet()
                , () => NoAD());

            void NoInternet() { }
            void NoAD() { }

            void Reward()
            {
                if (allowBothClaimOptions)
                    rewardButtons[PlayerPrefs.GetInt("ClaimDay") + PlayerPrefs.GetInt("Day")].button.onClick.AddListener(OnGetRewardButton);
                
                rewardButtons[PlayerPrefs.GetInt("ClaimDay") + PlayerPrefs.GetInt("Day")].button.interactable = true;
                PlayerPrefs.SetString("LastRewarded", "0");
                string lastSavedTime = PlayerPrefs.GetString("LastRewarded");
                lastRewarded = ulong.Parse(lastSavedTime);
            }
        }
    }
}
