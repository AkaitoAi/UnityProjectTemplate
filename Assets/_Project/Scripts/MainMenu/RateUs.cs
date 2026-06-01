using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : MonoBehaviour
{
    [SerializeField] private Button[] rateUsButtons;
    [SerializeField] private Sprite simpleStar, glowStar;
    [SerializeField] private GameObject rateusThanksObj;
    [SerializeField] private AudioSource thanksAudio;
    [SerializeField] private string url = "www.google.com";
    private bool hasOpened = false;

    private void Start()
    {
        if (rateUsButtons == null || rateUsButtons.Length <= 0) return;

        for (int i = 0; i < rateUsButtons.Length; i++)
        {
            // Create local copy of index for closure
            int index = i;
            rateUsButtons[i].onClick.AddListener(() => RateUsStarButton(index));
        }
    }

    public void RateUsStarButton(int index)
    {
        //TODO Sounds calling

        if (simpleStar == null || glowStar == null) return;

        foreach (Button btn in rateUsButtons)
            btn.image.sprite = simpleStar;

        for (int i = index; i >= 0; i--)
            rateUsButtons[i].image.sprite = glowStar;

        if (index < 2) return;

        //DisableGameObectStateAfterDelay(rateusThanksObj, 5f);

        if (index < 3) return;

        hasOpened = true;

        Application.OpenURL(url);
    }

    private void DisableGameObectStateAfterDelay(GameObject go, float delay)
    {
        if (go.activeInHierarchy) return;

        StartCoroutine(PlayAudio());

        go.SetActive(true);

        if (!go.activeInHierarchy) return;

        StartCoroutine(DisableAfterDelay(go, delay));

        IEnumerator DisableAfterDelay(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);

            go.SetActive(false);
        }

        IEnumerator PlayAudio()
        {
            //RacerSettings.instance.MusicSlider.value = .125f;
            thanksAudio.Play();

            while (thanksAudio.isPlaying)
                yield return null;

            //RacerSettings.instance.MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!hasOpened) return;

        if (!focus)
        {
            //rateusThanksObj.SetActive(false);

            //thanksAudio.Stop();

            //RacerSettings.instance.MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");

            return;
        }

        if (focus)
        {
            //StartCoroutine(Delay());

            //IEnumerator Delay()
            //{
            //    yield return new WaitForSeconds(1f);

            //    //DisableGameObectStateAfterDelay(rateusThanksObj, 5f);
            //}


            hasOpened = false;

            return;
        }
    }
}
