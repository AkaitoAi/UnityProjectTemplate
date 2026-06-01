using UnityEngine;

public class Reseter : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void OnTriggerEnter(Collider other)
    {
        if (target == null) return;

        target = other.transform;

        target.position = new Vector3(0f, 2f, 0f);
        target.rotation = Quaternion.identity;
    }
}
