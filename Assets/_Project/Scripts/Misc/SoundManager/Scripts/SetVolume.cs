using AkaitoAi;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SetVolume : MonoBehaviour
{
    public enum VolumeType {Master, Menu, Gameplay, SFX };

    [SerializeField] private VolumeType volumeType = VolumeType.SFX;

    [GetComponent]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SetupSO prefsSO;

    private void OnEnable()
    {
        if(audioSource == null) return;

        audioSource.volume = volumeType switch
        {
            VolumeType.Master => PlayerPrefs.GetFloat(prefsSO.masterVolumePref),
            VolumeType.Menu => PlayerPrefs.GetFloat(prefsSO.bGVolumePref),
            VolumeType.Gameplay => PlayerPrefs.GetFloat(prefsSO.bGGPVolumePref),
            VolumeType.SFX => PlayerPrefs.GetFloat(prefsSO.sFXVolumePref),
            _ => audioSource.volume
        };
    }
}
