using System;
using UnityEngine;

/// <summary>
/// Sistema de guardado simple usando PlayerPrefs (compatible con WebGL/itch.io).
/// Para proyectos más grandes considera usar IndexedDB vía JSLib en WebGL.
/// </summary>
[System.Serializable]
public class SaveData
{
    public int currentLevel = 0;
    public float playTimeSeconds = 0f;
    public string lastSaveDateTime = "";
    public int difficulty = 1; // 0=Easy, 1=Normal, 2=Hard
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    const string SAVE_KEY = "horror_game_save";
    const string HAS_SAVE_KEY = "horror_has_save";

    SaveData _currentSave;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSaveData()
    {
        return PlayerPrefs.GetInt(HAS_SAVE_KEY, 0) == 1;
    }

    public void NewGame(int difficulty = 1)
    {
        _currentSave = new SaveData { difficulty = difficulty };
        Save();
    }

    public void Save()
    {
        _currentSave.lastSaveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        string json = JsonUtility.ToJson(_currentSave);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] Guardado: " + json);
    }

    public SaveData Load()
    {
        if (!HasSaveData()) return null;
        string json = PlayerPrefs.GetString(SAVE_KEY, "{}");
        _currentSave = JsonUtility.FromJson<SaveData>(json);
        return _currentSave;
    }

    public void LoadLatestSlot()
    {
        _currentSave = Load();
    }

    public SaveData CurrentSave => _currentSave;

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 0);
        PlayerPrefs.Save();
        _currentSave = null;
    }
}