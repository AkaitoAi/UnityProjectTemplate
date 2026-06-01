using UnityEngine;

namespace AkaitoAi
{
    public class RandomTexture : MonoBehaviour
    {
        [SerializeField] private Material m_Material;
        [SerializeField] private ScriptableObject[] textures;

        [SerializeField] private string prefsName = "TextureIndex";

        internal int switchIndex = 0;
        internal int index;

        private void Start() => OnTextureSwitch();

        public void OnTextureSwitch()
        {
            switchIndex = PlayerPrefs.GetInt(prefsName, switchIndex);

            for (index = 0; index < textures.Length; index++)
            {
                if (index == switchIndex)
                {
                    if (m_Material != null)
                        UpdateTexture((TextureSO)textures[index]);
                    else UpdateMaterial((TextureSO)textures[index]);
                }
            }

            if (switchIndex < textures.Length - 1) switchIndex++;
            else switchIndex = 0;

            PlayerPrefs.SetInt(prefsName, switchIndex);
        }

        private void UpdateTexture(TextureSO _textureSO)
        {
            m_Material.color = _textureSO.textureColor;
            m_Material.mainTexture = _textureSO.mainTexture;
            m_Material.EnableKeyword("_NORMALMAP");
            m_Material.SetTexture("_BumpMap", _textureSO.normalmapTexture);
            m_Material.mainTextureScale = new Vector2(_textureSO.mainTextureTiling.x, _textureSO.mainTextureTiling.y);
            m_Material.SetTextureScale("_BumpMap", _textureSO.normalmapTiling);
        }

        private void UpdateMaterial(TextureSO _material)
        {
            if (!TryGetComponent<MeshRenderer>(out MeshRenderer mRend))
            {
                Debug.LogError("Mesh Renderer Not Assigned!");
            }
            else
            {
                mRend.material = _material.material;
            }
        }
    }
}
