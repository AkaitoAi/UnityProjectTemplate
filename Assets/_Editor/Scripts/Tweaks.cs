using AkaitoAi.Singleton;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AkaitoAi
{
    public class Tweaks : SingletonPresistent<Tweaks>
    {
        #region Singleton
        //private static Tweaks instance;
        //public static Tweaks Instance { get { if (instance == null) instance = GameObject.FindObjectOfType<Tweaks>(); return instance; } }

        public int SystemMemorySize { get => systemMemorySize; private set => systemMemorySize = value; }
        #endregion

        [SerializeField][Tooltip("Should the game show unity debug logs?")] private bool debugMode = false;
        [SerializeField][Tooltip("Should change the quality settings automatically, based on system memory size, divided in 3 steps.")] private bool autoDetectGraphicSettings = true;
        [SerializeField][Tooltip("-1: Uncapped | 0: Cap to Refreshrate | n: Cap to n")][Range(-1, 240)] private int lockFramerate = 60;
        [SerializeField][Tooltip("-1: Skips | 1: Quality | 0.9: Balanced | 0.75 : Performance, below 0.5 is ignored")][Range(-1, 1)] private float resolutionScale = -1;
        [SerializeField] private int lowMemoryDevice = 3072;

        private int systemMemorySize;

        protected override void Awake()
        {
            base.Awake();

            systemMemorySize = SystemInfo.systemMemorySize;

            //if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
            //else instance = this;

            //DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        // Called everytime scene loads.

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Prevent execution before Awake
            if (!Application.isPlaying)
                return;

            resolutionScale = Mathf.Clamp(resolutionScale, -1f, 1f);
            lockFramerate = Mathf.Clamp(lockFramerate, -1, 240);

            Apply();
        }
#endif

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Apply();

            //Debug.Log("OnSceneLoaded: " + scene.name);
            //Debug.Log(mode);

        } // Calls the functions everytime scene loads

        private void Apply()
        {
            SetFrameRate();
            if (!debugMode) DisableLogging();
            if (resolutionScale != -1) SetResolution(resolutionScale);
            if (autoDetectGraphicSettings) AutoDetectGraphicSettings();
            //DisableVSync();
            DisableScreenTimeout();
        }

        private void DisableVSync()
        {
            if (!Application.isMobilePlatform) return;

            QualitySettings.vSyncCount = 0;
        } // Disables VSync on mobile devices
        private void DisableLogging()
        {
            //Disable Logging on the Device...
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        } // Disables debug calls in build
        private void DisableScreenTimeout() => Screen.sleepTimeout = SleepTimeout.NeverSleep; // Keep device screen awake

        private void SetFrameRate()
        {
            QualitySettings.vSyncCount = 0;

            Application.targetFrameRate = systemMemorySize <= lowMemoryDevice || lockFramerate != 0? 
                lockFramerate : CalculateTargetFrameRate();

            int CalculateTargetFrameRate()
            {
                int refreshRate = Mathf.RoundToInt(
                (float)Screen.currentResolution.refreshRateRatio.value
            );
                return Mathf.Max(60, refreshRate);
            }

        } // Sets max fps with regards to device ram
        public void CollectGarbage() => System.GC.Collect(); // Use this to manually collect garage if incremental GC is disabled
        public void SetResolution(float percentage)
        {
            float scale = Mathf.Clamp(percentage, .5f, 1f);
            ScalableBufferManager.ResizeBuffers(percentage, percentage);

            //Debug.Log( $"Scale: {scale} | " + $"WidthScale: {ScalableBufferManager.widthScaleFactor} | " + 
            //    $"HeightScale: {ScalableBufferManager.heightScaleFactor} | " + $"Screen: {Screen.width}x{Screen.height}");

            //For URP
            //var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            //urpAsset.renderScale = Mathf.Clamp(scale, 0.5f, 1.5f);


            // Wont work on mobile, it is for desktop only
            //percentage = Mathf.Clamp01(percentage);

            //// Get the current screen resolution
            //int baseWidth = Screen.width;
            //int baseHeight = Screen.height;

            //// Calculate the new resolution based on the percentage
            //int newWidth = Mathf.RoundToInt(baseWidth * percentage);
            //int newHeight = Mathf.RoundToInt(baseHeight * percentage);

            //// Set the new screen resolution
            //Screen.SetResolution(newWidth, newHeight, Screen.fullScreen);

            //Debug.Log($"Resolution set to: {newWidth}x{newHeight} ({percentage * 100}%)");
        }
        private void AutoDetectGraphicSettings()
        {
            AutoChangeQualitySetting(SystemMemorySize <= 3072? 0 : 
                SystemMemorySize > 3072 && SystemMemorySize <= 4096? 1 : 2);

            void AutoChangeQualitySetting(int _qualityIndex)
            {
                PlayerPrefs.SetInt("QualitySetting", _qualityIndex);

                QualitySettings.SetQualityLevel(_qualityIndex, true);
                QualitySettings.GetQualityLevel();
            }
        }

        //? Add in funtion to know from where the function us being called
        //print((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name); 

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
