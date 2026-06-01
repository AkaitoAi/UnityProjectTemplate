using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<ICollectible>(out ICollectible collectible))
            return;

        collectible.OnCollect();
    }
}
