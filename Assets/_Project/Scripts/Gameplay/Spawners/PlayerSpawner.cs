using UnityEngine;
using AkaitoAi.GameBase;

public class PlayerSpawner : MonoBehaviour
{
    private GameObject parentObj;
    private GameObject selected;
    [SerializeField] private int addressableParentIndex;
    private SetupSO setupScriptable;

    EventBinding<OnLevelSpawned> spawnedLevelEventBinding;

    private void Awake()
    {
        setupScriptable = RuntimeSetup.GetInstance().Setup;
    }

    private void OnEnable()
    {
        spawnedLevelEventBinding = new EventBinding<OnLevelSpawned>(SpawnPlayer);
        EventBus<OnLevelSpawned>.Register(spawnedLevelEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnLevelSpawned>.Deregister(spawnedLevelEventBinding);
    }

    private void SpawnPlayer(OnLevelSpawned level)
    {
        SpawnParentObj();

        selected = parentObj.transform.GetChild(setupScriptable.selectIndex).gameObject;

        if (level.currentLevel.TryGetComponent(out LevelInfo levelInfo) &&
            levelInfo.spawn != null)
        {
            selected.transform.position = levelInfo.spawn.position;
            selected.transform.rotation = levelInfo.spawn.rotation;
        }

        selected.SetActive(true);

        EventBus<OnPlayerSpawned>.Raise(new OnPlayerSpawned { currentPlayer = selected });
    }

    private void SpawnParentObj()
    {
        parentObj = parentObj != null ? parentObj : GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].playersParentObj != null ?
                            GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].playersParentObj :
                            AssetLoader_Addressable.instance?.InstantiateAsset_Index_Return(addressableParentIndex);

        parentObj.SetActive(true);
    }
}
