using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using AkaitoAi.GameBase;

#if DOTWEEN
using DG.Tweening;
#endif
public class TimerController : MonoBehaviour
{
    public GameObject clockObj;
    public Text timerText;
    [Tooltip("If yes, Input time in seconds")]
    [SerializeField] private bool useSeconds = true;
    public float time;

    private float minutes, seconds;
    private GameManager gameManager;
    private bool aboutToEnd = false;
#if DOTWEEN
    private Tweener timerTween;
#endif

    public UnityEvent OnTimerEndEvent;

    private void Start()
    {
        if (!useSeconds) time *= 60f;

        Reset();

        gameManager = GameManager.GetInstance();
    }

    private void OnEnable()
    {
        Reset();
    }

    private void Update()
    {
        if (gameManager.levelTimer.timeRunning) Timer();
    }

    private void Timer()
    {

        if (time > 0f)
        {
            time -= Time.deltaTime;
            DisplayTime(time);
        }
        else
        {
            time = 0f;

            Reset();

            OnTimerEndEvent?.Invoke();
            gameManager.levelTimer.timeRunning = false;

            EventBus<OnLevelFailed>.Raise(new OnLevelFailed { reason = "TimeOut" });
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        minutes = Mathf.FloorToInt(timeToDisplay / 60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);


        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (time <= 15f)
        {
            //timerText.color = Color.red;
            //timerText.text = seconds.ToString();
            //GameManager.Instance.hurryUpObj.SetActive(true);

            if (aboutToEnd) return;

            aboutToEnd = true;

            //TODO Sounds Calling
            if (PlayerPrefs.GetFloat(AkaitoAi.SoundManager.Instance.setupScriptable.bGGPVolumePref) > .25f)
                AkaitoAi.SoundManager.Instance.gameplayBGAudioSource.volume = .25f;

#if DOTWEEN
            if (timerTween != null) timerTween.Kill();
            timerTween = timerText.transform.DOScale(.95f, .5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            timerTween.Play();
#endif
        }
    }

    private void OnDisable()
    {
        Reset();
    }

    private void Reset()
    {
#if DOTWEEN
        if (timerTween != null)
        {
            timerTween.Kill();

            timerTween = null;
        }
#endif
        aboutToEnd = false;

        //TODO Sounds Calling
        AkaitoAi.SoundManager.Instance.gameplayBGAudioSource.volume =
            PlayerPrefs.GetFloat(AkaitoAi.SoundManager.Instance.setupScriptable.bGGPVolumePref);
    }

    public void TimeRunningState(bool _state)
    {
        gameManager.levelTimer.timeRunning = _state;
        clockObj.SetActive(_state);
        timerText.enabled = _state;
    }
}
