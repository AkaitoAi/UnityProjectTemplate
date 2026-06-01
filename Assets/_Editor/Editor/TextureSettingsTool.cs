using UnityEngine;
using UnityEditor;

namespace AkaitoAi
{
    public class TextureSettingsTool : EditorWindow
    {
        // Texture settings variables
        private bool applyTextureType = true;
        private TextureImporterType textureType = TextureImporterType.Default;

        private bool applyTextureShape = true;
        private TextureImporterShape textureShape = TextureImporterShape.Texture2D;

        // Platform settings
        private string[] platforms = { "Android", "iOS", "Standalone" };
        private int selectedPlatformIndex = 0;

        // Android settings
        private bool overrideForAndroid = true;
        private TextureImporterFormat androidFormat = TextureImporterFormat.ASTC_12x12;
        private int androidCompressionQuality = 100;
        private int androidMaxSizeIndex = 5; // Default 1024

        // iOS settings
        private bool overrideForiOS = true;
        private TextureImporterFormat iosFormat = TextureImporterFormat.ASTC_4x4;
        private int iosCompressionQuality = 50;
        private int iosMaxSizeIndex = 5; // Default 1024

        // Standalone settings
        private bool overrideForStandalone = true;
        private TextureImporterFormat standaloneFormat = TextureImporterFormat.DXT5;
        private int standaloneCompressionQuality = 50;
        private int standaloneMaxSizeIndex = 5; // Default 1024

        private int[] maxSizeOptions = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };

        private bool applyReadWrite = true;
        private bool readWriteEnabled = false;

        private bool applyMipmap = true;
        private bool mipMapEnabled = true;

        private bool applyWrapMode = true;
        private TextureWrapMode wrapMode = TextureWrapMode.Repeat;

        private bool applyAnisoLevel = true;
        private int anisoLevel = 8;

        [MenuItem("AkaitoAi/Tools/Texture Settings")]
        public static void ShowWindow()
        {
            GetWindow<TextureSettingsTool>("Texture Settings Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Set Texture Settings", EditorStyles.boldLabel);

            // Texture Type
            applyTextureType = EditorGUILayout.Toggle("Apply Texture Type", applyTextureType);
            if (applyTextureType)
            {
                textureType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", textureType);
            }

            // Texture Shape
            applyTextureShape = EditorGUILayout.Toggle("Apply Texture Shape", applyTextureShape);
            if (applyTextureShape)
            {
                textureShape = (TextureImporterShape)EditorGUILayout.EnumPopup("Texture Shape", textureShape);
            }

            // Platform Selection
            GUILayout.Label("Platform Specific Settings", EditorStyles.boldLabel);
            selectedPlatformIndex = EditorGUILayout.Popup("Select Platform", selectedPlatformIndex, platforms);

            // Show platform-specific settings based on selected platform
            switch (selectedPlatformIndex)
            {
                case 0: // Android
                    overrideForAndroid = EditorGUILayout.Toggle("Override for Android", overrideForAndroid);
                    androidFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android Format", androidFormat);
                    androidCompressionQuality = EditorGUILayout.IntSlider("Compression Quality", androidCompressionQuality, 0, 100);
                    androidMaxSizeIndex = EditorGUILayout.Popup("Max Size", androidMaxSizeIndex, maxSizeOptionsToStringArray());
                    break;

                case 1: // iOS
                    overrideForiOS = EditorGUILayout.Toggle("Override for iOS", overrideForiOS);
                    iosFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("iOS Format", iosFormat);
                    iosCompressionQuality = EditorGUILayout.IntSlider("Compression Quality", iosCompressionQuality, 0, 100);
                    iosMaxSizeIndex = EditorGUILayout.Popup("Max Size", iosMaxSizeIndex, maxSizeOptionsToStringArray());
                    break;

                case 2: // Standalone
                    overrideForStandalone = EditorGUILayout.Toggle("Override for Standalone", overrideForStandalone);
                    standaloneFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Standalone Format", standaloneFormat);
                    standaloneCompressionQuality = EditorGUILayout.IntSlider("Compression Quality", standaloneCompressionQuality, 0, 100);
                    standaloneMaxSizeIndex = EditorGUILayout.Popup("Max Size", standaloneMaxSizeIndex, maxSizeOptionsToStringArray());
                    break;
            }

            // Read/Write Enabled
            applyReadWrite = EditorGUILayout.Toggle("Apply Read/Write", applyReadWrite);
            if (applyReadWrite)
            {
                readWriteEnabled = EditorGUILayout.Toggle("Read/Write Enabled", readWriteEnabled);
            }

            // MipMap Enabled
            applyMipmap = EditorGUILayout.Toggle("Apply MipMap", applyMipmap);
            if (applyMipmap)
            {
                mipMapEnabled = EditorGUILayout.Toggle("MipMap Enabled", mipMapEnabled);
            }

            // Wrap Mode
            applyWrapMode = EditorGUILayout.Toggle("Apply Wrap Mode", applyWrapMode);
            if (applyWrapMode)
            {
                wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Wrap Mode", wrapMode);
            }

            // Aniso Level
            applyAnisoLevel = EditorGUILayout.Toggle("Apply Aniso Level", applyAnisoLevel);
            if (applyAnisoLevel)
            {
                anisoLevel = EditorGUILayout.IntSlider("Aniso Level", anisoLevel, 1, 16);
            }

            // Apply Button
            if (GUILayout.Button("Apply to Selected Textures"))
            {
                ApplyToSelectedTextures();
            }
        }

