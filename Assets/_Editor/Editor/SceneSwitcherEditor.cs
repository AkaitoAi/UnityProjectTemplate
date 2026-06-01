using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AkaitoAi
{
    public class SceneSwitcherEditor : EditorWindow
    {
        private string[] sceneNames;
        private int selectedSceneIndex = 0;
        private bool allowSceneSwitching = true;
        private Vector2 scrollPos;

        [MenuItem("AkaitoAi/Scene Switcher")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SceneSwitcherEditor), false, "Scene Switcher");
        }

        private void OnEnable()
        {
            RefreshSceneNames();
            EditorBuildSettings.sceneListChanged += RefreshSceneNames;
        }

        private void OnDisable()
        {
            EditorBuildSettings.sceneListChanged -= RefreshSceneNames;
        }

        private void RefreshSceneNames()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            sceneNames = new string[scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
            {
                sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
            }

            Repaint();
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Switcher", EditorStyles.boldLabel);

            if (sceneNames == null || sceneNames.Length == 0)
            {
                RefreshSceneNames();
            }

            if (GUILayout.Button("Play From First Scene"))
            {
                PlayFromFirstScene();
            }

            GUILayout.Space(10); // Space below "Play From First Scene"

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play From This Scene", GUILayout.Width(160)))
            {
                PlayFromSceneByIndex(selectedSceneIndex);
            }

            int newSelectedIndex = EditorGUILayout.Popup(selectedSceneIndex, sceneNames);
            if (newSelectedIndex != selectedSceneIndex)
            {
                selectedSceneIndex = newSelectedIndex;
                SwitchToSelectedScene();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10); // Space above the scroll view scene buttons

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (GUILayout.Button($"Switch to {sceneNames[i]}"))
                {
                    SwitchToSceneByIndex(i);
                    selectedSceneIndex = i;
                    RefreshSceneNames();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void SwitchToSelectedScene()
        {
            if (selectedSceneIndex >= 0 && selectedSceneIndex < sceneNames.Length)
            {
                string selectedScenePath = EditorBuildSettings.scenes[selectedSceneIndex].path;

                if (SceneManager.GetActiveScene().path != selectedScenePath)
                {
                    EditorSceneManager.OpenScene(selectedScenePath);
                }
            }
        }

        private void PlayFromFirstScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0));
            EditorApplication.isPlaying = true;
            allowSceneSwitching = false;
        }

        private void PlayFromSceneByIndex(int index)
        {
            if (index >= 0 && index < sceneNames.Length)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string scenePath = EditorBuildSettings.scenes[index].path;
                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
                allowSceneSwitching = false;
            }
        }

        private void SwitchToSceneByIndex(int index)
        {
            if (index >= 0 && index < sceneNames.Length)
            {
                string scenePath = EditorBuildSettings.scenes[index].path;

                if (SceneManager.GetActiveScene().path != scenePath)
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
        }
    }
}
