using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AkaitoAi
{
    public class Weather : MonoBehaviour
    {
        [SerializeField] private LightSettingSO lightSetting;
        [SerializeField] private Transform[] switchObjects;
        [SerializeField] private Button[] switchButtons;
        internal int switchIndex = 0;

        [SerializeField] private string prefsName = "CurrentWeather";
        internal int index;

        public static event Action<int> OnWeatherChangedAction;
        public UnityEvent OnStartEvent;
        public UnityEvent OnWeatherChangedEvent;

        private void Start()
        {
            for (int i = 0; i < switchButtons.Length; i++)
            {
                int buttonIndex = i;
                switchButtons[i].onClick.AddListener(() => OnWeatherSwitch(buttonIndex));
            }

            OnStartEvent?.Invoke();
        }

        public void OnWeatherSwitch(int index)
        {
            foreach (Transform switchObject in switchObjects)
                switchObject.gameObject.SetActive(false);

            foreach (Button btn in switchButtons)
                btn.interactable = true;

            switchObjects[index].gameObject.SetActive(true);
            switchButtons[index].interactable = false;
            lightSetting.SceneLightSetup(lightSetting.lightSettings[index]);

            PlayerPrefs.SetInt(prefsName, index);

            OnWeatherChangedAction?.Invoke(index);
            OnWeatherChangedEvent?.Invoke();
        }

        public void SequenceWeatherSwitch()
        {
            switchIndex = Mathf.Clamp(PlayerPrefs.GetInt(prefsName, switchIndex), 0, switchObjects.Length - 1);

            for (index = 0; index < switchObjects.Length; index++)
            {
                if (index == switchIndex)
                {
                    switchObjects[index].gameObject.SetActive(true);
                    lightSetting.SceneLightSetup(lightSetting.lightSettings[index]);
                }
                else switchObjects[index].gameObject.SetActive(false);
            }

            if (switchIndex < switchObjects.Length - 1) switchIndex++;
            else switchIndex = 0;

            PlayerPrefs.SetInt(prefsName, switchIndex);

            OnWeatherChangedAction?.Invoke(switchIndex);
            OnWeatherChangedEvent?.Invoke();
        }

        public void RandomWeatherSwitch()
        {
            int randomIndex = UnityEngine.Random.Range(0, switchObjects.Length);

            foreach (Transform switchObject in switchObjects)
                switchObject.gameObject.SetActive(false);

            switchObjects[randomIndex].gameObject.SetActive(true);

            PlayerPrefs.SetInt(prefsName, randomIndex);

            OnWeatherChangedAction?.Invoke(randomIndex);
            OnWeatherChangedEvent?.Invoke();
        }
    }
}
