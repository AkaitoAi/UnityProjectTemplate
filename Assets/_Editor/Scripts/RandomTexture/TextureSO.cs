using UnityEngine;

[CreateAssetMenu(fileName = "Texture", menuName = "ScriptableObjects/Texture", order = 1)]
public class TextureSO : ScriptableObject
{
    public Color textureColor;
    public Texture2D mainTexture;
    public Texture2D normalmapTexture;

    public Vector2 mainTextureTiling;
    public Vector2 normalmapTiling;

    public Material material;
}
