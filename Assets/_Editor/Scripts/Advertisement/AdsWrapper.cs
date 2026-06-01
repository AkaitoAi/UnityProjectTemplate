using AkaitoAi.Singleton;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AkaitoAi.Extensions;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace AkaitoAi.Advertisement
{
    public class AdsWrapper : SingletonPresistent<AdsWrapper>
    {
        [SerializeField] private GameObject loadingCanvas, loadingPanel, 
            loadingTextPanel, adNotAvailable;
        [Space]

        [Header("Loading Count")]
        [SerializeField] private GameObject loadingCountPanel;
        [SerializeField] private Text loadingCountText;
        [SerializeField] private float countdownTime = 3;
        [SerializeField] private float adAfterSeconds = 60;
        private Coroutine runtimeInterstitial;
        [Space]

        [Header("Filler Loading")]
        [SerializeField] private GameObject fillerLoadingPanel;
        [SerializeField] private Image fillerImage;
        [SerializeField] private Text fillerProgressText;
        [SerializeField] private float fillerLoadingTotalDuration = 2f;
        [SerializeField] private float fillerLoadingAdTriggerFraction = 0.75f;
        [Space]


        private float currentTime;

        [Header("Internet Reachability")]
        [SerializeField] private bool isCheckingInternet = false;
        [SerializeField] private GameObject noInternetPanel;
        [SerializeField] private Button openSettingsButton;
        [SerializeField] private float checkInterval = 5f;
        private float lastCheckTime;
        private bool isChecking;
        private const float INITIAL_DELAY = 1f;


        #region Ads Calling

        //Show Ads Calling
        public void ShowSmallBannerTop()
        {
            //AdsController.Instance.ShowBannerAd_Admob(0);
        }
        public void ShowSmallBannerTopLeft()
        {
            //AdsController.Instance.ShowBannerAd_Admob(1);
        }
        public void ShowSmallBannerTopRight()
        {
            //AdsController.Instance.ShowBannerAd_Admob(2);
        }
        public void ShowMediumBannerBottomLeft()
        {
            //AdsController.Instance.ShowBannerAd_Admob(3);
        }
        public void ShowMediumBannerBottomRight()
        {
            //AdsController.Instance.ShowBannerAd_Admob(4);
        }
        public void ShowAdaptiveBannerTop()
        {
            //AdsController.Instance.ShowBannerAd_Admob(5);
        }
        public void ShowCustomAdaptiveBannerTopLeft()
        {
            //AdsController.Instance.ShowBannerAd_Admob(6);
        }
        public void ShowCustomAdaptiveBannerTopRight()
        {
            //AdsController.Instance.ShowBannerAd_Admob(7);
        }

        public void ShowInterstitial()
        {
            //AdsController.Instance.ShowInterstitialAd_Admob();
        }

        // Interstitial Panels
        public void ShowInterstitialWithLoadingPanel(Action Behaviour = null, Action noInternet = null)
        {
            InternetReachability(() => StartCoroutine(LoadInterstital()), () =>
            {
                Behaviour?.Invoke();
                noInternet?.Invoke();
            });

            IEnumerator LoadInterstital()
            {
                loadingCanvas.SetActive(true);
                loadingPanel.SetActive(true);

                yield return new WaitForSecondsRealtime(1.5f);

                //AdsController.Instance.ShowInterstitialAd_Admob();

                yield return new WaitForSecondsRealtime(.5f);

                loadingCanvas.SetActive(false);
                loadingPanel.SetActive(false);

                Behaviour?.Invoke();
            }
        }
        public void ShowInterstitialWithLoadingText(Action Behaviour = null)
        {
            InternetReachability(() => StartCoroutine(LoadInterstital()), () => Behaviour?.Invoke());

            IEnumerator LoadInterstital()
            {
                loadingCanvas.SetActive(true);
                loadingTextPanel.SetActive(true);

                yield return new WaitForSecondsRealtime(1.5f);

                //AdsController.Instance.ShowInterstitialAd_Admob();

                loadingCanvas.SetActive(false);
                loadingTextPanel.SetActive(false);

                Behaviour?.Invoke();
            }
        }
        public void ShowInterstitialWithLoadingCount(Action Behaviour = null)
        {
            InternetReachability(() => {

                currentTime = countdownTime;
                loadingCanvas.SetActive(true);
                loadingCountPanel.SetActive(true);

                StartCoroutine(LoadInterstital());
            }
            , () => Behaviour?.Invoke());

            IEnumerator LoadInterstital()
            {
                while (currentTime > 0)
                {
                    //loadingCountText.text = $"AD in {currentTime.ToString()} ";
                    loadingCountText.text = $"{currentTime.ToString()}";

                    //TODO Sounds Calling
                    //SoundManager.Instance?.PlayNormalTimerSound();

                    yield return new WaitForSeconds(1f);

                    currentTime--;
                }

                //AdsController.Instance.ShowInterstitialAd_Admob();

                //TODO Sounds Calling
                //SoundManager.Instance?.sfxAudioSource.Stop();

                loadingCanvas.SetActive(false);
                loadingCountPanel.SetActive(false);

                Behaviour?.Invoke();
            }
        }
        public void ShowInterstitialWithFillerLoading(Action Behaviour = null)
        {
            Time.timeScale = 1f;

            InternetReachability(() => StartCoroutine(LoadInterstital()), () => Behaviour?.Invoke());

            IEnumerator LoadInterstital()
            {
                loadingCanvas.SetActive(true);
                fillerLoadingPanel.SetActive(true);

                fillerProgressText.text = "0%";
                fillerImage.fillAmount = 0f;

                float adTriggerTime = fillerLoadingTotalDuration * fillerLoadingAdTriggerFraction;
                float counterTime = 0f;

                while (counterTime <= fillerLoadingTotalDuration)
                {
                    counterTime += Time.deltaTime;
                    float progress = Mathf.Clamp01(counterTime / fillerLoadingTotalDuration);

                    if (fillerProgressText != null)
                        fillerProgressText.text = Mathf.RoundToInt(progress * 100).ToString() + "%";
                    if (fillerImage != null)
                        fillerImage.fillAmount = progress;

                    if (counterTime >= adTriggerTime && counterTime - Time.deltaTime < adTriggerTime)
                    {
                        //AdsController.Instance.ShowInterstitialAd_Admob();
                    }

                    yield return new WaitForEndOfFrame();
                }

                loadingCanvas.SetActive(false);
                fillerLoadingPanel.SetActive(false);
                Behaviour?.Invoke();
            }
        }
        public void StartRuntimeCommercial()
        {
            StopRuntimeCommercial();

            runtimeInterstitial =
                StartCoroutine(AkaitoAiExtensions.SimpleDelay(
                    adAfterSeconds, () => {
                        ShowInterstitialWithLoadingCount(() => {
                            StartRuntimeCommercial();
                        });
                    }));
        }
        public void StopRuntimeCommercial()
        {
            if (runtimeInterstitial != null) StopCoroutine(runtimeInterstitial);
        }
        //

        public void HideSmallBannerTop()
        {
            //AdsController.Instance.HideBannerAd_Admob(0);
        }
        public void HideSmallBannerTopLeft()
        {
            //AdsController.Instance.HideBannerAd_Admob(1);
        }
        public void HideSmallBannerTopRight()
        {
            //AdsController.Instance.HideBannerAd_Admob(2);
        }
        public void HideMediumBannerBottomLeft()
        {
            //AdsController.Instance.HideBannerAd_Admob(3);
        }
        public void HideMediumBannerBottomRight()
        {
            //AdsController.Instance.HideBannerAd_Admob(4);
        }
        public void HideAdaptiveBannerTop()
        {
            //AdsController.Instance.HideBannerAd_Admob(5);
        }
        public void HideCustomAdaptiveBannerTopLeft()
        {
            //AdsController.Instance.HideBannerAd_Admob(6);
        }
        public void HideCustomAdaptiveBannerTopRight()
        {
            //AdsController.Instance.HideBannerAd_Admob(7);
        }
        public void HideAllBanners()
        {
            //AdsController.Instance.HideAllBanners_Admob();
        }
        #endregion

        #region Internet Reachability

        public void InternetReachability(Action withInternet)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                withInternet?.Invoke();
        }

        public void InternetReachability(Action withInternet, Action withoutInternet)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                withInternet?.Invoke();
            else withoutInternet?.Invoke();
        }

        public void InternetReachability(IEnumerator withInternet, Action withoutInternet)
        {
             if (Application.internetReachability != NetworkReachability.NotReachable)
                StartCoroutine(withInternet);
            else withoutInternet?.Invoke();
        }

        public void InternetReachability(Action withInternet, IEnumerator withoutInternet)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                withInternet?.Invoke();
            else StartCoroutine(withoutInternet);
        }

        public void InternetReachability(IEnumerator withInternet, IEnumerator withoutInternet)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                StartCoroutine(withInternet);
            else StartCoroutine(withoutInternet);
        }

        //AndroidManifest.xml: Create this file in Assets/Plugins/Android/
        //<?xml version="1.0" encoding="utf-8"?>
        //<manifest xmlns:android="http://schemas.android.com/apk/res/android">
        //<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
        //<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
        //<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
        //<application>
        //<activity android:name="com.unity3d.player.UnityPlayerActivity">
        //</activity>
        //</application>
        //</manifest>
        public void CheckInternetConnection()
        {
            if (isChecking) return; // Prevent overlapping checks

            isChecking = true;
            lastCheckTime = Time.time;

            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    ShowNoInternetUI();
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    HideNoInternetUI();
                    break;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    HideNoInternetUI();
                    break;
            }

            isChecking = false;
        }
        private void ShowNoInternetUI()
        {
            loadingCanvas.SetActive(true);
            noInternetPanel.SetActive(true);
        }

        private void HideNoInternetUI()
        {
            loadingCanvas.SetActive(false);
            noInternetPanel.SetActive(false);
        }
        private void OpenWifiSettings()
        {
#if UNITY_ANDROID
            try
            {
                // Check and request permission
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                    return; // Will check again after permission granted
                }

                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
                {
                    intentObject.Call<AndroidJavaObject>("setAction", "android.settings.WIFI_SETTINGS");
                    currentActivity.Call("startActivity", intentObject);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error opening WiFi settings: " + e.Message);
            }
#else
        //statusText.text = "WiFi settings only available on Android";
#endif
        }

        #endregion

        #region Rewarded Ads
        public void ShowRewardedAdsLoading(Action reward,
            Action noInternet, Action adNotAvailable)
        {
#if UNITY_EDITOR

            reward?.Invoke();

#else
            if (Application.internetReachability
                == NetworkReachability.NotReachable) noInternet?.Invoke();
            //else AdsController.Instance.ShowRewardedAd_Loading_Admob(reward, adNotAvailable);
#endif
        }
        public void ShowRewardedAds(Action reward,
            Action noInternet, Action adNotAvailable)
        {
#if UNITY_EDITOR

            reward?.Invoke();

#else
            if (Application.internetReachability
                == NetworkReachability.NotReachable) noInternet?.Invoke();
            //else AdsController.Instance.ShowRewardedAd_Admob(reward, adNotAvailable);
#endif
        }

        public void ShowRewardedAds(Action reward, Action adNotAvailable)
        {
#if UNITY_EDITOR

            reward?.Invoke();

#else
            //AdsController.Instance.ShowRewardedAd_Admob(reward, adNotAvailable);
#endif
        }

        public void ShowRewardedAdsLoading(Action reward, Action adNotAvailable)
        {
#if UNITY_EDITOR

            reward?.Invoke();

#else
            //AdsController.Instance.ShowRewardedAd_Loading_Admob(reward, adNotAvailable);
#endif
        }

        public void ShowAdNotAvailable()
        {
            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                loadingCanvas.SetActive(true);
                adNotAvailable.SetActive(true);

                yield return new WaitForSecondsRealtime(2f);

                loadingCanvas.SetActive(false);
                adNotAvailable.SetActive(false);
            }
        }


        #endregion

        #region Firebase
        public void FirebaseLog(string message)
        {
            InternetReachability(() =>
            {
                //FirebaseAnalyticsHandler.Instance.LogFirebaseEvent(message);
            });
        }
        public void FirebaseLog(string message1, string message2, string message3)
        {
            InternetReachability(() =>
            {
                //FirebaseAnalyticsHandler.Instance.LogFirebaseEvent(message1, message2, message3);
            });
        }
        public void FirebaseLogGroup(string eventName, int totalParameters, string[] parameterName, string[] parameterValue)
        {
            InternetReachability(() =>
            {
                //FirebaseAnalyticsHandler.Instance.LogFirebaseEvent_Group(eventName, totalParameters, parameterName, parameterValue);
            });
        }
        #endregion

        #region Events

        EventBinding<OnEnterLeftSidedAd> enterLeftSidedAdEventBinding;
        EventBinding<OnExitLeftSidedAd> exitLeftSidedAdEventBinding;

        private void Start()
        {
            if (!isCheckingInternet) return;

            noInternetPanel.SetActive(false);

            Invoke(nameof(CheckInternetConnection), INITIAL_DELAY);

            openSettingsButton.onClick.AddListener(OpenWifiSettings);
        }
        private void Update()
        {
            if (!isCheckingInternet) return;

            if (Time.time - lastCheckTime >= checkInterval && !isChecking)
            {
                CheckInternetConnection();
            }
        }
        private void OnEnable()
        {
            enterLeftSidedAdEventBinding = new EventBinding<OnEnterLeftSidedAd>(() => { 
                HideSmallBannerTopRight();
                ShowMediumBannerBottomLeft();
            });
            EventBus<OnEnterLeftSidedAd>.Register(enterLeftSidedAdEventBinding);

            exitLeftSidedAdEventBinding = new EventBinding<OnExitLeftSidedAd>(() => {
                HideMediumBannerBottomLeft();
                ShowSmallBannerTopRight();
            });
            EventBus<OnExitLeftSidedAd>.Register(exitLeftSidedAdEventBinding);
        }

        private void OnDisable()
        {
            EventBus<OnEnterLeftSidedAd>.Deregister(enterLeftSidedAdEventBinding);
            EventBus<OnExitLeftSidedAd>.Deregister(exitLeftSidedAdEventBinding);
        }

        #endregion
    }
    public struct OnEnterLeftSidedAd : IEvent { }
    public struct OnExitLeftSidedAd : IEvent { }
}