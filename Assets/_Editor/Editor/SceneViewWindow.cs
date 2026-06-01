using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AkaitoAi
{
    public class SceneViewWindow : EditorWindow
    {
        private Vector2 scrollPos;

        TextureImporterType type = TextureImporterType.Default;
        bool showAlphaOptions = false;
        bool inputTextureAlpha = true;
        bool fromGrayscale = true;

        [MenuItem("AkaitoAi/Tools/SceneView Window", false, 1)]
        internal static void Init()
        {
            var window = (SceneViewWindow)GetWindow(typeof(SceneViewWindow), false, "Scene View");
            window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
        }

        internal void OnGUI()
        {
            #region Build
            EditorGUILayout.LabelField("Build", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });
            // Calculate label width
            float labelWidth = EditorStyles.boldLabel.CalcSize(new GUIContent("Build and Run")).x;

            // Begin horizontal layout
            // GUILayout.BeginHorizontal();

            // Define a GUIStyle for square buttons with bright blue background
            GUIStyle brightBlueButtonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
            brightBlueButtonStyle.fixedHeight = 20; // Set the height for buttons

            // Define a GUIStyle for square buttons with bright purple background
            GUIStyle brightPurpleButtonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
            brightPurpleButtonStyle.fixedHeight = 20; // Set the height for buttons

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Build APK", brightBlueButtonStyle, GUILayout.ExpandWidth(true)))
            {
                BuildGame(false, false);
            }

            if (GUILayout.Button("Build and Run APK", brightPurpleButtonStyle, GUILayout.ExpandWidth(true)))
            {
                BuildGame(true, false); // Pass true to indicate Build and Run
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Build AAB", brightBlueButtonStyle, GUILayout.ExpandWidth(true)))
            {
                BuildGame(false, true);
            }

            if (GUILayout.Button("Build and Run AAB", brightPurpleButtonStyle, GUILayout.ExpandWidth(true)))
            {
                BuildGame(true, true); // Pass true to indicate Build and Run
            }
            // EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.EndHorizontal();


            // End horizontal layout
            // GUILayout.EndHorizontal();



            #endregion

            #region SceneView
            EditorGUILayout.BeginVertical();


            // Scroll view
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

            // Scenes in Build section

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 15;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Scenes In Build", style);
            // Add search bar for filtering scenes
            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label("Search Scene", GUILayout.Width(100f));
            GUILayout.Label("Search Scene", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });

            GUI.SetNextControlName("SearchBar");
            string searchQuery = EditorGUILayout.TextField("", EditorPrefs.GetString("SceneViewWindow_SearchQuery"), GUILayout.ExpandWidth(true));
            EditorPrefs.SetString("SceneViewWindow_SearchQuery", searchQuery);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Filter scenes based on search query
            int count = 0;
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.enabled)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    if (string.IsNullOrEmpty(searchQuery) || sceneName.ToLower().Contains(searchQuery.ToLower()))
                    {
                        count++;
                        EditorGUILayout.BeginHorizontal();
                        var buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
                        buttonStyle.alignment = TextAnchor.MiddleLeft;
                        buttonStyle.stretchWidth = true;

                        if (EditorSceneManager.GetActiveScene().path == scene.path)
                        {
                            // Set the color of the button to green for the active scene
                            buttonStyle.normal.textColor = Color.green;
                        }

                        var pressed = GUILayout.Button(i + ": " + sceneName, buttonStyle);

                        if (pressed)
                        {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(scene.path);
                            }
                        }

                        if (GUILayout.Button("Select", GUILayout.Width(60f)))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scene.path));
                        }

                        if (GUILayout.Button("Duplicate", GUILayout.Width(80f)))
                        {
                            var confirm = EditorUtility.DisplayDialog("Duplicate Scene", "Do you want to duplicate the scene " + sceneName + "? ", "Yes", "No");
                            if (confirm)
                            {
                                var newPath = AssetDatabase.GenerateUniqueAssetPath(scene.path);
                                AssetDatabase.CopyAsset(scene.path, newPath);
                                AssetDatabase.Refresh();
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            if (count == 0)
            {
                GUILayout.Label("No scenes found.", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion

            #region PlayerPref

            // Delete PlayerPrefs section 
            EditorGUILayout.LabelField("Delete All PlayerPrefs", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });

            GUILayout.Space(10);
            if (GUILayout.Button("Clear Prefs", new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft }))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("PlayerPrefs cleared.");
            }


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion

            #region Singleton
            // Add Singleton section
            GUILayout.Space(10);
            //GUILayout.Label("Add Singleton class in Selected Script", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Add Singleton class in Selected Script", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });

            //GUILayout.Space(10);
            if (GUILayout.Button("Add Singleton", new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft }))
            {
                AddSingleton();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion

            #region TextureSelection
            GUILayout.Space(10);
            // Add Singleton section
            EditorGUILayout.LabelField("Select Textures By Type", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });

            type = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", type);

            if (type == TextureImporterType.Default)
            {
                showAlphaOptions = true;
            }
            else
            {
                showAlphaOptions = false;
                inputTextureAlpha = true;
                fromGrayscale = true;
            }

            if (showAlphaOptions)
            {
                inputTextureAlpha = EditorGUILayout.ToggleLeft("Input Texture Alpha", inputTextureAlpha);
                fromGrayscale = EditorGUILayout.ToggleLeft("From Grayscale", fromGrayscale);
            }

            if (GUILayout.Button("Select Textures"))
            {

                FindTextures();
            }

            #endregion

            #region AutoAnchor
            // Add Singleton section
            GUILayout.Space(10);
            //GUILayout.Label("Add Singleton class in Selected Script", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("AutoAnchor the Rect Object", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });

            //GUILayout.Space(10);
            if (GUILayout.Button("AutoAnchor _F1", new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft }))
            {
                AnchorSelectedUIObjects();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion



            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        #region BUILD

        [MenuItem("AkaitoAi/Tools/BUILDS/Build Game &B")]
        private static void BuildGameWithoutRun()
        {
            BuildGame(false, false);
        }

        [MenuItem("AkaitoAi/Tools/BUILDS/Build Game with Run &#B")]
        private static void BuildGameWithRun()
        {
            BuildGame(true, false);
        }
        public static void BuildGame(bool run, bool AppBundle)
        {
            // Check if the editor is in play mode
            if (UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogError("Build aborted: Cannot build or run while in play mode.");
                return;
            }
            // Save the current scene if it's not saved
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveOpenScenes();
            }

            string productName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            int bundleVersionCode = PlayerSettings.Android.bundleVersionCode;

            // Build settings
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            buildPlayerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            // Add Clean Build option if Needed
            // Fetch the previous build bundle and version
            string previousBuildBundle = EditorPrefs.GetString("BundleKey");
            string previousVersion = EditorPrefs.GetString("VersionKey");
            Debug.Log("previousBuildBundle  " + previousBuildBundle);
            Debug.Log("previousVersion " + previousVersion);

            // Get the current build bundle and version
            string currentBuildBundle = PlayerSettings.applicationIdentifier;
            string currentVersion = PlayerSettings.bundleVersion;

            // Check if build bundle or version has changed
            bool buildBundleChanged = !string.Equals(previousBuildBundle, currentBuildBundle);
            bool versionChanged = !string.Equals(previousVersion, currentVersion);

            // Create build options
            BuildOptions buildOptions = BuildOptions.None;

            // Check if either build bundle or version has changed, then set CleanBuildCache option
            if (buildBundleChanged || versionChanged)
            {
                buildOptions |= BuildOptions.CleanBuildCache;
                Debug.LogError("CleanBuildCache option is enabled due to changes in build bundle or version.");
            }


            // Save the current build bundle and version for the next comparison
            EditorPrefs.SetString("BundleKey", currentBuildBundle);
            EditorPrefs.SetString("VersionKey", currentVersion);

            if (AppBundle)
            {
                EditorUserBuildSettings.buildAppBundle = true;
                buildPlayerOptions.locationPathName = "Builds/" + productName + "_v" + version + "_b" + bundleVersionCode + ".aab";
            }
            else
            {
                EditorUserBuildSettings.buildAppBundle = false;
                buildPlayerOptions.locationPathName = "Builds/" + productName + "_v" + version + "_b" + bundleVersionCode + ".apk";
            }
            buildPlayerOptions.target = BuildTarget.Android;

            // Add BuildOptions.AutoRunPlayer if 'run' is true
            buildPlayerOptions.options = run ? BuildOptions.AutoRunPlayer : BuildOptions.None;


            // Perform the build
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                // Open build location
                EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");

            }
        }
        #endregion
        void FindTextures()
        {
            Object[] textures = Selection.objects;
            List<Object> selectedTextures = new List<Object>();
            foreach (Object texture in textures)
            {
                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
                if (importer != null && importer.textureType == type && importer.alphaSource == GetAlphaSource())
                {
                    selectedTextures.Add(texture);
                }
            }
            if (selectedTextures.Count > 0)
            {
                Selection.objects = selectedTextures.ToArray();
            }
            else
            {
                EditorUtility.DisplayDialog("No Textures Found", "No textures of type " + type + " with alpha source " + GetAlphaSource() + " were found.", "OK");
            }

        }

        TextureImporterAlphaSource GetAlphaSource()
        {
            if (inputTextureAlpha)
            {
                return TextureImporterAlphaSource.FromInput;
            }
            else if (fromGrayscale)
            {
                return TextureImporterAlphaSource.FromGrayScale;
            }
            else
            {
                return TextureImporterAlphaSource.None;
            }
        }

        void AddSingleton()
        {
            MonoScript script = Selection.activeObject as MonoScript;

            if (script == null)
            {
                Debug.LogError("Please select a script first.");
                return;
            }

            string scriptPath = AssetDatabase.GetAssetPath(script);

            if (!scriptPath.EndsWith(".cs"))
            {
                Debug.LogError("Please select a C# script.");
                return;
            }

            // Ask for confirmation before adding singleton to the script
            bool addSingleton = EditorUtility.DisplayDialog("Add Singleton", "Do you want to add a singleton to " + script.name + "?", "Yes", "No");

            if (!addSingleton)
            {
                return;
            }

            string scriptContents = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(TextAsset)).ToString();
            string className = Path.GetFileNameWithoutExtension(scriptPath);

            string singletonCode =
                "public class " + className + " : MonoBehaviour\n" +
                "{\n" +
                "    private static " + className + " _instance;\n" +
                "\n" +
                "    public static " + className + " Instance\n" +
                "    {\n" +
                "        get\n" +
                "        {\n" +
                "            if (_instance == null)\n" +
                "            {\n" +
                "                _instance = FindObjectOfType<" + className + ">();\n" +
                "                if (_instance == null)\n" +
                "                {\n" +
                "                    GameObject singletonObject = new GameObject(\"" + className + "Singleton\");\n" +
                "                    _instance = singletonObject.AddComponent<" + className + ">();\n" +
                //"                    DontDestroyOnLoad(singletonObject);\n" +
                "                }\n" +
                "            }\n" +
                "\n" +
                "            return _instance;\n" +
                "        }\n" +
                "    }\n" +
                "\n" +
                "    private void Awake()\n" +
                "    {\n" +
                "        if (_instance == null)\n" +
                "        {\n" +
                "            _instance = this;\n" +
                "            DontDestroyOnLoad(this.gameObject);\n" +
                "        }\n" +
                "        else\n" +
                "        {\n" +
                "            Destroy(this.gameObject);\n" +
                "        }\n" +
                "    }\n" +
                "}\n";

            int classIndex = scriptContents.IndexOf("public class");
            if (classIndex == -1)
            {
                Debug.LogError("Could not find class definition in script.");
                return;
            }

            string newContents = scriptContents.Insert(classIndex, singletonCode);

            // AssetDatabase.DeleteAsset(scriptPath);
            AssetDatabase.OpenAsset(script, 0);
            EditorGUIUtility.PingObject(script);

            File.WriteAllText(scriptPath, newContents);

            AssetDatabase.Refresh();
        }

        private static void Anchor(RectTransform rectTransform)
        {
            RectTransform parentRectTransform = null;
            if (rectTransform.transform.parent)
                parentRectTransform = rectTransform.transform.parent.GetComponent<RectTransform>();

            if (!parentRectTransform)
                return;

            Undo.RecordObject(rectTransform, "Anchor UI Object");
            Rect parentRect = parentRectTransform.rect;
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x + (rectTransform.offsetMin.x / parentRect.width), rectTransform.anchorMin.y + (rectTransform.offsetMin.y / parentRect.height));
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x + (rectTransform.offsetMax.x / parentRect.width), rectTransform.anchorMax.y + (rectTransform.offsetMax.y / parentRect.height));
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);



        }
        [MenuItem("AkaitoAi/Tools/Anchor Selected UI Objects _F1")]
        private static void AnchorSelectedUIObjects()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                RectTransform rectTransform = Selection.gameObjects[i].GetComponent<RectTransform>();
                if (rectTransform)
                    Anchor(rectTransform);
            }
        }
    }
}
