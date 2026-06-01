using AkaitoAi.Singleton;
using UnityEngine;

public class RuntimeSetup : SingletonPresistent<RuntimeSetup>
{
    [SerializeField] private SetupSO setup;
    [SerializeField] private SelectSpecsSO[] specs;
    [SerializeField] private LevelSelectSO[] levels;

    public SetupSO Setup
    {
        get
        {
            if (setup == null)
            {
                setup = Resources.Load<SetupSO>("ScriptableObjects/Setup/Setup");

                if (setup == null)
                {
                    Debug.LogError(
                        "RuntimeSetup: SetupSO not found at Resources/ScriptableObjects/Setup/Setup"
                    );
                }
            }

            return setup;
        }
    }
    public SelectSpecsSO[] Specs
    {
        get
        {
            if (specs == null || specs.Length == 0)
            {
                specs = Resources.LoadAll<SelectSpecsSO>("ScriptableObjects/SelectSpecs");

                if (specs == null || specs.Length == 0)
                {
                    Debug.LogError(
                        "RuntimeSetup: No SelectSpecsSO found at Resources/ScriptableObjects/SelectSpecs"
                    );
                }
            }

            return specs;
        }
    }
    public LevelSelectSO[] Levels
    {
        get
        {
            if (levels == null || levels.Length == 0)
            {
                levels = Resources.LoadAll<LevelSelectSO>("ScriptableObjects/LevelSelect");

                if (levels == null || levels.Length == 0)
                {
                    Debug.LogError(
                        "RuntimeSetup: No LevelSelectSO found at Resources/ScriptableObjects/LevelSelect"
                    );
                }
            }

            return levels;
        }
    }
}
