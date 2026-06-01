using UnityEngine;

namespace AkaitoAi
{
    public class RandomTextureSwaper : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private Texture[] textures;

        [System.Serializable]
        public struct Texture
        {
            public Texture2D mainTexture;
            public Texture2D normalTexture;

            public Vector2 tiling;
            public Color mainTextureColor;

            public Cubemap cubemap;
            public Color cubemapColor;
        }

        private void Start()
        {
            int randomTexture = Random.Range(0, textures.Length);

            // Changes Main Texture and it's color
            material.mainTexture = textures[randomTexture].mainTexture;
            material.color = textures[randomTexture].mainTextureColor;

            // Applies Scaling to the texture
            material.mainTextureScale = new Vector2(
                        textures[randomTexture].tiling.x,
                        textures[randomTexture].tiling.y);

            // Changes Normalmap Texture and it's scaling with respect to Main Texture
            if (CheckNormalTexture(randomTexture))
            {
                material.EnableKeyword("_NORMALMAP");
                material.SetTexture("_BumpMap",
                    textures[randomTexture].normalTexture);

                material.SetTextureScale("_BumpMap", new Vector2(
                textures[randomTexture].tiling.x,
                textures[randomTexture].tiling.y));
            }

            bool CheckNormalTexture(int randomTexture)
            {
                return textures[randomTexture].normalTexture != null;
            }

            //Applies Cubemap and it's color
            if (CheckCubemap(randomTexture))
            {
                material.SetTexture("_Cube",
                    textures[randomTexture].cubemap);

                material.SetColor("_ReflectColor",
                    textures[randomTexture].cubemapColor);

            }

            bool CheckCubemap(int randomTexture)
            {
                return textures[randomTexture].cubemap != null;
            }
        }
    }
}
