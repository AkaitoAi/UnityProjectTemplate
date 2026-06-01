using UnityEngine;

namespace AkaitoAi.AudioSystem
{
    public class RandomAudioClip : MonoBehaviour, IAudioClip
    {
        [SerializeField] private AudioClip[] audioClips;
        public AudioClip GetAudioClip() =>
            audioClips != null ? audioClips[Random.Range(0, audioClips.Length)] : null;
    }
}
