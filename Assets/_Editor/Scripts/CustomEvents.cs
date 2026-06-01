using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AkaitoAi.Events
{
    public class CustomEvents : MonoBehaviour
    {
        [Header("Use Your Desired Events")]
        public bool DisableOrNot = false;
        public float TimeToDisable = 5f;
        public float DelayEventTime = 5f;
        public string TriggerTagToDetect = "Player";

        public UnityEvent AwakeEvent;
        public UnityEvent OnStartEvent;
        public UnityEvent OnEnableEvent;
        public UnityEvent OnDisableEvent;
        public UnityEvent OnDestroyEvent;
        public UnityEvent OnTriggerEnterEvent;
        public UnityEvent EventAfterTime;
        public UnityEvent OntriggerExitEvent;

        public void Awake() => AwakeEvent?.Invoke();
        public void OnEnable() => OnEnableEvent?.Invoke();

        void Start()
        {
            OnStartEvent?.Invoke();
            StartCoroutine(PerformActionAfterTime());

            if (DisableOrNot)
                StartCoroutine(DisableThisObject());
        }

        IEnumerator DisableThisObject()
        {
            yield return new WaitForSeconds(TimeToDisable);
            gameObject.SetActive(false);
        }

        IEnumerator PerformActionAfterTime()
        {
            yield return new WaitForSeconds(DelayEventTime);
            EventAfterTime?.Invoke();
        }

        private void OnDisable() => OnDisableEvent?.Invoke();
        private void OnDestroy() => OnDestroyEvent?.Invoke();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TriggerTagToDetect))
                OnTriggerEnterEvent?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TriggerTagToDetect))
                OntriggerExitEvent?.Invoke();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CustomEvents))]
        public class CustomEventsEditor : Editor
        {
            // Track which events are expanded
            private bool showAwakeEvent = false;
            private bool showOnStartEvent = false;
            private bool showOnEnableEvent = false;
            private bool showOnDisableEvent = false;
            private bool showOnDestroyEvent = false;
            private bool showOnTriggerEnterEvent = false;
            private bool showOnTriggerExitEvent = false;
            private bool showEventAfterTime = false;

            // Serialized properties for other fields
            SerializedProperty disableOrNot;
            SerializedProperty timeToDisable;
            SerializedProperty delayEventTime;
            SerializedProperty triggerTagToDetect;

            private void OnEnable()
            {
                // Link serialized properties to the fields
                disableOrNot = serializedObject.FindProperty("DisableOrNot");
                timeToDisable = serializedObject.FindProperty("TimeToDisable");
                delayEventTime = serializedObject.FindProperty("DelayEventTime");
                triggerTagToDetect = serializedObject.FindProperty("TriggerTagToDetect");
            }

            public override void OnInspectorGUI()
            {
                // Update serialized object
                serializedObject.Update();

                // Display custom fields
                EditorGUILayout.PropertyField(disableOrNot, new GUIContent("Disable Object After Time"));
                EditorGUILayout.PropertyField(timeToDisable, new GUIContent("Time to Disable"));
                EditorGUILayout.PropertyField(delayEventTime, new GUIContent("Delay Event Time"));
                EditorGUILayout.PropertyField(triggerTagToDetect, new GUIContent("Trigger Tag to Detect"));

                // Retrieve serialized properties for each UnityEvent
                SerializedProperty awakeEvent = serializedObject.FindProperty("AwakeEvent");
                SerializedProperty onStartEvent = serializedObject.FindProperty("OnStartEvent");
                SerializedProperty onEnableEvent = serializedObject.FindProperty("OnEnableEvent");
                SerializedProperty onDisableEvent = serializedObject.FindProperty("OnDisableEvent");
                SerializedProperty onDestroyEvent = serializedObject.FindProperty("OnDestroyEvent");
                SerializedProperty onTriggerEnterEvent = serializedObject.FindProperty("OnTriggerEnterEvent");
                SerializedProperty onTriggerExitEvent = serializedObject.FindProperty("OntriggerExitEvent");
                SerializedProperty eventAfterTime = serializedObject.FindProperty("EventAfterTime");

                // Helper method to check if a UnityEvent has any listeners
                bool HasListeners(UnityEvent unityEvent)
                {
                    return unityEvent != null && unityEvent.GetPersistentEventCount() > 0;
                }

                // Toggle buttons to show/hide event fields
                showAwakeEvent = HasListeners((target as CustomEvents).AwakeEvent) || showAwakeEvent;
                if (GUILayout.Button(showAwakeEvent ? "Hide Awake Event" : "Show Awake Event"))
                {
                    showAwakeEvent = !showAwakeEvent;
                }
                if (showAwakeEvent)
                {
                    EditorGUILayout.PropertyField(awakeEvent);
                }

                showOnStartEvent = HasListeners((target as CustomEvents).OnStartEvent) || showOnStartEvent;
                if (GUILayout.Button(showOnStartEvent ? "Hide On Start Event" : "Show On Start Event"))
                {
                    showOnStartEvent = !showOnStartEvent;
                }
                if (showOnStartEvent)
                {
                    EditorGUILayout.PropertyField(onStartEvent);
                }

                showOnEnableEvent = HasListeners((target as CustomEvents).OnEnableEvent) || showOnEnableEvent;
                if (GUILayout.Button(showOnEnableEvent ? "Hide On Enable Event" : "Show On Enable Event"))
                {
                    showOnEnableEvent = !showOnEnableEvent;
                }
                if (showOnEnableEvent)
                {
                    EditorGUILayout.PropertyField(onEnableEvent);
                }

                showOnDisableEvent = HasListeners((target as CustomEvents).OnDisableEvent) || showOnDisableEvent;
                if (GUILayout.Button(showOnDisableEvent ? "Hide On Disable Event" : "Show On Disable Event"))
                {
                    showOnDisableEvent = !showOnDisableEvent;
                }
                if (showOnDisableEvent)
                {
                    EditorGUILayout.PropertyField(onDisableEvent);
                }

                showOnDestroyEvent = HasListeners((target as CustomEvents).OnDestroyEvent) || showOnDestroyEvent;
                if (GUILayout.Button(showOnDestroyEvent ? "Hide On Destroy Event" : "Show On Destroy Event"))
                {
                    showOnDestroyEvent = !showOnDestroyEvent;
                }
                if (showOnDestroyEvent)
                {
                    EditorGUILayout.PropertyField(onDestroyEvent);
                }

                showOnTriggerEnterEvent = HasListeners((target as CustomEvents).OnTriggerEnterEvent) || showOnTriggerEnterEvent;
                if (GUILayout.Button(showOnTriggerEnterEvent ? "Hide On Trigger Enter Event" : "Show On Trigger Enter Event"))
                {
                    showOnTriggerEnterEvent = !showOnTriggerEnterEvent;
                }
                if (showOnTriggerEnterEvent)
                {
                    EditorGUILayout.PropertyField(onTriggerEnterEvent);
                }

                showOnTriggerExitEvent = HasListeners((target as CustomEvents).OntriggerExitEvent) || showOnTriggerExitEvent;
                if (GUILayout.Button(showOnTriggerExitEvent ? "Hide On Trigger Exit Event" : "Show On Trigger Exit Event"))
                {
                    showOnTriggerExitEvent = !showOnTriggerExitEvent;
                }
                if (showOnTriggerExitEvent)
                {
                    EditorGUILayout.PropertyField(onTriggerExitEvent);
                }

                showEventAfterTime = HasListeners((target as CustomEvents).EventAfterTime) || showEventAfterTime;
                if (GUILayout.Button(showEventAfterTime ? "Hide Event After Time" : "Show Event After Time"))
                {
                    showEventAfterTime = !showEventAfterTime;
                }
                if (showEventAfterTime)
                {
                    EditorGUILayout.PropertyField(eventAfterTime);
                }

                // Apply changes to serialized properties
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
