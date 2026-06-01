using System.Collections;
using UnityEngine;
using AkaitoAi.Extensions;

namespace AkaitoAi.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour
    {
        public AudioData Data { get; private set; }
        [SerializeField] private AudioSource audioSource;
        [SerializeField] Coroutine playingCoroutine;

        private void Awake()
        {
            audioSource = gameObject.GetOrAddComponent<AudioSource>();
        }

        public void Play()
        {
            if (playingCoroutine != null) StopCoroutine(playingCoroutine);

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForAudioToEnd());
        }

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            audioSource.Stop();
            AudioManager.GetInstance().ReturnToPool(this);
        }

        private IEnumerator WaitForAudioToEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            AudioManager.GetInstance().ReturnToPool(this);
        }

        public void Initialize(AudioData _audioData)
        {
            Data = _audioData;
            audioSource.clip = _audioData.audioClip.GetComponent<IAudioClip>().GetAudioClip();
            audioSource.outputAudioMixerGroup = _audioData.audioMixerGroup;
            audioSource.volume = _audioData.volume;
            audioSource.pitch = _audioData.pitch;
            audioSource.playOnAwake = _audioData.playOnAwak;
            audioSource.loop = _audioData.loop;
            audioSource.mute = _audioData.mute;
        }

        //internal void WithRandomPitch(float min = -.05f, float max = .05f) =>
        //    audioSource.pitch += Random.Range(min, max);
        internal void WithRandomPitch(Vector2 minMax) =>
            audioSource.pitch += Random.Range(minMax.x, minMax.y);
    }
}
