using UnityEngine;

namespace AkaitoAi.AudioSystem
{
    public class SingleAudioClip : MonoBehaviour, IAudioClip
    {
        [SerializeField] private AudioClip audioClip;

        public AudioClip GetAudioClip() => audioClip != null ? audioClip : null;
    }
}
