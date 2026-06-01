using UnityEngine;

public class Interactor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out ITriggerEnterable trigger))
            return;

        trigger.TriggerEnter(this);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out ITriggerStayable trigger))
            return;

        trigger.TriggerStay(this);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out ITriggerExitable trigger))
            return;

        trigger.TriggerExit(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ICollisionEnterable col))
            return;

        col.CollisionEnter(this);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ICollisionStayable col))
            return;

        col.CollisionStay(this);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ICollisionExitable col))
            return;

        col.CollisionExit(this);
    }
}
