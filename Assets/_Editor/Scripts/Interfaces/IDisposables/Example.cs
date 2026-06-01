using AkaitoAi.Extensions;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Start()
    {
        using (new DisposableLogStopwatch())
        {
            //TODO do something
        }
    }
}
