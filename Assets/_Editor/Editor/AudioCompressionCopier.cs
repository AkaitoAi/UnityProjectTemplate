#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class AudioCompressionCopier : EditorWindow
{
    private enum AudioCopyDirection
    {
        AndroidToiOS,
        iOSToAndroid,
        AndroidToStandalone,
        StandaloneToAndroid,
        iOSToStandalone,
        StandaloneToiOS
    }

    private AudioCopyDirection copyDirection;
    private bool applyToEntireProject;
    private bool dryRunMode;
    private bool forceOverrideIfMissing;
    private bool useDefaultIfMissing;
    private bool filterByLoadType;
    private AudioClipLoadType loadTypeFilter;

    private const string PrefPrefix = "AudioCompressionCopier_";

    [MenuItem("AkaitoAi/Tools/Audio Compression Copier Pro")]
    public static void ShowWindow()
    {
        GetWindow<AudioCompressionCopier>("Audio Compression Copier Pro");
    }

    private void OnEnable()
    {
        copyDirection = (AudioCopyDirection)EditorPrefs.GetInt(PrefPrefix + "CopyDirection", 0);
        applyToEntireProject = EditorPrefs.GetBool(PrefPrefix + "ApplyToEntireProject", false);
        dryRunMode = EditorPrefs.GetBool(PrefPrefix + "DryRunMode", false);
        forceOverrideIfMissing = EditorPrefs.GetBool(PrefPrefix + "ForceOverride", false);
        useDefaultIfMissing = EditorPrefs.GetBool(PrefPrefix + "UseDefaultIfMissing", true);
        filterByLoadType = EditorPrefs.GetBool(PrefPrefix + "FilterByLoadType", false);
        loadTypeFilter = (AudioClipLoadType)EditorPrefs.GetInt(PrefPrefix + "LoadTypeFilter", 0);
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt(PrefPrefix + "CopyDirection", (int)copyDirection);
        EditorPrefs.SetBool(PrefPrefix + "ApplyToEntireProject", applyToEntireProject);
        EditorPrefs.SetBool(PrefPrefix + "DryRunMode", dryRunMode);
        EditorPrefs.SetBool(PrefPrefix + "ForceOverride", forceOverrideIfMissing);
        EditorPrefs.SetBool(PrefPrefix + "UseDefaultIfMissing", useDefaultIfMissing);
        EditorPrefs.SetBool(PrefPrefix + "FilterByLoadType", filterByLoadType);
        EditorPrefs.SetInt(PrefPrefix + "LoadTypeFilter", (int)loadTypeFilter);
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Audio Compression Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        copyDirection = (AudioCopyDirection)EditorGUILayout.EnumPopup("Copy Direction", copyDirection);
        applyToEntireProject = EditorGUILayout.Toggle("Apply to Entire Project", applyToEntireProject);
        dryRunMode = EditorGUILayout.Toggle("Dry Run (Preview Only)", dryRunMode);
        forceOverrideIfMissing = EditorGUILayout.Toggle("Force Override if Missing", forceOverrideIfMissing);
        useDefaultIfMissing = EditorGUILayout.Toggle("Use Default Settings If Not Overridden", useDefaultIfMissing);

        filterByLoadType = EditorGUILayout.Toggle("Filter by Load Type", filterByLoadType);
        if (filterByLoadType)
        {
            loadTypeFilter = (AudioClipLoadType)EditorGUILayout.EnumPopup("Load Type Filter", loadTypeFilter);
        }

        if (GUILayout.Button("Copy Compression Settings"))
        {
            CopyCompressionSettings();
        }

        EditorGUILayout.HelpBox(
            "Copies audio import compression settings from one platform to another.\nSupports dry run, load type filtering, and default fallback. Settings persist.",
            MessageType.Info);
    }

    private void CopyCompressionSettings()
    {
        string[] guids = applyToEntireProject ? AssetDatabase.FindAssets("t:AudioClip") : Selection.assetGUIDs;

        string sourcePlatform = GetSourcePlatform();
        string targetPlatform = GetTargetPlatform();

        int updatedCount = 0;
        int skippedCount = 0;
        string reportPath = Path.Combine(Application.dataPath, "../AudioCompressionReport.txt");
        using StreamWriter report = new StreamWriter(reportPath);

        AssetDatabase.StartAssetEditing();

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar("Copying Audio Compression", path, i / (float)guids.Length);

                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                if (importer == null)
                    continue;

                AudioImporterSampleSettings sourceSettings = importer.GetOverrideSampleSettings(sourcePlatform);
                bool isOverridden = importer.GetOverrideSampleSettings(sourcePlatform).compressionFormat != AudioCompressionFormat.PCM;

                if (!isOverridden)
                {
                    if (useDefaultIfMissing)
                    {
                        sourceSettings = importer.defaultSampleSettings;
                    }
                    else if (!forceOverrideIfMissing)
                    {
                        skippedCount++;
                        continue;
                    }
                }

                if (filterByLoadType && sourceSettings.loadType != loadTypeFilter)
                {
                    skippedCount++;
                    continue;
                }

                if (dryRunMode)
                {
                    report.WriteLine($"[Dry Run] Would copy from {sourcePlatform} to {targetPlatform}: {path}");
                    continue;
                }

                importer.SetOverrideSampleSettings(targetPlatform, sourceSettings);
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

        EditorUtility.DisplayDialog("Audio Compression Copier Pro",
            $"Copied audio settings for {updatedCount} clip(s).\nSkipped {skippedCount} clip(s).\nReport saved to: AudioCompressionReport.txt",
            "OK");
    }

    private string GetSourcePlatform()
    {
        return copyDirection switch
        {
            AudioCopyDirection.AndroidToiOS => "Android",
            AudioCopyDirection.AndroidToStandalone => "Android",
            AudioCopyDirection.iOSToAndroid => "iOS",
            AudioCopyDirection.iOSToStandalone => "iOS",
            AudioCopyDirection.StandaloneToAndroid => "Standalone",
            AudioCopyDirection.StandaloneToiOS => "Standalone",
            _ => "Android"
        };
    }

    private string GetTargetPlatform()
    {
        return copyDirection switch
        {
            AudioCopyDirection.AndroidToiOS => "iOS",
            AudioCopyDirection.AndroidToStandalone => "Standalone",
            AudioCopyDirection.iOSToAndroid => "Android",
            AudioCopyDirection.iOSToStandalone => "Standalone",
            AudioCopyDirection.StandaloneToAndroid => "Android",
            AudioCopyDirection.StandaloneToiOS => "iOS",
            _ => "iOS"
        };
    }
}
#endif
