using System;
using UnityEngine;
using UnityEngine.UI;

public class RadioManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private ToggleButton playPauseButton;

    [SerializeField] private Button incrementButton, decrementButton;

    [SerializeField] private string[] titles;

    [Serializable]
    public struct ToggleButton
    {
        public Sprite onSprite;
        public Sprite offSprite;

        public Button button;
    }

    private void Start()
    {
        playPauseButton.button.onClick.AddListener(() =>
        {
            //TODO Sounds Calling
            AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

            ToggleRadio();
        });

        incrementButton.onClick.AddListener(() =>
        {
            //TODO Sounds Calling
            AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

            AkaitoAi.SoundManager.Instance?.PlayNextBGSong();

            UpdateRadioTitle();

            //AdsWrapper.GetInstance()?.ShowRewardedAdsWithLoadingPanel(() =>
            //{
            //    SoundManager.Instance?.PlayNextBGSong();
                
            //    UpdateRadioTitle();
            //});
        });

        decrementButton.onClick.AddListener(() =>
        {
            //TODO Sounds Calling
            AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

            AkaitoAi.SoundManager.Instance?.PlayPreviousBGSong();

            UpdateRadioTitle();

            //AdsWrapper.GetInstance()?.ShowRewardedAdsWithLoadingPanel(() =>
            //{
            //    SoundManager.Instance?.PlayPreviousBGSong();

            //    UpdateRadioTitle();
            //});
        });

        ToggleRadio();

        UpdateRadioTitle();
    }

    private void UpdateRadioTitle()
    {
        if (titleText == null || titles == null || titles.Length <= 0) return;

        titleText.text = titles[AkaitoAi.SoundManager.Instance.loopIndex];
    }

    public void ToggleRadio()
    {
        AkaitoAi.SoundManager.Instance?.PlayToggleBGSong(() =>
        {
            playPauseButton.button.image.sprite = playPauseButton.onSprite;
        }, () =>
        {
            playPauseButton.button.image.sprite = playPauseButton.offSprite;
        });
    }

    private void OnDestroy()
    {
        //ToggleRadio();

        AkaitoAi.SoundManager.Instance?.PlayToggleBGSong(); 
    }
}
