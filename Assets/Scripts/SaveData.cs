using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

[System.Serializable]
public struct EventfulProperty<T>
{
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChange?.Invoke(value);
        }
    }

    private T _value;

    public delegate void PropertyChangeEvent(T newValue);

    public event PropertyChangeEvent OnPropertyChange;
}

[System.Serializable]
public class SaveData
{
    static bool SaveAlreadyMade = false;
    /// <summary>
    ///  A quick helper function to load a save from a file.
    ///  You shouldn't have two Save's active at the same time for the same path.
    ///  Just saving you some future headache, future programmer
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SaveData GetSaveFromFile(string path)
    {
        //
        if (SaveAlreadyMade)
        {
            Debug.Log("Two SaveData instances have been made from GetSaveFromFile, this warning is just incase this means something bad.");
        }
        SaveAlreadyMade = true;
        //
        SaveData sd = new SaveData(path);
        sd.Load();
        return sd;
    }

    /// <summary>
    ///  How much alien dna the player has
    ///  Used for player upgrades
    /// </summary>
    public int alienDNA;

    public EventfulProperty<string> test = new EventfulProperty<string>();

    /// <summary>
    ///  The path, relative to persistentDataPath this save is located at
    /// </summary>
    [System.NonSerialized]
    public string path;

    /// <summary>
    ///  The full path of the save, cannot be changed.
    ///  Is recalculated every time and I'm not sure of the time complexity of the function
    /// </summary>
    /// <returns></returns>
    public string fullPath
    { get => Path.Combine(Application.persistentDataPath, path); }

    public SaveData(string path)
    {
        this.path = path;
    }

    /// <summary>
    ///  Saves the data to the path of the SaveData.
    /// </summary>
    /// <returns>Was the operation successful?</returns>
    public bool Save()
    {
        try
        {
            File.WriteAllText(fullPath, ToJson());
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///  Loads the data from the path of the SaveData. Will overwrite any new changes if already loaded.
    /// </summary>
    /// <returns>Was the operation successful?</returns>
    public bool Load()
    {
        try
        {
            string contents = File.ReadAllText(fullPath);
            LoadFromJson(contents);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///  Converts the object to a JSON representation
    /// </summary>
    /// <returns>The JSON version of the save</returns>
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    ///  Inserts data into the SaveData object from the json string
    /// </summary>
    /// <param name="jsonString">The string containing the json</param>
    public void LoadFromJson(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }
}