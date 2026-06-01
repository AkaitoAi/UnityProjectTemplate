using AkaitoAi.Advertisement;
using AkaitoAi.Extensions;
using AkaitoAi.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ScenesToSwapBetween
{
    public static readonly string[] All = { "Day", "Night" };
}

public struct OnLoadScene : IEvent { public SceneLoadCommand command; }
public struct OnSwapScene : IEvent { public SceneLoadCommand command; }

[Serializable]
public class SceneLoadCommand
{
    public int[] sceneIndices;
    public string[] sceneNames;

    public bool IsByName => sceneNames != null && sceneNames.Length > 0;

    public SceneLoadCommand(int[] indices)
    {
        sceneIndices = indices;
        sceneNames = null;
    }

    public SceneLoadCommand(string[] names)
    {
        sceneNames = names;
        sceneIndices = null;
    }

    public string[] GetSceneNames() => IsByName ? sceneNames : null;
    public int[] GetSceneIndexes() => IsByName ? null : sceneIndices;
}

public class SceneLoaderFacade
{
    private readonly Loading loader;
    private readonly LoadingStrategyFactory strategyFactory;

    public SceneLoaderFacade(Loading loader, float duration, StaticLoadingProgression progression)
    {
        this.loader = loader;
        strategyFactory = new LoadingStrategyFactory(duration, progression);
    }

    public void LoadScenes(SceneLoadCommand command, SceneLoadingType type)
    {
        ILoadingStrategy strategy = strategyFactory.CreateStrategy(type);

        if (command.IsByName)
            loader.SetSceneNames(command.sceneNames);
        else
            loader.SetSceneIndices(command.sceneIndices);

        loader.StartLoadingWithStrategy(strategy);
    }

    public void UnloadScenes(SceneLoadCommand command)
    {
        if (command.IsByName)
            loader.StartCoroutine(loader.UnloadScenesCoroutine(command.sceneNames, null));
        else
            loader.StartCoroutine(loader.UnloadScenesCoroutine(null, command.sceneIndices));
    }

    public void SwapScenes(SceneLoadCommand unloadCommand, SceneLoadCommand loadCommand, SceneLoadingType type)
    {
        ILoadingStrategy strategy = type switch
        {
            SceneLoadingType.Hybrid => new HybridSwapLoadingStrategy(5f, StaticLoadingProgression.Duration,
                unloadCommand?.sceneNames != null && unloadCommand.sceneNames.Length > 0
                    ? unloadCommand.sceneNames[0]
                    : SceneManager.GetSceneByBuildIndex(unloadCommand.sceneIndices[0]).name),
            _ => strategyFactory.CreateStrategy(type)
        };

        if (loadCommand.IsByName)
            loader.SetSceneNames(loadCommand.sceneNames);
        else
            loader.SetSceneIndices(loadCommand.sceneIndices);

        loader.StartLoadingWithStrategy(strategy);
    }


    private IEnumerator SwapScenesCoroutine(SceneLoadCommand unloadCommand, SceneLoadCommand loadCommand, SceneLoadingType type)
    {
        string sceneToUnload = null;
        if (unloadCommand?.sceneNames != null && unloadCommand.sceneNames.Length > 0)
            sceneToUnload = unloadCommand.sceneNames[0];
        else if (unloadCommand?.sceneIndices != null && unloadCommand.sceneIndices.Length > 0)
            sceneToUnload = SceneManager.GetSceneByBuildIndex(unloadCommand.sceneIndices[0]).name;

        ILoadingStrategy strategy = type switch
        {
            SceneLoadingType.Hybrid => new HybridSwapLoadingStrategy(5f, StaticLoadingProgression.Duration, sceneToUnload),
            _ => strategyFactory.CreateStrategy(type)
        };

        if (loadCommand.IsByName)
            loader.SetSceneNames(loadCommand.sceneNames);
        else
            loader.SetSceneIndices(loadCommand.sceneIndices);

        yield return loader.StartCoroutine(strategy.LoadScenes(loader.sceneIndices, loader.sceneNames, loader));
    }
}

public enum SceneLoadingType { Static, Dynamic, Hybrid }
public enum StaticLoadingProgression { Speed, Duration }

