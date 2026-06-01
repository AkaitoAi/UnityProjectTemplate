using System.Collections.Generic;
using UnityEngine;

#if USE_ADDRESSABLES_AkaitoAi
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

// Enum for better clarity of asset loading statuses
public enum AssetStatus { NotLoaded, Loading, Loaded, Failed }

public class AssetLoader_Addressable : MonoBehaviour
{
    public static AssetLoader_Addressable instance { get; private set; }

#if USE_ADDRESSABLES_AkaitoAi
    [Header("Addressable")]
    public AssetLabelReference[] AssetReferences;
    public AssetLabelReference[] ScriptableObjectReferences;
#endif

    // Dictionary to track loaded assets by label
    private Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();

    // Dictionary to track loaded ScriptableObjects
    private Dictionary<string, ScriptableObject> loadedScriptableObjects = new Dictionary<string, ScriptableObject>();

    // Dictionary to track the status of each asset (using AssetStatus enum)
    internal Dictionary<string, AssetStatus> assetStatus = new Dictionary<string, AssetStatus>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        LoadAsset_Index(0);
    }

    private void OnDisable()
    {
        ReleaseAllAssets();
    }

    // Load all assets at once
    public void LoadAssets_All()
    {
#if USE_ADDRESSABLES_AkaitoAi
        for (int i = 0; AssetReferences.Length > i; i++)
        {
            LoadAsset(AssetReferences[i]);
        }
#endif
    }

    // Load a single asset based on its reference
#if USE_ADDRESSABLES_AkaitoAi
    void LoadAsset(AssetLabelReference assetReference)
    {
        if (assetStatus.ContainsKey(assetReference.labelString))
        {
            if (assetStatus[assetReference.labelString] == AssetStatus.Loading || assetStatus[assetReference.labelString] == AssetStatus.Loaded)
            {
                Debug.LogWarning($"Asset {assetReference.labelString} is already in the process of loading or already loaded.");
                return;
            }
        }
        else
        {
            assetStatus[assetReference.labelString] = AssetStatus.Loading; // Set to loading status
        }

        // Begin loading the asset
        Addressables.LoadAssetAsync<GameObject>(assetReference).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[assetReference.labelString] = handle.Result;  // Store the loaded asset
                assetStatus[assetReference.labelString] = AssetStatus.Loaded;  // Mark it as loaded
                Debug.Log($"Asset loaded: {handle.Result.name}");

            }
            else
            {
                assetStatus[assetReference.labelString] = AssetStatus.Failed;  // Mark it as failed
                Debug.LogWarning($"Failed to load asset for reference: {assetReference.labelString}");

            }
        };
    }
#endif

#if USE_ADDRESSABLES_AkaitoAi
    void LoadAsset_immediate(AssetLabelReference assetReference, Transform parent)
    {
        if (assetStatus.ContainsKey(assetReference.labelString))
        {
            if (assetStatus[assetReference.labelString] == AssetStatus.Loading || assetStatus[assetReference.labelString] == AssetStatus.Loaded)
            {
                Debug.LogWarning($"Asset {assetReference.labelString} is already in the process of loading or already loaded.");
                return;
            }
        }
        else
        {
            assetStatus[assetReference.labelString] = AssetStatus.Loading; // Set to loading status
        }

        // Begin loading the asset
        Addressables.LoadAssetAsync<GameObject>(assetReference).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject asset = loadedAssets[assetReference.labelString] = handle.Result;  // Store the loaded asset
                assetStatus[assetReference.labelString] = AssetStatus.Loaded;  // Mark it as loaded
                Debug.Log($"Asset loaded: {handle.Result.name}");

                //loadedAssets[assetReference.labelString];
                if (asset != null)
                {
                    if (parent != null)
                    {
                        GameObject instance = Instantiate(asset, parent);  // Instantiate the loaded asset
                        instance.SetActive(true);
                    }
                    else
                    {
                        GameObject instance = Instantiate(asset);  // Instantiate the loaded asset
                        instance.SetActive(true);
                    }
                    Debug.Log($"Asset instantiated: {asset.name}");
                }

            }
            else
            {
                assetStatus[assetReference.labelString] = AssetStatus.Failed;  // Mark it as failed
                Debug.LogWarning($"Failed to load asset for reference: {assetReference.labelString}");

            }
        };
    }
#endif

