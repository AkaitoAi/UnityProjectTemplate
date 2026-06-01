using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSwitcherToolbarEditor : Editor
{
    [InitializeOnLoad]
    public class RecentSceneToolbarGUI
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChangedInEditMode;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }
        private static void EditorSceneManager_activeSceneChangedInEditMode(Scene current, Scene next) { }
        internal static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            string buttonText = $"  Scene Switcher  ▼ ";
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 20f,
                margin = new RectOffset(5, 5, 2, 2),
                padding = new RectOffset(8, 8, 0, 0),
                normal = { textColor = Color.white },
                hover = { textColor = Color.yellow },
                active = { textColor = Color.grey },
                stretchWidth = false
            };

            var sceneIconTex = EditorGUIUtility.IconContent(@"d_BuildSettings.SelectedIcon").image;
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(buttonText, sceneIconTex), style, GUILayout.Width(250f));
            if (GUI.Button(buttonRect, new GUIContent(buttonText, sceneIconTex, "Switch, Locate or Play any scene"), style))
            {
                Vector2 buttonPos = GUIUtility.GUIToScreenPoint(new Vector2(buttonRect.x, buttonRect.y + buttonRect.height));
                SceneListPopup.ShowPopup(buttonRect);
            }
            GUILayout.Space(5f);
        }

        private static void ShowPlaySceneDropdown()
        {
            GenericMenu menu = new GenericMenu();
            var scenes = EditorBuildSettings.scenes;
            string activeScenePath = SceneManager.GetActiveScene().path;

            foreach (var scene in scenes)
            {
                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                bool isActiveScene = scenePath == activeScenePath;

                menu.AddItem(new GUIContent(sceneName), isActiveScene, () => PlayScene(scenePath));
            }
            menu.ShowAsContext();
        }
        public static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
        public static void PlayScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }
        }
        public static void LocateScene(string scenePath)
        {
            UnityEngine.Object sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset != null)
            {
                EditorGUIUtility.PingObject(sceneAsset);
                Selection.activeObject = sceneAsset;
            }
        }
    }
    public class SceneListPopup : PopupWindowContent
    {
        private Vector2 scrollPos;
        private float buttonWidth;
        private Rect buttonRect;

        private const float ButtonHeight = 24f;
        private const float MaxPopupHeight = 400f;
        private const float PopupWidth = 250f;

        public SceneListPopup(Rect buttonRect, float buttonWidth)
        {
            this.buttonRect = buttonRect;
            this.buttonWidth = buttonWidth;
        }

        public static void ShowPopup(Rect buttonScreenRect)
        {
            UnityEditor.PopupWindow.Show(buttonScreenRect, new SceneListPopup(buttonScreenRect, PopupWidth));
        }
        public override Vector2 GetWindowSize()
        {
            int sceneCount = EditorBuildSettings.scenes.Length;
            float calculatedHeight = sceneCount * ButtonHeight;
            float finalHeight = Mathf.Min(calculatedHeight, MaxPopupHeight);
            return new Vector2(PopupWidth, finalHeight);
        }

        public override void OnGUI(Rect rect)
        {
            var scenes = EditorBuildSettings.scenes;
            string activeScenePath = SceneManager.GetActiveScene().path;
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.25f, 0.3f));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(rect.width), GUILayout.Height(rect.height));

            Color originalColor = GUI.color;
            GUI.color = new Color(0.15f, 0.15f, 0.15f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            foreach (var scene in scenes)
            {
                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                bool isActiveScene = scenePath == activeScenePath;

                EditorGUILayout.BeginHorizontal();

                GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
                {
                    normal = { textColor = Color.white },
                    hover = { textColor = Color.yellow },
                    active = { textColor = Color.grey },
                };

                if (GUILayout.Button("Play From → ", btnStyle,  GUILayout.Width(90)))
                {
                    RecentSceneToolbarGUI.PlayScene(scenePath);
                }

                GUIStyle sceneStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = isActiveScene ? FontStyle.Bold : FontStyle.Normal,
                    normal = { textColor = isActiveScene ? Color.green : Color.white },
                    hover = { textColor = Color.yellow }

                };

                if (GUILayout.Button(sceneName, sceneStyle, GUILayout.Width(87)))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scene.path);
                        editorWindow.Close();
                    }
                }
                GUI.enabled = true;

                if (GUILayout.Button("Locate", btnStyle, GUILayout.Width(60)))
                {
                    RecentSceneToolbarGUI.LocateScene(scenePath);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    #region UnityToolBarExtender
    public static class ToolbarCallback
    {
        static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        static Type m_guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        static Type m_iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
        static PropertyInfo m_windowBackend = m_guiViewType.GetProperty("windowBackend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
        static PropertyInfo m_viewVisualTree = m_guiViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        static ScriptableObject m_currentToolbar;

        public static Action OnToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (m_currentToolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (m_currentToolbar != null)
                {
#if UNITY_2021_1_OR_NEWER
                    var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(m_currentToolbar);
                    var mRoot = rawRoot as VisualElement;
                    RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                    RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);
                        var parent = new VisualElement()
                        {
                            style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        var container = new IMGUIContainer();
                        container.style.flexGrow = 1;
                        container.onGUIHandler += () => { cb?.Invoke(); };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
#else
#if UNITY_2020_1_OR_NEWER
                    var windowBackend = m_windowBackend.GetValue(m_currentToolbar);
                    var visualTree = (VisualElement)m_viewVisualTree.GetValue(windowBackend, null);
#else
                    var visualTree = (VisualElement)m_viewVisualTree.GetValue(m_currentToolbar, null);
#endif
                    var container = (IMGUIContainer)visualTree[0];
                    var handler = (Action)m_imguiContainerOnGui.GetValue(container);
                    handler -= OnGUI;
                    handler += OnGUI;
                    m_imguiContainerOnGui.SetValue(container, handler);
#endif
                }
            }
        }
        static void OnGUI()
        {
            var handler = OnToolbarGUI;
            if (handler != null) handler();
        }
    }

    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        static int m_toolCount;
        static GUIStyle m_commandStyle = null;

        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

        static ToolbarExtender()
        {
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");

#if UNITY_2019_1_OR_NEWER
            string fieldName = "k_ToolCount";
#else
            string fieldName = "s_ShownToolIcons";
#endif

            FieldInfo toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_3_OR_NEWER
            m_toolCount = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 8;
#elif UNITY_2019_1_OR_NEWER
            m_toolCount = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
            m_toolCount = toolIcons != null ? ((Array)toolIcons.GetValue(null)).Length : 6;
#else
            m_toolCount = toolIcons != null ? ((Array)toolIcons.GetValue(null)).Length : 5;
#endif

            ToolbarCallback.OnToolbarGUI = OnGUI;
            ToolbarCallback.OnToolbarGUILeft = GUILeft;
            ToolbarCallback.OnToolbarGUIRight = GUIRight;
        }

#if UNITY_2019_3_OR_NEWER
        public const float space = 8;
#else
        public const float space = 10;
#endif
        public const float largeSpace = 20;
        public const float buttonWidth = 32;
        public const float dropdownWidth = 80;
#if UNITY_2019_1_OR_NEWER
        public const float playPauseStopWidth = 140;
#else
        public const float playPauseStopWidth = 100;
#endif

        static void OnGUI()
        {
            if (m_commandStyle == null)
            {
                m_commandStyle = new GUIStyle("CommandLeft");
            }
            var screenWidth = EditorGUIUtility.currentViewWidth;
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - playPauseStopWidth) / 2);

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += space;
            leftRect.xMin += buttonWidth * m_toolCount;
