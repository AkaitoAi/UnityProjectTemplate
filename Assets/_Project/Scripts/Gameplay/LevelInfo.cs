using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public Transform spawn;
    public bool useTimer = false;
    public float time = 90f;
    public float winDelay = 5f;

    [Header("Objective Dialogues")]
    public bool showObjectiveInStart = false;
    public DialogueSO dialogueSO;
}
