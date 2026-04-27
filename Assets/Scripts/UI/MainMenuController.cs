using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Menú principal con transiciones suaves.
/// Actualizado para integrarse con el nuevo SceneLoader de Happy Hounds Studio.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("── Grupos de UI ──────────────────")]
    [SerializeField] CanvasGroup titleGroup;
    [SerializeField] CanvasGroup menuGroup;

    [Header("── Botones ──────────────────────")]
    [SerializeField] Button btnNewGame;
    [SerializeField] Button btnOptions;
    [SerializeField] Button btnCredits;
    [SerializeField] Button btnQuit;

    [Header("── Paneles secundarios ──────────")]
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;

    [Header("── Opcionales ───────────────────")]
    [SerializeField] TMP_Text versionText;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip menuTheme;
    [SerializeField] AudioClip confirmSFX;
    [SerializeField] AudioClip backSFX;

    [Header("── Tiempos ──────────────────────")]
    [SerializeField] float titleFadeIn = 1.2f;
    [SerializeField] float menuDelay = 0.6f;
    [SerializeField] float menuFadeIn = 0.8f;
    [SerializeField] float panelFadeTime = 0.35f;
    [SerializeField] float panelSlideAmt = 30f;

    void Start()
    {
        SetGroup(titleGroup, 0f, false);
        SetGroup(menuGroup, 0f, false);

        if (versionText != null)
            versionText.text = $"v{Application.version} — DEMO";

        // Suscripción de eventos
        if (btnNewGame != null) btnNewGame.onClick.AddListener(OnNewGame);
        if (btnOptions != null) btnOptions.onClick.AddListener(OnOptions);
        if (btnCredits != null) btnCredits.onClick.AddListener(OnCredits);
        if (btnQuit != null) btnQuit.onClick.AddListener(OnQuit);

        StartCoroutine(PlayIntroSequence());

        if (menuTheme != null && musicSource != null)
            StartCoroutine(FadeInMusic());
    }

    // ── Intro ─────────────────────────────────────────────────────────

    IEnumerator PlayIntroSequence()
    {
        yield return FadeGroup(titleGroup, 0f, 1f, titleFadeIn);
        yield return new WaitForSeconds(menuDelay);
        yield return FadeGroup(menuGroup, 0f, 1f, menuFadeIn);
        SetGroup(menuGroup, 1f, true);

        if (btnNewGame != null)
            EventSystem.current?.SetSelectedGameObject(btnNewGame.gameObject);
    }

    IEnumerator FadeInMusic()
    {
        musicSource.clip = menuTheme;
        musicSource.loop = true;
        musicSource.volume = 0f;
        musicSource.Play();
        float t = 0f;
        while (t < 2f)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 0.7f, t / 2f);
            yield return null;
        }
        musicSource.volume = 0.7f;
    }

    // ── Handlers de botones ───────────────────────────────────────────

    void OnNewGame()
    {
        Debug.Log("[MainMenu] New Game");
        PlaySFX(confirmSFX);

        if (SceneLoader.Instance != null)
        {
            // CAMBIO: Ahora usamos la carga con pantalla intermedia para ir al juego
            SceneLoader.Instance.LoadSceneWithLoading(SceneLoader.SCENE_GAME);
        }
        else
        {
            Debug.LogError("[MainMenu] SceneLoader no encontrado en la escena.");
        }
    }

    void OnOptions()
    {
        PlaySFX(confirmSFX);
        if (optionsPanel != null) StartCoroutine(OpenPanel(optionsPanel));
    }

    void OnCredits()
    {
        PlaySFX(confirmSFX);
        if (creditsPanel != null) StartCoroutine(OpenPanel(creditsPanel));
    }

    void OnQuit()
    {
        PlaySFX(confirmSFX);
        StartCoroutine(QuitSequence());
    }

    IEnumerator QuitSequence()
    {
        yield return FadeGroup(menuGroup, 1f, 0f, 0.4f);
        yield return FadeGroup(titleGroup, 1f, 0f, 0.5f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadMainMenuDirectly(); 
#else
        Application.Quit();
#endif
    }

    // ── Transiciones de paneles ───────────────────────────────────────

    IEnumerator OpenPanel(GameObject panel)
    {
        SetGroup(menuGroup, menuGroup.alpha, false);

        float elapsed = 0f;
        float startAlphaTitle = titleGroup != null ? titleGroup.alpha : 0f;
        float startAlphaMenu = menuGroup != null ? menuGroup.alpha : 0f;

        while (elapsed < panelFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / panelFadeTime);
            if (titleGroup != null) titleGroup.alpha = Mathf.Lerp(startAlphaTitle, 0f, t);
            if (menuGroup != null) menuGroup.alpha = Mathf.Lerp(startAlphaMenu, 0f, t);
            yield return null;
        }

        if (titleGroup != null) titleGroup.alpha = 0f;
        if (menuGroup != null) menuGroup.alpha = 0f;

        panel.SetActive(true);

        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        if (panelCG == null) panelCG = panel.AddComponent<CanvasGroup>();

        panelCG.alpha = 0f;
        panelCG.interactable = false;
        panelCG.blocksRaycasts = false;

        RectTransform panelRT = panel.GetComponent<RectTransform>();
        Vector2 targetPos = panelRT.anchoredPosition;
        Vector2 startPos = targetPos + Vector2.down * panelSlideAmt;
        panelRT.anchoredPosition = startPos;

        elapsed = 0f;
        while (elapsed < panelFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / panelFadeTime);
            panelCG.alpha = Mathf.Lerp(0f, 1f, t);
            panelRT.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        panelCG.alpha = 1f;
        panelRT.anchoredPosition = targetPos;
        panelCG.interactable = true;
        panelCG.blocksRaycasts = true;
    }

    IEnumerator ClosePanel(GameObject panel)
    {
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        RectTransform panelRT = panel.GetComponent<RectTransform>();

        if (panelCG != null)
        {
            panelCG.interactable = false;
            panelCG.blocksRaycasts = false;
        }

        Vector2 startPos = panelRT != null ? panelRT.anchoredPosition : Vector2.zero;
        Vector2 targetPos = startPos + Vector2.down * panelSlideAmt;

        float elapsed = 0f;
        float startAlpha = panelCG != null ? panelCG.alpha : 1f;

        while (elapsed < panelFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / panelFadeTime);
            if (panelCG != null) panelCG.alpha = Mathf.Lerp(startAlpha, 0f, t);
            if (panelRT != null) panelRT.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        panel.SetActive(false);
        if (panelRT != null) panelRT.anchoredPosition = startPos;

        elapsed = 0f;
        while (elapsed < panelFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / panelFadeTime);
            if (titleGroup != null) titleGroup.alpha = Mathf.Lerp(0f, 1f, t);
            if (menuGroup != null) menuGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        SetGroup(titleGroup, 1f, false);
        SetGroup(menuGroup, 1f, true);

        if (btnNewGame != null)
            EventSystem.current?.SetSelectedGameObject(btnNewGame.gameObject);
    }

    public void CloseOptions() => StartCoroutine(ClosePanel(optionsPanel));
    public void CloseCredits() => StartCoroutine(ClosePanel(creditsPanel));

    // ── Utilidades ────────────────────────────────────────────────────

    void SetGroup(CanvasGroup cg, float alpha, bool interactable)
    {
        if (cg == null) return;
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
    }

    IEnumerator FadeGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}