#if UNITY_2019_3_OR_NEWER
            leftRect.xMin += space;
#else
            leftRect.xMin += largeSpace;
#endif
            leftRect.xMin += 64 * 2;
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += m_commandStyle.fixedWidth * 3;
            rightRect.xMax = screenWidth;
            rightRect.xMax -= space;
            rightRect.xMax -= dropdownWidth;
            rightRect.xMax -= space;
            rightRect.xMax -= dropdownWidth;
#if UNITY_2019_3_OR_NEWER
            rightRect.xMax -= space;
#else
            rightRect.xMax -= largeSpace;
#endif
            rightRect.xMax -= dropdownWidth;
            rightRect.xMax -= space;
            rightRect.xMax -= buttonWidth;
            rightRect.xMax -= space;
            rightRect.xMax -= 78;

            leftRect.xMin += space;
            leftRect.xMax -= space;
            rightRect.xMin += space;
            rightRect.xMax -= space;

#if UNITY_2019_3_OR_NEWER
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;
#else
            leftRect.y = 5;
            leftRect.height = 24;
            rightRect.y = 5;
            rightRect.height = 24;
#endif

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in LeftToolbarGUI)
                {
                    handler();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in RightToolbarGUI)
                {
                    handler();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        public static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in LeftToolbarGUI)
            {
                handler();
            }
            GUILayout.EndHorizontal();
        }

        public static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in RightToolbarGUI)
            {
                handler();
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion
}
#endif