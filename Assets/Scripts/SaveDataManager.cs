using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    /// <summary>
    ///  Memo of the SaveDataManager
    /// </summary>
    static SaveDataManager sdm;

    /// <summary>
    ///  Get the SaveDataManager
    /// </summary>
    /// <returns></returns>
    public static SaveDataManager GetSaveDataManager() {
        if (sdm) return sdm;
        sdm = UnityUtil.GetRootComponent<SaveDataManager>();
        return sdm;
    }

    /// <summary>
    ///  Get the SaveData from the SaveDataManager
    /// </summary>
    /// <returns></returns>
    public static SaveData GetSaveData() {
        if (sdm) return sdm._save;
        return GetSaveDataManager()._save;
    }

    /// <summary>
    ///  The path of the save managed by the SaveDataManager
    /// </summary>
    [Tooltip("The path of the save")]
    public string SavePath = "Save01.dat";

    /// <summary>
    ///  The amount of time that must elapse between saves, to prevent spam saves
    /// </summary>
    [Tooltip("The amount of time that must elapse between saves, to prevent spam saves")]
    public float MaxSaveInterval = 0.2f;

    private SaveData _save;

    private float _lastSave;

    void Awake() {
        _save = SaveData.GetSaveFromFile(SavePath);
    }

    void OnApplicationQuit() {
        Save();
    }

    void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) return;
        Save();
    }

    private void Save() {
        if (_lastSave + MaxSaveInterval > Time.time) return;

        if (_save.Save()) {
            Debug.Log("Saved!");
        } else {
            Debug.Log("Save Fail :(");
        }

        _lastSave = Time.time;
    }
}
