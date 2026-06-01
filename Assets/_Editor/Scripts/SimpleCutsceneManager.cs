using AkaitoAi.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using AkaitoAi.GameBase;

public class SimpleCutsceneManager : MonoBehaviour
{
    [Header("CutScene Setup")]
    [SerializeField] private bool playLevelStartCS = false;
    [SerializeField] private GameObject brainCamera;
    [SerializeField] private GameObject cutsceneContainer;
    public CutSceneSetup[] levelCutScenes;
    internal int cutSceneCount = 0;

    private GameManager gameManager;

    public UnityEvent OnStartEvent;

    private void Start()
    {
        gameManager = GameManager.GetInstance();

        if(playLevelStartCS) PlayCutScene();

        OnStartEvent?.Invoke();
    }

    public void PlayCutScene()
    {
        if (cutSceneCount >= levelCutScenes.Length) return;

        EnableCutScene(levelCutScenes[cutSceneCount].cutsceneObj,
                levelCutScenes[cutSceneCount].duration);
    }

    private void EnableCutScene(GameObject csObj, float duration)
    {
        if (cutSceneCount > levelCutScenes.Length) return;

        cutsceneContainer.SetActive(true);

        gameManager.state = GameplayScreens.CutScene;
        gameManager.UpdateGameplayState();

        //gameManager.selectedVehicleRCC.Rigid.constraints = RigidbodyConstraints.FreezeAll;

        EnableCutScene(csObj);

        void EnableCutScene(GameObject csObj)
        {
            csObj.SetActive(true);

            StartCoroutine(DisableCutScene(csObj, duration));

            IEnumerator DisableCutScene(GameObject csObj, float delay)
            {
                yield return AkaitoAiExtensions.Seconds(delay);

                //gameManager.selectedVehicleRCC.Rigid.constraints = RigidbodyConstraints.None;

                csObj.SetActive(false);

                gameManager.state = GameplayScreens.Resume;
                gameManager.UpdateGameplayState();

                //gameManager.selectedVehicleRCC.canControl = true;

                cutsceneContainer.SetActive(false);

                cutSceneCount++;
            }
        }
    }
}
