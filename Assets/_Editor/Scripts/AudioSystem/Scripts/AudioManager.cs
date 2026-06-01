using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using AkaitoAi.Singleton;

namespace AkaitoAi.AudioSystem
{
    public class AudioManager : SingletonPresistent<AudioManager>
    {
        private IObjectPool<AudioEmitter> audioEmitterPool;
        readonly List<AudioEmitter> activeAudioEmitters = new();
        //public readonly Dictionary<AudioData, int> Counts = new();
        public readonly Queue<AudioEmitter> FrequentAudioEmitters = new();

        [SerializeField] private AudioEmitter audioEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxAudioInstances = 30;

        void Start()
        {
            InitializePool();
        }

        public AudioBuilder CreateAudio() => new AudioBuilder(this);

        public AudioEmitter Get() => audioEmitterPool.Get();
        public void ReturnToPool(AudioEmitter _audioEmitter) => audioEmitterPool.Release(_audioEmitter);
        //public bool CanPlayAudio(AudioData _audioData) => !Counts.TryGetValue(_audioData, out int count)  || count < maxAudioInstances;

        public bool CanPlayAudio(AudioData _audioData)
        {
            if (!_audioData.frequentAudio) return true;

            if (FrequentAudioEmitters.Count >= maxAudioInstances &&
            FrequentAudioEmitters.TryDequeue(out AudioEmitter audioEmitter))
            {
                try
                {
                    audioEmitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("AudioEmitter is already released!");
                }
                return false;
            }
            return true;
        }

        private void InitializePool()
        {
            audioEmitterPool = new ObjectPool<AudioEmitter>(
                CreateAudioEmitter,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }

        private AudioEmitter CreateAudioEmitter()
        {
            AudioEmitter audioEmitter = Instantiate(audioEmitterPrefab);
            audioEmitter.gameObject.SetActive(false);
            return audioEmitter;
        }

        private void OnTakeFromPool(AudioEmitter _audioEmitter)
        {
            _audioEmitter.gameObject.SetActive(true);
            activeAudioEmitters.Add(_audioEmitter);
        }

        private void OnReturnToPool(AudioEmitter _audioEmitter)
        {
            // if(Counts.TryGetValue(_audioEmitter.Data , out int count))
            //     Counts[_audioEmitter.Data] -= count > 0 ? 1 : 0; 


            _audioEmitter.gameObject.SetActive(false);
            activeAudioEmitters.Remove(_audioEmitter);
        }

        private void OnDestroyPoolObject(AudioEmitter _audioEmitter)
        {
            Destroy(_audioEmitter.gameObject);
        }
    }
}
