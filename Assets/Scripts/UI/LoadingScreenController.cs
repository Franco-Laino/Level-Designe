using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // Nuevo sistema de Input

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] CanvasGroup panelGroup;
    [SerializeField] Image progressBarFill;
    [SerializeField] TMP_Text progressText;
    [SerializeField] TMP_Text pressAnyKeyText;

    [Header("Configuración")]
    [SerializeField] float barSmoothSpeed = 3f;
    [SerializeField] float fadeInDuration = 0.5f;

    private float _targetProgress = 0f;
    private float _displayProgress = 0f;
    private bool _waitingForInput = false;

    void OnEnable()
    {
        SceneLoader.OnProgressChanged += HandleProgress;
        SceneLoader.OnLoadComplete += HandleLoadComplete;
    }

    void OnDisable()
    {
        SceneLoader.OnProgressChanged -= HandleProgress;
        SceneLoader.OnLoadComplete -= HandleLoadComplete;
    }

    void Start()
    {
        panelGroup.alpha = 0f;
        pressAnyKeyText.gameObject.SetActive(false);
        progressBarFill.fillAmount = 0f;
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return FadeGroup(panelGroup, 0f, 1f, fadeInDuration);
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.StartLoadingTargetScene();
    }

    void HandleProgress(float progress)
    {
        _targetProgress = progress;
        if (progressText != null) progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
    }

    void HandleLoadComplete()
    {
        pressAnyKeyText.gameObject.SetActive(true);
        _waitingForInput = true;
    }

    void Update()
    {
        _displayProgress = Mathf.Lerp(_displayProgress, _targetProgress, Time.deltaTime * barSmoothSpeed);
        progressBarFill.fillAmount = _displayProgress;

        if (_waitingForInput)
        {
            bool inputDetected = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                                 (Pointer.current != null && Pointer.current.press.wasPressedThisFrame);

            if (inputDetected)
            {
                _waitingForInput = false;
                SceneLoader.Instance.AllowActivation();
            }
        }
    }

    IEnumerator FadeGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}