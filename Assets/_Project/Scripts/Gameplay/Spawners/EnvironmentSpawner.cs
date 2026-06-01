using UnityEngine;
using AkaitoAi.GameBase;

public class EnvironmentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject selected; 
    [SerializeField] private int addressableIndex;
    [SerializeField] private LightingSnapshot lightingSnapshot;
    
    private SetupSO setupScriptable;

    private void Awake()
    {
        setupScriptable = RuntimeSetup.GetInstance().Setup;

        Spawn();
    }

    private void Spawn()
    {
        selected = selected != null ? selected : GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].environmentObj != null ?
               Instantiate(GameManager.GetInstance().modeLevelSetup[setupScriptable.modeIndex].environmentObj) :
                AssetLoader_Addressable.instance?.InstantiateAsset_Index_Return(addressableIndex); selected.SetActive(true);

        //EventBus<OnEnvironmentSpawned>.Raise(new OnEnvironmentSpawned { currentEnvironment = selected });

        CachedEvent<OnEnvironmentSpawned>.Raise(new OnEnvironmentSpawned { currentEnvironment = selected });
        CachedEvent<OnLightingChanged>.Raise(new OnLightingChanged { lightingSnapshot = lightingSnapshot });
    }
}
