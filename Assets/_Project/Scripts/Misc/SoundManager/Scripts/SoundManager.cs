using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AkaitoAi
{
    public class SoundManager : MonoBehaviour
    {
        #region Singleton
        private static SoundManager instance;
        public static SoundManager Instance { get { if (instance == null) instance = GameObject.FindObjectOfType<SoundManager>(); return instance; } }
        #endregion

        [Header("Audio Sources")]
        public AudioSource menuBGAudioSource;
        public AudioSource gameplayBGAudioSource;
        public AudioSource sfxAudioSource;

        [Header("Audio Clips")]
        [HideInInspector] public bool inMenu; // true in menu and false in gameplay
        [SerializeField] private AudioClip[] bgAudioClips;
        [SerializeField] private AudioClip[] hornAudioClips;
        [SerializeField] private AudioClip[] winAudioClips;
        [SerializeField] private AudioClip[] failedAudioClips;
        [SerializeField] private AudioClip[] wooshAudioClips;
        [SerializeField] private AudioClip[] radioTuningAudioClips;

        [SerializeField] private AudioClip menuAudioClip;
        [SerializeField] private AudioClip gamePlayAudioClip;
        [SerializeField] private AudioClip onButtonAudioClip;
        [SerializeField] private AudioClip onTapAudioClip;
        [SerializeField] private AudioClip timerAudioClip;
        [SerializeField] private AudioClip parkingAudioClip;
        [SerializeField] private AudioClip levelWinAudioClip;
        [SerializeField] private AudioClip coinPickupAudioClip;
        [SerializeField] private AudioClip scrollRectAudioClip;
        [SerializeField] private AudioClip purchaseAudioClip;
        [SerializeField] private AudioClip purchaseFaliedClip;
        [SerializeField] private AudioClip engineStartClip;
        [SerializeField] private AudioClip thunderClapAudioClip;
        [SerializeField] private AudioClip checkpointAudioClip;
        [SerializeField] private AudioClip starSoundAudioClip;
        [SerializeField] private AudioClip engineAudioClip;
        [SerializeField] private AudioClip flashAudioClip;
        [SerializeField] private AudioClip gearSwitchAudioClip;
        [SerializeField] private AudioClip typingAudioClip;
        [SerializeField] private AudioClip winAudioClip;
        [SerializeField] private AudioClip blastAudioClip;

        public UnityEvent OnStartEvent;


        private float masterVolumePref;
        private float bgVolume;
        private float gpBGVolume;
        private float bgVolumePref;
        private float bgGPVolumePref;
        private float sfxVolume;
        private float sfxVolumePref;
        private int musicMute;
        private int musicMutePref;
        private int sfxMute;
        private int sfxMutePref;
        private int muteAudioPref;
        private int lastBGAudio = -1;
        private int lastSFXAudio = -1;

        internal SetupSO setupScriptable;

        internal int loopIndex = 0;
        private int currentPlayingIndex = -1;
        private bool isRadioPlaying = false;
        private Coroutine currentSwitchCoroutine;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }
            DontDestroyOnLoad(this.gameObject);

            setupScriptable = RuntimeSetup.GetInstance().Setup;

            masterVolumePref = PlayerPrefs.GetFloat(setupScriptable.masterVolumePref);
            bgVolumePref = PlayerPrefs.GetFloat(setupScriptable.bGVolumePref);
            bgGPVolumePref = PlayerPrefs.GetFloat(setupScriptable.bGGPVolumePref);
            sfxVolumePref = PlayerPrefs.GetFloat(setupScriptable.sFXVolumePref);
            musicMutePref = PlayerPrefs.GetInt(setupScriptable.musicMutePref);
            sfxMutePref = PlayerPrefs.GetInt(setupScriptable.sFXMutePref);
            muteAudioPref = PlayerPrefs.GetInt(setupScriptable.muteAudioPref);

            if (PlayerPrefs.GetInt("SoundManagerInit") == 0)
            {
                menuBGAudioSource.volume = 1;
                sfxAudioSource.volume = 1;

                PlayerPrefs.SetFloat(setupScriptable.bGVolumePref, menuBGAudioSource.volume);
                PlayerPrefs.SetFloat(setupScriptable.bGGPVolumePref, gameplayBGAudioSource.volume);
                PlayerPrefs.SetFloat(setupScriptable.sFXVolumePref, sfxAudioSource.volume);

                PlayerPrefs.SetInt("SoundManagerInit", 1);
            }
            else
            {
                menuBGAudioSource.volume = PlayerPrefs.GetFloat(setupScriptable.bGVolumePref);
                gameplayBGAudioSource.volume = PlayerPrefs.GetFloat(setupScriptable.bGGPVolumePref);
                sfxAudioSource.volume = PlayerPrefs.GetFloat(setupScriptable.sFXVolumePref);
            }

            bgVolume = menuBGAudioSource.volume;
            gpBGVolume = gameplayBGAudioSource.volume;
            sfxVolume = sfxAudioSource.volume;

            if (menuBGAudioSource)
                bgVolume = bgVolumePref;
            if (gameplayBGAudioSource)
                gpBGVolume = bgGPVolumePref;
            if (sfxAudioSource)
                sfxVolume = sfxVolumePref;
        }

        private void Start()
        {
            OnStartEvent?.Invoke();
        }

        public void PlayBGSound() => PlayRandomClip(bgAudioClips);
        public void PlayBGSoundFromArray(int _clipIndex) => PlayClipFromArray(bgAudioClips, _clipIndex);

        public void PlayMenuBG() => PlayBGClip(menuAudioClip);
        public void PlayGameplayBG() => PlayBGClip(gamePlayAudioClip);
        public void PlayOnButtonSound() => PlayOneShotClip(onButtonAudioClip);
        public void PlayOnTapSound() => PlayOneShotClip(onTapAudioClip);
        public void PlayLevelWinSound() => PlayNormalClip(levelWinAudioClip);
        public void PlayWinSound() => PlayNormalClip(winAudioClip);
        public void PlayParkingSound() => PlayNormalClip(parkingAudioClip);
        public void PlayCoinPickupSound() => PlayOneShotClip(coinPickupAudioClip);
        public void PlayPurchaseSound() => PlayNormalClip(purchaseAudioClip);
        public void PlayPurchaseFailedSound() => PlayNormalClip(purchaseFaliedClip);
        public void PlayScrollRectSound() => PlayOneShotClip(scrollRectAudioClip);
        public void PlayLevelWinRewardSound() => PlayNormalClip(scrollRectAudioClip);
        public void PlayEngineStartSound() => PlayNormalClip(engineStartClip);
        public void PlayWarningSound() => PlayOneShotClip(purchaseFaliedClip);
        public void PlayHornSound() => PlayNormalRandomClip(hornAudioClips);
        public void PlayThunderClapSound() => PlayNormalClip(thunderClapAudioClip);
        public void PlayCheckpointSound() => PlayOneShotClip(checkpointAudioClip);
        public void PlayStarSound() => PlayOneShotClip(starSoundAudioClip);
        public void PlayEngineSound() => PlayOneShotClip(engineAudioClip);
        public void PlayFlashSound() => PlayNormalClip(flashAudioClip);
        public void PlayBlastSound() => PlayNormalClip(blastAudioClip);
        public void PlayGearSwitchSound() => PlayNormalClip(gearSwitchAudioClip);
        public void PlayNormalRandomWinSound() => PlayNormalRandomClip(winAudioClips);
        public void PlayNormalRandomFailedSound() => PlayNormalRandomClip(failedAudioClips);
        public void PlayNormalRandomWooshSound() => PlayNormalRandomClip(wooshAudioClips);
        public void PlayTypeSound() => PlayOneShotClip(typingAudioClip);
        public void PlayNextBGSong() => RadioSwitch(1, bgAudioClips, radioTuningAudioClips, gameplayBGAudioSource);
        public void PlayPreviousBGSong() => RadioSwitch(-1, bgAudioClips, radioTuningAudioClips, gameplayBGAudioSource);
        public void PlayToggleBGSong(Action onToggle = null, Action offToggle = null) => RadioToggle(gameplayBGAudioSource, bgAudioClips, onToggle, offToggle);
        public void PlayNormalTimerSound() => PlayNormalClip(timerAudioClip);

        public void ChangeVolume(float _musicVolume, float _sFXVolume)
        {
            menuBGAudioSource.volume = _musicVolume;
            sfxAudioSource.volume = _sFXVolume;
        }

        private void PlayRandomClip(AudioClip[] _audioClips)
        {
            if (_audioClips.Length == 0) return;

            int randomClip = UnityEngine.Random.Range(0, _audioClips.Length);

            while (randomClip == lastBGAudio)
                randomClip = UnityEngine.Random.Range(0, _audioClips.Length);

            lastBGAudio = randomClip;

            menuBGAudioSource.clip = _audioClips[randomClip];
            menuBGAudioSource.Play();
        }

        private void PlayBGClip(AudioClip _audioClip)
        {
            if (_audioClip == null) return;

            menuBGAudioSource.clip = _audioClip;
            menuBGAudioSource.Play();
        }

        private void PlayClipFromArray(AudioClip[] _audioClips, int _clipIndex)
        {
            if (_audioClips.Length == 0) return;

            menuBGAudioSource.clip = _audioClips[_clipIndex];
            menuBGAudioSource.Play();
        }

        private void PlayNormalRandomClip(AudioClip[] _audioClips)
        {
            if (_audioClips.Length == 0) return;

            int randomClip = UnityEngine.Random.Range(0, _audioClips.Length);

            while (randomClip == lastSFXAudio)
                randomClip = UnityEngine.Random.Range(0, _audioClips.Length);

            lastSFXAudio = randomClip;

            sfxAudioSource.clip = _audioClips[randomClip];
            sfxAudioSource.Play();
        }

        public void PlayOneShotClip(AudioClip _audioClip)
        {
            if (_audioClip == null) return;

            sfxAudioSource.clip = _audioClip;
            sfxAudioSource.PlayOneShot(_audioClip);
        }

        private void PlayNormalClip(AudioClip _audioClip)
        {
            if (_audioClip == null) return;

            sfxAudioSource.clip = _audioClip;
            sfxAudioSource.Play();
        }

        public void PlayNormalClipFromArray(AudioClip[] _audioClips, int _clipIndex)
        {
            if (_audioClips.Length == 0) return;

            sfxAudioSource.clip = _audioClips[_clipIndex];
            sfxAudioSource.Play();
        }

        public void PlayOneShotClipFromArray(AudioClip[] _audioClips, int _clipIndex)
        {
            if (_audioClips.Length == 0) return;

            sfxAudioSource.clip = _audioClips[_clipIndex];
            sfxAudioSource.PlayOneShot(_audioClips[_clipIndex]);
        }

        public AudioClip[] GetAudioClips(AudioClip[] _audioClips)
        {
            return _audioClips;
        }

        private void RadioSwitch(int direction = 1, AudioClip[] _audioClips = null,
        AudioClip[] _tuningAudioClips = null, AudioSource _audioSource = null)
        {
            if (_audioClips == null || _audioClips.Length == 0) return;
            if (_audioSource == null) return;

            Debug.Log("Music Index Pre: " + loopIndex);

            int newIndex = (loopIndex + direction + _audioClips.Length) % _audioClips.Length;

            Debug.Log("Music Index Post: " + newIndex);

            loopIndex = newIndex;

            if (isRadioPlaying && loopIndex == currentPlayingIndex && _audioSource.isPlaying)
            {
                Debug.Log("Returned to current index, no change needed.");
                return;
            }

            if (currentSwitchCoroutine != null)
            {
                StopCoroutine(currentSwitchCoroutine);
                currentSwitchCoroutine = null;
            }

            if (isRadioPlaying)
            {
                if (_tuningAudioClips == null || _tuningAudioClips.Length == 0)
                {
                    _audioSource.clip = _audioClips[loopIndex];
                    _audioSource.Play();
                    currentPlayingIndex = loopIndex;
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, _tuningAudioClips.Length);
                    currentSwitchCoroutine = StartCoroutine(PlayNextSong(rand, loopIndex, _tuningAudioClips, _audioClips, _audioSource));
                }
            }
            else
            {
                _audioSource.clip = _audioClips[loopIndex];
                _audioSource.Stop();
            }
        }

        private IEnumerator PlayNextSong(int tuningIndex, int clipIndex,
            AudioClip[] _tuningAudioClip, AudioClip[] _audioClips, AudioSource _audioSource)
        {
            AudioClip tuningClip = _tuningAudioClip[tuningIndex];
            _audioSource.clip = tuningClip;
            _audioSource.Play();

            yield return new WaitForSeconds(tuningClip.length);

            _audioSource.clip = _audioClips[clipIndex];
            _audioSource.Play();

            currentPlayingIndex = clipIndex;

            currentSwitchCoroutine = null;
        }

        private void RadioToggle(AudioSource _audioSource, AudioClip[] _audioClips,
            Action onToggle = null, Action offToggle = null)
        {
            if (_audioClips == null || _audioClips.Length == 0) return;
            if (_audioSource == null) return;

            isRadioPlaying = !isRadioPlaying;

            if (isRadioPlaying)
            {
                if (_audioSource.clip == null)
                {
                    _audioSource.clip = _audioClips[loopIndex];
                    currentPlayingIndex = loopIndex;
                }

                if (onToggle != null) onToggle?.Invoke();
                _audioSource.Play();
            }
            else
            {
                if (onToggle != null) offToggle?.Invoke();
                _audioSource.Pause();
                currentPlayingIndex = -1;
            }
        }


        //private void RadioIncrement(AudioClip[] _audioClips, AudioClip[] _tuningAudioClip, AudioSource _audioSource)
        //{
        //    if (_audioClips.Length == 0) return;
        //    if (_tuningAudioClip.Length == 0) return;

        //    Debug.Log("Music Index Pre: " + loopIndex);

        //    loopIndex++;
        //    if (loopIndex >= _audioClips.Length) loopIndex = 0;
        //    Debug.Log("Music Index Post: " + loopIndex);

        //    int rand = UnityEngine.Random.Range(0, _tuningAudioClip.Length);

        //    StartCoroutine(PlayNextSong());

        //    IEnumerator PlayNextSong()
        //    {
        //        _audioSource.clip = _tuningAudioClip[rand];
        //        _audioSource.Play();

        //        //! Wait for AudioClip Length
        //        //yield return new WaitForSeconds(_tuningAudioClip[rand].length);
        //        yield return new WaitForSeconds(2.5f);

        //        Debug.Log("Music Index Selected: " + loopIndex);

        //        _audioSource.clip = _audioClips[loopIndex];

        //        _audioSource.Play();
        //    }
        //}

        //private void RadioDecrement(AudioClip[] _audioClips, AudioClip[] _tuningAudioClip, AudioSource _audioSource)
        //{
        //    if (_audioClips.Length == 0) return;
        //    if (_tuningAudioClip.Length == 0) return;

        //    Debug.Log("Music Index Pre: " + loopIndex);

        //    loopIndex--;
        //    if (loopIndex < 0) loopIndex = _audioClips.Length - 1;
        //    Debug.Log("Music Index Post: " + loopIndex);

        //    int rand = UnityEngine.Random.Range(0, _tuningAudioClip.Length);

        //    StartCoroutine(PlayNextSong());

        //    IEnumerator PlayNextSong()
        //    {
        //        _audioSource.clip = _tuningAudioClip[rand];
        //        _audioSource.Play();

        //        //! Wait for AudioClip Length
        //        //yield return new WaitForSeconds(_tuningAudioClip[rand].length);
        //        yield return new WaitForSeconds(2.5f);

        //        Debug.Log("Music Index Selected: " + loopIndex);

        //        _audioSource.clip = _audioClips[loopIndex];

        //        _audioSource.Play();
        //    }
        //}

        //private void PlayScrollRectSound(ScrollRect _scroll, float _scrollSoundDely, float _oldPos)
        //{
        //    if (sfxVolumePref > 0.0f)
        //    {
        //        if (_scroll.horizontalNormalizedPosition > (_oldPos + _scrollSoundDely))
        //        {
        //            _oldPos = _scroll.horizontalNormalizedPosition;
        //            PlayNormalClip(scrollRectAudioClip);
        //        }
        //        else if (_scroll.horizontalNormalizedPosition < (_oldPos - _scrollSoundDely))
        //        {
        //            _oldPos = _scroll.horizontalNormalizedPosition;
        //            PlayNormalClip(scrollRectAudioClip);
        //        }
        //    }
        //}
    }
}