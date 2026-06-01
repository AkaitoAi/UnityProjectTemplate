using UnityEngine;
using UnityEditor;

namespace AkaitoAi
{
    public class FBXSettingsTool : EditorWindow
    {
        // Model tab settings
        private bool applyModelSettings = true;
        private bool applyGlobalScale = true;
        private float globalScale = 1.0f;
        private ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.High;
        private bool readWriteEnabled = false;
        private bool optimizeMesh = true;
        private bool importBlendShapes = false;
        private bool importVisibility = false;
        private bool importCameras = false;
        private bool importLights = false;
        private ModelImporterMaterialName materialName = ModelImporterMaterialName.BasedOnMaterialName;
        private ModelImporterMaterialSearch materialSearch = ModelImporterMaterialSearch.Local;

        // Rig tab settings
        private bool applyRigSettings = true;
        private ModelImporterAnimationType animationType = ModelImporterAnimationType.None;

        // Animation tab settings
        private bool applyAnimationSettings = true;
        private bool importAnimation = false;
        private ModelImporterAnimationCompression animationCompression = ModelImporterAnimationCompression.Off;
        private bool bakeAnimations = false;
        private bool resampleCurves = true;
        private bool optimizeGameObjects = false;

        [MenuItem("AkaitoAi/Tools/FBX Settings")]
        public static void ShowWindow()
        {
            GetWindow<FBXSettingsTool>("FBX Settings Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Model Settings", EditorStyles.boldLabel);

            // Model Tab Settings
            applyModelSettings = EditorGUILayout.Toggle("Apply Model Settings", applyModelSettings);
            if (applyModelSettings)
            {
                applyGlobalScale = EditorGUILayout.Toggle("Apply Global Scale", applyGlobalScale);
                if (applyGlobalScale)
                {
                    globalScale = EditorGUILayout.FloatField("Global Scale", globalScale);
                }

                meshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", meshCompression);
                readWriteEnabled = EditorGUILayout.Toggle("Read/Write Enabled", readWriteEnabled);
                optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", optimizeMesh);
                importBlendShapes = EditorGUILayout.Toggle("Import BlendShapes", importBlendShapes);
                importVisibility = EditorGUILayout.Toggle("Import Visibility", importVisibility);
                importCameras = EditorGUILayout.Toggle("Import Cameras", importCameras);
                importLights = EditorGUILayout.Toggle("Import Lights", importLights);
                materialName = (ModelImporterMaterialName)EditorGUILayout.EnumPopup("Material Naming", materialName);
                materialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Material Search", materialSearch);
            }

            GUILayout.Space(10);
            GUILayout.Label("Rig Settings", EditorStyles.boldLabel);

            // Rig Tab Settings
            applyRigSettings = EditorGUILayout.Toggle("Apply Rig Settings", applyRigSettings);
            if (applyRigSettings)
            {
                animationType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup("Animation Type", animationType);
            }

            GUILayout.Space(10);
            GUILayout.Label("Animation Settings", EditorStyles.boldLabel);

            // Animation Tab Settings
            applyAnimationSettings = EditorGUILayout.Toggle("Apply Animation Settings", applyAnimationSettings);
            if (applyAnimationSettings)
            {
                importAnimation = EditorGUILayout.Toggle("Import Animation", importAnimation);
                animationCompression = (ModelImporterAnimationCompression)EditorGUILayout.EnumPopup("Animation Compression", animationCompression);
                bakeAnimations = EditorGUILayout.Toggle("Bake Animations", bakeAnimations);
                resampleCurves = EditorGUILayout.Toggle("Resample Curves", resampleCurves);
                optimizeGameObjects = EditorGUILayout.Toggle("Optimize GameObjects", optimizeGameObjects);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Apply to Selected FBX Files"))
            {
                ApplySettingsToSelectedFBXFiles();
            }
        }

        private void ApplySettingsToSelectedFBXFiles()
        {
            Object[] selectedObjects = Selection.objects;

            foreach (Object obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

                if (modelImporter != null)
                {
                    Undo.RecordObject(modelImporter, "Modify FBX Settings");

                    if (applyModelSettings)
                    {
                        if (applyGlobalScale)
                        {
                            modelImporter.globalScale = globalScale;
                        }

                        modelImporter.meshCompression = meshCompression;
                        modelImporter.isReadable = readWriteEnabled;
                        modelImporter.optimizeMeshPolygons = optimizeMesh;
                        modelImporter.importBlendShapes = importBlendShapes;
                        modelImporter.importVisibility = importVisibility;
                        modelImporter.importCameras = importCameras;
                        modelImporter.importLights = importLights;
                        modelImporter.materialName = materialName;
                        modelImporter.materialSearch = materialSearch;
                    }

                    if (applyRigSettings)
                    {
                        modelImporter.animationType = animationType;
                    }

                    if (applyAnimationSettings)
                    {
                        modelImporter.importAnimation = importAnimation;
                        modelImporter.animationCompression = animationCompression;
                        //modelImporter.bakeSimulation = bakeAnimations;
                        modelImporter.resampleCurves = resampleCurves;
                        modelImporter.optimizeGameObjects = optimizeGameObjects;
                    }

                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    EditorUtility.SetDirty(modelImporter);
                }
            }
        }
    }
}
