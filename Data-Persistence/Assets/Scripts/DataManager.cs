using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    private string filePath;

    private JObject minigameSaveData;
    public static event Action OnDataUpdated;
    
    public static DataManager getInstance
    {
        get
        {
            if (instance == null)
            {
                // Look for an existing instance in the scene.
                instance = FindObjectOfType<DataManager>();

                // If none exists, create a new one.
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("DataManager");
                    instance = singletonObject.AddComponent<DataManager>();
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (minigameSaveData == null)
        {
            minigameSaveData = new JObject();
        }
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadFromFile();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    private string GetSaveFilePath(string playerID)
    {
        return Application.persistentDataPath + "/gameData.json";
    }
    void SaveJsonToFile(string json, string filePath)
    {
        try
        {
            // Write the JSON string to the file
            File.WriteAllText(filePath, json);
            OnDataUpdated?.Invoke();
            Debug.Log($"JSON file saved to: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save JSON file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        {
            string filePath = Application.persistentDataPath + "/gameData.json";
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    minigameSaveData = JObject.Parse(jsonContent);
                    Debug.Log($"Loaded save data: {jsonContent}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load save file: {ex.Message}");
                    minigameSaveData = new JObject();
                }
            }
            else
            {
                Debug.Log("No save file found, creating new data");
                minigameSaveData = new JObject();
            }
        }
    }

    public void SetFlag(string playerID, string flagName, bool value)
    {
        ToJson(playerID, flagName, value);
    }

    public void SetFloat(string playerID, string floatName, float value)
    {
        ToJson(playerID, floatName, value);
    }
    public void SetInt(string playerID, string intName, int value)
    {
        ToJson(playerID, intName, value);
    }
    public void SetVector3(string playerID, string vectorName, Vector3 value)
    {
        JObject vectorObject = new JObject
        {
            ["x"] = value.x,
            ["y"] = value.y,
            ["z"] = value.z
        };
        ToJson(playerID, vectorName, vectorObject);
    }
    
    public bool GetFlag(string playerID, string flagName)
    {
        JToken token = FromJson(playerID, flagName);
        Debug.Log(token);
        if (token != null)
        {
            return token.ToObject<bool>();
        }
        
        return false;
    }

    public float GetFloat(string playerID, string floatName)
    {
        JToken token = FromJson(playerID, floatName);
        if (token != null)
        {
            return token.ToObject<float>();
        }
        else
        {
            return 0;
        }
    }
    public int GetInt(string playerID, string intName)
    {
        JToken token = FromJson(playerID, intName);
        if (token != null)
        {
            return token.ToObject<int>();
        }
        return 0;
    }
    
    public Vector3 GetVector3(string playerID, string vectorName)
    {
        JToken token = FromJson(playerID, vectorName);
        if (token != null && token is JObject vectorObject)
        {
            return new Vector3(
                (float)vectorObject["x"],
                (float)vectorObject["y"],
                (float)vectorObject["z"]
            );
        }
        return Vector3.zero;
    }


    private void ToJson(string playerID, string key, JToken value)
    {
        string newKey = string.Format("{0}_{1}", playerID, key);
        minigameSaveData[newKey] = value;
        SaveJsonToFile(minigameSaveData.ToString(),Application.persistentDataPath + "/gameData.json");
    }
    private JToken FromJson(string playerID, string key)
    {
        if (minigameSaveData == null)
        {
            Debug.LogError("minigameSaveData is null");
            return null;
        }

        string newKey = $"{playerID}_{key}";
        Debug.Log($"Retrieving key: {newKey}");
        return minigameSaveData.GetValue(newKey);
    }
}
