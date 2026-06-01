using UnityEngine;
using AkaitoAi.AudioSystem;

public class PlayAudio : MonoBehaviour
{
    [SerializeField] private AudioData singleAudioData;
    [SerializeField] private AudioData randomAudioData;
    [SerializeField] private AudioData loopAudioData;
    public void OnPlaySingleAudioButton()
    {
        AudioManager.GetInstance().CreateAudio()
            .WithAudioData(singleAudioData)
            .WithRandomPitch()
            .WithPosition(transform.position)
            .Play();

    }
    public void OnPlayRandomAudioButton()
    {
        AudioManager.GetInstance().CreateAudio()
            .WithAudioData(randomAudioData)
            .WithRandomPitch()
            .WithPosition(transform.position)
            .Play();

    }
    public void OnPlayLoopAudioButton()
    {
        AudioManager.GetInstance().CreateAudio()
            .WithAudioData(loopAudioData)
            .WithRandomPitch()
            .WithPosition(transform.position)
            .Play();

    }
}
