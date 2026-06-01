using System;
using UnityEngine;
using AkaitoAi.GameBase;

[CreateAssetMenu(fileName = "LevelSelect", menuName = "ScriptableObjects/LevelSelect_", order = 1)]
public class LevelSelectSO : ScriptableObject
{
    public int totalLevels;
    public bool useLevelStatus;
    public bool useMoreThan2Scenes = false;
    
    [Space(10)]

    public BoolIndex separateLevelNumber;

    public BoolIndex levelLock;

    public BoolIndex levelToPlay;

    public BoolIndex levelComplete;

    [Space(10)]
    [Header("ScrollRect Settings")]
    public float scrollSoundDelay;
    public float ContentLeft;
    public float ContentRight;

    [Header("Level Preferences")]
    public string namePref;
    public string isPlayedPref;
    public string isUnlockedPref;
    public int unlockedLevels;

    public void UnlockLevel(int currentLevel)
    {
        int sequentialUnlocked = PlayerPrefs.GetInt(namePref, 0);

        if (currentLevel >= sequentialUnlocked)
        {
            PlayerPrefs.SetInt(isPlayedPref + currentLevel, 1);

            int nextLevel = currentLevel + 1;

            // Always update prefName to mark the highest unlocked level
            PlayerPrefs.SetInt(namePref, nextLevel);

            if (nextLevel < totalLevels)
            {
                PlayerPrefs.SetInt(isUnlockedPref + nextLevel, 1);
            }
        }

        PlayerPrefs.Save();

        //int unlockedLevels = PlayerPrefs.GetInt(prefName, 0);
        //int unlockedIndex = Mathf.Max(unlockedLevels, currentLevel + 1);

        //if (unlockedIndex < totalLevels)
        //{
        //    PlayerPrefs.SetInt(prefName, unlockedIndex);
        //    PlayerPrefs.SetInt(isUnlockedPref + unlockedIndex, 1);
        //}

        //if (unlockedLevels <= currentLevel)
        //{
        //    int nextLevel = unlockedLevels + 1;

        //    if (nextLevel >= totalLevels)
        //    {
        //        nextLevel = 0;
        //    }

        //    PlayerPrefs.SetInt(prefName, nextLevel);

        //    Debug.Log("Total Levels " + totalLevels);
        //}
    }
    public void UnlockLevel()
    {
        int currentLevel = GetCurrentLevel(); // Replacing parameter with method call
        int unlockedLevels = PlayerPrefs.GetInt(namePref, 0);
        int unlockedIndex = Mathf.Max(unlockedLevels, currentLevel + 1);

        if (unlockedIndex < totalLevels)
        {
            PlayerPrefs.SetInt(namePref, unlockedIndex);
            PlayerPrefs.SetInt(isUnlockedPref + unlockedIndex, 1);
        }
    }

    public int GetCurrentLevel()
    {
        int savedLevel = PlayerPrefs.GetInt(namePref, 0);

        if (savedLevel >= totalLevels)
        {
            savedLevel = 0;
            PlayerPrefs.SetInt(namePref, 0);
        }

        return savedLevel;
    }

    public bool IsLevelUnlocked(int level)
    {
        return PlayerPrefs.GetInt(isUnlockedPref + level, 0) == 1;
    }

    public void LevelUnlockReward(int levelIndex, Action behaviour = null)
    {
        PlayerPrefs.SetInt(isUnlockedPref + levelIndex, 1);

        behaviour?.Invoke();

        PlayerPrefs.Save();
    }

    //int _level = levelNumber;

    //UnlockLevel(_level, currentLevelSelectSO.levelForm, currentLevelSelectSO.totalLevels, currentLevelSelectSO.prefName);

    //void UnlockLevel(int level, int currentUnlockedLevel, int totalLevels, string levelPrefKey)
    //{
    //    currentUnlockedLevel = PlayerPrefs.GetInt(levelPrefKey, 0);

    //    if (currentUnlockedLevel <= level)
    //    {
    //        int nextLevel = currentUnlockedLevel + 1;

    //        if (nextLevel >= totalLevels)
    //        {
    //            nextLevel = 0;
    //        }

    //        PlayerPrefs.SetInt(levelPrefKey, nextLevel);
    //        levelNumber = nextLevel;
    //    }
    //}

    //void UnlockLevel(int _level, int _levelForm, int _levelsLength, string _levelFormPref) //! Unlock's next level
    //{
    //    _levelForm = PlayerPrefs.GetInt(_levelFormPref);

    //    if (_levelForm <= _level)
    //    {
    //        if (_level < _levelsLength)
    //        {
    //            _levelForm++;
    //            PlayerPrefs.SetInt(_levelFormPref, _levelForm);

    //            //if(selectedMode == 0) levelNumber = PlayerPrefs.GetInt(currentLevelSelectSO.prefName) < currentLevelSelectSO.totalLevels ? PlayerPrefs.GetInt(currentLevelSelectSO.prefName) : 0;

    //            Debug.Log("Unlock Level: " + levelNumber);

    //            // Rateus during after Level Win
    //            //if (levelNumber % 2 == 0)
    //            //{
    //            //    if (PlayerPrefs.GetInt("RateUs") == 0)
    //            //        RateUs();
    //            //}
    //        }
    //    }
    //}
    //private void UnlockLevel()
    //{
    //    int _level = levelNumber;

    //    UnlockLevel(_level, currentLevelSelectSO.unlockedLevels, currentLevelSelectSO.totalLevels, currentLevelSelectSO.prefName);

    //    void UnlockLevel(int level, int currentUnlockedLevel, int totalLevels, string levelPrefKey)
    //    {
    //        currentUnlockedLevel = PlayerPrefs.GetInt(levelPrefKey, 0);

    //        if (currentUnlockedLevel <= level)
    //        {
    //            int nextLevel = currentUnlockedLevel + 1;

    //            if (nextLevel >= totalLevels)
    //            {
    //                nextLevel = 0;
    //            }

    //            PlayerPrefs.SetInt(levelPrefKey, nextLevel);
    //            levelNumber = nextLevel;

    //            Debug.Log("Level " + levelNumber + " Unlocked");
    //            Debug.Log("Total Levels " + totalLevels);
    //        }
    //    }

    //}
}