public interface ILoadingStrategy
{
    IEnumerator LoadScenes(int[] sceneIndices, string[] sceneNames, Loading loading);
}

public class StaticLoadingStrategy : ILoadingStrategy
{
    private readonly float duration;
    private readonly StaticLoadingProgression progression;

    public StaticLoadingStrategy(float duration, StaticLoadingProgression progression)
    {
        this.duration = duration;
        this.progression = progression;
    }

    public IEnumerator LoadScenes(int[] sceneIndices, string[] sceneNames, Loading loading)
    {
        SceneLoadTracker.Clear();

        float time = 0f;
        float progress = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            progress = progression switch
            {
                StaticLoadingProgression.Duration => Mathf.SmoothStep(0f, 1f, t),
                StaticLoadingProgression.Speed => t,
                _ => t
            };

            loading.UpdateProgressUI(progress);
            yield return null;
        }

        loading.UpdateProgressUI(1f);

        int count = sceneNames?.Length > 0 ? sceneNames.Length : sceneIndices.Length;
        for (int i = 0; i < count; i++)
        {
            if (sceneNames != null && sceneNames.Length > 0)
            {
                SceneManager.LoadScene(sceneNames[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                SceneLoadTracker.RegisterScene(sceneNames[i], -1);
            }
            else
            {
                SceneManager.LoadScene(sceneIndices[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                SceneLoadTracker.RegisterScene(null, sceneIndices[i]);
            }
        }
    }
}

public class DynamicLoadingStrategy : ILoadingStrategy
{
    public IEnumerator LoadScenes(int[] sceneIndices, string[] sceneNames, Loading loading)
    {
        SceneLoadTracker.Clear();

        int count = sceneNames?.Length > 0 ? sceneNames.Length : sceneIndices.Length;
        AsyncOperation[] operations = new AsyncOperation[count];

        for (int i = 0; i < count; i++)
        {
            if (sceneNames != null && sceneNames.Length > 0)
            {
                operations[i] = SceneManager.LoadSceneAsync(sceneNames[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                operations[i].completed += _ => SceneLoadTracker.RegisterScene(sceneNames[i], -1);
            }
            else
            {
                int index = sceneIndices[i];
                operations[i] = SceneManager.LoadSceneAsync(index, i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                operations[i].completed += _ => SceneLoadTracker.RegisterScene(null, index);
            }
        }

        while (true)
        {
            float totalProgress = 0f;
            bool allDone = true;

            foreach (var op in operations)
            {
                totalProgress += Mathf.Clamp01(op.progress / 0.9f);
                if (!op.isDone)
                    allDone = false;
            }

            float averageProgress = totalProgress / count;
            loading.UpdateProgressUI(averageProgress);

            if (allDone) break;

            yield return null;
        }

        loading.UpdateProgressUI(1f);
    }
}

public class HybridLoadingStrategy : ILoadingStrategy
{
    private readonly float duration;
    private readonly StaticLoadingProgression progression;

    public HybridLoadingStrategy(float duration, StaticLoadingProgression progression)
    {
        this.duration = duration;
        this.progression = progression;
    }

    public IEnumerator LoadScenes(int[] sceneIndices, string[] sceneNames, Loading loading)
    {
        SceneLoadTracker.Clear();

        int count = sceneNames?.Length > 0 ? sceneNames.Length : sceneIndices.Length;
        AsyncOperation[] operations = new AsyncOperation[count];

        for (int i = 0; i < count; i++)
        {
            if (sceneNames != null && sceneNames.Length > 0)
            {
                operations[i] = SceneManager.LoadSceneAsync(sceneNames[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                operations[i].allowSceneActivation = false;
                operations[i].completed += _ => SceneLoadTracker.RegisterScene(sceneNames[i], -1);
            }
            else
            {
                int index = sceneIndices[i];
                operations[i] = SceneManager.LoadSceneAsync(index, i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                operations[i].allowSceneActivation = false;
                operations[i].completed += _ => SceneLoadTracker.RegisterScene(null, index);
            }

            operations[i].allowSceneActivation = false;
        }

        float time = 0f;
        float progress = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            progress = progression switch
            {
                StaticLoadingProgression.Duration => Mathf.SmoothStep(0f, 1f, t),
                StaticLoadingProgression.Speed => t,
                _ => t
            };

            loading.UpdateProgressUI(progress);
            yield return null;
        }

        loading.UpdateProgressUI(1f);

        foreach (var op in operations)
            op.allowSceneActivation = true;

        bool allDone;
        do
        {
            allDone = true;
            foreach (var op in operations)
            {
                if (!op.isDone) allDone = false;
            }
            yield return null;
        } while (!allDone);

        Time.timeScale = 1f;
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
}

public class HybridSwapLoadingStrategy : ILoadingStrategy
{
    private readonly float duration;
    private readonly StaticLoadingProgression progression;
    private readonly string sceneToUnload;

    public HybridSwapLoadingStrategy(float duration, StaticLoadingProgression progression, string sceneToUnload)
    {
        this.duration = duration;
        this.progression = progression;
        this.sceneToUnload = sceneToUnload;
    }

    public IEnumerator LoadScenes(int[] sceneIndices, string[] sceneNames, Loading loading)
    {
        Scene oldScene = SceneManager.GetSceneByName(sceneToUnload);
        bool shouldUnload = !string.IsNullOrEmpty(sceneToUnload) && oldScene.IsValid() && oldScene.isLoaded;

        int count = sceneNames?.Length > 0 ? sceneNames.Length : sceneIndices.Length;
        AsyncOperation[] loadOps = new AsyncOperation[count];

        for (int i = 0; i < count; i++)
        {
            if (sceneNames != null && sceneNames.Length > 0)
            {
                string sceneNameCopy = sceneNames[i];
                loadOps[i] = SceneManager.LoadSceneAsync(sceneNameCopy, LoadSceneMode.Additive);
                loadOps[i].completed += _ => SceneLoadTracker.RegisterScene(sceneNameCopy, -1);
            }
            else
            {
                int indexCopy = sceneIndices[i];
                loadOps[i] = SceneManager.LoadSceneAsync(indexCopy, LoadSceneMode.Additive);
                loadOps[i].completed += _ => SceneLoadTracker.RegisterScene(null, indexCopy);
            }

            loadOps[i].allowSceneActivation = false;
        }

        Time.timeScale = 0;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            float progress = progression switch
            {
                StaticLoadingProgression.Duration => Mathf.SmoothStep(0f, 1f, t),
                StaticLoadingProgression.Speed => t,
                _ => t
            };

            loading.UpdateProgressUI(progress);
            yield return null;
        }

        loading.UpdateProgressUI(1f);

        foreach (var op in loadOps)
        {
            if (op != null)
                op.allowSceneActivation = true;
        }

        bool allLoaded;
        do
        {
            allLoaded = true;
            foreach (var op in loadOps)
            {
                if (op != null && !op.isDone)
                {
                    allLoaded = false;
                    break;
                }
            }
            yield return null;
        } while (!allLoaded);

        Scene newActiveScene = sceneNames != null && sceneNames.Length > 0
            ? SceneManager.GetSceneByName(sceneNames[0])
            : SceneManager.GetSceneByBuildIndex(sceneIndices[0]);

        if (newActiveScene.IsValid())
            SceneManager.SetActiveScene(newActiveScene);

        if (shouldUnload)
        {
            Debug.Log($"Unloading old environment: {sceneToUnload}");
            var unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);

            while (!unloadOp.isDone)
                yield return null;

            SceneLoadTracker.Remove(sceneToUnload);
        }

        Time.timeScale = 1f;
        Physics.simulationMode = SimulationMode.FixedUpdate;

    }

}

public class LoadingStrategyFactory
{
    private readonly float duration;
    private readonly StaticLoadingProgression progression;

    public LoadingStrategyFactory(float duration, StaticLoadingProgression progression)
    {
        this.duration = duration;
        this.progression = progression;
    }

    public ILoadingStrategy CreateStrategy(SceneLoadingType type)
    {
        return type switch
        {
            SceneLoadingType.Static => new StaticLoadingStrategy(duration, progression),
            SceneLoadingType.Dynamic => new DynamicLoadingStrategy(),
            SceneLoadingType.Hybrid => new HybridLoadingStrategy(duration, progression),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public class Loading : SingletonPresistent<Loading> /*MonoBehaviour*/
{
    [Header("Scene Loading Settings")]
    [SerializeField] private SceneLoadCommand command;
    [SerializeField] private SceneLoadingType loadingType = SceneLoadingType.Hybrid;
    internal int[] sceneIndices;
    internal string[] sceneNames;

    [Header("Progress UI")]
    public GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillerImage;
    [SerializeField] private Text progressText;
    [SerializeField] private string loadingPercentPostfix = "%";

    [Header("Tips")]
    [SerializeField] private Text tipsText;
    [SerializeField] private string[] tips;
    [SerializeField] private string tipPrefix = "Tip: ";
    [SerializeField] private string prefsName = "TipShown";
    private int switchIndex = 0;

    [Header("Loading Text")]
    [SerializeField] private Text loadingText;
    [SerializeField] private string[] loadingString;

    [Header("Static Loading Config")]
    [SerializeField] private StaticLoadingProgression staticProgression = StaticLoadingProgression.Duration;
    [SerializeField] private float loadingDuration = 5f;

    private SetupSO setupScriptable;
    public UnityEvent OnStartEvent;
    private bool isLoading;

    private void Start()
    {
        setupScriptable = RuntimeSetup.GetInstance().Setup;
        OnStartEvent?.Invoke();
    }

    public void ShowLoadingScreen()
    {
        if (loadingScreen != null && !loadingScreen.activeSelf)
            loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        if (loadingScreen != null && loadingScreen.activeSelf)
            loadingScreen.SetActive(false);
    }

    public void Load()
    {
        if (isLoading) return;

        if (command.IsByName)
            SetSceneNames(command.sceneNames);
        else
            SetSceneIndices(command.sceneIndices);

        LoadingStrategyFactory factory = new LoadingStrategyFactory(loadingDuration, staticProgression);
        ILoadingStrategy strategy = factory.CreateStrategy(loadingType);

        StartThisLoadingWithStrategy(strategy);

        void StartThisLoadingWithStrategy(ILoadingStrategy strategy)
        {
            if (isLoading) return;
            isLoading = true;

            ShowLoadingScreen();
            StartCoroutine(LoadingRoutine(strategy));

            Time.timeScale = 0f;
            Physics.simulationMode = SimulationMode.Script;

            InvokeRepeating(nameof(ShowLoadingText), 0f, 2.5f);

        }
    }

    public void StartLoadingWithStrategy(ILoadingStrategy strategy)
    {
        if (isLoading) return;
        isLoading = true;
        
        ShowLoadingScreen();
        StartCoroutine(LoadingRoutine(strategy));

        Time.timeScale = 0f;
        Physics.simulationMode = SimulationMode.Script;

        InvokeRepeating(nameof(ShowLoadingText), 0f, 2.5f);
        //StartCoroutine(strategy.LoadScenes(sceneIndices, sceneNames, this));
        StartCoroutine(AkaitoAiExtensions.SimpleRealtimeDelay(2.5f, () =>
            AdsWrapper.GetInstance()?.ShowInterstitial()));
    }
    private IEnumerator LoadingRoutine(ILoadingStrategy strategy)
    {
        yield return StartCoroutine(strategy.LoadScenes(sceneIndices, sceneNames, this));

        yield return null;

        HideLoadingScreen();
        isLoading = false;
    }
    public void SetSceneIndices(int[] indices) { sceneIndices = indices; sceneNames = null; }
    public void SetSceneNames(string[] names) { sceneNames = names; sceneIndices = null; }

    public void UpdateProgressUI(float progress)
    {
        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + loadingPercentPostfix;
        if (slider != null) slider.value = progress;
        if (fillerImage != null) fillerImage.fillAmount = progress;
    }

    private void ShowLoadingText()
    {
        if (tipsText == null)
        {
            CancelInvoke(nameof(ShowLoadingText));
            return;
        }
        ShowRepeatingText(tips, tipsText);
    }

    private void ShowRepeatingText(string[] strings, Text text)
    {
        switchIndex = PlayerPrefs.GetInt(prefsName, switchIndex);
        if (switchIndex >= 0 && switchIndex < strings.Length)
            text.text = tipPrefix + strings[switchIndex];
        switchIndex = (switchIndex + 1) % strings.Length;
        PlayerPrefs.SetInt(prefsName, switchIndex);
    }

    EventBinding<OnLoadScene> sceneLoadEventBinding;
    EventBinding<OnSwapScene> sceneSwapEventBinding;
    void OnEnable()
    {
        sceneLoadEventBinding = new EventBinding<OnLoadScene>(LoadSceneCommand);
        EventBus<OnLoadScene>.Register(sceneLoadEventBinding);

        sceneSwapEventBinding = new EventBinding<OnSwapScene>(SwapSpecificScene);
        EventBus<OnSwapScene>.Register(sceneSwapEventBinding);
    }

    void OnDisable()
    {
        EventBus<OnLoadScene>.Deregister(sceneLoadEventBinding);
        EventBus<OnSwapScene>.Deregister(sceneSwapEventBinding);
    }

    private void LoadSceneCommand(OnLoadScene load)
    {
        SceneLoaderFacade facade = new SceneLoaderFacade(this, loadingDuration, staticProgression);
        if (load.command.IsByName)
            facade.LoadScenes(new SceneLoadCommand(load.command.sceneNames), loadingType);
        else
            facade.LoadScenes(new SceneLoadCommand(load.command.sceneIndices), loadingType);
    }

    private void SwapScenes(OnSwapScene swap)
    {
        SceneLoaderFacade facade = new SceneLoaderFacade(this, loadingDuration, staticProgression);

        if (swap.command.IsByName)
        {
            string[] loadedScenes = SceneLoadCommandHelper.GetAllLoadedSceneNames();

            string sceneToUnload = loadedScenes.Length > 0 ? loadedScenes[loadedScenes.Length - 1] : null;

            if (!string.IsNullOrEmpty(sceneToUnload) && sceneToUnload != swap.command.sceneNames[0])
            {
                facade.SwapScenes(
                    new SceneLoadCommand(new string[] { sceneToUnload }),
                    new SceneLoadCommand(swap.command.sceneNames),
                    loadingType
                );
            }
        }
        else
        {
            int[] loadedIndices = SceneLoadCommandHelper.GetAllLoadedSceneIndices();
            int sceneToUnload = loadedIndices.Length > 0 ? loadedIndices[loadedIndices.Length - 1] : -1;

            if (sceneToUnload >= 0 && sceneToUnload != swap.command.sceneIndices[0])
            {
                facade.SwapScenes(
                    new SceneLoadCommand(new int[] { sceneToUnload }),
                    new SceneLoadCommand(swap.command.sceneIndices),
                    loadingType
                );
            }
        }
    }

    private void SwapSpecificScene(OnSwapScene swap)
    {
        SceneLoaderFacade facade = new SceneLoaderFacade(this, loadingDuration, staticProgression);

        if (swap.command.IsByName)
        {
            string[] loadedScenes = SceneLoadCommandHelper.GetAllLoadedSceneNames();

            string sceneToUnload = Array.Find(loadedScenes, s => ScenesToSwapBetween.All.Contains(s));

            if (!string.IsNullOrEmpty(sceneToUnload) && sceneToUnload != swap.command.sceneNames[0])
            {
                facade.SwapScenes(
                    new SceneLoadCommand(new string[] { sceneToUnload }),
                    new SceneLoadCommand(swap.command.sceneNames),
                    loadingType
                );
            }
            else if (string.IsNullOrEmpty(sceneToUnload))
            {
                facade.LoadScenes(new SceneLoadCommand(swap.command.sceneNames), loadingType);
            }
        }
        else
        {
            int[] loadedIndices = SceneLoadCommandHelper.GetAllLoadedSceneIndices();

            int sceneToUnload = Array.Find(loadedIndices, i =>
            {
                string sceneName = SceneManager.GetSceneByBuildIndex(i).name;
                return ScenesToSwapBetween.All.Contains(sceneName);
            });

            if (sceneToUnload >= 0 && sceneToUnload != swap.command.sceneIndices[0])
            {
                facade.SwapScenes(
                    new SceneLoadCommand(new int[] { sceneToUnload }),
                    new SceneLoadCommand(swap.command.sceneIndices),
                    loadingType
                );
            }
            else if (sceneToUnload < 0)
            {
                facade.LoadScenes(new SceneLoadCommand(swap.command.sceneIndices), loadingType);
            }
        }
    }

    public IEnumerator UnloadScenesCoroutine(string[] sceneNames, int[] sceneIndices)
    {
        int count = sceneNames?.Length > 0 ? sceneNames.Length : sceneIndices.Length;
        AsyncOperation[] operations = new AsyncOperation[count];

        for (int i = 0; i < count; i++)
        {
            if (sceneNames != null && sceneNames.Length > 0)
            {
                if (SceneManager.GetSceneByName(sceneNames[i]).isLoaded)
                    operations[i] = SceneManager.UnloadSceneAsync(sceneNames[i]);
            }
            else
            {
                int index = sceneIndices[i];
                if (SceneManager.GetSceneByBuildIndex(index).isLoaded)
                    operations[i] = SceneManager.UnloadSceneAsync(index);
            }
        }

        bool allDone;
        do
        {
            allDone = true;
            foreach (var op in operations)
            {
                if (op != null && !op.isDone) allDone = false;
            }
            yield return null;
        } while (!allDone);
    }
}

public static class SceneLoadCommandHelper
{
    public static SceneLoadCommand FromIndices(params int[] indices)
    => new SceneLoadCommand(indices);

    public static SceneLoadCommand FromNames(params string[] names)
        => new SceneLoadCommand(names);

    public static Scene? GetAdditiveSceneByName(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
                return scene;
        }
        return null;
    }

    public static Scene? GetAdditiveSceneByBuildIndex(int buildIndex)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex == buildIndex && scene.isLoaded)
                return scene;
        }
        return null;
    }

    public static Scene[] GetAllLoadedScenes()
    {
        int count = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[count];

        for (int i = 0; i < count; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        return loadedScenes;
    }

    public static int[] GetAllLoadedSceneIndices()
    {
        int count = SceneManager.sceneCount;
        int[] indices = new int[count];

        for (int i = 0; i < count; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            indices[i] = scene.buildIndex;
        }

        return indices;
    }

    public static string[] GetAllLoadedSceneNames()
    {
        int count = SceneManager.sceneCount;
        string[] names = new string[count];

        for (int i = 0; i < count; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            names[i] = scene.name;
        }

        return names;
    }

    public static string[] GetLoadedSceneNamesInOrder()
       => SceneLoadTracker.GetSceneNamesInLoadOrder();

    public static int[] GetLoadedSceneIndicesInOrder()
        => SceneLoadTracker.GetSceneIndicesInLoadOrder();
}

public static class SceneLoadTracker
{
    private static readonly List<string> loadedSceneNamesInOrder = new();
    private static readonly List<int> loadedSceneIndicesInOrder = new();
    private static bool isProcessing = false;

    public static void RegisterScene(string name, int index)
    {
        if (isProcessing) return;
        isProcessing = true;

        if (!string.IsNullOrEmpty(name) && !loadedSceneNamesInOrder.Contains(name))
            loadedSceneNamesInOrder.Add(name);
        if (index >= 0 && !loadedSceneIndicesInOrder.Contains(index))
            loadedSceneIndicesInOrder.Add(index);

        isProcessing = false;
    }

    public static void Remove(string name)
    {
        if (isProcessing || string.IsNullOrEmpty(name)) return;
        isProcessing = true;

        loadedSceneNamesInOrder.Remove(name);

        isProcessing = false;
    }

    public static string[] GetSceneNamesInLoadOrder() => loadedSceneNamesInOrder.ToArray();
    public static int[] GetSceneIndicesInLoadOrder() => loadedSceneIndicesInOrder.ToArray();

    public static void Clear()
    {
        if (isProcessing) return;
        isProcessing = true;

        loadedSceneNamesInOrder.Clear();
        loadedSceneIndicesInOrder.Clear();

        isProcessing = false;
    }
}