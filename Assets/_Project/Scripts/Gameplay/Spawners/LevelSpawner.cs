using UnityEngine;
using UnityEngine.Events;
using AkaitoAi.GameBase;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private GameObject parentObj;
    [SerializeField] private GameObject selected;
    [SerializeField] private int addressableParentIndex;
    
    private SetupSO setupScriptable;
    private int winReward;
    private int timeReward;

    public UnityEvent OnAwakeEvent;

    EventBinding<OnEnvironmentSpawned> spawnedEnvironmentEventBinding;

    private void Awake()
    {
        setupScriptable = RuntimeSetup.GetInstance().Setup;

        OnAwakeEvent.Invoke();
    }

    private void OnEnable()
    {
        spawnedEnvironmentEventBinding = new EventBinding<OnEnvironmentSpawned>(Spawn);
        EventBus<OnEnvironmentSpawned>.Register(spawnedEnvironmentEventBinding);

        CachedEvent<OnEnvironmentSpawned>.Subscribe((env) => { Spawn(); }, invokeWithLast: true);
    }

    private void OnDisable()
    {
        EventBus<OnEnvironmentSpawned>.Deregister(spawnedEnvironmentEventBinding);
        CachedEvent<OnEnvironmentSpawned>.Unsubscribe((env) => { Spawn(); });
    }

    public void Spawn()
    {
        if (selected == null) SpawnParentObj();

        selected = selected != null ? selected : parentObj.transform.GetChild(setupScriptable.levelIndex).gameObject;
        selected.SetActive(true);

        winReward = GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].levelWinReward;
        timeReward = GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].levelTimeReward;

        EventBus<OnLevelSpawned>.Raise(new OnLevelSpawned { levelsParentGO = parentObj, currentLevel = selected, winReward = winReward, timeReward = timeReward });
    }

    private void SpawnParentObj()
    {
        parentObj = parentObj != null ? parentObj : GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].levelsParentObj != null ?
                    GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].levelsParentObj :
                    AssetLoader_Addressable.instance?.InstantiateAsset_Index_Return(addressableParentIndex);

        parentObj.SetActive(true);
    }
}
