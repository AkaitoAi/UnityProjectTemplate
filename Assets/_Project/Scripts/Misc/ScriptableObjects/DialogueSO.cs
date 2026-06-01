using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class DialogueSO : ScriptableObject
{
    [TextArea] public string[] dialogues;
    internal int currentDialogueIndex = 0;

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += ResetOnPlay;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= ResetOnPlay;
    }

    private void ResetOnPlay(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
            currentDialogueIndex = 0;
    }
#endif

    public string GetDialogue()
    {
        return currentDialogueIndex >= dialogues.Length?  dialogues[currentDialogueIndex = 0] : dialogues[currentDialogueIndex++];
    }
}
