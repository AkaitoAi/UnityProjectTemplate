using AkaitoAi;
using AkaitoAi.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AkaitoAi
{
    public enum UpdateType
    {
        FrameBased,
        TimeBased
    }

    [DefaultExecutionOrder(-100)]
    public class AdvanceUpdateManager : SingletonPresistent<AdvanceUpdateManager>
    {
        #region Key

        private struct GroupKey : IEquatable<GroupKey>
        {
            public UpdateType Type;
            public int FrameInterval;
            public int TimeMs; // time in milliseconds

            public GroupKey(UpdateType type, float timeInterval, int frameInterval)
            {
                Type = type;
                FrameInterval = frameInterval;
                TimeMs = Mathf.RoundToInt(timeInterval * 1000f);
            }

            public bool Equals(GroupKey other)
            {
                return Type == other.Type &&
                       FrameInterval == other.FrameInterval &&
                       TimeMs == other.TimeMs;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Type, FrameInterval, TimeMs);
            }
        }

        #endregion

        private class SubscriberGroup
        {
            public UpdateType Type;
            public int FrameInterval;
            public float TimeInterval;

            public List<Action> Subscribers = new();

            public int LastFrame;
            public float Timer;
        }

        private readonly Dictionary<GroupKey, SubscriberGroup> groups = new();

        #region Unity Update

        private void Update()
        {
            int currentFrame = Time.frameCount;
            float delta = Time.deltaTime;

            foreach (var group in groups.Values)
            {
                if (group.Type == UpdateType.FrameBased)
                    ProcessFrameGroup(group, currentFrame);
                else
                    ProcessTimeGroup(group, delta);
            }
        }

        private void ProcessFrameGroup(SubscriberGroup group, int currentFrame)
        {
            int deltaFrames = currentFrame - group.LastFrame;

            if (deltaFrames < group.FrameInterval)
                return;

            int ticks = deltaFrames / group.FrameInterval;
            group.LastFrame += ticks * group.FrameInterval;

            for (int i = 0; i < ticks; i++)
                InvokeGroup(group);
        }

        private void ProcessTimeGroup(SubscriberGroup group, float delta)
        {
            group.Timer += delta;

            while (group.Timer >= group.TimeInterval)
            {
                group.Timer -= group.TimeInterval;
                InvokeGroup(group);
            }
        }

        #endregion

        #region Invocation Safety

        private void InvokeGroup(SubscriberGroup group)
        {
            for (int i = group.Subscribers.Count - 1; i >= 0; i--)
            {
                var action = group.Subscribers[i];

                if (action == null || action.Target == null)
                {
                    group.Subscribers.RemoveAt(i);
                    continue;
                }

                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }


        #endregion

        #region Public API

        public void Register(Action callback, UpdateType type, float timeInterval = 0.1f, int frameInterval = 5)
        {
            if (callback == null) return;

            var key = new GroupKey(type, timeInterval, frameInterval);

            if (!groups.TryGetValue(key, out var group))
            {
                group = new SubscriberGroup
                {
                    Type = type,
                    FrameInterval = frameInterval,
                    TimeInterval = Mathf.Max(0.001f, timeInterval),
                    LastFrame = Time.frameCount,
                    Timer = 0f
                };

                groups.Add(key, group);
            }

            if (!group.Subscribers.Contains(callback))
                group.Subscribers.Add(callback);
        }

        public void Unregister(Action callback)
        {
            foreach (var kv in groups.ToList())
            {
                var group = kv.Value;

                if (!group.Subscribers.Remove(callback))
                    continue;

                if (group.Subscribers.Count == 0)
                    groups.Remove(kv.Key);

                return;
            }
        }

        #endregion

        #region Debug

#if UNITY_EDITOR
        [ContextMenu("📋 List Active Groups")]
        public void ListGroups()
        {
            if (groups.Count == 0)
            {
                Debug.Log("AdvanceUpdateManager: No active groups.");
                return;
            }

            foreach (var kv in groups)
            {
                Debug.Log(
                    $"{kv.Key.Type} | {kv.Value.Subscribers.Count} subs | " +
                    $"{kv.Value.TimeInterval}s | {kv.Value.FrameInterval} frames"
                );
            }
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 350, 25 + groups.Count * 22), "AdvanceUpdateManager");

            int y = 35;
            foreach (var g in groups.Values)
            {
                GUI.Label(new Rect(20, y, 330, 20),
                    $"{g.Type}  |  Subs: {g.Subscribers.Count}  |  " +
                    $"T:{g.TimeInterval:0.###}  F:{g.FrameInterval}");
                y += 22;
            }
        }
#endif

        #endregion
    }
}

//OnEnable() => AdvanceUpdateManager.GetInstance()?.Register(Action, UpdateType.TimeBased, 0.3f);
//OnDisable() => AdvanceUpdateManager.GetInstance()?.Unregister(Action);
