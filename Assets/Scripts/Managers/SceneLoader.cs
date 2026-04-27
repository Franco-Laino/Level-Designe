using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public const string SCENE_SPLASH = "0_Splash";
    public const string SCENE_MAINMENU = "1_MainMenu";
    public const string SCENE_LOADING = "SampleScene";
    public const string SCENE_GAME = "SampleScene";

    public static string TargetScene { get; private set; }
    public static float LoadProgress { get; private set; }

    public static event Action<float> OnProgressChanged;
    public static event Action OnLoadComplete;

    private AsyncOperation _currentLoadingOperation;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Método para ir al menú sin pantalla de carga
    public void LoadMainMenuDirectly()
    {
        SceneManager.LoadScene(SCENE_MAINMENU);
    }

    // Método para ir al juego con pantalla de carga
    public void LoadSceneWithLoading(string sceneName)
    {
        TargetScene = sceneName;
        SceneManager.LoadScene(SCENE_LOADING);
    }

    public void StartLoadingTargetScene()
    {
        StartCoroutine(LoadAsync(TargetScene));
    }

    public void AllowActivation()
    {
        if (_currentLoadingOperation != null)
            _currentLoadingOperation.allowSceneActivation = true;
    }

    IEnumerator LoadAsync(string sceneName)
    {
        yield return new WaitForSeconds(0.3f);
        _currentLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
        _currentLoadingOperation.allowSceneActivation = false;

        while (!_currentLoadingOperation.isDone)
        {
            float progress = Mathf.Clamp01(_currentLoadingOperation.progress / 0.9f);
            LoadProgress = progress;
            OnProgressChanged?.Invoke(progress);

            if (_currentLoadingOperation.progress >= 0.9f)
            {
                LoadProgress = 1f;
                OnProgressChanged?.Invoke(1f);
                OnLoadComplete?.Invoke();
                yield break;
            }
            yield return null;
        }
    }
}