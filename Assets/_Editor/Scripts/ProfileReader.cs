using UnityEngine;
using UnityEngine.UI;
using System.Text;
using AkaitoAi.Singleton;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_5
using UnityEngine.Profiling;
#endif

namespace AkaitoAi
{
    public class ProfileReader : SingletonPresistent<ProfileReader>
    {
        StringBuilder SB;
        public Text txt;
        float updateInterval = 1.0f;
        float lastInterval;                     // Last interval end time
        float frames = 0;                       // Frames over current interval
        float framesavtick = 0;
        float framesav = 0.0f;

        // Buffer to store FPS of each frame
        private List<float> fpsBuffer = new List<float>();
        private const int maxBufferCount = 1000; // Buffer size for last 1000 frames

        void Start()
        {
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
            framesav = 0;
            SB = new StringBuilder();
            SB.Capacity = 200;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void OnDisable()
        {
            if (txt)
                DestroyImmediate(txt.gameObject);
        }

        void Update()
        {
            ++frames;

            // Record current FPS using real time between frames
            float currentFps = 1f / Time.unscaledDeltaTime;
            fpsBuffer.Add(currentFps);
            if (fpsBuffer.Count > maxBufferCount)
            {
                fpsBuffer.RemoveAt(0); // Remove oldest to maintain circular buffer
            }

            var timeNow = Time.realtimeSinceStartup;

            if (timeNow > lastInterval + updateInterval)
            {
                if (!txt)
                {
                    GameObject go = new GameObject("FPS Display", typeof(Text));
                    go.hideFlags = HideFlags.HideAndDontSave;
                    go.transform.position = new Vector3(0, 0, 0);
                    txt = go.GetComponent<Text>();
                }

                float fps = frames / (timeNow - lastInterval);
                float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
                ++framesavtick;
                framesav += fps;
                float fpsav = framesav / framesavtick;
                SB.Length = 0;

                // Display Time, Current FPS, AvgFPS first
                SB.AppendFormat("Time : {0} ms \nCurrent FPS: {1}     AvgFPS: {2}", ms, fps, fpsav);

                // Add 1% Low and 0.1% Low FPS immediately below
                if (fpsBuffer.Count > 0)
                {
                    var sortedFps = new List<float>(fpsBuffer);
                    sortedFps.Sort();
                    int total = sortedFps.Count;
                    int onePercentK = Mathf.CeilToInt(0.01f * total);
                    float onePercentLow = (onePercentK > 0 && onePercentK <= total) ? sortedFps[onePercentK - 1] : 0f;
                    int zeroPointOnePercentK = Mathf.CeilToInt(0.001f * total);
                    float zeroPointOnePercentLow = (zeroPointOnePercentK > 0 && zeroPointOnePercentK <= total) ? sortedFps[zeroPointOnePercentK - 1] : 0f;
                    SB.AppendFormat("\n1% Low FPS: {0}\n0.1% Low FPS: {1}\n", onePercentLow, zeroPointOnePercentLow);
                }
                else
                {
                    SB.Append("\n1% Low FPS: N/A\n0.1% Low FPS: N/A\n");
                }

                // Continue with the rest of the system info
                SB.AppendFormat("\nCPU : {0} \nCPU Count : {1}\nCPU Freq : {2}mhz\nQuality : {3}\nOS : {4}\n",
                    SystemInfo.processorType,
                    SystemInfo.processorCount,
                    SystemInfo.processorFrequency,
                    QualitySettings.GetQualityLevel(),
                    SystemInfo.operatingSystem)
                .AppendFormat("\nTotal Memory : {0}mb\nTotalAllocatedMemory : {1}mb\nTotalReservedMemory : {2}mb\nTotalUnusedReservedMemory : {3}mb\n\nGPU : {4}\nTotal GPU Memory : {5}mb",
                    SystemInfo.systemMemorySize,
                    UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576,
                    UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576,
                    UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1048576,
                    SystemInfo.graphicsDeviceName,
                    SystemInfo.graphicsMemorySize
                );

#if UNITY_EDITOR
                SB.AppendFormat("\nDrawCalls : {0}\nUsed Texture Memory : {1}\nRenderedTextureCount : {2}", UnityStats.drawCalls, UnityStats.usedTextureMemorySize / 1048576, UnityStats.usedTextureCount);
#endif

                txt.text = SB.ToString();
                frames = 0;
                lastInterval = timeNow;
            }
        }
    }
}