        private void ApplyToSelectedTextures()
        {
            Object[] selectedObjects = Selection.objects;

            foreach (Object obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                if (textureImporter != null)
                {
                    Undo.RecordObject(textureImporter, "Modify Texture Settings");

                    if (applyTextureType)
                    {
                        textureImporter.textureType = textureType;
                    }

                    if (applyTextureShape)
                    {
                        textureImporter.textureShape = textureShape;
                    }

                    // Apply platform-specific settings based on selected platform
                    switch (selectedPlatformIndex)
                    {
                        case 0: // Android
                            ApplyPlatformSettings(textureImporter, "Android", overrideForAndroid, androidFormat, androidCompressionQuality, maxSizeOptions[androidMaxSizeIndex]);
                            break;

                        case 1: // iOS
                            ApplyPlatformSettings(textureImporter, "iPhone", overrideForiOS, iosFormat, iosCompressionQuality, maxSizeOptions[iosMaxSizeIndex]);
                            break;

                        case 2: // Standalone
                            ApplyPlatformSettings(textureImporter, "Standalone", overrideForStandalone, standaloneFormat, standaloneCompressionQuality, maxSizeOptions[standaloneMaxSizeIndex]);
                            break;
                    }

                    if (applyReadWrite)
                    {
                        textureImporter.isReadable = readWriteEnabled;
                    }

                    if (applyMipmap)
                    {
                        textureImporter.mipmapEnabled = mipMapEnabled;
                    }

                    if (applyWrapMode)
                    {
                        textureImporter.wrapMode = wrapMode;
                    }

                    if (applyAnisoLevel)
                    {
                        textureImporter.anisoLevel = anisoLevel;
                    }

                    // Apply changes and reimport
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    EditorUtility.SetDirty(textureImporter);
                }
            }
        }

        private void ApplyPlatformSettings(TextureImporter importer, string platform, bool overrideForPlatform, TextureImporterFormat format, int quality, int maxSize)
        {
            var platformSettings = importer.GetPlatformTextureSettings(platform);
            platformSettings.overridden = overrideForPlatform;
            platformSettings.format = format;
            platformSettings.compressionQuality = quality;
            platformSettings.maxTextureSize = maxSize;
            importer.SetPlatformTextureSettings(platformSettings);
        }

        private string[] maxSizeOptionsToStringArray()
        {
            string[] maxSizeStrings = new string[maxSizeOptions.Length];
            for (int i = 0; i < maxSizeOptions.Length; i++)
            {
                maxSizeStrings[i] = maxSizeOptions[i].ToString();
            }
            return maxSizeStrings;
        }
    }
}
