using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRotator : MonoBehaviour
{
    [Header("Speed & Feel")]
    [SerializeField] private float dragSpeed = 0.15f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Pitch Limits")]
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Options")]
    [SerializeField] private bool invertYaw;
    [SerializeField] private bool invertPitch;
    [SerializeField] private bool lockYaw;
    [SerializeField] private bool lockPitch;

    [Header("Idle Reset")]
    [SerializeField] private bool canReset = true;
    [SerializeField] private float timeToReset = 2f;
    [SerializeField] private float resetSpeed = 2f;

    private float currentYaw;
    private float currentPitch;
    private float targetYaw;
    private float targetPitch;
    private bool isDragging;
    private float lastInputTime;
    private float startYaw;
    private float startPitch;
    private float startRoll;
    private float pixelToDegree;

    private void OnEnable()
    {
        float dpi = Screen.dpi > 10f ? Screen.dpi : 160f;
        pixelToDegree = (160f / dpi) * dragSpeed;

        Vector3 e = transform.eulerAngles;
        startYaw = Mathf.Repeat(e.y, 360f);
        startPitch = Mathf.DeltaAngle(0, e.x);
        startRoll = e.z;

        targetYaw = currentYaw = startYaw;
        targetPitch = currentPitch = Mathf.Clamp(startPitch, minPitch, maxPitch);

        lastInputTime = Time.time;

        ApplyRotation();
    }

    public void BeginDrag()
    {
        isDragging = true;
        lastInputTime = Time.time;
    }

    public void OnDrag(PointerEventData data)
    {
        if (!isDragging) return;

        lastInputTime = Time.time;

        float yawDir = invertYaw ? -1f : 1f;
        float pitchDir = invertPitch ? 1f : -1f;

        if (!lockYaw)
            targetYaw += data.delta.x * pixelToDegree * yawDir;

        if (!lockPitch)
            targetPitch += data.delta.y * pixelToDegree * pitchDir;

        targetYaw = Mathf.Repeat(targetYaw, 360f);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
    }

    public void EndDrag()
    {
        isDragging = false;
        lastInputTime = Time.time;
    }

    private void Update()
    {
        if (canReset && !isDragging && Time.time - lastInputTime > timeToReset)
        {
            targetYaw = Mathf.MoveTowardsAngle(targetYaw, startYaw, resetSpeed * 100f * Time.deltaTime);
            targetPitch = Mathf.MoveTowards(targetPitch, startPitch, resetSpeed * 80f * Time.deltaTime);
        }

        float lerpFactor = isDragging ? 1f : Time.deltaTime * smoothSpeed;

        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, lerpFactor);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, lerpFactor);

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    public void ResetToStart()
    {
        targetYaw = startYaw;
        targetPitch = startPitch;
        lastInputTime = Time.time;
    }
}