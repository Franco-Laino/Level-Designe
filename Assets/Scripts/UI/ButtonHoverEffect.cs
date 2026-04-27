using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Efecto hover cinematográfico para botones de menú.
/// CORRECCIÓN: Captura la posición original basándose en frames locales del objeto,
/// evitando errores de Layout al cambiar de escenas.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("Referencias UI")]
    [SerializeField] TMP_Text buttonText;
    [SerializeField] Image indicatorDot;
    [SerializeField] Image underlineLine;

    [Header("Colores")]
    [SerializeField] Color normalColor = new Color(0.55f, 0.55f, 0.55f, 1f);
    [SerializeField] Color hoverColor = Color.white;
    [SerializeField] Color dotColor = new Color(0.85f, 0.15f, 0.15f, 1f);

    [Header("Movimiento")]
    [SerializeField] float hoverOffsetX = 14f;
    [SerializeField] float hoverScale = 1.04f;
    [SerializeField] float animDuration = 0.14f;

    // ── Estado interno ──────────────────────────────────────────────────
    private RectTransform _rt;
    private Vector2 _originalPos;
    private bool _positionCaptured = false;
    private int _framesSinceEnabled = 0; // Contador de frames local
    private Coroutine _anim;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        // Estado inicial de los elementos
        if (buttonText != null) buttonText.color = normalColor;

        if (indicatorDot != null)
        {
            indicatorDot.color = dotColor;
            indicatorDot.gameObject.SetActive(false);
        }

        if (underlineLine != null)
        {
            underlineLine.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0f);
            underlineLine.rectTransform.localScale = new Vector3(0f, 1f, 1f);
        }
    }

    void OnEnable()
    {
        // Resetear cada vez que el botón se activa (por si el menú se cierra y abre)
        _positionCaptured = false;
        _framesSinceEnabled = 0;
    }

    void Update()
    {
        if (_positionCaptured) return;

        // Esperamos 5 frames locales para asegurar que el LayoutGroup posicionó el botón
        _framesSinceEnabled++;
        if (_framesSinceEnabled < 5) return;

        _originalPos = _rt.anchoredPosition;
        _positionCaptured = true;
    }

    // ── Eventos de interfaz ─────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData data) => TriggerHover(true);
    public void OnPointerExit(PointerEventData data) => TriggerHover(false);
    public void OnSelect(BaseEventData data) => TriggerHover(true);
    public void OnDeselect(BaseEventData data) => TriggerHover(false);

    private void TriggerHover(bool entering)
    {
        // Si aún no capturamos la posición, no intentamos animar para evitar saltos
        if (!_positionCaptured) return;

        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(entering ? AnimateIn() : AnimateOut());
    }

    // ── Animaciones ─────────────────────────────────────────────────────

    IEnumerator AnimateIn()
    {
        Vector2 startPos = _rt.anchoredPosition;
        Vector2 targetPos = _originalPos + Vector2.right * hoverOffsetX;
        Vector3 startScale = _rt.localScale;
        Color startColor = buttonText != null ? buttonText.color : Color.white;

        if (indicatorDot != null) indicatorDot.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / animDuration));

            _rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            _rt.localScale = Vector3.Lerp(startScale, Vector3.one * hoverScale, t);

            if (buttonText != null)
                buttonText.color = Color.Lerp(startColor, hoverColor, t);

            if (underlineLine != null)
            {
                underlineLine.rectTransform.localScale = new Vector3(t, 1f, 1f);
                Color c = underlineLine.color;
                c.a = t;
                underlineLine.color = c;
            }

            yield return null;
        }

        _rt.anchoredPosition = targetPos;
        _rt.localScale = Vector3.one * hoverScale;
        if (buttonText != null) buttonText.color = hoverColor;
    }

    IEnumerator AnimateOut()
    {
        Vector2 startPos = _rt.anchoredPosition;
        Vector3 startScale = _rt.localScale;
        Color startColor = buttonText != null ? buttonText.color : Color.white;

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / animDuration));

            _rt.anchoredPosition = Vector2.Lerp(startPos, _originalPos, t);
            _rt.localScale = Vector3.Lerp(startScale, Vector3.one, t);

            if (buttonText != null)
                buttonText.color = Color.Lerp(startColor, normalColor, t);

            if (underlineLine != null)
            {
                underlineLine.rectTransform.localScale = new Vector3(Mathf.Lerp(1, 0, t), 1f, 1f);
                Color c = underlineLine.color;
                c.a = Mathf.Lerp(1, 0, t);
                underlineLine.color = c;
            }

            yield return null;
        }

        _rt.anchoredPosition = _originalPos;
        _rt.localScale = Vector3.one;
        if (buttonText != null) buttonText.color = normalColor;
        if (indicatorDot != null) indicatorDot.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (!_positionCaptured) return;
        if (_anim != null) StopCoroutine(_anim);

        // Reset físico para evitar que el botón quede desplazado
        _rt.anchoredPosition = _originalPos;
        _rt.localScale = Vector3.one;

        if (buttonText != null) buttonText.color = normalColor;
        if (indicatorDot != null) indicatorDot.gameObject.SetActive(false);
        if (underlineLine != null)
        {
            underlineLine.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            Color c = underlineLine.color;
            c.a = 0f;
            underlineLine.color = c;
        }
    }
}