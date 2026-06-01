using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InhouseAdsManager : MonoBehaviour
{
    [Header("Inhouse Ads Controller")]
    public Button[] buttons;
    public string[] urls;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; 
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
            buttons[i].gameObject.SetActive(i == 0); 
        }

    }
    void OnButtonClicked(int index)
    {
        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();
        
        if (!string.IsNullOrEmpty(urls[index]))
        {
            Application.OpenURL(urls[index]);
        }

        int nextIndex = (index + 1) % buttons.Length;
        StartCoroutine(SwitchButtonsAfterDelay(index, nextIndex, 1f));
    }

    IEnumerator SwitchButtonsAfterDelay(int currentIndex, int nextIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        buttons[currentIndex].gameObject.SetActive(false);
        buttons[nextIndex].gameObject.SetActive(true);
    }

}
