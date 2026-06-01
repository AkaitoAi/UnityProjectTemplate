using UnityEngine;

public class CameraFar : MonoBehaviour
{
    private void Start()
    {
        CameraFarInit();
    }
    private void CameraFarInit()
    {
        //if (TryGetComponent(out PerLayerCulling perLayerCulling))
        //    perLayerCulling.DefaultCullDistance = PlayerPrefs.GetFloat(
        //        GameManager.GetInstance().setupScriptable.cameraFarPref);

        //if (TryGetComponent(out Camera camera))
        //    camera.farClipPlane = PlayerPrefs.GetFloat(
        //        GameManager.GetInstance().setupScriptable.cameraFarPref);
        
        //else if (TryGetComponent(out CinemachineVirtualCamera cinemachineVirtualCamera))
        //    cinemachineVirtualCamera.m_Lens.FarClipPlane = PlayerPrefs.GetFloat(
        //        GameManager.GetInstance().setupScriptable.cameraFarPref);
    }

    EventBinding<OnSettingSave> settingSaveEventBinding;
    private void OnEnable()
    {
        settingSaveEventBinding = new EventBinding<OnSettingSave>(() => {
            CameraFarInit();
        });
        EventBus<OnSettingSave>.Register(settingSaveEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnSettingSave>.Deregister(settingSaveEventBinding);
    }
}
