using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel de opciones — volumen y resolución.
///
/// La lista de resoluciones va de MENOR a MAYOR.
/// Al presionar "‹" baja la resolución, "›" la sube.
/// Al abrir el panel por primera vez, arranca en la resolución actual.
/// </summary>
public class OptionsMenuController : MonoBehaviour
{
    const string KEY_VOLUME = "opt_volume";
    const string KEY_RES = "opt_res_index";

    [Header("── Volumen ──────────────────────")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Text volumeValueText;

    [Header("── Resolución ───────────────────")]
    [SerializeField] Button btnResPrev;   // ‹  — resolución más baja
    [SerializeField] Button btnResNext;   // ›  — resolución más alta
    [SerializeField] TMP_Text resValueText;

    Resolution[] _resolutions;  // de menor a mayor
    int _resIndex;     // índice actual

    // ── Inicialización ────────────────────────────────────────────────

    void Start()
    {
        BuildResolutionList();

        // Volumen — conectar slider
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Resolución — conectar botones
        if (btnResPrev != null)
            btnResPrev.onClick.AddListener(() => ChangeResolution(-1));
        if (btnResNext != null)
            btnResNext.onClick.AddListener(() => ChangeResolution(+1));

        // En WebGL no tiene sentido cambiar la resolución
#if UNITY_WEBGL
        HideResolutionRow();
#endif
    }

    // Cada vez que el panel se activa, cargar los valores guardados
    void OnEnable()
    {
        BuildResolutionList();
        LoadSettings();
    }

    // ── Lista de resoluciones ─────────────────────────────────────────

    void BuildResolutionList()
    {
        // Filtrar resoluciones únicas (Unity repite con distintos refresh rates)
        var unique = new List<Resolution>();
        foreach (var r in Screen.resolutions)
        {
            bool exists = false;
            foreach (var u in unique)
                if (u.width == r.width && u.height == r.height)
                { exists = true; break; }
            if (!exists)
                unique.Add(r);
        }

        // Ordenar de menor a mayor (menor índice = menor resolución)
        unique.Sort((a, b) => a.width != b.width
            ? a.width.CompareTo(b.width)
            : a.height.CompareTo(b.height));

        _resolutions = unique.ToArray();

        Debug.Log($"[Options] {_resolutions.Length} resoluciones disponibles.");
    }

    // ── Cambiar resolución ────────────────────────────────────────────

    /// <summary>
    /// direction: -1 = resolución más chica, +1 = resolución más grande.
    /// </summary>
    void ChangeResolution(int direction)
    {
        if (_resolutions == null || _resolutions.Length == 0) return;

        int newIndex = _resIndex + direction;

        // Clamp: no salir de los límites
        newIndex = Mathf.Clamp(newIndex, 0, _resolutions.Length - 1);

        if (newIndex == _resIndex) return; // ya está en el límite, no hacer nada

        _resIndex = newIndex;
        ApplyResolution(_resIndex);

        PlayerPrefs.SetInt(KEY_RES, _resIndex);
        PlayerPrefs.Save();

        // Actualizar estado de los botones (deshabilitar en los extremos)
        UpdateResButtonStates();
    }

    void ApplyResolution(int index)
    {
        if (_resolutions == null || index < 0 || index >= _resolutions.Length) return;

        Resolution r = _resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);

        if (resValueText != null)
            resValueText.text = $"{r.width} × {r.height}";

        Debug.Log($"[Options] Resolución aplicada: {r.width}x{r.height}");
    }

    /// <summary>Deshabilita el botón si ya estamos en el extremo de la lista.</summary>
    void UpdateResButtonStates()
    {
        if (btnResPrev != null)
            btnResPrev.interactable = _resIndex > 0;
        if (btnResNext != null)
            btnResNext.interactable = _resIndex < _resolutions.Length - 1;
    }

    // ── Volumen ───────────────────────────────────────────────────────

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;

        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(value * 10f).ToString();

        PlayerPrefs.SetFloat(KEY_VOLUME, value);
    }

    // ── Cargar settings guardados ─────────────────────────────────────

    void LoadSettings()
    {
        // Volumen
        float vol = PlayerPrefs.GetFloat(KEY_VOLUME, 0.8f);
        AudioListener.volume = vol;
        if (volumeSlider != null) volumeSlider.value = vol;
        if (volumeValueText != null) volumeValueText.text = Mathf.RoundToInt(vol * 10f).ToString();

        // Resolución — buscar índice de la resolución actual
        int savedIndex = PlayerPrefs.GetInt(KEY_RES, FindCurrentResolutionIndex());
        _resIndex = Mathf.Clamp(savedIndex, 0, _resolutions.Length - 1);

        // Mostrar sin aplicar (ya está aplicada)
        if (_resolutions != null && _resIndex < _resolutions.Length)
        {
            Resolution r = _resolutions[_resIndex];
            if (resValueText != null)
                resValueText.text = $"{r.width} × {r.height}";
        }

        UpdateResButtonStates();
    }

    int FindCurrentResolutionIndex()
    {
        if (_resolutions == null) return 0;
        int currentW = Screen.currentResolution.width;
        int currentH = Screen.currentResolution.height;

        for (int i = 0; i < _resolutions.Length; i++)
            if (_resolutions[i].width == currentW && _resolutions[i].height == currentH)
                return i;

        // Si no se encuentra, devolver el índice más alto (resolución nativa)
        return _resolutions.Length - 1;
    }

    // ── Cerrar panel ──────────────────────────────────────────────────

    public void Close()
    {
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }

    // ── WebGL: ocultar fila de resolución ─────────────────────────────

    void HideResolutionRow()
    {
        // Ocultar el padre de los botones de resolución
        if (btnResPrev != null && btnResPrev.transform.parent != null)
            btnResPrev.transform.parent.gameObject.SetActive(false);
    }
}