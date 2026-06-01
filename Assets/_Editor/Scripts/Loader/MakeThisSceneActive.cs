using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MakeThisSceneActive : MonoBehaviour
{
    public UnityEvent OnStartEvent;

    private void Start()
    {
        OnStartEvent?.Invoke();
    }

    public void SetActiveScene()
    {
        Scene currentScene = gameObject.scene;

        if (currentScene.IsValid() && currentScene.isLoaded)
        {
            SceneManager.SetActiveScene(currentScene);
            Debug.Log($"[Lighting] Scene '{currentScene.name}' set as active to control lighting.");
        }
        else
        {
            Debug.LogWarning($"[Lighting] Scene '{currentScene.name}' is not valid or not loaded.");
        }
    }

    public void SetActiveSceneAsync()
    {
        StartCoroutine(SetSceneActiveNextFrame());

        IEnumerator SetSceneActiveNextFrame()
        {
            yield return null; // wait 1 frame to ensure full load
            Scene currentScene = gameObject.scene;
            if (currentScene.IsValid() && currentScene.isLoaded)
            {
                SceneManager.SetActiveScene(currentScene);
                Debug.Log($"[Lighting] Scene '{currentScene.name}' set as active to control lighting.");
            }
        }
    }
}
