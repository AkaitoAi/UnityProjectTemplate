using UnityEngine;

[CreateAssetMenu(fileName = "Setup", menuName = "ScriptableObjects/Setup", order = 1)]
public class SetupSO : ScriptableObject
{
    [Header ("Runtime Data")]
    public int selectIndex;
    public int modeIndex;
    public int levelIndex;
    public int[] scenesToLoad;

    [Space(10)]
    [Header("Sound Preferences")]
    public string masterVolumePref;
    public string sFXVolumePref;
    public string bGVolumePref;
    public string bGGPVolumePref;
    public string muteAudioPref;
    public string musicMutePref;
    public string sFXMutePref;

    [Space(10)]
    [Header("Control Preferences")]
    public string controlPref;

    [Space(10)]
    public string levelCoinsPref;
    public string totalCoinsPref;
    
    [Space(10)]
    [Header("Selection Shop Preferences")]
    public string selectBoughtPref;
    public string selectTryOncePref;
    public string selectedPref;

    [Space(10)]
    [Header("Settings Preferences")]
    public string qualitySettingPref;
    public string shadowSettingPref;
    public string cameraFarPref;

    [Space(10)]
    public string mapPref;
    public string speedMeterPref;
    public string vibrationPref;
    public string steeringSensitivityPref;
    public string trafficDensityPref;
    public string pedestriansDensityPref;

    [Space(10)]
    public string setupPref;
}
