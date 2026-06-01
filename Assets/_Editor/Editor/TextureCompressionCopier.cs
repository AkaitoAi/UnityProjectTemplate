#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureCompressionCopier : EditorWindow
{
    private enum CopyDirection
    {
        AndroidToiOS,
        iOSToAndroid,
        AndroidToStandalone,
        StandaloneToAndroid,
        iOSToStandalone,
        StandaloneToiOS
    }

    private CopyDirection copyDirection;
    private bool applyToEntireProject;
    private bool dryRunMode;
    private bool forceOverrideIfMissing;
    private bool filterByTextureType;
    private TextureImporterType textureTypeFilter;
    private bool includeNormalMaps;
    private bool includeLightmaps;

    private const string PrefPrefix = "TextureCompressionCopier_";

    [MenuItem("AkaitoAi/Tools/Texture Compression Copier Pro")]
    public static void ShowWindow()
    {
        GetWindow<TextureCompressionCopier>("Texture Compression Copier Pro");
    }

    private void OnEnable()
    {
        copyDirection = (CopyDirection)EditorPrefs.GetInt(PrefPrefix + "CopyDirection", 0);
        applyToEntireProject = EditorPrefs.GetBool(PrefPrefix + "ApplyToEntireProject", false);
        dryRunMode = EditorPrefs.GetBool(PrefPrefix + "DryRunMode", false);
        forceOverrideIfMissing = EditorPrefs.GetBool(PrefPrefix + "ForceOverride", false);
        filterByTextureType = EditorPrefs.GetBool(PrefPrefix + "FilterByType", false);
        textureTypeFilter = (TextureImporterType)EditorPrefs.GetInt(PrefPrefix + "TextureType", 0);
        includeNormalMaps = EditorPrefs.GetBool(PrefPrefix + "IncludeNormalMaps", true);
        includeLightmaps = EditorPrefs.GetBool(PrefPrefix + "IncludeLightmaps", false);
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt(PrefPrefix + "CopyDirection", (int)copyDirection);
        EditorPrefs.SetBool(PrefPrefix + "ApplyToEntireProject", applyToEntireProject);
        EditorPrefs.SetBool(PrefPrefix + "DryRunMode", dryRunMode);
        EditorPrefs.SetBool(PrefPrefix + "ForceOverride", forceOverrideIfMissing);
        EditorPrefs.SetBool(PrefPrefix + "FilterByType", filterByTextureType);
        EditorPrefs.SetInt(PrefPrefix + "TextureType", (int)textureTypeFilter);
        EditorPrefs.SetBool(PrefPrefix + "IncludeNormalMaps", includeNormalMaps);
        EditorPrefs.SetBool(PrefPrefix + "IncludeLightmaps", includeLightmaps);
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Texture Compression Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        copyDirection = (CopyDirection)EditorGUILayout.EnumPopup("Copy Direction", copyDirection);
        applyToEntireProject = EditorGUILayout.Toggle("Apply to Entire Project", applyToEntireProject);
        dryRunMode = EditorGUILayout.Toggle("Dry Run (Preview Only)", dryRunMode);
        forceOverrideIfMissing = EditorGUILayout.Toggle("Force Override if Missing", forceOverrideIfMissing);

        filterByTextureType = EditorGUILayout.Toggle("Filter by Texture Type", filterByTextureType);
        if (filterByTextureType)
        {
            textureTypeFilter = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", textureTypeFilter);
        }

        includeNormalMaps = EditorGUILayout.Toggle("Include Normal Maps", includeNormalMaps);
        includeLightmaps = EditorGUILayout.Toggle("Include Lightmaps", includeLightmaps);

        if (GUILayout.Button("Copy Compression Settings"))
        {
            CopyCompressionSettings();
        }

        EditorGUILayout.HelpBox(
            "Copies compression settings from one platform to another. Skips uncompressed textures.\n" +
            "Optional dry run, type filtering, and normal/lightmap inclusion. Settings are saved between sessions.",
            MessageType.Info);
    }

    private void CopyCompressionSettings()
    {
        string[] guids = applyToEntireProject ? AssetDatabase.FindAssets("t:Texture") : Selection.assetGUIDs;

        string sourcePlatform = GetSourcePlatform();
        string targetPlatform = GetTargetPlatform();

        int updatedCount = 0;
        int skippedCount = 0;
        string reportPath = Path.Combine(Application.dataPath, "../CompressionReport.txt");
        using StreamWriter report = new StreamWriter(reportPath);

        AssetDatabase.StartAssetEditing();

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar("Copying Texture Compression", path, i / (float)guids.Length);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                    continue;

                if (!includeNormalMaps && importer.textureType == TextureImporterType.NormalMap)
                {
                    skippedCount++;
                    continue;
                }

                if (!includeLightmaps && importer.textureType == TextureImporterType.Lightmap)
                {
                    skippedCount++;
                    continue;
                }

                if (filterByTextureType && importer.textureType != textureTypeFilter)
                {
                    skippedCount++;
                    continue;
                }

                TextureImporterPlatformSettings source = importer.GetPlatformTextureSettings(sourcePlatform);

                if (!source.overridden && !forceOverrideIfMissing)
                {
                    skippedCount++;
                    continue;
                }

                if (IsUncompressedFormat(source.format))
                {
                    skippedCount++;
                    continue;
                }

                if (dryRunMode)
                {
                    report.WriteLine($"[Dry Run] Would copy from {sourcePlatform} to {targetPlatform}: {path}");
                    continue;
                }

                TextureImporterPlatformSettings target = new TextureImporterPlatformSettings
                {
                    name = targetPlatform,
                    overridden = true,
                    format = source.format,
                    maxTextureSize = source.maxTextureSize,
                    compressionQuality = source.compressionQuality,
                    resizeAlgorithm = source.resizeAlgorithm,
                    textureCompression = source.textureCompression,
                    crunchedCompression = source.crunchedCompression,
                    allowsAlphaSplitting = source.allowsAlphaSplitting
                };

                importer.SetPlatformTextureSettings(target);
                AssetDatabase.ImportAsset(path);
                updatedCount++;

                report.WriteLine($"Copied from {sourcePlatform} to {targetPlatform}: {path}");
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        report.WriteLine($"\nSummary: {updatedCount} updated, {skippedCount} skipped.");
        report.Close();

        EditorUtility.DisplayDialog("Texture Compression Copier Pro",
            $"Copied compression settings for {updatedCount} texture(s).\nSkipped {skippedCount} texture(s).\nReport saved to: CompressionReport.txt",
            "OK");
    }

    private string GetSourcePlatform()
    {
        return copyDirection switch
        {
            CopyDirection.AndroidToiOS => "Android",
            CopyDirection.AndroidToStandalone => "Android",
            CopyDirection.iOSToAndroid => "iOS",
            CopyDirection.iOSToStandalone => "iOS",
            CopyDirection.StandaloneToAndroid => "Standalone",
            CopyDirection.StandaloneToiOS => "Standalone",
            _ => "Android"
        };
    }

    private string GetTargetPlatform()
    {
        return copyDirection switch
        {
            CopyDirection.AndroidToiOS => "iOS",
            CopyDirection.AndroidToStandalone => "Standalone",
            CopyDirection.iOSToAndroid => "Android",
            CopyDirection.iOSToStandalone => "Standalone",
            CopyDirection.StandaloneToAndroid => "Android",
            CopyDirection.StandaloneToiOS => "iOS",
            _ => "iOS"
        };
    }

    private static bool IsUncompressedFormat(TextureImporterFormat format)
    {
        return format == TextureImporterFormat.RGBA32 ||
               format == TextureImporterFormat.RGB24 ||
               format == TextureImporterFormat.RGBA16 ||
               format == TextureImporterFormat.RGBAHalf ||
               format == TextureImporterFormat.RGBAFloat ||
               format == TextureImporterFormat.R8;
    }
}
#endif
