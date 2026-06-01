using UnityEngine;
using UnityEngine.Audio;

namespace AkaitoAi.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "Audio System/AudioData", order = 1)]
    public class AudioData : ScriptableObject
    {
        public GameObject audioClip;
        public AudioMixerGroup audioMixerGroup;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public Vector2 minMaxPitch = new Vector2(-.5f, .5f);
        public bool playOnAwak = true;
        public bool loop = false;
        public bool mute = false;
        public bool frequentAudio;

    }
}
