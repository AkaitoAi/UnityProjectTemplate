using UnityEngine;

[CreateAssetMenu(fileName = "TextureSwaper", menuName = "ScriptableObjects/TextureSwaper", order = 1)]
public class TextureSwaperSO : ScriptableObject
{
    public MaterialTexture[] textureSwapData;

    [System.Serializable]
    public struct MaterialTexture
    {
        public Texture2D org_Texture, new_Texture;
        public Material material;
    }

    public void SwapTexture(bool isOrginial)
    {
        if (isOrginial)
        {
            foreach (MaterialTexture mt in textureSwapData)
                mt.material.mainTexture = mt.new_Texture;

            return;
        }
        
        if (!isOrginial)
        {
            foreach (MaterialTexture mt in textureSwapData)
                mt.material.mainTexture = mt.org_Texture;

            return;
        }
    }
}
