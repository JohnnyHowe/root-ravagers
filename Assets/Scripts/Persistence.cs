using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Persistence : MonoBehaviour
{
    [NonSerialized]
    public Data Data;

    private string _path;
    // Start is called before the first frame update
    void Start()
    {
        _path = Path.Join(Application.persistentDataPath, "data.json");
        Load();
        //Print();
    }

    private void Print()
    {
        foreach (var score in Data.Scores)
        {
            Debug.Log($"{score.Key}: {score.Value}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Data.Scores.Add($"Bob{UnityEngine.Random.Range(1, 100)}", UnityEngine.Random.Range(1, 100));
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Print();
        //}
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
        File.WriteAllText(_path, json);
    }

    public void Load()
    {
        try
        {
            string json = File.ReadAllText(_path);
            Data = JsonConvert.DeserializeObject<Data>(json);
        } catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            Data = new Data();
        }
    }
}

[Serializable]
public class Data
{
    public Dictionary<string, int> Scores = new();
}