using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AkaitoAi.Advertisement
{
    public class RateUsHandler : MonoBehaviour
    {
        [SerializeField] private string link = "www.Google.com";

        [SerializeField] private Button[] buttons;
        [SerializeField] private GameObject rateusThanksObj;
        [SerializeField] private AudioSource thanksAudio;
        [SerializeField] private ButtonMagicNumbers values;

        private GameObject[] rateusStarGlows;
        private bool hasOpened = false;

        [System.Serializable]
        public class ButtonMagicNumbers
        {
            public int glowChildIndex = 0;
            public float thanksDialogeDisableDelay = 5f;
            public Vector2 atStarToShowThanksDialogueAndLink = new Vector2(2, 3);
            public float afterFocusDelay = 1f;
        }

        private void Start()
        {
            if (buttons == null || buttons.Length <= 0) return;

            rateusStarGlows = new GameObject[buttons.Length];

            int totalBtns = buttons.Length;
            for (int i = 0; i < totalBtns; i++)
            {
                int buttonIndex = i;
                buttons[i].onClick.AddListener(() => RateUsStarButton(buttonIndex));

                if (buttons[i].transform.childCount <= values.glowChildIndex) continue;

                rateusStarGlows[i] = buttons[i].transform.GetChild(values.glowChildIndex).gameObject;
            }
        }

        public void RateUsStarButton(int index)
        {
            // TODO Button press sound

            if (rateusStarGlows == null) return;

            foreach (GameObject rusg in rateusStarGlows)
                rusg.SetActive(false);

            for (int i = index; i >= 0; i--)
                rateusStarGlows[i].SetActive(true);

            if (index < values.atStarToShowThanksDialogueAndLink.x) return;

            DisableGameObectStateAfterDelay(rateusThanksObj, values.thanksDialogeDisableDelay);

            if (index < values.atStarToShowThanksDialogueAndLink.y) return;

            hasOpened = true;
            
            OpenURL();
        }

        public void OpenURL() => Application.OpenURL(link);

        private void DisableGameObectStateAfterDelay(GameObject go, float delay)
        {
            if (go == null || go.activeSelf) return;

            if (thanksAudio == null) return;

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
                //TODO lower the volume of main volume
                float originalVolume = AudioListener.volume;
                AudioListener.volume *= 0.5f; // Reduce volume by 50%

                thanksAudio.Play();

                while (thanksAudio.isPlaying)
                    yield return null;

                //TODO reset main volume to original value
                AudioListener.volume = originalVolume; // Restore volume
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!hasOpened) return;

            if (!focus)
            {
                rateusThanksObj.SetActive(false);

                thanksAudio.Stop();

                //TODO reset main volume to original value

                return;
            }

            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return new WaitForSeconds(values.afterFocusDelay);

                DisableGameObectStateAfterDelay(rateusThanksObj, values.thanksDialogeDisableDelay);
            }

            hasOpened = false;
        }

        //TODO Slider Working

        //[SerializeField] private Slider rateusSlider;
        //private bool hasRated = false;

        //public void RateUsBtn()
        //{
        //    //ClickBtn();

        //    if (rateusSlider.value > .6)
        //    {
        //        rateusSlider.value = 1f;

        //        if (hasRated) return; hasRated = true;
        //        OpenURL();
        //    }
        //}

        //[SerializeField] private Slider rateusSlider;
        //private bool hasRated = false;

        //public void RateUsSlider()
        //{
        //    if (rateusSlider == null || hasRated) return;

        //    if (rateusSlider.value > 0.6f)
        //    {
        //        rateusSlider.value = 1f;
        //        hasRated = true;

        //        OpenURL();
        //        ToggleThanksDialog(true); // Optional: Show dialog
        //    }
        //}
    }
}
