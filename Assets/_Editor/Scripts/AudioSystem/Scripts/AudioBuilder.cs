using UnityEngine;

namespace AkaitoAi.AudioSystem
{
    public class AudioBuilder : MonoBehaviour
    {
        readonly AudioManager audioManager;
        private AudioData audioData;
        private Vector3 position = Vector3.zero;
        private bool randomPitch;

        public AudioBuilder(AudioManager _audioManager)
        {
            this.audioManager = _audioManager;
        }

        public AudioBuilder WithAudioData(AudioData _audioData)
        {
            this.audioData = _audioData;
            return this;
        }

        public AudioBuilder WithPosition(Vector3 _position)
        {
            this.position = _position;
            return this;
        }

        public AudioBuilder WithRandomPitch()
        {
            this.randomPitch = true;
            return this;
        }

        public void Play()
        {
            if (!audioManager.CanPlayAudio(audioData)) return;

            AudioEmitter audioEmitter = audioManager.Get();
            audioEmitter.Initialize(audioData);
            audioEmitter.transform.position = position;
            audioEmitter.transform.parent = audioManager.transform;

            if (randomPitch)
                audioEmitter.WithRandomPitch(audioData.minMaxPitch);


            // audioManager.Counts[audioData] = 
            //     audioManager.Counts.TryGetValue(audioData, out int count) ? count + 1 : 1;

            if (audioData.frequentAudio)
                audioManager.FrequentAudioEmitters.Enqueue(audioEmitter);

            audioEmitter.Play();
        }
    }
}