#if USE_ADDRESSABLES_AkaitoAi
    // Instantiate a specific asset based on its label
    void InstantiateAsset(AssetLabelReference assetReference, Transform parent)
    {
        if (assetStatus.ContainsKey(assetReference.labelString) && assetStatus[assetReference.labelString] == AssetStatus.Loaded)
        {
            GameObject asset = loadedAssets[assetReference.labelString];
            if (asset != null)
            {
                GameObject instance = Instantiate(asset);  // Instantiate the loaded asset
                instance.SetActive(true);
                Debug.Log($"Asset instantiated: {asset.name}");
            }
        }
        else
        {
            LoadAsset_immediate(assetReference, parent);
            Debug.LogWarning($"No loaded asset found with label: {assetReference.labelString} or the asset is still loading.");
        }
    }
#endif

#if USE_ADDRESSABLES_AkaitoAi
    // Release a specific asset based on its label
    void ReleaseAsset(AssetLabelReference assetReference)
    {
        if (loadedAssets.ContainsKey(assetReference.labelString))
        {
            GameObject asset = loadedAssets[assetReference.labelString];
            if (asset != null)
            {
                Addressables.Release(asset);  // Release the asset from memory
                loadedAssets.Remove(assetReference.labelString);  // Remove from loaded assets
                assetStatus.Remove(assetReference.labelString);  // Remove from status tracking
                Debug.Log($"Asset {asset.name} released.");
            }
        }
        else
        {
            Debug.LogWarning($"Asset with label {assetReference.labelString} not found.");
        }
    }
#endif
    // Optional: Release all loaded assets
    void ReleaseAllAssets()
    {
#if USE_ADDRESSABLES_AkaitoAi
        foreach (var asset in loadedAssets.Values)
        {
            Addressables.Release(asset);  // Release each asset
        }

        foreach (var so in loadedScriptableObjects.Values)
            Addressables.Release(so);
#endif
        loadedAssets.Clear();  // Clear loaded assets dictionary
        loadedScriptableObjects.Clear();
        assetStatus.Clear();  // Clear asset status dictionary
        Debug.Log("All assets and scriptable objects released.");

    }

    // Helper method to get the status of an asset by its label
    AssetStatus GetAssetStatus(string label)
    {
        if (assetStatus.ContainsKey(label))
        {
            return assetStatus[label];
        }
        return AssetStatus.NotLoaded;  // Return NotLoaded if status is not found
    }

    // New function to get a loaded asset by index
    public GameObject GetAsset_Index(int index)
    {
#if USE_ADDRESSABLES_AkaitoAi
        if (index < 0 || index >= AssetReferences.Length)
        {
            Debug.LogWarning($"Invalid index {index} for AssetReferences array.");
            return null;
        }

        string label = AssetReferences[index].labelString;
        if (assetStatus.ContainsKey(label) && assetStatus[label] == AssetStatus.Loaded)
        {
            return loadedAssets[label];
        }
        Debug.LogWarning($"Asset at index {index} (label: {label}) is not loaded or failed to load.");
#endif
        return null;
    }

    //Examples

    private void StatusCheck()
    {
#if USE_ADDRESSABLES_AkaitoAi
        AssetStatus status = AssetLoader_Addressable.instance.GetAssetStatus(AssetLoader_Addressable.instance.AssetReferences[0].labelString);

        if (status == AssetStatus.Loading)
        {
        }
        else if (status == AssetStatus.Loaded)
        {
        }
        else if (status == AssetStatus.NotLoaded)
        {
        }
        else if (status == AssetStatus.Failed)
        {
        }
#endif
    }

    public void LoadAsset_Index(int index)
    {
#if USE_ADDRESSABLES_AkaitoAi
        LoadAsset(AssetReferences[index]);
#endif
    }
    public void InstantiateAsset_Index(int index, Transform parent = null)
    {
#if USE_ADDRESSABLES_AkaitoAi
        InstantiateAsset(AssetReferences[index], parent);
#endif
    }

    public void ReleaseAsset_Index(int index)
    {
#if USE_ADDRESSABLES_AkaitoAi
        ReleaseAsset(AssetReferences[index]);
#endif
    }
    public void ReleaseAsset_All()
    {
        ReleaseAllAssets();
    }

    public GameObject InstantiateAsset_Index_Return(int index, Transform parent = null)
    {
#if USE_ADDRESSABLES_AkaitoAi
        if (index < 0 || index >= AssetReferences.Length)
        {
            Debug.LogWarning($"Invalid index {index} for AssetReferences array.");
            return null;
        }

        string label = AssetReferences[index].labelString;
        if (assetStatus.ContainsKey(label) && assetStatus[label] == AssetStatus.Loaded)
        {
            GameObject asset = loadedAssets[label];
            if (asset != null)
            {
                GameObject instance = parent != null ? Instantiate(asset, parent) : Instantiate(asset);
                instance.SetActive(true);
                Debug.Log($"Asset instantiated: {asset.name}");
                return instance;
            }
        }
        else
        {
            // Load the asset immediately and instantiate it
            assetStatus[label] = AssetStatus.Loading;
            Addressables.LoadAssetAsync<GameObject>(AssetReferences[index]).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject asset = handle.Result;
                    loadedAssets[label] = asset;
                    assetStatus[label] = AssetStatus.Loaded;
                    Debug.Log($"Asset loaded: {asset.name}");

                    GameObject instance = parent != null ? Instantiate(asset, parent) : Instantiate(asset);
                    instance.SetActive(true);
                    Debug.Log($"Asset instantiated: {asset.name}");
                    // Note: We can't return the instance here due to async callback
                }
                else
                {
                    assetStatus[label] = AssetStatus.Failed;
                    Debug.LogWarning($"Failed to load asset for reference: {label}");
                }
            };
        }
        Debug.LogWarning($"Asset at index {index} (label: {label}) is not loaded or still loading.");
