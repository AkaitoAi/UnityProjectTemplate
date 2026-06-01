using System;
using UnityEngine.Events;

namespace AkaitoAi.Events
{
    [Serializable]
    public class AnimationEvent
    {
        public string eventName;
        public UnityEvent OnAnimationEvent;
    }
}