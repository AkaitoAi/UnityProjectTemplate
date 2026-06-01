using UnityEngine;

public class EnvLoader : MonoBehaviour
{
    public bool CanInstantiate = true, CanRelease = true, CanLoad = true;
    public int AssetIndextoInstantiate = 0, AssetIndextoLoad = 0;

    //public AssetLoader_Addressable assetLoader_Addressable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnEnable()
    {
        if (CanInstantiate)
        {
            AssetLoader_Addressable.instance.InstantiateAsset_Index(AssetIndextoInstantiate, null);
        }
        if (CanLoad)
        {
            AssetLoader_Addressable.instance.LoadAsset_Index(AssetIndextoLoad);
        }
    }
    void OnDisable()
    {
        if (CanRelease)
        {
            AssetLoader_Addressable.instance.ReleaseAsset_Index(AssetIndextoInstantiate);
        }
    }
}