#endif
        return null;
    }

    // Load ScriptableObject by label
#if USE_ADDRESSABLES_AkaitoAi
    public void LoadScriptableObject<T>(AssetLabelReference labelRef, System.Action<T> onLoaded = null) where T : ScriptableObject
    {
        string label = labelRef.labelString;

        if (assetStatus.ContainsKey(label) &&
            (assetStatus[label] == AssetStatus.Loading || assetStatus[label] == AssetStatus.Loaded))
        {
            Debug.LogWarning($"ScriptableObject {label} is already loading or loaded.");
            return;
        }

        assetStatus[label] = AssetStatus.Loading;

        Addressables.LoadAssetAsync<T>(labelRef).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                T so = handle.Result;
                loadedScriptableObjects[label] = so;
                assetStatus[label] = AssetStatus.Loaded;
                Debug.Log($"ScriptableObject loaded: {so.name}");
                onLoaded?.Invoke(so);
            }
            else
            {
                assetStatus[label] = AssetStatus.Failed;
                Debug.LogWarning($"Failed to load ScriptableObject for label: {label}");
            }
        };
    }
#endif
    public T GetLoadedScriptableObject<T>(string label) where T : ScriptableObject
    {
        if (loadedScriptableObjects.TryGetValue(label, out ScriptableObject so))
            return so as T;

        Debug.LogWarning($"ScriptableObject with label {label} not loaded.");
        return null;
    }

    public void ReleaseScriptableObject(string label)
    {
#if USE_ADDRESSABLES_AkaitoAi
        if (loadedScriptableObjects.TryGetValue(label, out ScriptableObject so))
        {
            Addressables.Release(so);
            loadedScriptableObjects.Remove(label);
            assetStatus.Remove(label);
            Debug.Log($"ScriptableObject {so.name} released.");
        }
#endif
    }

    public void LoadScriptableObject_Index<T>(int index, System.Action<T> onLoaded = null) where T : ScriptableObject
    {
#if USE_ADDRESSABLES_AkaitoAi
        if (index < 0 || index >= ScriptableObjectReferences.Length)
        {
            Debug.LogWarning($"Invalid index {index} for ScriptableObjectReferences.");
            return;
        }

        LoadScriptableObject<T>(ScriptableObjectReferences[index], onLoaded);
#endif
    }

    //[SerializeField] private AssetLabelReference levelSelectLabel;

    //void Start()
    //{
    //    AssetLoader_Addressable.instance.LoadScriptableObject<LevelSelectSO>(levelSelectLabel, so =>
    //    {
    //        currentLevelSelectSO = so;
    //        Debug.Log("LevelSelectSO loaded: " + so.name);
    //    });
    //}

    //void Start()
    //{
    //    LoadScriptableObject_Index<LevelSelectSO>(0, so =>
    //    {
    //        currentLevelSelectSO = so;
    //        Debug.Log("Loaded SO at start: " + so.name);
    //    });

    //    AssetLoader_Addressable.instance.LoadScriptableObject<LevelSelectSO>(levelSelectLabel, so =>
    //    {
    //        currentLevelSelectSO = so;
    //    });

    //    AssetLoader_Addressable.instance.LoadScriptableObject_Index<LevelSelectSO>(0, so =>
    //    {
    //        currentLevelSelectSO = so;
    //    });

    //    LevelSelectSO so = AssetLoader_Addressable.instance.GetLoadedScriptableObject<LevelSelectSO>("LevelSelect_Easy");

    //}


}