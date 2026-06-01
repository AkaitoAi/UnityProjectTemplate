using UnityEngine;
using AkaitoAi.GameBase;

[RequireComponent(typeof(Renderer))]
public class MaterialBlinker : MonoBehaviour
{
    [SerializeField] private Color blinkColor = Color.red;
    [Tooltip("One-way blink time")][SerializeField] private float blinkDuration = 0.1f;
    [Tooltip("Total loops (2 = one blink)")][SerializeField] private int blinkLoopCount = 2;

    private MaterialColorFader colorFader;

    private void Awake()
    {
        colorFader = new MaterialColorFader(GetComponent<Renderer>());
    }

    public void Blink() => Blink(blinkDuration, blinkLoopCount);
    public void Blink(float duration, int loops)
    {
        colorFader.Blink(blinkColor, duration, loops);
    }

    private void OnDestroy()
    {
        colorFader?.ResetColors();
    }
}
